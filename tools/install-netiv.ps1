param(
    [string]$InstallDir = "C:\Tools\Netiv"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$compiler = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

if (-not (Test-Path -LiteralPath $compiler)) {
    throw "C# compiler not found: $compiler"
}

New-Item -ItemType Directory -Force -Path $InstallDir | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $InstallDir "build") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $InstallDir "src") | Out-Null

foreach ($oldGuide in @("LANGUAGE_GUIDE.md", "AI_GUIDE.md", "DISCLAIMER.md")) {
    $oldPath = Join-Path $InstallDir $oldGuide
    if (Test-Path -LiteralPath $oldPath) {
        Remove-Item -LiteralPath $oldPath -Force
    }
}
if (Test-Path -LiteralPath (Join-Path $InstallDir "docs")) {
    Remove-Item -LiteralPath (Join-Path $InstallDir "docs") -Recurse -Force
}

$hostOutput = Join-Path $repoRoot "bin\netiv.exe"
$launcherSource = Join-Path $PSScriptRoot "netiv_launcher.cs"

& $compiler /nologo /platform:x64 /optimize+ /target:exe /out:$hostOutput $launcherSource

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

Copy-Item -LiteralPath (Join-Path $repoRoot "bin\netiv.exe") -Destination (Join-Path $InstallDir "netiv-host.exe") -Force
Copy-Item -LiteralPath (Join-Path $PSScriptRoot "netiv.cmd") -Destination (Join-Path $InstallDir "netiv.cmd") -Force
Copy-Item -LiteralPath (Join-Path $repoRoot "build\bootstrap_compiler.bin") -Destination (Join-Path $InstallDir "build\bootstrap_compiler.bin") -Force
Copy-Item -LiteralPath (Join-Path $repoRoot "build\netiv_build_entry.bin") -Destination (Join-Path $InstallDir "build\netiv_build_entry.bin") -Force
Copy-Item -LiteralPath (Join-Path $repoRoot "src\compiler.ntv") -Destination (Join-Path $InstallDir "src\compiler.ntv") -Force
Copy-Item -LiteralPath (Join-Path $PSScriptRoot "netiv.sqlite-template") -Destination (Join-Path $InstallDir "netiv.sqlite-template") -Force
Copy-Item -LiteralPath (Join-Path $repoRoot "CONTRIBUTING.md") -Destination (Join-Path $InstallDir "CONTRIBUTING.md") -Force
if (Test-Path -LiteralPath (Join-Path $repoRoot "docs")) {
    New-Item -ItemType Directory -Force -Path (Join-Path $InstallDir "docs") | Out-Null
    Copy-Item -Path (Join-Path $repoRoot "docs\*") -Destination (Join-Path $InstallDir "docs") -Recurse -Force
}
if (Test-Path -LiteralPath (Join-Path $PSScriptRoot "scaffold")) {
    New-Item -ItemType Directory -Force -Path (Join-Path $InstallDir "scaffold") | Out-Null
    Copy-Item -Path (Join-Path $PSScriptRoot "scaffold\*") -Destination (Join-Path $InstallDir "scaffold") -Recurse -Force
}

$userPath = [Environment]::GetEnvironmentVariable("Path", "User")
$parts = @()
if ($userPath) {
    $parts = $userPath -split ";" | Where-Object { $_ -and $_.Trim() -ne "" }
}
$hasInstallDir = $parts | Where-Object { $_.TrimEnd("\") -ieq $InstallDir.TrimEnd("\") }
if (-not $hasInstallDir) {
    $parts += $InstallDir
    [Environment]::SetEnvironmentVariable("Path", ($parts -join ";"), "User")
}

Write-Host "Installed Netiv CLI to $InstallDir"
Write-Host "Open a new PowerShell window, then run: netiv witness"
