# NetivLang Language Guide

NetivLang is an experimental programming language and compiler architecture focused on deterministic source structure, graph-aware compilation, database-backed code memory, and AI-auditable development workflows.

This guide serves two readers at once:

1. Humans need to understand the philosophy, syntax, file layout, commands, and examples.
2. AI agents need deterministic rules, canonical formatting, valid and invalid examples, and clear "do not invent syntax" boundaries.

The basic philosophy is:

> AI writes. Humans audit. The database remembers. The compiler trusts deterministic artifacts.

## 1. What NetivLang Is

NetivLang is an experimental language architecture where source code, graph structure, metadata, compilation artifacts, and AI-auditable context are treated as first-class parts of the programming model.

Traditional compilers parse source files, build internal structures, compile them, and then discard much of the useful intermediate relationship data. NetivLang is designed to preserve that relationship data so humans, compilers, databases, and AI agents can inspect it later.

NetivLang is not intended to be a clone of Rust, C, C++, JavaScript, Python, Haskell, Mojo, or any other language. Similarities may exist at the surface, but syntax and semantics must be defined by this guide or by future repo-managed specs.

## 2. Design Goals

NetivLang is designed around the following goals:

1. Deterministic compilation.
2. Human-auditable source structure.
3. AI-safe editing boundaries.
4. Graph-visible code relationships.
5. Database-backed source memory.
6. Inspectable lowered artifacts.
7. Clear separation between methods, functions, pages, metadata, and edges.
8. Stable canonical export.

NetivLang intentionally trades some simplicity and storage efficiency for auditability, graph visibility, and deterministic AI workflows.

## 3. Source File Structure

Canonical source files use a wrapped anthology shape:

```netiv
<Netiv="example.netiv">
○|
  □|
    <meta>{
      name: "example",
      version: "0.1.0"
    };

    <edges>{
      %% Mermaid-style graph information goes here.
    };

    <book>{
      °function •main() {
        °return;
      };
    };
  |■
|●
```

A Netiv source file is wrapped in a `<Netiv="filename">` header.

Inside the file is an anthology block:

- `○|` opens the anthology.
- `|●` closes the anthology.

Inside an anthology are one or more bookend blocks:

- `□|` opens a bookend.
- `|■` closes a bookend.

Each bookend contains three major sections in this order:

1. `<meta>`: structured metadata.
2. `<edges>`: graph and relationship information.
3. `<book>`: executable or declarative language content.

## 4. Anthologies and Bookends

An anthology is the outer collection of one or more bookends.

A bookend is an atomic source unit. It may contain one or more functions, methods, pages, or declarations, but it must preserve the required section order:

1. `<meta>`
2. `<edges>`
3. `<book>`

The compiler may reject files that omit required sections or place them out of order.

Every bookend should be independently inspectable, hashable, and exportable.

## 5. Metadata Section

```netiv
<meta>{
  name: "compile_operation",
  kind: "method",
  error: "terminal",
  version: "0.1.0"
};
```

The `<meta>` section describes the bookend. It may include the unit name, kind, version, error policy, visibility, hashes, monotonic tick metadata, and other compiler-relevant data.

The metadata section is not executable code. It exists so the compiler, database, tooling, and AI agents can identify what the bookend is and how it should be handled.

Recommended minimum fields:

- `name`
- `kind`
- `version`
- `error`

## 6. Edges Section

```netiv
<edges>{
  %% graph TD
  %%   CompileOperation --> ParseSource
  %%   ParseSource --> LowerToAssembly
};
```

The `<edges>` section records graph relationships.

Edges may represent:

- file-to-file relationships
- function-to-function calls
- method-to-function orchestration
- method-to-method relationships
- page-to-component relationships
- compile-stage dependencies
- database node relationships

The edges section is also where comments are permitted.

Comments are allowed in `<edges>` and `<meta>` only. Comments are not allowed inside `<book>` unless the language later defines a formal comment syntax for executable content.

## 7. Book Section

```netiv
<book>{
  °function •main() {
    °return;
  };
};
```

The `<book>` section contains NetivLang code.

This is where functions, methods, pages, declarations, calls, returns, conditionals, error handling, and observable operations are written.

Every executable or processable unit must end with `;`.

## 8. Reserved Words

