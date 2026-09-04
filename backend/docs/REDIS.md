# Redis Strategy

Redis is not currently registered in the backend and must not be treated as an architectural requirement merely because the intended stack mentions it.

When introduced, use it only for shared ephemeral concerns with explicit key and TTL policies:

| Use | Key shape | TTL | Failure behavior |
|---|---|---:|---|
| Rate limiting | `rate:{scope}:{window}` | window | fail closed for protected AI commands |
| Distributed lock | `lock:{operation}:{id}` | short lease | do not perform duplicate work |
| Idempotency coordination | `idem:{tenant}:{operation}:{key}` | 24h+ | PostgreSQL remains source of truth |
| Cache | versioned resource key | bounded | fall back to PostgreSQL |

Serialization must be versioned and size monitored. Correctness-critical state belongs in PostgreSQL.
