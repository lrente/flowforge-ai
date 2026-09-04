# RAG Evaluation

RAG changes must be compared against a fixed dataset rather than judged by anecdotal answers. Each case contains a question, expected relevant document/chunk identifiers, and a reference answer or required facts.

```json
[
  {
    "id": "flowforge-auth-01",
    "question": "How does FlowForge isolate client data?",
    "relevantTerms": ["ClientId", "membership", "authorization"],
    "expectedFacts": ["resource access is scoped to a client membership"]
  },
  {
    "id": "flowforge-rag-01",
    "question": "What happens when a document is processed?",
    "relevantTerms": ["extract", "chunk", "embedding", "vector search"],
    "expectedFacts": ["text is chunked and embedded before similarity search"]
  }
]
```

Record retrieval relevance/recall, context precision, answer relevance, groundedness, latency, input/output tokens, and estimated cost. Store the RAG version, embedding model, prompt version, and dataset version with every run so version A and B are comparable.
