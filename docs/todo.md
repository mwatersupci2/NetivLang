# Netiv Lang TODO

Track planned Netiv language and toolchain work here.

## 1. High-Priority Standard Libraries (`std`)

These core packages establish the baseline system abstractions and decouple compiler features from target platforms:

* [x] `std.core` — Language primitives and low-level x64 instruction emitters (`src/std_core.ntv`).
* [x] `std.io` — Terminal writing, path crawling, files, and print streams (`src/std_io.ntv`).
* [x] `std.mem` — Memory management, custom copy/fill, and Arena Allocators (`src/std_mem.ntv`).
* [x] `std.time` — System clock ticks, precise millisecond delays, and epoch conversions (`src/std_time.ntv`).
* [x] `std.math` — Newton-Raphson square root, limits, absolute values, and exponents (`src/std_math.ntv`).
* [x] `std.graph` — Active semantic node structures, compiler package edges, and layouts (`src/std_graph.ntv`).
* [x] `std.build` — Packaging build system manifests and compiler orchestration (`src/std_build.ntv`).
* [x] `std.test` — Compiler assertion tests and audit testing suites (`src/std_test.ntv`).
* [x] `std.trace` — Log tracers, diagnostic warning outputs, and metrics (`src/std_trace.ntv`).
* [x] `std.mcp` — AI Model Context Protocol stdio and JSON-RPC stubs (`src/std_mcp.ntv`).

---

## 2. High-Priority Adjunct Libraries (`adj`)

These secondary libraries will support advanced graphics, local storage, network protocols, and visualization:

* [x] `adj.mermaid` — Graph structure exports, dependency visualizers, and documentation generators (`src/adj_mermaid.ntv`).
* [x] `adj.tui` — Dynamic terminal user interfaces, cursor positioning, and styled panels (`src/adj_tui.ntv`).
* [x] `adj.http` — Socket structures and basic web client/server communication (`src/adj_http.ntv`).
* [x] `adj.sqlite` — Local database caching client and structured queries (`src/adj_sqlite.ntv`).
* [x] `adj.nray.*` — Color profiles, Vector3 3D spaces, and Raylib stubs (`src/adj_nray.ntv`).

---

## Completed Tasks

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