The following is the official **NetivLang 1.01 Master Reserved / Protected Symbol List** specifying lexical sigils, structural tags, core language keywords, registers, operators, protected namespaces, and protected API symbols:

### 8.0. Lexical Sigils
`°` marks compiler/language keywords.
`•` marks user/API identifiers.

### 8.1. Structural Tags & Boundaries
`<Netiv>`
`<Netiv="...">`
`</Netiv>`
`○|`
`|●`
`□|`
`|■`
`<meta>`
`<edges>`
`<book>`
`</book>`

### 8.2. Core Netiv Language Keywords (Reserved Keywords)
`°function`
`°method`
`°page`
`°if`
`°otherwise`
`°and`
`°or`
`°not`
`°return`
`°call`
`°define`
`°set`
`°write`
`°unsafe`
`°error`
`°observe`
`°recover`
`°todo`

### 8.3. Looping / Branching Keywords (Reserved Keywords)
`°while`
`°for`
`°break`
`°continue`
`°switch`
`°case`
`°default`

### 8.4. Type System Keywords (Reserved Keywords)
`°void`
`°bool`
`°true`
`°false`
`°i64`
`°u8`
`°f32`
`°f64`
`°usize`
`°scalar`
`°pointer`
`°mutable`
`°array`
`°tuple`
`°vector`
`°embedding`
`°struct`
`°enum`
`°const`

#### Recommended Future Additions:
`°i8`, `°i16`, `°i32`, `°u16`, `°u32`, `°u64`, `°isize`, `°char`, `°str`, `°string`, `°bytes`, `°null`

### 8.5. Low-Level Memory / Compiler Intrinsics (Reserved Keywords)
`°addr`
`°deref`
`°index`
`°sizeof`
`°cast`
`°extern`
`°intrinsic`

### 8.6. Legacy / Low-Level Backend Reserved Words (Reserved Backend Symbols)
`fn`
`return`
`if`
`goto`
`label`
`syscall`
`EMIT_PE`
`call`

### 8.7. CPU General Purpose Registers (Reserved Backend Symbols)
`rax`, `rcx`, `rdx`, `rbx`, `rsp`, `rbp`, `rsi`, `rdi`, `r8`, `r9`, `r10`, `r11`, `r12`, `r13`, `r14`, `r15`

### 8.8. CPU Sub-Registers / Floating Point Registers (Reserved Backend Symbols)
`rax_u8`, `rax_u16`, `rax_u32`, `xmm0`, `xmm1`, `xmm2`, `xmm3`, `xmm4`, `xmm5`, `xmm6`, `xmm7`, `xmm8`, `xmm9`, `xmm10`, `xmm11`, `xmm12`, `xmm13`, `xmm14`, `xmm15`

### 8.9. Low-Level Memory Dereferencing Operators (Reserved Backend Symbols)
`*(u8*)`, `*(u16*)`, `*(u32*)`, `*(u64*)`, `*(f32*)`, `*(f64*)`

### 8.10. Low-Level Assembler Conditionals (Reserved Backend Symbols)
`cmp_rax`, `if_z_goto`, `if_ne_goto`, `if_z_return`, `rax_is_null`

#### Recommended Future Additions:
`if_eq_goto`, `if_neq_goto`, `if_lt_goto`, `if_lte_goto`, `if_gt_goto`, `if_gte_goto`, `if_null_goto`, `if_not_null_goto`, `if_eq_return`, `if_neq_return`, `if_lt_return`, `if_lte_return`, `if_gt_return`, `if_gte_return`, `if_null_return`, `if_not_null_return`

### 8.11. Operators / Grammar Symbols (Reserved Grammar Symbols)
`+`, `-`, `*`, `/`, `%`, `&`, `|`, `^`, `!`, `<<`, `>>`, `=`, `==`, `!=`, `<`, `>`, `<=`, `>=`, `&&`, `||`, `:`, `::`, `.`, `,`, `;`, `->`, `=>`, `@`, `?`

### 8.12. Grouping / Container Symbols (Reserved Grammar Symbols)
`(`, `)`, `[`, `]`, `{`, `}`

### 8.13. Protected Standard Library Namespaces (Protected Namespaces)
`std.core`, `std.io`, `std.mem`, `std.time`, `std.math`, `std.graph`, `std.build`, `std.test`, `std.trace`, `std.mcp`, `std.restful`, `std.llm`, `std.serv`, `std.http`

