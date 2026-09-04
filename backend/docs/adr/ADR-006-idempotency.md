# ADR-006: Idempotency

**Status:** Proposed

Enforce idempotency at the application command boundary and with database uniqueness constraints. PostgreSQL is authoritative; Redis may coordinate concurrent requests but cannot be the only record of a business operation.
