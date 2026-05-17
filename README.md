# Netiv Language (NetivLang)

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

## 🤝 Contributing

Contributions are welcome! Please feel free to open issues or submit pull requests.

1. Fork the Repository
2. Create a Feature Branch (`git checkout -b feature/amazing-feature`)
3. Commit your Changes (`git commit -m 'Add some amazing feature'`)
4. Push to the Branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.
