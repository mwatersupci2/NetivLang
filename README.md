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

### Building the Compiler

The compiler handles its own build cycle using the driver routine inside `src/build.ntv`.

To trigger the native compilation pipeline:
```bash
# Execute the Netiv native build tool on build.ntv
netiv build.ntv
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

## 🔒 License & Usage Restrictions

NetivLang is **proprietary, experimental software**.

This repository is publicly visible for documentation, timestamping, research transparency, and project demonstration purposes only.

Public visibility does **not** grant permission to use, copy, fork, modify, compile, redistribute, sublicense, commercialize, reverse engineer, or create derivative works from this software.

All rights are reserved by the author unless explicit written permission is granted.

No license is granted by implication, estoppel, public availability, repository visibility, download access, or any other means.

You may not use this software in personal, academic, commercial, production, research, training, benchmarking, or derivative projects without prior written permission from the author.

Unauthorized use, redistribution, or modification is prohibited.

For permission, collaboration, or licensing inquiries, contact the author directly.

