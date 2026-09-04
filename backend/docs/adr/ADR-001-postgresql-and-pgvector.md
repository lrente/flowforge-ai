# ADR-001: PostgreSQL and pgvector

**Status:** Accepted

Keep relational application data and embeddings in PostgreSQL with pgvector. This preserves transactional tenant/resource boundaries and avoids an additional vector database until scale or workload measurements prove a separate system necessary. Add an approximate vector index after measuring dataset size and query plans.
