# Netiv Lang TODO

Track planned Netiv language and toolchain work here.

## 1. High-Priority Standard Libraries (`std`)

*No active items.* All standard libraries have been fully drafted, implemented, and compiled natively.

---

## 2. High-Priority Adjunct Libraries (`adj`)

*No active items.* All adjunct/secondary modules are fully completed.

---

## 3. Reserved Keyword & Library API Unit Testing Backlog

This backlog outlines the comprehensive unit testing checklist for every single reserved keyword, library namespace, and exposed function symbol, ensuring absolute language and API stability:

### A. Structural Tags & Boundaries
* [ ] Test `<Netiv>` and `</Netiv>` root tags validation.
* [ ] Test `<meta>{...};` metadata layout parsing.
* [ ] Test `<edges>{...};` active dependency link resolution.
* [ ] Test `<book>` and `</book>` module boundary enclosures.
* [ ] Test `○|` and `|●` Unicode anthology segments.
* [ ] Test `□|` and `|■` Unicode bookend layouts.

### B. Control Flow & Function Keywords
* [ ] Test `fn` function declarations and parameter lists.
* [ ] Test `return` x64 stack frame tear-downs.
* [ ] Test `if` conditional branches.
* [ ] Test `goto` absolute labels.
* [ ] Test `label` local assembler offsets.
* [ ] Test `syscall` delegate table routing.
* [ ] Test `EMIT_PE` standard Windows binary generation.

### C. Low-Level Assembler Conditionals
* [ ] Test `if_z_goto` conditional zero jumps.
* [ ] Test `if_ne_goto` conditional non-zero jumps.
* [ ] Test `if_z_return` zero branch exits.
* [ ] Test `rax_is_null` compiler register validations.

### D. Canonical Keywords & Boolean Primitives
* [ ] Test `°struct` structure size mappings and padding.
* [ ] Test `°enum` enumeration indices.
* [ ] Test `°const` compile-time values.
* [ ] Test `°bool` validation using boolean primitives `°true` and `°false`.

### E. Core Mathematical & Integer Types
* [ ] Test `°scalar` (float/int precision mappings).
* [ ] Test `°f32` (IEEE single precision).
* [ ] Test `°f64` (IEEE double precision).
* [ ] Test `°usize` (word-aligned unsigned values).

### F. High-Dimensional Array & Layout Types
* [ ] Test `°array` contiguous elements sequence.
* [ ] Test `°tuple` anonymous structure offsets.
* [ ] Test `°vector` mathematical dimension spaces.
* [ ] Test `°embedding` deep learning vector arrays.

### G. Low-Level Pointer & Mutability Modifiers
* [ ] Test `°pointer` address dereferences.
* [ ] Test `°mutable` write-access constraints.
* [ ] Test `°void` empty function outputs.

### H. Standard Library Namespaces & Exposed APIs
* [ ] Test `std.core` sizing, register operators, and machine-code instruction emitters.
* [ ] Test `std.io` print streams, filesystem saving, and recursive path crawling.
* [ ] Test `std.mem` copy/fill and Align-8 Arena Allocator.
* [ ] Test `std.time` millisecond clocks and wait-delays.
* [ ] Test `std.math` Newton-Raphson square root, absolute values, and exponents.
* [ ] Test `std.graph` semantic compiler nodes and Edge structures.
* [ ] Test `std.build` package configuration and manifest targets.
* [ ] Test `std.test` assertion validations.
* [ ] Test `std.trace` diagnostic warnings and logging levels.
* [ ] Test `std.mcp` AI Model Context Protocol JSON-RPC responses and server contexts.
* [ ] Test `std.restful` REST client request payloads and HTTP headers.
* [ ] Test `std.llm` AI completion requests and models configurations.
* [ ] Test `std.serv` server socket routers and listening loops.
* [ ] Test `std.http` low-level socket connections.

### I. Adjunct Library Namespaces & Exposed APIs
* [ ] Test `adj.mermaid` diagram builders and dependency exporters.
* [ ] Test `adj.tui` ANSI slate panels and double-border layouts.
* [ ] Test `adj.sqlite` database cached queries.
* [ ] Test `adj.nray` Raylib window initialization and Vector3/Color space math.

---

## Completed Tasks

* [x] Expand reserved words list to include all 14 standard libraries, 4 adjunct libraries, and their 82 core API function symbols at `docs/reserved_words.list`.
* [x] Formulate complete reserved words list at `docs/reserved_words.list`.
* [x] Promote HTTP library `adj.http` to high-priority standard `std.http` (`src/std_http.ntv`).
* [x] Implement standard libraries `std.restful` for REST API request/responses, `std.llm` for AI completions client stubs, and `std.serv` for HTTP web servers.
* [x] Implement AI Model Context Protocol `std.mcp` library supporting JSON-RPC response structures and initialize frames (`src/std_mcp.ntv`).
* [x] Implement all 5 secondary adjunct libraries (`adj.mermaid`, `adj.tui`, `adj.http`, `adj.sqlite`, `adj.nray`) in standard canonical page formats under `src/`.
* [x] Implement the entire suite of 9 core standard libraries (`std.core`, `std.io`, `std.mem`, `std.time`, `std.math`, `std.graph`, `std.build`, `std.test`, `std.trace`) in standard canonical page formats under `src/`.
* [x] Draft the standalone native CLI driver in `src/cli.ntv` replacing `netiv_launcher.cs` to execute C#-free command routing and Windows API outputs.
* [x] Extend the native compiler's PE builder (`EMIT_PE` in `src/compiler.ntv`) to natively support PE Import Address Table (IAT) directories and symbols.
* [x] Create self-contained Newton-Raphson square root solver in `std.math` and Arena Allocator in `std.mem`.
* [x] Formulate and integrate `std.core` exposing sizing profiles and machine code instruction emitters for operators and primitives (`src/std_core.ntv`).
* [x] Unify and implement `std.io` exposing print streams, time, filesystem resolving/saving, and recursive path crawling (`src/std_io.ntv`).
* [x] Extend C# CLI host and launcher table to inject a custom filesystem listing syscall (slot 10).
* [x] Integrate 10 new advanced data types and keywords (`°struct`, `°enum`, `°const`, `°bool`, `°scalar`, `°f32`, `°f64`, `°usize`, `°array`, `°tuple`, `°vector`, `°embedding`).
* [x] Design x64 assembly lowering rules and primitive memory sizes.
* [x] Create native type lowering library `src/types_assembler.ntv`.
* [x] Inject dynamic execution entry points for types into C# host and netiv CLI launcher.
* [x] Verify CLI argument routes via `netiv invoke <lowering_fn>`.
* [x] Scaffold stable canonical decoupling libraries `src/std_io.ntv`, `src/std_fs.ntv`, and `src/std_sys.ntv` passing 100% green audits.