#### Recommended Additions:
`std.db`, `std.fs`, `std.path`, `std.env`, `std.os`, `std.abi`, `std.asm`, `std.binary`, `std.hash`, `std.lex`, `std.parse`, `std.ast`, `std.ir`, `std.codegen`, `std.link`, `std.pkg`, `std.error`, `std.log`

### 8.14. Protected Trit Native Namespaces (Protected Namespaces)
`trit.core`, `trit.mem`, `trit.math`, `trit.cpu`, `trit.asm`, `trit.tensor`, `trit.graph`

### 8.15. Protected Adjunct Library Namespaces (Protected Namespaces)
`adj.mermaid`, `adj.tui`, `adj.sqlite`, `adj.nray`

#### Recommended Additions:
`adj.raylib`, `adj.git`, `adj.github`, `adj.vercel`, `adj.cloudflare`, `adj.htmx`, `adj.json`, `adj.toml`, `adj.yaml`, `adj.markdown`, `adj.svg`, `adj.graphviz`

### 8.16. Protected Standard Library API Symbols (Protected API Symbols)
`•print`, `•println`, `•println_native`, `•print_raw`, `•now`, `•get_ticks`, `•delay`, `•to_seconds`, `•save_binary`, `•resolve`, `•get_files`, `•copy`, `•fill`, `•arena_init`, `•arena_alloc`, `•abs`, `•abs_int`, `•min`, `•max`, `•pow`, `•sqrt`, `•node_init`, `•edge_init`, `•package_init`, `•assert_true`, `•assert_eq_int`, `•trace_log`, `•trace_metric`, `•mcp_init`, `•mcp_tool_init`, `•mcp_write_response`, `•rest_init`, `•rest_send`, `•llm_init`, `•llm_chat`, `•serv_init`, `•serv_register_route`, `•serv_listen_and_serve`, `•http_init`, `•http_connect`, `•get_primitive_size`, `•emit_op_addr`, `•emit_op_deref_byte`, `•emit_op_deref_dword`, `•emit_op_deref_qword`, `•emit_op_index`, `•emit_op_sizeof`, `•emit_cast_double_to_int`, `•emit_cast_int_to_double`, `•emit_op_if_z`, `•get_function_count`, `•get_function_name`

### 8.17. Protected Adjunct API Symbols (Protected API Symbols)
`•mermaid_init`, `•mermaid_write_str`, `•mermaid_add_node`, `•mermaid_add_edge`, `•mermaid_save`, `•tui_clear`, `•tui_color`, `•tui_move`, `•tui_reset`, `•tui_show_cursor`, `•tui_draw_header`, `•sqlite_init`, `•sqlite_execute`, `•nray_init_window`, `•nray_draw_pixel`

### 8.18. Recommended Compiler Pipeline API Symbols (Protected API Symbols)
`•lex`, `•parse`, `•lower`, `•compile`, `•assemble`, `•link`, `•emit`, `•emit_binary`, `•emit_asm`, `•emit_ir`, `•emit_obj`, `•emit_exe`, `•emit_pe`, `•emit_elf`, `•validate`, `•hash`, `•semantic_hash`, `•canonical_export`, `•pretty_export`, `•condensed_export`, `•load_source`, `•save_source`, `•load_graph`, `•save_graph`, `•export_graph`, `•export_meta`

### 8.19. Recommended Database / Graph API Symbols (Protected API Symbols)
`•db_open`, `•db_close`, `•db_get`, `•db_set`, `•db_delete`, `•db_query`, `•db_exec`, `•db_begin`, `•db_commit`, `•db_rollback`, `•node_save`, `•node_load`, `•edge_save`, `•edge_load`, `•edge_query`


## 9. User Identifiers

User-defined identifiers use the `•` prefix in canonical form.

Examples:

```netiv
•main
•CompileOperation
•EmitReturn
•out_ptr
•user.account.balance
```

Only the root identifier in an access chain requires the `•` prefix:

```netiv
•user.account.balance
```

Do not write:

```netiv
•user.•account.•balance
```

## 10. Functions

```netiv
°function •AddOne(°i64 •value) -> °i64 {
  °return •value + 1;
};
```

