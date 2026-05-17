# Database Research Direction

## Research Database Direction

Current research is exploring a monotonic, multi-agent database architecture built around ordered tick-based history, graph relationships, and pointer-oriented storage.

The design direction combines:

- SQL-like query behavior
- B-tree-style indexing
- node/edge graph relationships
- monotonic tick history
- append-first storage semantics
- multi-agent read/write coordination
- artifact and metadata persistence

Rather than treating records as simple overwrite operations, the system models changes as ordered events recorded against monotonic ticks. This allows historical reconstruction, deterministic replay, provenance tracking, graph inspection, and conflict-aware multi-agent workflows.

The database is being explored as a hybrid between:

- relational indexing
- graph traversal
- event sourcing
- compiler artifact storage
- deterministic build history

Research areas currently include:

- tick-based transaction ordering
- optimistic multi-agent concurrency
- graph-native storage layers
- pointer-oriented object relationships
- immutable history chains
- artifact provenance
- deterministic rebuild pipelines
- metadata-aware indexing

## Current Dependency Boundary

SQLite is currently used for project-local database state, metadata tracking, inspection workflows, and related tooling support. SQLite is an external dependency and is not authored or owned by Netiv.

The long-term research direction is to investigate whether a database written in Netiv could eventually replace SQLite for Netiv-specific toolchain workflows. That possible replacement is not claimed as complete or production-ready. It is a research direction for a toolchain-native storage layer that matches Netiv's philosophy of explicit structure, inspectable metadata, graph-aware relationships, and deterministic build history.
