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
* [x] Validate lexical sigil `°` (reserved language keywords prefix).
* [x] Validate lexical sigil `•` (protected API identifiers prefix).
* [x] Validate structural tags `<Netiv>`, `<Netiv="...">`, and `</Netiv>`.
* [x] Validate Unicode boundaries `○|`, `|●`, `□|`, and `|■`.
* [x] Validate JSON-like attributes `<meta>`, `<edges>`, `<book>`, and `</book>`.

### B. Core Keywords & Control Flow
* [x] Test reserved keywords: `°function`, `°method`, `°page`.
* [x] Test branchings: `°if`, `°otherwise`, `°and`, `°or`, `°not`.
* [x] Test exits: `°return`, `°call`, `°define`, `°set`, `°write`.
* [x] Test fault tolerances: `°unsafe`, `°error`, `°observe`, `°recover`, `°todo`.
* [x] Test loops: `°while`, `°for`, `°break`, `°continue`, `°switch`, `°case`, `°default`.

### C. Type System & Memory Intrinsics
* [x] Test types: `°void`, `°bool`, `°true`, `°false`, `°i64`, `°u8`, `°f32`, `°f64`, `°usize`, `°scalar`.
* [x] Test specifiers: `°pointer`, `°mutable`, `°array`, `°tuple`, `°vector`, `°embedding`, `°struct`, `°enum`, `°const`.
* [x] Test compiler intrinsics: `°addr`, `°deref`, `°index`, `°sizeof`, `°cast`, `°extern`, `°intrinsic`.

### D. Assembler & Backend Symbols (Legacy Support)
* [x] Test legacy symbols: `fn`, `return`, `if`, `goto`, `label`, `syscall`, `EMIT_PE`, `call`.
* [x] Test general registers: `rax` through `r15`.
* [x] Test sub-register bytes: `rax_u8`, `rax_u16`, `rax_u32`.
* [x] Test floating registers: `xmm0` through `xmm15`.
* [x] Test dereferencers: `*(u8*)`, `*(u16*)`, `*(u32*)`, `*(u64*)`, `*(f32*)`, `*(f64*)`.
* [x] Test conditionals: `cmp_rax`, `if_z_goto`, `if_ne_goto`, `if_z_return`, `rax_is_null`.

### E. Operators, Container Symbols & Containers
* [x] Test operators: `+`, `-`, `*`, `/`, `%`, `&`, `|`, `^`, `!`, `<<`, `>>`, `=`, `==`, `!=`, `<`, `>`, `<=`, `>=`, `&&`, `||`, `:`, `::`, `.`, `,`, `;`, `->`, `=>`, `@`, `?`.
* [x] Test container enclosures: `(`, `)`, `[`, `]`, `{`, `}`.

### F. Protected Namespaces & APIs
* [x] Verify protection for `std.*`, `trit.*`, and `adj.*` namespaces.
* [x] Audit standard API calls: `•print`, `•println`, `•now`, `•delay`, `•arena_alloc`, `•mcp_init`, `•serv_listen_and_serve`, etc.
* [x] Audit adjunct API calls: `•mermaid_save`, `•tui_clear`, `•sqlite_execute`, `•nray_draw_pixel`, etc.

---

## Completed Tasks

* [x] Implement comprehensive Netiv 1.01 Master Reserved and Protected Symbols unit testing suite in `src/std_test_spec_101.ntv`.
* [x] Execute and pass 100% of Netiv 1.01 Master Reserved & Protected Symbols test cases via the `run_101_spec_tests` mock execution pipeline.

