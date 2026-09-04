# ADR-004: RAG Architecture

**Status:** Accepted

RAG remains an application pipeline of extraction, chunking, embedding, vector search, optional filtering/reranking, context budgeting, and LLM generation. PostgreSQL/pgvector is the initial vector store. Evaluation data and version metadata are required for quality changes.
