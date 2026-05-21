# ROCN Research Library

ROCN is the Netiv research fork target for a future ROCm-oriented compute layer.

The name intentionally differs from ROCm: ROCN means **ROCm for Netiv**. This folder is a research staging area, not a production runtime or supported hardware backend yet.

## Purpose

ROCN exists to explore how Netiv could describe, organize, and eventually compile GPU-oriented compute work for AMD ROCm environments while keeping Netiv's normal rules intact:

- AI-authored, human-audited source structure.
- Explicit metadata and graph edges.
- Deterministic library boundaries.
- No hidden runtime claims before implementation exists.
- Research-first design notes before compiler integration.

## Initial Scope

The first ROCN library should stay narrow:

1. Define ROCN terminology.
2. Sketch Netiv-facing GPU concepts.
3. Separate host orchestration from device/kernel intent.
4. Prepare a future bridge between Netiv source and ROCm/HIP-style execution models.
5. Avoid pretending that Netiv currently compiles ROCm kernels.

## Folder Layout

```text
research/rocn/
├── README.md          # Research overview and scope fence
└── rocn.ntv           # Starter Netiv library scaffold
```

## Current Status

Status: research scaffold.

ROCN is not yet wired into the compiler, build driver, standard library, or runtime. Any implementation added here should be treated as design-stage code until explicitly promoted into `src`, `std`, `adj`, or another active compiler/library path.
