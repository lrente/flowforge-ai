# Security

Configuration contains no credentials. Supply connection strings, JWT keys, and provider keys through environment variables, user secrets, or a managed secret store. Rotate any credentials previously committed to repository history.

The API now requires a JWT signing key of at least 32 characters, uses configurable CORS origins, applies a fixed-window API limiter, and exposes dependency-aware readiness separately from liveness. Authorization must remain resource- and tenant-scoped at the application/data access boundary.

Production follow-ups include centralized ProblemDetails policy verification, upload content/size validation, security headers, endpoint-specific AI limits, audit retention, dependency scanning, secret scanning, and integration tests for IDOR/tenant isolation. Never log tokens, API keys, prompts, full provider responses, or internal exception details.
