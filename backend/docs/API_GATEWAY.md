# API Gateway Decision

A custom gateway is not justified for the current modular monolith. ASP.NET Core already owns routing, JWT authentication, CORS, rate limiting, health checks, and request telemetry. Azure API Management becomes relevant when FlowForge has multiple independently deployed services or needs centralized external products, quotas, transformations, and consumer governance. Reassess with traffic and operational evidence rather than adding a gateway preemptively.
