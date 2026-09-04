# n8n Boundary

n8n is an optional external integration and automation system, not FlowForge's source of truth or primary workflow engine. It may trigger a FlowForge API command or consume a signed webhook/event. FlowForge owns authentication, tenant authorization, workflow execution state, AI provider policy, idempotency, and auditability.

Do not duplicate workflow state transitions or provider calls in n8n. Every inbound trigger needs authentication, an idempotency key, timeout handling, and a correlation ID. Outbound events must avoid sensitive AI payloads unless explicitly configured.
