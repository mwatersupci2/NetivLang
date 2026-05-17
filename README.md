# Netiv Language (NetivLang)

> ⚠️ **Warning:** NetivLang is proprietary, experimental software. Public visibility does not grant permission to use, copy, fork, modify, redistribute, or create derivative works. No warranty is provided. Use or reliance on this code is entirely at your own risk and only with express written permission from the author.

Welcome to the **Netiv Lang** repository! This is the official home of the self-hosted, high-performance system programming language and native compiler toolchain.

Netiv is designed for developers who need close-to-metal control, high-efficiency code generation, and direct native execution with zero host dependencies.

---

## 🚀 Key Features

- **Self-Hosted Compiler**: Built and compiled natively using Netiv itself.
- **Native PE Generation**: Compiles direct, host-less Windows Portable Executable (`.exe`) binaries.
- **Low-Level Native Control**: Direct register manipulation (`rax`, `rdx`, etc.), syscall bindings, and high-performance instruction emitting.
- **Modern Syntax & Primitives**: Combines modern syntax structures with raw assembler flexibility.
- **SQLite Database Driven Metadata**: Integrates compiler database tracking for efficient build caching and incremental compilation.

---

## 📂 Repository Structure

```
├── src/                      # Active source code of the Netiv compiler
│   ├── account.ntv           # Account module
│   ├── build.ntv             # Compiler build/driver routines
│   ├── cli.ntv               # CLI driver interface
│   ├── compiler.ntv          # Core compiler implementation
│   ├── driver.ntv            # Compilation driver
│   ├── lexer.ntv             # Lexer / tokenizer
│   ├── parser.ntv            # Parser
│   ├── parser_lib.ntv        # Parser helper libraries
│   ├── low_level_emitter.ntv # Low-level code emitter (PE generator)
│   └── generated/            # Generated compiler sources and test files
├── .gitignore                # Files excluded from git tracking
└── README.md                 # Project documentation (this file)
```

---

## 🛠 Getting Started

### Prerequisites

- A Windows environment (64-bit).
- Netiv native bootstrap tools.

### Installing the CLI on Windows

Netiv can be installed as a local Windows command by placing the toolchain files
under a folder already on PATH, such as `C:\Tools\Netiv`.

From the repository root:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\install-netiv.ps1
```

The installed layout is:

```text
C:\Tools\Netiv\netiv.cmd
C:\Tools\Netiv\netiv-host.exe
C:\Tools\Netiv\build\bootstrap_compiler.bin
C:\Tools\Netiv\build\netiv_build_entry.bin
C:\Tools\Netiv\src\compiler.ntv
C:\Tools\Netiv\netiv.sqlite-template
```

After installation, a new PowerShell window can run:

```powershell
netiv help
netiv witness
```

### Initializing a Project

From an empty project folder:

```powershell
netiv init
```

Or create the scaffold at a specific path:

```powershell
netiv init "C:\Netiv Test Project"
```

The scaffold creates `src`, `docs`, `build`, `bin`, `db`, `logs`,
`logs\graph`, `logs\list`, and `logs\meta`, plus project-local `db\netiv.db`,
starter `src\main.ntv`, `src\build.ntv`, `docs\language-guide.md`,
`docs\todo.md`, `docs\asbuilt.md`, `docs\project.md`, `.gitignore`, and
`README.md`, and `LICENSE` files.

Generated path lists and database-list dumps belong under `logs\list`.

### Building the Compiler

The compiler handles its own build cycle using the driver routine inside `src/build.ntv`.

To trigger the native compilation pipeline:
```powershell
netiv write
```

### Core Commands

```powershell
netiv read     # inspect source pages
netiv audit    # validate the project scaffold
netiv write    # compile and emit the artifact
netiv invoke   # execute the newest emitted artifact
netiv prune    # remove build and binary artifacts
netiv graph    # export logs\graph\netiv.graph.mmd
netiv list     # export logs\list\netiv.files.list
netiv meta     # export logs\meta\netiv.meta.json
```
---

## ⚠️ Experimental Software Disclaimer

NetivLang is highly experimental software.

This project is under active research and development. It may contain bugs, incomplete features, unstable behavior, breaking changes, security issues, data-loss risks, incorrect compiler output, or platform-specific failures.

By using, modifying, compiling, distributing, or running this code, you do so entirely at your own risk.

The author makes no guarantees that this software is safe, correct, secure, stable, performant, production-ready, or suitable for any particular purpose. No warranty is provided, express or implied.

The author is not responsible for any damage, loss of data, system failure, security breach, financial loss, business interruption, legal issue, or other harm resulting from the use or misuse of this software.

This project should not be used in production systems, safety-critical systems, financial systems, medical systems, legal systems, military systems, or any environment where failure could cause harm unless you have independently audited, tested, and accepted all risks yourself.

Use at your own risk.

---

## 🤝 Private Collaboration

NetivLang is not currently open source.

This public repository exists so interested engineers, researchers, employers, and technical reviewers can inspect the project, evaluate the architecture, and understand the direction of the language.

I am open to private collaboration with selected contributors.

Contribution access is granted by invitation only. Anyone interested in contributing should contact the author directly. Accepted contributors may be given access to private repositories, private planning materials, internal design notes, or assigned project areas under separate written permission.

Public visibility of this repository does not grant contribution rights, usage rights, redistribution rights, or permission to fork this work into a competing or derivative project.

Do not submit unsolicited large changes, forks, ports, rewrites, or derivative implementations without prior written permission.

---

## 🔒 License & Usage Restrictions

NetivLang is **proprietary, experimental software**.

This repository is publicly visible for portfolio review, professional demonstration, timestamping, recruitment of private collaborators, and technical transparency.

Public visibility does **not** mean open source.

No permission is granted to use, copy, fork, modify, compile, redistribute, sublicense, commercialize, reverse engineer, train models on, or create derivative works from this software without prior express written permission from the author.

All rights are reserved by the author.

Private contribution may be allowed by invitation only. Any contributor access, permissions, rights, ownership terms, confidentiality expectations, or licensing terms must be agreed to separately in writing before contribution.

No license is granted by implication, estoppel, public availability, repository visibility, download access, cloning access, issue participation, pull request submission, discussion participation, or any other means.

This software is experimental and provided without warranty of any kind. The author is not responsible for damage, data loss, system failure, security issues, financial loss, business interruption, legal exposure, or other harm resulting from access to, use of, misuse of, reliance on, or inability to use this software.

For employment review, private collaboration, or licensing inquiries, contact the author directly.