Functions perform lower-level work. They are allowed to define values, set values, return values, and eventually perform controlled memory operations depending on language rules.

Functions should be small, inspectable, and directly testable.

A function should do one concrete operation or transformation.

## 11. Methods

```netiv
°method •CompileOperation() -> °void {
  °call •ParseSource();
  °call •LowerToAssembly();
  °call •EmitBinary();
  °return;
};
```

Methods orchestrate work. They should call functions or other methods rather than directly performing low-level memory manipulation.

A method describes process flow. A function performs operation logic.

## 12. Pages

```netiv
°page •HomePage {
  title: "Home",
  route: "/"
};
```

Pages represent nodal content, routes, UI surfaces, or document-like source units. Pages are not the same as functions or methods.

A page may describe renderable content, metadata, relationships, or application structure.

## 13. Statements and Semicolons

Every executable or processable unit ends with `;`.

Valid:

```netiv
°return;
```

Valid:

```netiv
°call •CompileOperation();
```

Invalid:

```netiv
°return
```

The semicolon is part of NetivLang's monotonic execution model. It marks a complete processable unit.

## 14. Error Handling

Terminal form:

```netiv
<meta>{
  error: "terminal"
};

<book>{
  °function •FailHard() -> °void {
    °error {
      code: "E001",
      message: "Terminal failure"
    };
  };
};
```

Recoverable form:

```netiv
<meta>{
  error: "recoverable"
};

<book>{
  °method •TryCompile() -> °void {
    °observe {
      stage: "parse"
    };

    °recover {
      strategy: "rollback"
    };

    °return;
  };
};
```

NetivLang supports explicit error policy through metadata.

If `error` is `terminal`, an error block may terminate the unit.

If `error` is `recoverable`, the unit must use `°observe` before `°recover` so the recovery action has recorded context.

## 15. Observability

```netiv
°observe {
  stage: "lowering",
  node: •CompileOperation,
  tick: 1024
};
```

Observability is a first-class part of NetivLang.

The language should make it possible to inspect what the compiler, runtime, or AI agent did, when it did it, and which source node was involved.

NetivLang should prefer visible state transitions over hidden compiler magic.

## 16. Database Storage Model

Every initialized Netiv project owns a project-local SQLite database:

```text
db\netiv.db
```

The database is reserved for source memory, metadata, graph relationships, deterministic lists, compile state, and later IDE/MCP integration.

Generated database-list dumps belong under:

```text
logs\list\
```

Do not place the project database at the repository root. The `db` folder exists so database files have a predictable home.

## 17. Graph Export

```powershell
netiv graph
```

Exports the project graph in the canonical deterministic graph format:

```text
logs\graph\netiv.graph.mmd
```

This command does not accept format options. The output format is defined by the toolchain.

Do not create alternate graph command forms such as:

```powershell
netiv graph --format mermaid
netiv graph --format json
netiv graph --format dot
```

One command means one canonical output.

## 18. Metadata Export

```powershell
netiv meta
```

Exports project metadata in the canonical deterministic metadata format:

```text
logs\meta\netiv.meta.json
```

This command does not accept format options. The output format is defined by the toolchain.

## 19. CLI Commands

Netiv uses intentional command names instead of copying Unix or Cargo naming as the primary vocabulary.

Core commands:

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

Compatibility aliases may exist for familiar workflows:

```powershell
netiv build    # alias for netiv write
netiv run      # alias for netiv invoke
netiv clean    # alias for netiv prune
netiv doctor   # alias for netiv witness
netiv archive  # alias for netiv init
```

Conceptual mapping:

| Cargo-style action | Netiv command | Meaning |
| --- | --- | --- |
| build | `netiv write` | compile and emit the artifact |
| run | `netiv invoke` | execute the compiled artifact |
| clean | `netiv prune` | remove build cache and artifacts |
| check | `netiv audit` | validate without final executable emission |

## 20. AI Authoring Rules

Prime rule: do not invent syntax.

If the syntax is not defined in this guide, mark the area as `TODO` or ask for a language decision.

Required canonical shape:

```netiv
<Netiv="filename">
○|
  □|
    <meta>{};

    <edges>{};

    <book>{};
  |■
|●
```

AI editing rules:

