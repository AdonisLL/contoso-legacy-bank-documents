# Modernization path

This repository is intentionally a deployable legacy baseline. Modernize incrementally while keeping the file contract and coexistence behavior stable.

1. Add contract fixtures and telemetry around throughput, latency, failures, and queue depth.
2. Extract interfaces around storage, clock, PDF rendering, and status publication.
3. Multi-target or move the core processing library to a supported modern .NET release while retaining the .NET Framework host during transition.
4. Add a modern Worker Service host and structured telemetry with secret redaction.
5. Replace local folders with durable object storage and a queue using lease/visibility semantics equivalent to the atomic claim.
6. Run both implementations against shadow traffic and compare PDFs and metadata.
7. Cut over producers and consumers gradually; retain the legacy worker as rollback until evidence supports retirement.

Avoid an in-place framework upgrade coupled with queue and PDF changes. Contract versioning, idempotency, reconciliation, and rollback should be validated independently.
