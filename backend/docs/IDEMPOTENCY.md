# Idempotency Strategy

Idempotency is required for chat writes, document ingestion, and background jobs that can be retried.

## Contract

Callers provide an `Idempotency-Key` for retryable commands. The server scopes the key by tenant, authenticated principal, operation, and route. A durable record stores the key, request fingerprint, status, response metadata, and timestamps.

- A completed matching request returns the stored result.
- A concurrent matching request waits for or returns the existing operation state.
- Reuse with a different request fingerprint returns a conflict.
- Keys are retained for at least 24 hours for API commands; job/message IDs are retained for the business retention period.
- Unique database constraints are the final protection against duplicate side effects.

The current repository has not yet added the durable idempotency table or middleware. Do not claim a command is idempotent until both the application check and database constraint exist.
