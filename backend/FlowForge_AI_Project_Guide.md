# FlowForge AI

## Visão Geral

FlowForge AI é uma plataforma composta por: - Backend: ASP.NET Core
(.NET 8) - Frontend: React + Vite + Tailwind CSS v4 - Base de dados:
PostgreSQL com extensão pgvector - Redis - n8n - pgAdmin

## Estrutura

    flowforge-ai/
    ├── backend/
    ├── frontend/
    ├── docker-compose.yml
    └── README.md

## Requisitos

-   Docker Desktop
-   .NET 8 SDK
-   Node.js 20+
-   npm

## Executar com Docker

``` bash
docker compose up --build
```

Serviços: - Frontend: http://localhost - Backend:
http://localhost:5000 - pgAdmin: http://localhost:5050 - n8n:
http://localhost:5678 - PostgreSQL: localhost:5432

## Base de Dados

A imagem utilizada deve ser:

``` yaml
image: pgvector/pgvector:pg16
```

Depois de iniciar o PostgreSQL:

``` sql
CREATE EXTENSION IF NOT EXISTS vector;
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
```

Verificar:

``` sql
\dx
```

## Executar Frontend

``` bash
cd frontend
npm install
npm run dev
```

## Executar Backend

``` bash
cd backend
dotnet restore
dotnet ef database update
dotnet run
```

## Funcionalidades

-   Autenticação JWT
-   Gestão de agentes
-   Upload de documentos
-   Conhecimento (RAG)
-   Conversações
-   Histórico de conversas
-   Integração OpenAI
-   Embeddings com pgvector

## API

### Chat

-   GET /api/chat
-   GET /api/chat/{conversationId}
-   POST /api/chat
-   POST /api/chat/{conversationId}/messages
-   DELETE /api/chat/{conversationId}

### Knowledge

-   Upload de documentos com barra de progresso.

## Troubleshooting

### Erro "type vector does not exist"

``` sql
CREATE EXTENSION vector;
```

### Verificar extensões

``` sql
\dx
```

### Tailwind não aplica estilos

-   Confirmar `@import "tailwindcss";` em `styles.css`
-   Confirmar `import "./styles.css"` em `main.jsx`

### React mostra página em branco

-   Verificar Console do browser.
-   Testar renderização do `App`.
-   Confirmar que não existem erros nas rotas.

## Tecnologias

-   ASP.NET Core
-   Entity Framework Core
-   PostgreSQL
-   pgvector
-   Redis
-   React
-   Vite
-   Tailwind CSS v4
-   Axios
-   Docker
