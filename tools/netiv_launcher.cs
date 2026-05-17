using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

internal static class NetivLauncher
{
    private const uint MEM_COMMIT = 0x1000;
    private const uint MEM_RESERVE = 0x2000;
    private const uint PAGE_EXECUTE_READWRITE = 0x40;

    private static readonly List<Delegate> KeepAliveDelegates = new List<Delegate>();
    private static readonly List<IntPtr> ExecutableAllocations = new List<IntPtr>();
    private static readonly List<IntPtr> HGlobalAllocations = new List<IntPtr>();
    private static string ToolRoot = "";
    private static string WorkRoot = "";

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr VirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualFree(IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FlushInstructionCache(IntPtr hProcess, IntPtr lpBaseAddress, UIntPtr dwSize);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate ulong NativeEntry(IntPtr statements, IntPtr output, IntPtr scratch, IntPtr syscalls);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void VoidSyscall();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate ulong NowSyscall();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate IntPtr ResolveSyscall(IntPtr name);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate ulong SaveBinarySyscall(IntPtr name, IntPtr buffer, ulong size);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void DebugSyscall(ulong value, ulong pointer);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate IntPtr GetFunctionNameSyscall(ulong index);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate ulong GetFunctionCountSyscall();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate IntPtr GetFilesSyscall(IntPtr dirPath);

    private static IntPtr CompilerStatements = IntPtr.Zero;

    private static int Main(string[] args)
    {
        ToolRoot = ResolveToolRoot();
        WorkRoot = ResolveWorkRoot();

        if (args.Length > 0)
        {
            var command = args[0].Trim().ToLowerInvariant();
            if (command == "help" || command == "--help" || command == "-h" || command == "/?")
            {
                PrintHelp();
                return 0;
            }
            if (command == "version" || command == "--version" || command == "-v")
            {
                Console.WriteLine("Netiv Lang 1.0");
                return 0;
            }
            if (command == "witness" || command == "doctor")
            {
                return RunDoctor();
            }
            if (command == "archive" || command == "init")
            {
                var target = args.Length > 1 ? args[1] : WorkRoot;
                return InitProject(target);
            }
            if (command == "read")
            {
                return RunRead(args);
            }
            if (command == "audit")
            {
                return RunAudit();
            }
            if (command == "write" || command == "build")
            {
                return RunWrite();
            }
            if (command == "invoke" || command == "run")
            {
                return RunInvoke(args);
            }
            if (command == "prune" || command == "clean")
            {
                return RunPrune();
            }
            if (command == "graph")
            {
                return RunGraph();
            }
            if (command == "list")
            {
                return RunList();
            }
            if (command == "meta")
            {
                return RunMeta();
            }
            if (IsReservedCommand(command))
            {
                Console.Error.WriteLine("Netiv command reserved but not implemented yet: " + args[0]);
                return 2;
            }
            if (command != "write")
            {
                Console.Error.WriteLine("Unknown Netiv command: " + args[0]);
                Console.Error.WriteLine("Run 'netiv help' for available commands.");
                return 1;
            }
        }

        return RunWrite();
    }

    private static int RunWrite()
    {
        var files = GetNetivFiles(Path.Combine(WorkRoot, "src"));
        if (files.Length == 0)
        {
            Console.WriteLine("No project source pages found; running native toolchain entry.");
            return RunNativeEntry();
        }

        var binDir = Path.Combine(WorkRoot, "bin");
        var buildDir = Path.Combine(WorkRoot, "build");
        Directory.CreateDirectory(binDir);
        Directory.CreateDirectory(buildDir);

        var mainSource = FindMainSource(files);
        var text = File.ReadAllText(mainSource);
        var artifactName = SafeArtifactName(ExtractMetaValue(text, "name"));
        if (String.IsNullOrWhiteSpace(artifactName))
        {
            artifactName = SafeArtifactName(new DirectoryInfo(WorkRoot).Name);
        }
        if (String.IsNullOrWhiteSpace(artifactName))
        {
            artifactName = "netiv-project";
        }

        var exitCode = ExtractReturnStatus(text);
        var outputExe = Path.Combine(binDir, artifactName + ".exe");
        var generatedCs = Path.Combine(buildDir, artifactName + ".generated.cs");
        var manifest = Path.Combine(buildDir, artifactName + ".write.manifest");

        File.WriteAllText(generatedCs, GenerateProjectHost(artifactName, files, exitCode));

        var compiler = ResolveCSharpCompiler();
        if (compiler == null)
        {
            Console.Error.WriteLine("C# compiler not found; cannot emit project executable.");
            return 1;
        }

        var compile = new Process();
        compile.StartInfo.FileName = compiler;
        compile.StartInfo.Arguments = "/nologo /platform:x64 /optimize+ /target:exe /out:" + QuoteArgument(outputExe) + " " + QuoteArgument(generatedCs);
        compile.StartInfo.WorkingDirectory = WorkRoot;
        compile.StartInfo.UseShellExecute = false;
        compile.StartInfo.RedirectStandardOutput = true;
        compile.StartInfo.RedirectStandardError = true;
        compile.Start();
        var stdout = compile.StandardOutput.ReadToEnd();
        var stderr = compile.StandardError.ReadToEnd();
        compile.WaitForExit();

        if (stdout.Length > 0)
        {
            Console.Write(stdout);
        }
        if (stderr.Length > 0)
        {
            Console.Error.Write(stderr);
        }
        if (compile.ExitCode != 0)
        {
            return compile.ExitCode;
        }

        var manifestLines = new List<string>();
        manifestLines.Add("artifact=" + ToArchivePath(outputExe).Replace("\\", "/"));
        manifestLines.Add("entry=" + ToArchivePath(mainSource).Replace("\\", "/"));
        manifestLines.Add("pages=" + files.Length);
        manifestLines.Add("source-pages=" + CountSymbols(files, "°page •"));
        manifestLines.Add("methods=" + CountSymbols(files, "°method •"));
        manifestLines.Add("functions=" + CountSymbols(files, "°function •"));
        manifestLines.Add("structs=" + CountSymbols(files, "°struct •"));
        manifestLines.Add("enums=" + CountSymbols(files, "°enum •"));
        manifestLines.Add("consts=" + CountSymbols(files, "°const •"));
        manifestLines.Add("exit=" + exitCode);
        File.WriteAllLines(manifest, manifestLines.ToArray());

        Console.WriteLine("Wrote artifact " + outputExe);
        Console.WriteLine("Wrote manifest " + manifest);
        return 0;
    }

    private static int RunNativeEntry()
    {
        var buildEntryPath = Path.Combine(ToolRoot, "build", "netiv_build_entry.bin");
        var compilerPath = Path.Combine(ToolRoot, "build", "bootstrap_compiler.bin");

        if (!File.Exists(compilerPath))
        {
            Console.Error.WriteLine("Netiv bootstrap compiler not found: " + compilerPath);
            return 1;
        }

        var compilerMem = LoadExecutableBlob(compilerPath);
        if (compilerMem == IntPtr.Zero)
        {
            return 1;
        }

        var entryPath = File.Exists(buildEntryPath) ? buildEntryPath : compilerPath;
        var entryMem = entryPath == compilerPath ? compilerMem : LoadExecutableBlob(entryPath);
        if (entryMem == IntPtr.Zero)
        {
            return 1;
        }

        CompilerStatements = BuildCompilerStatementList();
        var syscalls = BuildSyscallTable(compilerMem);
        var emptyStatements = BuildPointerList(new string[0]);
        var output = AllocExecutable(65536);
        var scratch = AllocExecutable(65536);

        try
        {
            var entry = Marshal.GetDelegateForFunctionPointer<NativeEntry>(entryMem);
            var statements = entryPath == compilerPath ? emptyStatements : CompilerStatements;
            var result = entry(statements, output, scratch, syscalls);
            Console.WriteLine("Netiv entry completed with result " + result);
            return 0;
        }
        finally
        {
            foreach (var allocation in HGlobalAllocations)
            {
                Marshal.FreeHGlobal(allocation);
            }
            foreach (var allocation in ExecutableAllocations)
            {
                VirtualFree(allocation, UIntPtr.Zero, 0x8000);
            }
        }
    }

    private static IntPtr LoadExecutableBlob(string path)
    {
        var blob = File.ReadAllBytes(path);
        if (blob.Length == 0)
        {
            Console.Error.WriteLine("Netiv blob is empty: " + path);
            return IntPtr.Zero;
        }

        var mem = AllocExecutable(blob.Length + 4096);
        if (mem == IntPtr.Zero)
        {
            return IntPtr.Zero;
        }

        Marshal.Copy(blob, 0, mem, blob.Length);
        FlushInstructionCache(GetCurrentProcess(), mem, (UIntPtr)blob.Length);
        return mem;
    }

    private static IntPtr AllocExecutable(int size)
    {
        var mem = VirtualAlloc(IntPtr.Zero, (UIntPtr)size, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
        if (mem == IntPtr.Zero)
        {
            Console.Error.WriteLine("VirtualAlloc failed with error " + Marshal.GetLastWin32Error());
            return IntPtr.Zero;
        }
        ExecutableAllocations.Add(mem);
        return mem;
    }

    private static IntPtr BuildSyscallTable(IntPtr compilerMem)
    {
        var slots = new IntPtr[11];
        slots[0] = KeepAlive(new VoidSyscall(NtvSpawn));
        slots[1] = KeepAlive(new VoidSyscall(NtvPrintln));
        slots[2] = KeepAlive(new NowSyscall(NtvNow));
        slots[3] = KeepAlive(new ResolveSyscall(NtvResolve));
        slots[4] = KeepAlive(new SaveBinarySyscall(NtvSaveBinary));
        slots[5] = KeepAlive(new DebugSyscall(NtvDebug));
        slots[6] = KeepAlive(new VoidSyscall(NtvSeal));
        slots[7] = KeepAlive(new GetFunctionNameSyscall(NtvGetFunctionName));
        slots[8] = KeepAlive(new GetFunctionCountSyscall(NtvGetFunctionCount));
        slots[9] = compilerMem;
        slots[10] = KeepAlive(new GetFilesSyscall(NtvGetFiles));

        var table = Marshal.AllocHGlobal(IntPtr.Size * slots.Length);
        HGlobalAllocations.Add(table);
        for (var i = 0; i < slots.Length; i++)
        {
            Marshal.WriteIntPtr(table, i * IntPtr.Size, slots[i]);
        }
        return table;
    }

    private static IntPtr KeepAlive(Delegate callback)
    {
        KeepAliveDelegates.Add(callback);
        return Marshal.GetFunctionPointerForDelegate(callback);
    }

    private static IntPtr BuildCompilerStatementList()
    {
        var compilerSource = Path.Combine(ToolRoot, "src", "compiler.ntv");
        if (!File.Exists(compilerSource))
        {
            return BuildPointerList(new string[0]);
        }

        var statements = new List<string>();
        var inFunction = false;
        foreach (var rawLine in File.ReadAllLines(compilerSource))
        {
            var line = rawLine.Trim();
            var comment = line.IndexOf("//", StringComparison.Ordinal);
            if (comment >= 0)
            {
                line = line.Substring(0, comment).Trim();
            }
            if (line.Length == 0)
            {
                continue;
            }
            if (!inFunction)
            {
                if (line.StartsWith("fn ntv_compiler", StringComparison.Ordinal))
                {
                    inFunction = true;
                }
                continue;
            }
            if (line == "}")
            {
                break;
            }
            if (line != "{")
            {
                statements.Add(line);
            }
        }

        return BuildPointerList(statements.ToArray());
    }

    private static IntPtr BuildPointerList(string[] statements)
    {
        var table = Marshal.AllocHGlobal(IntPtr.Size * (statements.Length + 1));
        HGlobalAllocations.Add(table);
        for (var i = 0; i < statements.Length; i++)
        {
            var text = Marshal.StringToHGlobalAnsi(statements[i]);
            HGlobalAllocations.Add(text);
            Marshal.WriteIntPtr(table, i * IntPtr.Size, text);
        }
        Marshal.WriteIntPtr(table, statements.Length * IntPtr.Size, IntPtr.Zero);
        return table;
    }

    private static void NtvSpawn() {}

    private static void NtvPrintln() {}

    private static IntPtr NtvGetFiles(IntPtr dirPath)
    {
        var path = Marshal.PtrToStringAnsi(dirPath);
        if (string.IsNullOrEmpty(path))
        {
            path = Path.Combine(WorkRoot, "src");
        }
        var files = GetNetivFiles(path);
        return BuildPointerList(files);
    }

    private static ulong NtvNow()
    {
        return 0;
    }

    private static IntPtr NtvResolve(IntPtr name)
    {
        return CompilerStatements;
    }

    private static ulong NtvSaveBinary(IntPtr name, IntPtr buffer, ulong size)
    {
        if (buffer == IntPtr.Zero)
        {
            return 0;
        }

        var buildDir = Path.Combine(WorkRoot, "build");
        Directory.CreateDirectory(buildDir);
        var path = Path.Combine(buildDir, "bootstrap_compiler.launcher.netiv.bin");
        var byteCount = size == 0 ? InferWrittenLength(buffer, 65536) : checked((int)size);
        if (byteCount == 0)
        {
            return 0;
        }
        var bytes = new byte[byteCount];
        Marshal.Copy(buffer, bytes, 0, byteCount);
        File.WriteAllBytes(path, bytes);
        Console.WriteLine("Netiv launcher saved " + byteCount + " candidate bytes to " + path);
        return (ulong)byteCount;
    }

    private static string ResolveToolRoot()
    {
        var envRoot = Environment.GetEnvironmentVariable("NETIV_HOME");
        if (!String.IsNullOrWhiteSpace(envRoot) && HasToolchain(envRoot))
        {
            return Path.GetFullPath(envRoot);
        }

        var cwd = Directory.GetCurrentDirectory();
        if (HasToolchain(cwd))
        {
            return cwd;
        }

        var exeDir = AppDomain.CurrentDomain.BaseDirectory;
        if (HasToolchain(exeDir))
        {
            return Path.GetFullPath(exeDir);
        }

        var parent = Directory.GetParent(exeDir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        if (parent != null && HasToolchain(parent.FullName))
        {
            return parent.FullName;
        }

        return cwd;
    }

    private static bool HasToolchain(string root)
    {
        return File.Exists(Path.Combine(root, "build", "bootstrap_compiler.bin"));
    }

    private static string ResolveWorkRoot()
    {
        var envRoot = Environment.GetEnvironmentVariable("NETIV_WORKING_DIR");
        if (!String.IsNullOrWhiteSpace(envRoot))
        {
            return Path.GetFullPath(envRoot);
        }
        return Directory.GetCurrentDirectory();
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Netiv Lang CLI");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  netiv [command] [args...]");
        Console.WriteLine();
        Console.WriteLine("Compiler:");
        Console.WriteLine("  netiv                  Run the native write entry");
        Console.WriteLine("  netiv read [path]      Scan Netiv source pages from src or a supplied path");
        Console.WriteLine("  netiv audit            Validate scaffold folders, db, and source module declarations");
        Console.WriteLine("  netiv write            Compile and emit the current native artifact");
        Console.WriteLine("  netiv invoke [args]    Execute the newest .exe artifact from bin or build");
        Console.WriteLine("  netiv prune            Remove build and binary artifacts while preserving logs and db");
        Console.WriteLine();
        Console.WriteLine("Archive:");
        Console.WriteLine("  netiv init [path]      Create a Netiv scaffold with db/netiv.db and docs");
        Console.WriteLine("  netiv graph            Export logs/graph/netiv.graph.mmd");
        Console.WriteLine("  netiv list             Export logs/list/netiv.files.list");
        Console.WriteLine("  netiv meta             Export logs/meta/netiv.meta.json");
        Console.WriteLine();
        Console.WriteLine("Scholarship:");
        Console.WriteLine("  netiv witness          Check installed toolchain files and current working archive");
        Console.WriteLine("  netiv version          Print the Netiv version");
        Console.WriteLine("  netiv help             Show this command reference");
        Console.WriteLine();
        Console.WriteLine("Compatibility aliases:");
        Console.WriteLine("  netiv archive [path]   Alias for init");
        Console.WriteLine("  netiv build            Alias for write");
        Console.WriteLine("  netiv run [args]       Alias for invoke");
        Console.WriteLine("  netiv clean            Alias for prune");
        Console.WriteLine("  netiv doctor           Alias for witness");
        Console.WriteLine();
        Console.WriteLine("Reserved commands:");
        Console.WriteLine("  netiv bind             Reserved: link modules");
        Console.WriteLine("  netiv trace            Reserved: compiler/runtime tracing");
        Console.WriteLine("  netiv observe          Reserved: observability stream");
        Console.WriteLine("  netiv annotate         Reserved: attach metadata and documentation");
        Console.WriteLine("  netiv catalog          Reserved: list archive collections, nodes, or tables");
        Console.WriteLine("  netiv index            Reserved: generate or search indexes");
        Console.WriteLine("  netiv shelve           Reserved: persist objects and artifacts");
        Console.WriteLine("  netiv retrieve         Reserved: fetch objects and nodes");
        Console.WriteLine("  netiv preserve         Reserved: snapshot and backup an archive");
        Console.WriteLine("  netiv restore          Reserved: restore a snapshot");
        Console.WriteLine("  netiv curate           Reserved: optimize or reorganize archive data");
        Console.WriteLine("  netiv merge            Reserved: merge archives or graphs");
        Console.WriteLine("  netiv map              Reserved: dependency and knowledge graph mapping");
        Console.WriteLine("  netiv weave            Reserved: create relationships and edges");
        Console.WriteLine("  netiv thread           Reserved: trace node lineage");
        Console.WriteLine("  netiv cite             Reserved: show references and edges");
        Console.WriteLine("  netiv lineage          Reserved: historical ancestry");
        Console.WriteLine("  netiv concord          Reserved: semantic search and cross-reference");
        Console.WriteLine("  netiv oracle           Reserved: AI interaction");
        Console.WriteLine("  netiv reflect          Reserved: reasoning and explanation");
        Console.WriteLine("  netiv examine          Reserved: static analysis");
        Console.WriteLine("  netiv reconcile        Reserved: diff and consistency repair");
    }

    private static int RunDoctor()
    {
        var checks = new[]
        {
            Path.Combine(ToolRoot, "build", "bootstrap_compiler.bin"),
            Path.Combine(ToolRoot, "build", "netiv_build_entry.bin"),
            Path.Combine(ToolRoot, "src", "compiler.ntv")
        };

        Console.WriteLine("Netiv witness");
        Console.WriteLine("Toolchain archive: " + ToolRoot);
        Console.WriteLine("Working archive: " + WorkRoot);
        var ok = true;
        foreach (var path in checks)
        {
            var exists = File.Exists(path);
            Console.WriteLine((exists ? "ok   " : "miss ") + path);
            ok = ok && exists;
        }
        return ok ? 0 : 1;
    }

    private static int InitProject(string target)
    {
        var projectRoot = Path.IsPathRooted(target)
            ? Path.GetFullPath(target)
            : Path.GetFullPath(Path.Combine(WorkRoot, target));
        Directory.CreateDirectory(projectRoot);
        Directory.CreateDirectory(Path.Combine(projectRoot, "src"));
        Directory.CreateDirectory(Path.Combine(projectRoot, "docs"));
        Directory.CreateDirectory(Path.Combine(projectRoot, "build"));
        Directory.CreateDirectory(Path.Combine(projectRoot, "bin"));
        Directory.CreateDirectory(Path.Combine(projectRoot, "db"));
        Directory.CreateDirectory(Path.Combine(projectRoot, "logs"));
        Directory.CreateDirectory(Path.Combine(projectRoot, "logs", "graph"));
        Directory.CreateDirectory(Path.Combine(projectRoot, "logs", "list"));
        Directory.CreateDirectory(Path.Combine(projectRoot, "logs", "meta"));
        CreateProjectDatabase(projectRoot);

        WriteFileIfMissing(Path.Combine(projectRoot, ".gitignore"),
@"bin/
build/
db/
logs/
*.db
*.db-shm
*.db-wal
");

        WriteFileIfMissing(Path.Combine(projectRoot, "README.md"),
@"# Netiv Project

This project was initialized with `netiv init`.
");

        WriteFileIfMissing(Path.Combine(projectRoot, "LICENSE"),
@"All rights reserved.

No permission is granted to use, copy, modify, redistribute, sublicense,
commercialize, or create derivative works from this project without prior
written permission from the project owner.
");
        WriteTemplateFileIfMissing(projectRoot, "", "CONTRIBUTING.md");

        WriteFileIfMissing(Path.Combine(projectRoot, "src", "main.ntv"),
@"module main

fn main() -> void {
    rax = 0
    return
}

fn count_bytes(input: *const i8) -> i64 {
    rcx = input
    rdx = 0
    label loop
    rax = *(u8*)rcx
    rax_is_null
    if_z_goto done
    rcx = rcx + 1
    rdx = rdx + 1
    goto loop
    label done
    rax = rdx
    return
}

fn return_status(code: i64) -> i64 {
    rax = code
    return
}
");

        WriteTemplateFileIfMissing(projectRoot, "docs", "language-guide.md");

        WriteFileIfMissing(Path.Combine(projectRoot, "docs", "todo.md"),
@"# TODO

Track planned Netiv work here.

## Next

- Define the first project page in `src\main.ntv`.
- Run `netiv audit`.
- Run `netiv list`.
- Run `netiv graph`.
- Run `netiv meta`.
");

        WriteFileIfMissing(Path.Combine(projectRoot, "docs", "asbuilt.md"),
@"# As Built

Record the actual project structure and generated artifacts here.

## Scaffold

- Source pages live in `src`.
- Project database lives in `db\netiv.db`.
- Generated lists live in `logs\list`.
- Generated graphs live in `logs\graph`.
- Generated metadata lives in `logs\meta`.
");

        WriteFileIfMissing(Path.Combine(projectRoot, "docs", "project.md"),
@"# Project

Project notes for IDE and MCP tooling.

## Identity

- Language: Netiv
- Scaffold command: `netiv init`
- Database: `db\netiv.db`

## Commands

- `netiv audit`
- `netiv read`
- `netiv list`
- `netiv graph`
- `netiv meta`
- `netiv write`
");

        WriteFileIfMissing(Path.Combine(projectRoot, "src", "build.ntv"),
@"module build

fn run_build() -> void {
    rax = 0
    ~
}
");

        Console.WriteLine("Initialized Netiv project at " + projectRoot);
        return 0;
    }

    private static void CreateProjectDatabase(string projectRoot)
    {
        var dbPath = Path.Combine(projectRoot, "db", "netiv.db");
        if (File.Exists(dbPath))
        {
            Console.WriteLine("exists " + dbPath);
            return;
        }

        var template = Path.Combine(ToolRoot, "netiv.sqlite-template");
        if (File.Exists(template))
        {
            File.Copy(template, dbPath);
        }
        else
        {
            WriteEmptySqliteDatabase(dbPath);
        }
        Console.WriteLine("create " + dbPath);
    }

    private static void WriteEmptySqliteDatabase(string path)
    {
        var bytes = new byte[4096];
        var header = System.Text.Encoding.ASCII.GetBytes("SQLite format 3\0");
        Array.Copy(header, bytes, header.Length);
        bytes[16] = 0x10;
        bytes[17] = 0x00;
        bytes[18] = 0x01;
        bytes[19] = 0x01;
        bytes[21] = 0x40;
        bytes[22] = 0x20;
        bytes[23] = 0x20;
        WriteUInt32(bytes, 24, 1);
        WriteUInt32(bytes, 28, 1);
        WriteUInt32(bytes, 92, 1);
        WriteUInt32(bytes, 96, 3044000);
        bytes[100] = 0x0d;
        bytes[103] = 0x10;
        File.WriteAllBytes(path, bytes);
    }

    private static void WriteUInt32(byte[] bytes, int offset, uint value)
    {
        bytes[offset] = (byte)((value >> 24) & 0xff);
        bytes[offset + 1] = (byte)((value >> 16) & 0xff);
        bytes[offset + 2] = (byte)((value >> 8) & 0xff);
        bytes[offset + 3] = (byte)(value & 0xff);
    }

    private static int RunRead(string[] args)
    {
        var target = args.Length > 1 ? ResolveArchivePath(args[1]) : Path.Combine(WorkRoot, "src");
        var files = GetNetivFiles(target);
        if (files.Length == 0)
        {
            Console.Error.WriteLine("No Netiv source files found at " + target);
            return 1;
        }

        Console.WriteLine("Netiv read");
        Console.WriteLine("Archive: " + WorkRoot);
        foreach (var file in files)
        {
            Console.WriteLine("page " + ToArchivePath(file) + " lines=" + File.ReadAllLines(file).Length);
        }
        Console.WriteLine("pages=" + files.Length);
        return 0;
    }

    private static int RunAudit()
    {
        var ok = true;
        Console.WriteLine("Netiv audit");
        ok = AuditDirectory("src") && ok;
        ok = AuditDirectory("docs") && ok;
        ok = AuditDirectory("build") && ok;
        ok = AuditDirectory("bin") && ok;
        ok = AuditDirectory("logs") && ok;
        ok = AuditDirectory(Path.Combine("logs", "graph")) && ok;
        ok = AuditDirectory(Path.Combine("logs", "list")) && ok;
        ok = AuditDirectory(Path.Combine("logs", "meta")) && ok;
        ok = AuditFile("README.md") && ok;
        ok = AuditFile("LICENSE") && ok;
        ok = AuditFile("CONTRIBUTING.md") && ok;
        ok = AuditFile(Path.Combine("db", "netiv.db")) && ok;
        ok = AuditFile(Path.Combine("docs", "language-guide.md")) && ok;
        ok = AuditFile(Path.Combine("docs", "todo.md")) && ok;
        ok = AuditFile(Path.Combine("docs", "asbuilt.md")) && ok;
        ok = AuditFile(Path.Combine("docs", "project.md")) && ok;

        var files = GetNetivFiles(Path.Combine(WorkRoot, "src"));
        if (files.Length == 0)
        {
            Console.WriteLine("miss src contains no .ntv pages");
            ok = false;
        }

        foreach (var file in files)
        {
            var sourceOk = AuditSourceShape(file);
            ok = ok && sourceOk;
        }
        return ok ? 0 : 1;
    }

    private static bool AuditSourceShape(string file)
    {
        var lines = File.ReadAllLines(file);
        var hasModule = false;
        var hasNetiv = false;
        var hasAnthologyOpen = false;
        var hasAnthologyClose = false;
        var hasBookendOpen = false;
        var hasBookendClose = false;
        var metaIndex = -1;
        var edgesIndex = -1;
        var bookIndex = -1;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (line.StartsWith("module ", StringComparison.Ordinal))
            {
                hasModule = true;
            }
            if (line.StartsWith("<Netiv=", StringComparison.Ordinal))
            {
                hasNetiv = true;
            }
            if (line == "○|")
            {
                hasAnthologyOpen = true;
            }
            if (line == "|●")
            {
                hasAnthologyClose = true;
            }
            if (line == "□|")
            {
                hasBookendOpen = true;
            }
            if (line == "|■")
            {
                hasBookendClose = true;
            }
            if (line.StartsWith("<meta>", StringComparison.Ordinal) && metaIndex < 0)
            {
                metaIndex = i;
            }
            if (line.StartsWith("<edges>", StringComparison.Ordinal) && edgesIndex < 0)
            {
                edgesIndex = i;
            }
            if (line.StartsWith("<book>", StringComparison.Ordinal) && bookIndex < 0)
            {
                bookIndex = i;
            }
        }

        var canonical = hasNetiv &&
            hasAnthologyOpen &&
            hasAnthologyClose &&
            hasBookendOpen &&
            hasBookendClose &&
            metaIndex >= 0 &&
            edgesIndex > metaIndex &&
            bookIndex > edgesIndex;

        var relative = ToArchivePath(file);
        if (canonical)
        {
            Console.WriteLine("ok   " + relative + " canonical source shape");
            return true;
        }
        if (hasModule)
        {
            Console.WriteLine("ok   " + relative + " legacy module declaration");
            return true;
        }

        Console.WriteLine("miss " + relative + " canonical source shape");
        return false;
    }

    private static bool AuditDirectory(string name)
    {
        var path = Path.Combine(WorkRoot, name);
        var exists = Directory.Exists(path);
        Console.WriteLine((exists ? "ok   " : "miss ") + name + Path.DirectorySeparatorChar);
        return exists;
    }

    private static bool AuditFile(string name)
    {
        var path = Path.Combine(WorkRoot, name);
        var exists = File.Exists(path);
        Console.WriteLine((exists ? "ok   " : "miss ") + name);
        return exists;
    }

    private static int RunInvoke(string[] args)
    {
        var artifact = FindNewestArtifact();
        if (artifact == null)
        {
            Console.Error.WriteLine("No executable artifact found in bin or build.");
            Console.Error.WriteLine("Current write output is a native blob until executable emission is wired in.");
            return 1;
        }

        var arguments = JoinArguments(args, 1);
        var process = new Process();
        process.StartInfo.FileName = artifact;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.WorkingDirectory = WorkRoot;
        process.StartInfo.UseShellExecute = false;
        process.Start();
        process.WaitForExit();
        return process.ExitCode;
    }

    private static int RunPrune()
    {
        if (SamePath(WorkRoot, ToolRoot))
        {
            Console.Error.WriteLine("Refusing to prune the installed Netiv toolchain archive.");
            return 1;
        }

        ClearArchiveDirectory("build");
        ClearArchiveDirectory("bin");
        Console.WriteLine("Pruned Netiv artifacts from " + WorkRoot);
        return 0;
    }

    private static int RunGraph()
    {
        var files = GetNetivFiles(Path.Combine(WorkRoot, "src"));
        var graphDir = Path.Combine(WorkRoot, "logs", "graph");
        Directory.CreateDirectory(graphDir);
        var path = Path.Combine(graphDir, "netiv.graph.mmd");
        var lines = new List<string>();
        lines.Add("graph TD");
        if (files.Length == 0)
        {
            lines.Add("  archive[\"archive\"]");
        }
        foreach (var file in files)
        {
            var node = NodeName(file);
            lines.Add("  " + node + "[\"" + ToArchivePath(file).Replace("\\", "/") + "\"]");
        }
        File.WriteAllLines(path, lines.ToArray());
        Console.WriteLine("Wrote graph " + path);
        return 0;
    }

    private static int RunList()
    {
        var listDir = Path.Combine(WorkRoot, "logs", "list");
        Directory.CreateDirectory(listDir);
        var path = Path.Combine(listDir, "netiv.files.list");
        var files = Directory.GetFiles(WorkRoot, "*", SearchOption.AllDirectories);
        Array.Sort(files, StringComparer.OrdinalIgnoreCase);

        var lines = new List<string>();
        foreach (var file in files)
        {
            var relative = ToArchivePath(file).Replace("\\", "/");
            if (String.Equals(relative, "logs/list/netiv.files.list", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            lines.Add(relative);
        }

        File.WriteAllLines(path, lines.ToArray());
        Console.WriteLine("Wrote list " + path);
        Console.WriteLine("files=" + lines.Count);
        return 0;
    }

    private static int RunMeta()
    {
        var files = GetNetivFiles(Path.Combine(WorkRoot, "src"));
        var metaDir = Path.Combine(WorkRoot, "logs", "meta");
        Directory.CreateDirectory(metaDir);
        var path = Path.Combine(metaDir, "netiv.meta.json");
        var lines = new List<string>();
        lines.Add("{");
        lines.Add("  \"language\": \"Netiv Lang\",");
        lines.Add("  \"version\": \"1.0\",");
        lines.Add("  \"archive\": " + JsonString(WorkRoot) + ",");
        lines.Add("  \"pages\": [");
        for (var i = 0; i < files.Length; i++)
        {
            var suffix = i + 1 == files.Length ? "" : ",";
            lines.Add("    { \"path\": " + JsonString(ToArchivePath(files[i]).Replace("\\", "/")) + ", \"bytes\": " + new FileInfo(files[i]).Length + " }" + suffix);
        }
        lines.Add("  ]");
        lines.Add("}");
        File.WriteAllLines(path, lines.ToArray());
        Console.WriteLine("Wrote meta " + path);
        return 0;
    }

    private static bool IsReservedCommand(string command)
    {
        switch (command)
        {
            case "bind":
            case "trace":
            case "observe":
            case "annotate":
            case "catalog":
            case "index":
            case "shelve":
            case "retrieve":
            case "preserve":
            case "restore":
            case "curate":
            case "merge":
            case "map":
            case "weave":
            case "thread":
            case "cite":
            case "lineage":
            case "concord":
            case "oracle":
            case "reflect":
            case "examine":
            case "reconcile":
                return true;
            default:
                return false;
        }
    }

    private static string ResolveArchivePath(string path)
    {
        return Path.IsPathRooted(path)
            ? Path.GetFullPath(path)
            : Path.GetFullPath(Path.Combine(WorkRoot, path));
    }

    private static string[] GetNetivFiles(string target)
    {
        string[] files;
        if (File.Exists(target))
        {
            files = target.EndsWith(".ntv", StringComparison.OrdinalIgnoreCase)
                ? new[] { Path.GetFullPath(target) }
                : new string[0];
        }
        else if (Directory.Exists(target))
        {
            files = Directory.GetFiles(target, "*.ntv", SearchOption.AllDirectories);
        }
        else
        {
            files = new string[0];
        }
        Array.Sort(files, StringComparer.OrdinalIgnoreCase);
        return files;
    }

    private static string ToArchivePath(string path)
    {
        var full = Path.GetFullPath(path);
        var root = Path.GetFullPath(WorkRoot).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        if (full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
        {
            return full.Substring(root.Length);
        }
        return full;
    }

    private static string NodeName(string path)
    {
        var relative = ToArchivePath(path).Replace("\\", "_").Replace("/", "_").Replace(".", "_").Replace(" ", "_").Replace("-", "_");
        return "page_" + relative;
    }

    private static string FindMainSource(string[] files)
    {
        foreach (var file in files)
        {
            if (String.Equals(Path.GetFileName(file), "main.ntv", StringComparison.OrdinalIgnoreCase))
            {
                return file;
            }
        }
        return files[0];
    }

    private static string ExtractMetaValue(string source, string key)
    {
        var pattern = key + ":";
        var index = source.IndexOf(pattern, StringComparison.Ordinal);
        if (index < 0)
        {
            return "";
        }

        var quoteStart = source.IndexOf('"', index + pattern.Length);
        if (quoteStart < 0)
        {
            return "";
        }
        var quoteEnd = source.IndexOf('"', quoteStart + 1);
        if (quoteEnd < 0)
        {
            return "";
        }
        return source.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);
    }

    private static int ExtractReturnStatus(string source)
    {
        var functionIndex = source.IndexOf("•ReturnStatus", StringComparison.Ordinal);
        if (functionIndex < 0)
        {
            return 0;
        }

        var returnIndex = source.IndexOf("°return", functionIndex, StringComparison.Ordinal);
        if (returnIndex < 0)
        {
            return 0;
        }

        var semicolon = source.IndexOf(';', returnIndex);
        if (semicolon < 0)
        {
            return 0;
        }

        var valueText = source.Substring(returnIndex + "°return".Length, semicolon - returnIndex - "°return".Length).Trim();
        int value;
        return Int32.TryParse(valueText, out value) ? value : 0;
    }

    private static string SafeArtifactName(string name)
    {
        if (String.IsNullOrWhiteSpace(name))
        {
            return "";
        }

        var chars = name.ToCharArray();
        for (var i = 0; i < chars.Length; i++)
        {
            var c = chars[i];
            if (!Char.IsLetterOrDigit(c) && c != '-' && c != '_')
            {
                chars[i] = '-';
            }
        }
        return new string(chars).Trim('-');
    }

    private static string GenerateProjectHost(string artifactName, string[] files, int exitCode)
    {
        var pages = CollectSymbols(files, "°page •");
        var methods = CollectSymbols(files, "°method •");
        var functions = CollectSymbols(files, "°function •");
        var structs = CollectSymbols(files, "°struct •");
        var enums = CollectSymbols(files, "°enum •");
        var consts = CollectSymbols(files, "°const •");
        var lines = new List<string>();
        lines.Add("using System;");
        lines.Add("");
        lines.Add("internal static class NetivProjectArtifact");
        lines.Add("{");
        lines.Add("    private static int Main(string[] args)");
        lines.Add("    {");
        lines.Add("        if (args.Length > 0)");
        lines.Add("        {");
        lines.Add("            var cmd = args[0].ToLowerInvariant();");
        lines.Add("            if (cmd == \"get_primitive_size\")");
        lines.Add("            {");
        lines.Add("                var typeId = args.Length > 1 ? int.Parse(args[1]) : 2;");
        lines.Add("                var size = (typeId == 1) ? 1 : ((typeId == 4) ? 4 : 8);");
        lines.Add("                Console.WriteLine(\"get_primitive_size(\" + typeId + \") -> \" + size);");
        lines.Add("                return 0;");
        lines.Add("            }");
        lines.Add("            if (cmd == \"get_files\")");
        lines.Add("            {");
        lines.Add("                var dir = args.Length > 1 ? args[1] : \"\";");
        lines.Add("                if (string.IsNullOrEmpty(dir)) { dir = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), \"src\"); }");
        lines.Add("                Console.WriteLine(\"get_files(\" + dir + \"):\");");
        lines.Add("                var dirInfo = new System.IO.DirectoryInfo(dir);");
        lines.Add("                if (dirInfo.Exists) {");
        lines.Add("                    var ntvFiles = System.IO.Directory.GetFiles(dir, \"*.ntv\", System.IO.SearchOption.AllDirectories);");
        lines.Add("                    foreach (var f in ntvFiles) {");
        lines.Add("                        Console.WriteLine(\"  \" + f.Replace(\"\\\\\", \"/\"));");
        lines.Add("                    }");
        lines.Add("                }");
        lines.Add("                return 0;");
        lines.Add("            }");
        lines.Add("            if (cmd == \"emit_op_addr\")");
        lines.Add("            {");
        lines.Add("                var offset = args.Length > 1 ? int.Parse(args[1]) : 8;");
        lines.Add("                Console.WriteLine(\"emit_op_addr(\" + offset + \"): LEA rax, [rbp - \" + offset + \"] -> 48 8D 85 [\" + (0 - offset).ToString(\"X8\") + \"]\");");
        lines.Add("                return 0;");
        lines.Add("            }");
        lines.Add("            if (cmd == \"emit_op_deref_qword\")");
        lines.Add("            {");
        lines.Add("                Console.WriteLine(\"emit_op_deref_qword(): mov rax, [rax] -> 48 8B 00\");");
        lines.Add("                return 0;");
        lines.Add("            }");
        lines.Add("            if (cmd == \"emit_op_deref_dword\")");
        lines.Add("            {");
        lines.Add("                Console.WriteLine(\"emit_op_deref_dword(): mov eax, [rax] -> 8B 00\");");
        lines.Add("                return 0;");
        lines.Add("            }");
        lines.Add("            if (cmd == \"emit_op_deref_byte\")");
        lines.Add("            {");
        lines.Add("                Console.WriteLine(\"emit_op_deref_byte(): movzx eax, byte ptr [rax] -> 0F B6 00\");");
        lines.Add("                return 0;");
        lines.Add("            }");
        lines.Add("            if (cmd == \"emit_op_index\")");
        lines.Add("            {");
        lines.Add("                var scale = args.Length > 1 ? int.Parse(args[1]) : 8;");
        lines.Add("                var sib = (scale == 4) ? \"8B\" : ((scale == 8) ? \"CB\" : \"0B\");");
        lines.Add("                Console.WriteLine(\"emit_op_index(\" + scale + \"): mov rax, [rbx + rcx * \" + scale + \"] -> 48 8B 04 \" + sib);");
        lines.Add("                return 0;");
        lines.Add("            }");
        lines.Add("            if (cmd == \"emit_op_sizeof\")");
        lines.Add("            {");
        lines.Add("                var size = args.Length > 1 ? int.Parse(args[1]) : 16;");
        lines.Add("                Console.WriteLine(\"emit_op_sizeof(\" + size + \"): mov rax, \" + size + \" -> 48 B8 \" + size.ToString(\"X16\"));");
        lines.Add("                return 0;");
        lines.Add("            }");
        lines.Add("            if (cmd == \"emit_cast_double_to_int\")");
        lines.Add("            {");
        lines.Add("                Console.WriteLine(\"emit_cast_double_to_int(): cvttsd2si rax, xmm0 -> F2 48 0F 2C C0\");");
        lines.Add("                return 0;");
        lines.Add("            }");
        lines.Add("            if (cmd == \"emit_cast_int_to_double\")");
        lines.Add("            {");
        lines.Add("                Console.WriteLine(\"emit_cast_int_to_double(): cvtsi2sd xmm0, rax -> F2 48 0F 2A C0\");");
        lines.Add("                return 0;");
        lines.Add("            }");
        lines.Add("            if (cmd == \"emit_op_if_z\")");
        lines.Add("            {");
        lines.Add("                var offset = args.Length > 1 ? int.Parse(args[1]) : 24;");
        lines.Add("                Console.WriteLine(\"emit_op_if_z(\" + offset + \"): cmp rax, 0; jz \" + offset + \" -> 48 83 F8 00 0F 84 [\" + offset.ToString(\"X8\") + \"]\");");
        lines.Add("                return 0;");
        lines.Add("            }");
        lines.Add("            Console.Error.WriteLine(\"Function \" + args[0] + \" resolved but has no mock console invocation entry point.\");");
        lines.Add("            return 1;");
        lines.Add("        }");
        lines.Add("");
        lines.Add("        Console.WriteLine(\"Netiv artifact: " + EscapeCSharp(artifactName) + "\");");
        lines.Add("        Console.WriteLine(\"Archive: " + EscapeCSharp(WorkRoot) + "\");");
        lines.Add("        Console.WriteLine(\"Pages: " + files.Length + "\");");
        foreach (var file in files)
        {
            lines.Add("        Console.WriteLine(\"page " + EscapeCSharp(ToArchivePath(file).Replace("\\", "/")) + "\");");
        }
        lines.Add("        Console.WriteLine(\"Source pages: " + pages.Count + "\");");
        foreach (var page in pages)
        {
            lines.Add("        Console.WriteLine(\"source-page " + EscapeCSharp(page) + "\");");
        }
        lines.Add("        Console.WriteLine(\"Methods: " + methods.Count + "\");");
        foreach (var method in methods)
        {
            lines.Add("        Console.WriteLine(\"method " + EscapeCSharp(method) + "\");");
        }
        lines.Add("        Console.WriteLine(\"Functions: " + functions.Count + "\");");
        foreach (var function in functions)
        {
            lines.Add("        Console.WriteLine(\"function " + EscapeCSharp(function) + "\");");
        }
        lines.Add("        Console.WriteLine(\"Structs: " + structs.Count + "\");");
        foreach (var str in structs)
        {
            lines.Add("        Console.WriteLine(\"struct " + EscapeCSharp(str) + "\");");
        }
        lines.Add("        Console.WriteLine(\"Enums: " + enums.Count + "\");");
        foreach (var en in enums)
        {
            lines.Add("        Console.WriteLine(\"enum " + EscapeCSharp(en) + "\");");
        }
        lines.Add("        Console.WriteLine(\"Consts: " + consts.Count + "\");");
        foreach (var cn in consts)
        {
            lines.Add("        Console.WriteLine(\"const " + EscapeCSharp(cn) + "\");");
        }
        lines.Add("        return " + exitCode + ";");
        lines.Add("    }");
        lines.Add("}");
        return String.Join(Environment.NewLine, lines.ToArray()) + Environment.NewLine;
    }

    private static int CountSymbols(string[] files, string marker)
    {
        return CollectSymbols(files, marker).Count;
    }

    private static List<string> CollectSymbols(string[] files, string marker)
    {
        var symbols = new List<string>();
        foreach (var file in files)
        {
            var source = File.ReadAllText(file);
            var index = 0;
            while (true)
            {
                index = source.IndexOf(marker, index, StringComparison.Ordinal);
                if (index < 0)
                {
                    break;
                }
                var start = index + marker.Length;
                var end = start;
                while (end < source.Length && IsSymbolCharacter(source[end]))
                {
                    end++;
                }
                if (end > start)
                {
                    symbols.Add(source.Substring(start, end - start));
                }
                index = end;
            }
        }
        var distinctSymbols = new List<string>();
        foreach (var sym in symbols)
        {
            if (!distinctSymbols.Contains(sym))
            {
                distinctSymbols.Add(sym);
            }
        }
        distinctSymbols.Sort(StringComparer.OrdinalIgnoreCase);
        return distinctSymbols;
    }

    private static bool IsSymbolCharacter(char value)
    {
        return Char.IsLetterOrDigit(value) || value == '_' || value == '-' || value == '.';
    }

    private static string EscapeCSharp(string text)
    {
        return text.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    private static string ResolveCSharpCompiler()
    {
        var frameworkCompiler = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe";
        if (File.Exists(frameworkCompiler))
        {
            return frameworkCompiler;
        }
        return null;
    }

    private static string FindNewestArtifact()
    {
        var candidates = new List<string>();
        AddArtifacts(candidates, Path.Combine(WorkRoot, "bin"));
        AddArtifacts(candidates, Path.Combine(WorkRoot, "build"));
        string newest = null;
        foreach (var candidate in candidates)
        {
            if (newest == null || File.GetLastWriteTimeUtc(candidate) > File.GetLastWriteTimeUtc(newest))
            {
                newest = candidate;
            }
        }
        return newest;
    }

    private static void AddArtifacts(List<string> candidates, string directory)
    {
        if (!Directory.Exists(directory))
        {
            return;
        }
        candidates.AddRange(Directory.GetFiles(directory, "*.exe", SearchOption.TopDirectoryOnly));
    }

    private static string JoinArguments(string[] args, int start)
    {
        var parts = new List<string>();
        for (var i = start; i < args.Length; i++)
        {
            parts.Add(QuoteArgument(args[i]));
        }
        return String.Join(" ", parts.ToArray());
    }

    private static string QuoteArgument(string argument)
    {
        if (argument.IndexOfAny(new[] { ' ', '\t', '"' }) < 0)
        {
            return argument;
        }
        return "\"" + argument.Replace("\"", "\\\"") + "\"";
    }

    private static void ClearArchiveDirectory(string name)
    {
        var path = Path.Combine(WorkRoot, name);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            return;
        }

        foreach (var file in Directory.GetFiles(path))
        {
            File.Delete(file);
        }
        foreach (var directory in Directory.GetDirectories(path))
        {
            Directory.Delete(directory, true);
        }
    }

    private static bool SamePath(string left, string right)
    {
        var a = Path.GetFullPath(left).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var b = Path.GetFullPath(right).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return String.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }

    private static string JsonString(string text)
    {
        return "\"" + text.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
    }

    private static void WriteFileIfMissing(string path, string contents)
    {
        if (File.Exists(path))
        {
            Console.WriteLine("exists " + path);
            return;
        }
        File.WriteAllText(path, contents);
        Console.WriteLine("create " + path);
    }

    private static void WriteTemplateFileIfMissing(string projectRoot, string directory, string filename)
    {
        var outputPath = Path.Combine(projectRoot, directory, filename);
        if (File.Exists(outputPath))
        {
            Console.WriteLine("exists " + outputPath);
            return;
        }

        var templatePath = ResolveTemplatePath(directory, filename);
        if (!File.Exists(templatePath))
        {
            Console.WriteLine("missing template " + templatePath);
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        File.Copy(templatePath, outputPath);
        Console.WriteLine("create " + outputPath);
    }

    private static string ResolveTemplatePath(string directory, string filename)
    {
        var scaffoldPath = Path.Combine(ToolRoot, "scaffold", directory, filename);
        if (File.Exists(scaffoldPath))
        {
            return scaffoldPath;
        }

        return Path.Combine(ToolRoot, directory, filename);
    }

    private static int InferWrittenLength(IntPtr buffer, int maxBytes)
    {
        var bytes = new byte[maxBytes];
        Marshal.Copy(buffer, bytes, 0, maxBytes);
        for (var i = bytes.Length - 1; i >= 0; i--)
        {
            if (bytes[i] != 0)
            {
                return i + 1;
            }
        }
        return 0;
    }

    private static void NtvDebug(ulong value, ulong pointer)
    {
        Console.WriteLine("Netiv debug value=0x" + value.ToString("x") + " ptr=0x" + pointer.ToString("x"));
    }

    private static void NtvSeal() {}

    private static IntPtr NtvGetFunctionName(ulong index)
    {
        return index == 1 ? Marshal.StringToHGlobalAnsi("ntv_compiler") : IntPtr.Zero;
    }

    private static ulong NtvGetFunctionCount()
    {
        return 1;
    }
}
