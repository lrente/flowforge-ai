# ADR-002: Asynchronous Processing

**Status:** Proposed

Use one durable job mechanism for document ingestion and long-running workflows. It must provide durable IDs, attempts, backoff, cancellation, idempotency, and dead-letter handling. Do not introduce Hangfire and another queue for the same work; choose based on the deployment target and operational constraints.
