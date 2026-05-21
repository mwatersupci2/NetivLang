# HIP Upstream Import Notes

Upstream repository: `ROCm/hip`

URL: `https://github.com/ROCm/hip`

Default branch observed: `develop`

License observed: MIT License

## Why this upstream matters

HIP is the closest upstream component for ROCN because it defines the programming model that ROCN needs to study first:

- host-side orchestration
- device/kernel code intent
- runtime API calls
- memory movement between host and device
- CUDA-like portability vocabulary
- AMD ROCm execution assumptions

## Import policy for ROCN research

Do **not** vendor the entire HIP repository into NetivLang yet.

For this research branch, treat HIP as an upstream reference source. Pull in only small notes, mapping tables, and original Netiv research files until ROCN has a clear compiler-facing design.

A full source import should only happen after one of these choices is made:

1. **Submodule**: keep HIP as external upstream source.
2. **Fork**: fork `ROCm/hip` separately and build ROCN against that fork.
3. **Reference-only**: never import HIP source; only model compatible Netiv abstractions.
4. **Selective adapter**: write Netiv-side adapters that target HIP APIs without copying HIP internals.

## First files/areas to inspect

```text
README.md
LICENSE.md
docs/understand/programming_model.rst
docs/how-to/hip_cpp_language_extensions.rst
docs/how-to/hip_runtime_api/
docs/how-to/hip_porting_guide.rst
include/hip/
cmake/
bin/
```

## ROCN mapping targets

| HIP concept | ROCN / Netiv research target |
|---|---|
| HIP runtime API | `rocn.host` orchestration layer |
| HIP kernel language | `rocn.kernel` intent layer |
| Device memory | explicit Netiv GPU memory regions |
| Host/device transfer | explicit movement commands, no hidden copies |
| Streams/events | future Netiv async graph edges |
| HIP build config | future Netiv build backend research |

## Current stance

ROCN is **not** a HIP clone yet.

ROCN is currently a Netiv research layer that studies HIP/ROCm structure so Netiv can eventually describe GPU work in a deterministic, auditable way.
