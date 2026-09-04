# FlowForge AI Backend

FlowForge is a .NET 9 modular monolith for tenant-scoped agents, conversations, document knowledge, RAG, and provider-neutral AI workflows.

## Run locally

Set these values with environment variables, user secrets, or a secret manager:

```text
ConnectionStrings__DefaultConnection
Jwt__Key
OpenAI__ApiKey
```

The database must provide PostgreSQL with the `vector` extension. Start the API with `dotnet run --project FlowForge.Api` after applying migrations.

Endpoints:

- `GET /liveness` checks only process health.
- `GET /readiness` checks PostgreSQL connectivity.
- Swagger is available in the development environment.

## Structure

- `FlowForge.Domain`: entities and domain contracts
- `FlowForge.Application`: DTOs, ports, workflow orchestration, and AI contracts
- `FlowForge.Infrastructure`: EF Core, repositories, parsing, provider adapters, and services
- `FlowForge.Api`: HTTP boundary, authentication, rate limiting, health, and composition root
- `FlowForge.Tests`: unit tests and deterministic application behavior tests

See [docs/ARCHITECTURE_GAP_ANALYSIS.md](docs/ARCHITECTURE_GAP_ANALYSIS.md) for the baseline, risks, and phased roadmap. Operational decisions are in [docs/](docs/).