1. Preserve the `<meta>`, `<edges>`, `<book>` order.
2. Preserve anthology and bookend delimiters.
3. Do not remove graph edges unless explicitly instructed.
4. Do not add comments inside `<book>`.
5. Use `°` only for reserved language words.
6. Use `•` for user-defined identifiers.
7. End every executable unit with `;`.
8. Prefer small functions.
9. Use methods for orchestration.
10. Use functions for low-level operations.
11. Do not silently change canonical formatting.
12. Do not create alternate export formats.
13. Do not assume NetivLang behaves like Rust, JavaScript, Python, C, or Haskell unless the guide explicitly says so.

When unsure, AI agents should produce:

```netiv
°todo {
  reason: "Undefined syntax decision required"
};
```

or leave a structured note in `<meta>` or `<edges>`, not inside executable `<book>` code.

## 21. Canonical Examples

Canonical minimal bookend:

```netiv
<Netiv="minimal.netiv">
○|
  □|
    <meta>{
      name: "minimal",
      kind: "function",
      version: "0.1.0",
      error: "terminal"
    };

    <edges>{};

    <book>{
      °function •main() -> °void {
        °return;
      };
    };
  |■
|●
```

Method orchestration:

```netiv
<Netiv="compile.netiv">
○|
  □|
    <meta>{
      name: "compile_operation",
      kind: "method",
      version: "0.1.0",
      error: "recoverable"
    };

    <edges>{
      %% graph TD
      %%   CompileOperation --> ParseSource
      %%   CompileOperation --> EmitBinary
    };

    <book>{
      °method •CompileOperation() -> °void {
        °observe {
          stage: "compile"
        };
        °call •ParseSource();
        °call •EmitBinary();
        °return;
      };
    };
  |■
|●
```

## 22. Invalid Examples

Invalid: missing semicolon.

```netiv
°return
```

Invalid: comments inside `<book>`.

```netiv
<book>{
  %% This is not allowed here.
  °return;
};
```

Invalid: repeated user identifier prefixes in an access chain.

```netiv
•user.•account.•balance
```

Invalid: section order changed.

```netiv
<book>{};
<meta>{};
<edges>{};
```

## 23. Glossary

Anthology: outer source collection opened by `○|` and closed by `|●`.

Bookend: atomic source unit opened by `□|` and closed by `|■`.

Metadata: non-executable description of a bookend.

Edges: graph relationship section.

Book: executable or processable content section.

Function: lower-level operation logic.

Method: orchestration flow.

Page: nodal content, route, UI surface, or document-like source unit.

Witness: installed toolchain and working archive inspection.

Graph: deterministic relationship export under `logs\graph`.

Meta: deterministic metadata export under `logs\meta`.

List: deterministic file path or database-list dump under `logs\list`.

## 24. Advanced Data Types and Operators

NetivLang supports advanced data types and operators to enable C-interoperability, math/SIMD vector spaces, and AI-oriented tooling.

### 24.1 Custom Declarations

Custom data structures and constants are declared in the `<book>` section with unique `•` user-defined identifiers.

#### Structs (`°struct`)
Used to declare structured memory layout. Required for representing geometries, matrices, and spatial objects like `•Vector3`, `•Matrix`, and `•Quaternion`.
```netiv
°struct •Vector3 {
  •x: °f32;
  •y: °f32;
  •z: °f32;
};
```

#### Enums (`°enum`)
Used to declare named integer enumerations (similar to C enums).
```netiv
°enum •Color {
  •Red;
  •Green;
  •Blue;
};
```

#### Constants (`°const`)
Used to declare immutable named values and compiler-level object-like macros.
```netiv
°const •PI: °f64 = 3.141592653589793;
°const •MAX_BUFFER: °usize = 4096;
```

---

### 24.2 Primitive Types

#### Boolean (`°bool`)
Represents single-bit truth values: `°true` or `°false`. Used in conditionals and comparison operations.
```netiv
°define •is_valid: °bool = °true;
```

#### Scalar (`°scalar`)
The canonical category for single arbitrary numeric values. Primarily used in mathematical algorithms, graph edge weights, and embeddings where float/integer distinction can be unified during abstract transformations.
```netiv
°define •weight: °scalar = 1.0;
```

