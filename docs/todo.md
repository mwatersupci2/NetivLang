# Netiv Lang TODO

Track planned Netiv language and toolchain work here.

## 1. High-Priority Standard Libraries (`std`)

*No active items.* All standard libraries have been fully drafted, implemented, and compiled natively.

---

## 2. High-Priority Adjunct Libraries (`adj`)

*No active items.* All adjunct/secondary modules are fully completed.

---

## 3. Machine-Lowered Reserved Words Unit Testing Backlog

This backlog outlines the comprehensive unit testing checklist for every single register, memory operator, and machine instruction emitter, validating the exact x64 binary assembly code generation:

### A. 64-Bit General Purpose CPU Registers
* [ ] Test `rax` code emitter offsets.
* [ ] Test `rcx` parameter assignment.
* [ ] Test `rdx` parameter assignment.
* [ ] Test `rbx` base pointer retention.
* [ ] Test `rsp` stack allocation and alignment.
* [ ] Test `rbp` stack base frame validation.
* [ ] Test `rsi` string source transfers.
* [ ] Test `rdi` string destination transfers.
* [ ] Test `r8` and `r9` extended register assignments.
* [ ] Test `r10` through `r15` compiler registers.

### B. Register Sub-Bytes & Floating Points
* [ ] Test `rax_u8` lower AL accumulator byte writes.
* [ ] Test `rax_u16` lower AX accumulator word writes.
* [ ] Test `rax_u32` lower EAX accumulator doubleword writes.
* [ ] Test SSE floating point registers `xmm0` through `xmm7` float logic.

### C. Low-Level Memory Dereferencing Operators
* [ ] Test `*(u8*)` 1-byte read/write dereferencing.
* [ ] Test `*(u16*)` 2-byte read/write dereferencing.
* [ ] Test `*(u32*)` 4-byte read/write dereferencing.
* [ ] Test `*(u64*)` 8-byte read/write dereferencing.
* [ ] Test `*(f32*)` single float memory reading.
* [ ] Test `*(f64*)` double float memory reading.

### D. Core Assembler Emitter Keywords
* [ ] Test `cmp_rax` immediate comparison emitter logic.
* [ ] Test `if_z_goto` conditional zero jump.
* [ ] Test `if_ne_goto` conditional non-zero jump.
* [ ] Test `if_z_return` conditional zero stack exit.
* [ ] Test `rax_is_null` register check branch.
* [ ] Test `syscall` compiler and host delegate jumps.
* [ ] Test `call` internal subroutine offset calculations.
* [ ] Test `goto` absolute address jumps.
* [ ] Test `label` local address anchor compilation.
* [ ] Test `return` instruction and stack restoration.
* [ ] Test `EMIT_PE` standalone native binary production.

### E. Binary x64 Mathematical & Logic Operators
* [ ] Test `+` (addition instruction `add`) operation.
* [ ] Test `-` (subtraction instruction `sub`) operation.
* [ ] Test `*` (multiplication instruction `imul`) operation.
* [ ] Test `/` (division instruction `idiv`) operation.
* [ ] Test `&` (bitwise AND instruction `and`) operation.
* [ ] Test `|` (bitwise OR instruction `or`) operation.
* [ ] Test `^` (bitwise XOR instruction `xor`) operation.
* [ ] Test `<<` (shift left `shl`) and `>>` (shift right `sar`) operations.
* [ ] Test comparisons `==`, `!=`, `<`, `>` (using `cmp` and set-byte conditional modifiers).

---

## Completed Tasks

* [x] Draft machine-lowered CPU registers, dereference operators, and assembler keyword listings at `docs/reserved_words.list`.
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
