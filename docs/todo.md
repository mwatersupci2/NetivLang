# Netiv Lang TODO

Track planned Netiv language and toolchain work here.

## 1. High-Priority Standard Libraries (`std`)

*No active items.* All standard libraries have been fully drafted, implemented, and compiled natively.

---

## 2. High-Priority Adjunct Libraries (`adj`)

*No active items.* All adjunct/secondary modules are fully completed.

---

## 3. Netiv 1.01 Master Reserved & Protected Symbols Testing Backlog

This backlog registers the systematic audit and compilation correctness validations for the complete Netiv 1.01 specification:

### A. Sigils & Structural Boundaries
* [ ] Validate lexical sigil `°` (reserved language keywords prefix).
* [ ] Validate lexical sigil `•` (protected API identifiers prefix).
* [ ] Validate structural tags `<Netiv>`, `<Netiv="...">`, and `</Netiv>`.
* [ ] Validate Unicode boundaries `○|`, `|●`, `□|`, and `|■`.
* [ ] Validate JSON-like attributes `<meta>`, `<edges>`, `<book>`, and `</book>`.

### B. Core Keywords & Control Flow
* [ ] Test reserved keywords: `°function`, `°method`, `°page`.
* [ ] Test branchings: `°if`, `°otherwise`, `°and`, `°or`, `°not`.
* [ ] Test exits: `°return`, `°call`, `°define`, `°set`, `°write`.
* [ ] Test fault tolerances: `°unsafe`, `°error`, `°observe`, `°recover`, `°todo`.
* [ ] Test loops: `°while`, `°for`, `°break`, `°continue`, `°switch`, `°case`, `°default`.

### C. Type System & Memory Intrinsics
* [ ] Test types: `°void`, `°bool`, `°true`, `°false`, `°i64`, `°u8`, `°f32`, `°f64`, `°usize`, `°scalar`.
* [ ] Test specifiers: `°pointer`, `°mutable`, `°array`, `°tuple`, `°vector`, `°embedding`, `°struct`, `°enum`, `°const`.
* [ ] Test compiler intrinsics: `°addr`, `°deref`, `°index`, `°sizeof`, `°cast`, `°extern`, `°intrinsic`.

### D. Assembler & Backend Symbols (Legacy Support)
* [ ] Test legacy symbols: `fn`, `return`, `if`, `goto`, `label`, `syscall`, `EMIT_PE`, `call`.
* [ ] Test general registers: `rax` through `r15`.
* [ ] Test sub-register bytes: `rax_u8`, `rax_u16`, `rax_u32`.
* [ ] Test floating registers: `xmm0` through `xmm15`.
* [ ] Test dereferencers: `*(u8*)`, `*(u16*)`, `*(u32*)`, `*(u64*)`, `*(f32*)`, `*(f64*)`.
* [ ] Test conditionals: `cmp_rax`, `if_z_goto`, `if_ne_goto`, `if_z_return`, `rax_is_null`.

### E. Operators, Container Symbols & Containers
* [ ] Test operators: `+`, `-`, `*`, `/`, `%`, `&`, `|`, `^`, `!`, `<<`, `>>`, `=`, `==`, `!=`, `<`, `>`, `<=`, `>=`, `&&`, `||`, `:`, `::`, `.`, `,`, `;`, `->`, `=>`, `@`, `?`.
* [ ] Test container enclosures: `(`, `)`, `[`, `]`, `{`, `}`.

### F. Protected Namespaces & APIs
* [ ] Verify protection for `std.*`, `trit.*`, and `adj.*` namespaces.
* [ ] Audit standard API calls: `•print`, `•println`, `•now`, `•delay`, `•arena_alloc`, `•mcp_init`, `•serv_listen_and_serve`, etc.
* [ ] Audit adjunct API calls: `•mermaid_save`, `•tui_clear`, `•sqlite_execute`, `•nray_draw_pixel`, etc.

---

## Completed Tasks

* [x] Integrate full Netiv 1.01 Master Reserved and Protected Symbols list into `docs/reserved_words.list`.
* [x] Draft exhaustive master reserved words list at `docs/reserved_words.list`.
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