#### Floating-Point (`°f32` and `°f64`)
Required for single-precision (float) and double-precision (double) values used in high-precision math, ray tracing, or spatial geometry.
```netiv
°define •force: °f32 = 9.8f;
°define •precision_val: °f64 = 0.000000123;
```

#### Unsigned Size Type (`°usize`)
Required for array sizes, iteration indexes, offset calculations, and count-like representations.
```netiv
°define •length: °usize = 256;
```

---

### 24.3 Composite & Advanced Types

#### Fixed-Size Array (`°array`)
An ordered collection of elements of a single type with a static size defined at compile-time. Required for indexing parameters and fixed-size fields.
```netiv
°define •buffer: °array<°u8, 64> = ...;
```

#### Unnamed Tuple (`°tuple`)
Fixed-size ordered sequence of heterogeneous values where fields do not require names.
```netiv
°define •pair: °tuple<°i64, °bool> = ...;
```

#### Geometry & SIMD Vector (`°vector`)
Ordered lanes of numeric values. Primarily optimized for mathematical vector operations, SIMD styling, and geometric transformations (e.g. 2D/3D coordinates).
```netiv
°define •velocity: °vector<°f32, 3> = ...;
```

#### AI Embeddings Vector (`°embedding`)
High-dimensional numeric arrays specialized for vector-search, machine learning representations, and AI search tool indexing, keeping them distinct from small geometry vectors.
```netiv
°define •document_vector: °embedding<°f32, 1536> = ...;
```

---

### 24.4 Operators & Native Control

#### Address-Of (`°addr`)
Retrieves the memory address of an identifier (creates a pointer).
```netiv
°define •ptr: °pointer<°i64> = °addr •my_var;
```

#### Dereference (`°deref`)
Accesses the value stored at a pointer address.
```netiv
°define •val: °i64 = °deref •ptr;
```

#### Element Index (`°index`)
Safely references an element inside arrays, tuples, or lanes using brackets or parameters.
```netiv
°define •item: °u8 = °index •buffer[12];
```

#### Size Inquiry (`°sizeof`)
Returns the memory size in bytes of a type or structure.
```netiv
°define •size: °usize = °sizeof(°struct •Vector3);
```

#### Type Casting (`°cast`)
Performs an explicit type casting/conversion.
```netiv
°define •whole_num: °i64 = °cast<°i64>(•force);
```

#### Foreign/External Binding (`°extern`)
Imports external C functions or variables (like DLL/dynamic library bindings).
```netiv
°extern °function •cos(•x: °f64) -> °f64;
```

#### Compiler Intrinsic (`°intrinsic`)
Direct compiler-builtin actions and specialized assembly actions.
```netiv
°intrinsic °function •prefetch(•ptr: °pointer<°void>) -> °void;
```

---

## 25. Disclaimer

NetivLang is highly experimental software.

This repository is public for visibility, review, demonstration, and potential collaboration discussion only. Public visibility does not grant permission to use, copy, redistribute, commercialize, modify, or incorporate this code into another project without express written permission from the repository owner.

The software is provided for inspection as an experimental work-in-progress. There are no guarantees of correctness, safety, stability, fitness for purpose, security, performance, or production readiness.

Do not use this project in production systems.

Do not rely on this project for safety-critical, financial, medical, legal, military, infrastructure, or other high-risk use cases.

The repository owner is not responsible for damages, losses, misuse, data loss, security incidents, operational failure, or consequences arising from unauthorized or experimental use of this code.

## 26. Contributing

NetivLang is currently a private-permission experimental project.

Public visibility of this repository does not mean the project is open source.

Contributions are welcome only by prior discussion and express permission. Contributors may be invited to work on private branches, private planning documents, or controlled issues depending on project needs.

Before contributing, you must receive express written permission from the author and agree to the applicable contribution, confidentiality, and ownership terms.

By proposing a contribution, you acknowledge that no right to use, redistribute, commercialize, or fork the project is granted unless explicitly agreed in writing.

Unsolicited pull requests, forks, rewrites, ports, derivative implementations, or copied code submissions are not authorized and may be closed or ignored.

## 27. Future Source Of Truth

The current single-file guide is the canonical human and AI reference. Future machine-readable specs may be added under `spec`, but they must agree with this guide.

Possible future spec files:

```text
spec\grammar.netivspec
spec\reserved_words.list
spec\cli_commands.list
spec\diagnostics.list
```
