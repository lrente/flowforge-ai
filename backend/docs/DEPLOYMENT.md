# Deployment

Development requires the .NET 9 SDK and a PostgreSQL instance with the `vector` extension. Set `ConnectionStrings__DefaultConnection`, `Jwt__Key`, and `OpenAI__ApiKey` outside tracked files before starting the API. Apply migrations using the deployment pipeline, not automatically in a multi-replica production startup without an explicit migration lock/process.

The Dockerfile builds a self-contained API image and serves HTTP on port 5000. Production deployment should add a non-root runtime user, container health checks, private database/Redis networking, secret-store references, image scanning, and separate development-only tooling such as pgAdmin.

For Azure, compare Container Apps/App Service with managed PostgreSQL, Redis, Key Vault, Container Registry, and Azure Monitor before adding IaC. Keep the first deployment maintainable and avoid provisioning unused services.
