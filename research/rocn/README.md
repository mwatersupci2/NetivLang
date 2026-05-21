# ROCN Research Library

ROCN is the Netiv research fork target for a future ROCm-oriented compute layer.

The name intentionally differs from ROCm: ROCN means **ROCm for Netiv**. This folder is a research staging area, not a production runtime or supported hardware backend yet.

## Upstream Attribution

ROCN is an independent Netiv research library inspired by and intended to study AMD's ROCm/HIP programming model.

Upstream reference:

- Project: ROCm/HIP
- Repository: `ROCm/hip`
- URL: `https://github.com/ROCm/hip`
- Default upstream branch observed: `develop`
- License observed: MIT License
- Copyright notice observed: `Copyright (C) Advanced Micro Devices, Inc.`

ROCm, HIP, AMD, and related project names, marks, code, documentation, and intellectual property belong to their respective owners, including Advanced Micro Devices, Inc. ROCN is not an official AMD project, is not endorsed by AMD, and should not be represented as part of ROCm unless a future written agreement or upstream relationship explicitly says otherwise.

Any future copied, modified, vendored, or forked portions of ROCm/HIP source must preserve the applicable upstream copyright notices, license text, and attribution requirements. Until such source is actually copied into this repository, this folder should be treated as reference research and original Netiv design work.

## Purpose

ROCN exists to explore how Netiv could describe, organize, and eventually compile GPU-oriented compute work for AMD ROCm environments while keeping Netiv's normal rules intact:

- AI-authored, human-audited source structure.
- Explicit metadata and graph edges.
- Deterministic library boundaries.
- No hidden runtime claims before implementation exists.
- Research-first design notes before compiler integration.
- Proper upstream attribution before any fork, adapter, or compatibility layer is promoted.

## Initial Scope

The first ROCN library should stay narrow:

1. Define ROCN terminology.
2. Sketch Netiv-facing GPU concepts.
3. Separate host orchestration from device/kernel intent.
4. Prepare a future bridge between Netiv source and ROCm/HIP-style execution models.
5. Avoid pretending that Netiv currently compiles ROCm kernels.
6. Preserve a clean boundary between original Netiv research and upstream ROCm/HIP intellectual property.

## Folder Layout

```text
research/rocn/
├── README.md          # Research overview, scope fence, and upstream attribution
├── rocn.ntv           # Starter Netiv library scaffold
└── upstream-hip.md    # HIP upstream import notes
```

## Current Status

Status: research scaffold.

ROCN is not yet wired into the compiler, build driver, standard library, or runtime. Any implementation added here should be treated as design-stage code until explicitly promoted into `src`, `std`, `adj`, or another active compiler/library path.

ROCN should remain a clearly attributed research layer until the project makes a deliberate decision to use one of these upstream strategies:

1. Reference-only study of ROCm/HIP.
2. Netiv adapter targeting HIP APIs without vendoring upstream source.
3. Git submodule pointing to upstream or forked HIP source.
4. Separate fork of `ROCm/hip` maintained outside this repository.
5. Selective vendoring with preserved MIT license notices and explicit attribution.
