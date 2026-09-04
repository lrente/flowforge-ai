# Observability

Every HTTP request and workflow execution must carry a correlation/trace ID. Structured events should include operation, workflow ID, request ID, tenant/user scope where appropriate, status, duration, provider/model, token counts, and estimated cost. Sensitive prompts, responses, credentials, and authorization headers are excluded by default.

Required metrics:

- HTTP request count, duration, and errors
- workflow duration and status
- job depth, attempts, and failures
- LLM latency, errors, tokens, and estimated cost
- RAG retrieval latency and result count
- database dependency latency and availability

OpenTelemetry is the target instrumentation standard. The current API has correlation-ready workflow data and health endpoints, but exporter/tracer/metric registration remains a follow-up implementation item.
