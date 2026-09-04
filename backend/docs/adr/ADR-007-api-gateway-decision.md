# ADR-007: API Gateway Decision

**Status:** Accepted

Do not build a custom API gateway initially. ASP.NET Core provides routing, authentication, rate limiting, health, and observability for the current modular monolith. Reassess Azure API Management when external consumer governance, multi-service routing, quotas, or centralized policy justify its cost and operational boundary.
