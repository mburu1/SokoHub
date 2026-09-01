# AGENTS.md

This file provides guidance to Codex (Codex.ai/code) when working with code in this repository.

## Common Commands

### Backend (.NET 10)
- **Build**: `dotnet build`
- **Run API**: `dotnet run --project backend/src/SokoHub.Api/SokoHub.Api.csproj`
- **Run All Tests**: `dotnet test`
- **Run Single Test**: `dotnet test --filter "FullyQualifiedName=Namespace.ClassName.MethodName"`
- **Clean**: `dotnet clean`

### Frontend (Nuxt.js)
- **Install**: `npm install` (in `frontend/` directory)
- **Dev Mode**: `npm run dev`
- **Build**: `npm run build`

### Infrastructure & Environment
- **Start Environment**: `docker-compose up -d`
- **Stop Environment**: `docker-compose down`
- **M-Pesa Local Dev**: Use the `ngrok` service in `docker-compose.yml` to tunnel callbacks from Daraja API to the local API.

## Architecture Overview

SokoHub is a production-grade multi-vendor e-commerce marketplace designed for the East African market, with specialized support for M-Pesa (STK Push).

### High-Level Structure
- `backend/`: .NET 10 implementation using Clean Architecture/DDD.
    - `SokoHub.Api`: Entry point, Controllers, Middleware, Scalar API docs.
    - `SokoHub.Application`: CQRS, Use Cases, DTOs, Validation.
    - `SokoHub.Domain`: Core Entities, Aggregates, Value Objects, Domain Events.
    - `SokoHub.Infrastructure`: Implementation of persistence, messaging, and external API clients (Daraja, Stripe).
    - `SokoHub.Identity`: Authentication (JWT), RBAC, Refresh Tokens.
    - `SokoHub.Notifications` & `SokoHub.Reporting`: Specialized background workers and analytics services.
- `frontend/`: Nuxt.js (Vue 3) SSR application with Pinia for state management.
- `infra/`: Terraform (IaC) and Kubernetes/Helm manifests for deployment to AKS/EKS.
- `docs/`: Architecture Decision Records (ADRs), UML diagrams (PlantUML), and API specs.

### Polyglot Persistence Strategy
The system uses multiple databases based on the bounded context:
- **MSSQL**: Primary relational store (Users, Orders, Products).
- **PostgreSQL**: Analytics and reporting read store.
- **MySQL**: Promotions and coupon engine.
- **MongoDB**: Audit trails and raw API payloads (e.g., Daraja callbacks).
- **Oracle**: Enterprise financial ledger for commissions and fees.
- **Redis**: Distributed caching, session management, and STK Push TTL.

### Messaging & Events
- **RabbitMQ**: Used for internal domain events (e.g., `OrderConfirmed` $\rightarrow$ `TriggerNotification`).
- **Kafka**: Used for high-throughput payment event streaming and audit log pipelines.
