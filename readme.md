# Recipe Optimization Application

## Local Docker Compose

The application is wired for a two-service local Docker setup:

- Frontend: http://localhost:4200
- Backend API: http://localhost:8080
- Backend health endpoint: http://localhost:8080/health

Start everything with:

```bash
docker-compose up --build
```

Stop everything with:

```bash
docker-compose down
```

The services are connected over a shared Docker network. The Angular frontend calls the backend through the configured runtime API URL, while the ASP.NET backend allows the frontend origin via CORS so the browser can make requests successfully during local development and Docker-based testing.

Health checks are enabled for both services in the compose configuration so Docker can wait until the backend is ready before starting the frontend and can report service health state cleanly.

## Run Locally Without Docker

Install the .NET 8 SDK, Node.js, and npm first. Run the backend and frontend in separate terminals from the repository root.

### Backend

```powershell
cd recipe-backend\Api
dotnet restore
dotnet run --launch-profile https
```

The API is available at https://localhost:7105, Swagger UI at https://localhost:7105/swagger, and its health endpoint at https://localhost:7105/health. If the local HTTPS development certificate is not trusted yet, run `dotnet dev-certs https --trust`.

### Frontend

```powershell
cd recipe-ui
npm ci
npm start
```

Open http://localhost:4200. The frontend's development API URL is configured in `src/assets/app-config.js`; its default, https://localhost:7105, matches the backend's HTTPS launch profile. Update `apiUrl` there if the backend uses a different address. The API's configured CORS origins include the default Angular development origin.

## Overview

This application solves the assessment problem of determining the optimal combination of recipes that can be produced from available ingredients in order to feed as many people as possible.

The solution uses:

- **Angular** for the frontend
- **ASP.NET Core / C#** for the backend
- **Minimal APIs** for the HTTP boundary
- **MediatR** for application request handling
- **Clean Architecture** for separation of concerns
- **SOLID, DRY and KISS** as guiding design principles

The assessment itself defines the core optimization problem but does not specify persistence, authentication, deployment infrastructure, API contracts, observability requirements or scalability targets. Those areas are therefore treated as architectural considerations and explicit assumptions rather than stated assessment requirements.

---

## AI Usage and Assistive Role

AI is used in this project as an assistive engineering tool rather than as an autonomous decision-maker. It supports  by helping to:

- accelerate architecture and design exploration
- draft clean abstractions, naming conventions and project structure
- generate boilerplate code, API contracts and model shapes
- suggest validation rules, edge cases and test scenarios
- support documentation, code review summaries and implementation walkthroughs
- compare alternative optimization strategies and trade-offs

All AI-generated output is reviewed against the actual assessment requirements, domain rules, security considerations and architectural principles described in this document. Human engineers remain responsible for correctness, maintainability, validation and final technical decisions. AI strengthens the development workflow, but it does not replace judgment, accountability or engineering ownership.

---

# Architecture

The high-level architecture is:

```text
┌──────────────────────────────────────────────┐
│                  Angular                     │
│                Presentation                  │
│                                              │
│ Components → Services → HTTP API             │
└──────────────────────┬───────────────────────┘
                       │ HTTPS / JSON
                       ▼
┌──────────────────────────────────────────────┐
│          ASP.NET Core Minimal API            │
│               Presentation                   │
│                                              │
│ Endpoint → HTTP concerns → MediatR           │
└──────────────────────┬───────────────────────┘
                       ▼
┌──────────────────────────────────────────────┐
│                 Application                  │
│                                              │
│ MediatR Requests / Handlers                  │
│ DTOs / Validation / Application interfaces   │
│ Use-case orchestration                       │
└──────────────────────┬───────────────────────┘
                       ▼
┌──────────────────────────────────────────────┐
│                   Domain                     │
│                                              │
│ Recipes / Ingredients / Rules                │
│ Domain invariants / Optimization             │
└──────────────────────┬───────────────────────┘
                       ▲
                       │
┌──────────────────────────────────────────────┐
│                Infrastructure                │
│                                              │
│ Persistence / External services / Telemetry  │
└──────────────────────────────────────────────┘
```

### Dependency direction

```text
Presentation → Application → Domain

Infrastructure → Application / Domain
```

The Domain should remain independent of Angular, ASP.NET Core, MediatR, databases, cloud providers and infrastructure implementations.

The purpose of this separation is to keep business rules independently testable and prevent framework concerns from becoming embedded in the core application logic.

---

# Frontend Architecture

Angular is responsible for presentation and user interaction.

A feature-oriented structure is appropriate:

```text
src/app/
├── core/
│   ├── services/
│   ├── interceptors/
│   └── models/
├── features/
│   └── optimization/
│       ├── components/
│       ├── services/
│       └── models/
└── shared/
    └── components/
```

### Frontend responsibilities

The Angular application is responsible for:

- presenting recipes and available ingredients
- collecting or presenting optimization inputs
- communicating with the backend API
- displaying optimization results
- displaying validation and API errors
- loading states
- accessibility
- responsive presentation

The frontend should not become the authoritative implementation of the optimization algorithm.

The backend owns the business calculation so that business rules are not duplicated across clients.

---

# Backend Architecture

The backend uses ASP.NET Core Minimal APIs.

A representative structure is:

```text
src/
├── RecipeOptimization.Api/
│   ├── Endpoints/
│   ├── Extensions/
│   └── Program.cs
│
├── RecipeOptimization.Application/
│   ├── Abstractions/
│   ├── Behaviors/
│   └── Features/
│       └── Optimization/
│
├── RecipeOptimization.Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Services/
│   └── Rules/
│
└── RecipeOptimization.Infrastructure/
    ├── Persistence/
    ├── Services/
    └── DependencyInjection.cs
```

The exact folder structure can evolve. The important requirement is that responsibilities and dependency direction remain clear.

---

# Minimal API

Minimal APIs provide the HTTP presentation boundary without unnecessary controller ceremony.

The intended request flow is:

```text
HTTP Request
    ↓
Minimal API Endpoint
    ↓
MediatR
    ↓
Application Handler
    ↓
Domain / Optimization Engine
    ↓
Result
    ↓
HTTP Response
```

Endpoints should remain thin.

They should primarily handle:

- HTTP request binding
- HTTP-level concerns
- invoking the application use case
- returning appropriate HTTP responses
- mapping application results to transport responses

The optimization algorithm should not live inside an endpoint.

---

# MediatR

MediatR is used at the application boundary.

The optimization operation can be represented as:

```text
OptimizeRecipesRequest
        ↓
OptimizeRecipesHandler
        ↓
Optimization Engine
        ↓
Optimization Result
```

MediatR also provides a suitable pipeline for cross-cutting application concerns such as:

- validation
- logging
- performance measurement
- tracing
- authorization where required

MediatR is not intended to replace domain design.

The domain and optimization logic should remain independently testable without requiring an HTTP request or MediatR.

---

# Domain Model

The domain should represent concepts that actually exist in the problem.

Potential concepts include:

```text
Ingredient
Recipe
RecipeIngredient
OptimizationRequest
OptimizationResult
RecipeSelection
```

The domain is responsible for business rules and invariants.

Examples:

- ingredient quantities cannot be negative
- recipe requirements cannot be negative
- recipes are whole units
- recipes cannot consume more ingredients than are available
- the optimization objective is to maximize people fed

HTTP request/response models should not automatically become domain models simply because their properties currently look similar.

---

# Optimization Design

The requirement is to determine an **optimal** combination rather than simply any combination that can be produced.

A greedy strategy cannot automatically be assumed to find the global optimum because selecting the most immediately valuable recipe can consume resources needed by a better overall combination.

The optimization responsibility should therefore be isolated:

```text
IRecipeOptimizer
       ↓
RecipeOptimizer
       ├── Available ingredients
       ├── Recipe definitions
       └── Optimization objective
```

For the small, bounded assessment input, a deterministic bounded optimization/search approach is appropriate.

If the number of recipes, ingredients or quantities becomes substantially larger, the algorithm should be reconsidered rather than allowing computational complexity to grow without control.

---

# Architecture Decisions

## Decision 001 — No ingredient persistence

**Decision:**

Available ingredient quantities are supplied by the user for each optimization request.

**Why:**

The assessment requires determining the optimal recipe combination from available ingredients but does not require inventory management or historical stock.

**Alternative:**

Persist ingredient inventory using a database.

**Trade-off:**

We cannot track stock across requests, but we avoid introducing persistence that isn't required by the business problem.

**Assessment benefit:**

Demonstrates scope discipline, stateless design, separation of concerns, and avoidance of unnecessary infrastructure.

## Decision 002 — Optimization is stateless

**Decision:**

The optimizer operates entirely on the data supplied in the request.

**Why:**

The calculation does not require previous requests or shared application state.

**Trade-off:**

The caller must provide current ingredient quantities every time.

**Assessment benefit:**

Makes the core business logic deterministic, testable and horizontally scalable.

## Decision 003 — User supplies quantities, application determines combination

**Decision:**

The caller supplies ingredient quantities; the optimization engine determines recipe quantities.

**Why:**

The optimization algorithm is the core business responsibility of the application.

**Assessment benefit:**

Keeps business logic inside the domain/application boundary rather than pushing decisions onto the client.

---

# API Design

A possible API boundary is:

```http
POST /api/v1/optimizations
```

The request represents:

- available ingredients
- recipes
- recipe ingredient requirements
- people fed by each recipe

The response can provide:

- maximum people fed
- selected recipe quantities
- remaining ingredients where useful
- multiple optimal solutions where applicable

API versioning becomes important if external consumers depend on the service.

The exact contract remains an implementation decision because the assessment does not specify a required API contract.

---

# Validation

Validation is both a correctness and security concern.

Application/API validation should cover:

- required values
- numeric ranges
- collection sizes
- malformed requests
- duplicate or conflicting definitions

Domain validation should protect business invariants.

Frontend validation can improve user experience, but backend validation remains authoritative.

Validation rules should not be unnecessarily duplicated across Angular, endpoints, handlers and domain objects.

---

# Error Handling

The API should use centralized error handling and a standard error representation such as `ProblemDetails`.

Example:

```json
{
  "title": "Invalid optimization request",
  "status": 400,
  "detail": "Ingredient quantity cannot be negative.",
  "traceId": "..."
}
```

Clients should not receive:

- stack traces
- internal exception details
- database connection information
- infrastructure-specific diagnostics

Expected validation failures should be distinguished from unexpected application failures.

---

# Security

Security is considered even though the assessment does not explicitly require authentication.

## Transport Security

Production communication should use HTTPS.

## Input Protection

All externally supplied input should be treated as untrusted.

The API should enforce reasonable:

- request-size limits
- ingredient quantity limits
- recipe count limits
- ingredient count limits
- string length limits

This is particularly important for the optimization endpoint because unrestricted inputs could create an excessively large computational search space.

## CORS

Production CORS should allow only known frontend origins.

Wildcard CORS should not be used as a production convenience.

## Authentication and Authorization

Authentication and authorization are not currently specified.

If the application later introduces users, administrators, saved plans, recipe management or organizational data, identity and authorization should become explicit application concerns.

## Secrets

Secrets must not be committed to source control.

Production credentials and keys should be provided through the deployment environment or a managed secret store.

## Dependency Security

Application and frontend dependencies should be monitored and scanned for known vulnerabilities.

---

# Observability

A production deployment should provide three primary observability signals:

```text
Logs
Metrics
Traces
```

## Structured Logging

Useful structured fields may include:

```text
traceId
requestId
operation
duration
recipeCount
ingredientCount
resultCount
```

Sensitive input should not be logged unnecessarily.

## Metrics

Potential application metrics include:

```text
optimization_requests_total
optimization_failures_total
optimization_duration
optimization_solution_count
```

## Distributed Tracing

OpenTelemetry can provide tracing across:

```text
Angular
   ↓
API
   ↓
Application
   ↓
External dependencies
```

This becomes increasingly useful if the application evolves into a distributed system.

---

# Health and Readiness

The backend should expose deployment-friendly health endpoints such as:

```text
/health/live
/health/ready
```

**Liveness** indicates that the process is running.

**Readiness** indicates that the application is ready to receive traffic and, where relevant, required dependencies are available.

This distinction becomes important when running behind a load balancer or container orchestration platform.

---

# Data and Persistence

The assessment does not specify a database or persistent storage requirement.

The current calculation is naturally stateless:

```text
Request → Calculate → Response
```

Therefore a database should not be introduced merely to make the architecture appear more enterprise-oriented.

If persistence is later required, it could support concepts such as:

- recipes
- ingredients
- saved optimization plans
- users
- audit history
- organizational configuration

EF Core and a relational database could then be evaluated based on the actual requirements.

A generic repository should not be introduced automatically.

---

# ACID and Concurrency

For the current stateless calculation, ACID is not central because no transactional state is being changed.

If the application later performs operations such as:

```text
Create plan
    ↓
Reserve ingredients
    ↓
Save plan
    ↓
Record transaction
```

then the following become relevant:

### Atomicity

The operation should not leave partially completed state.

### Consistency

Business invariants must remain valid after a transaction.

### Isolation

Concurrent operations must not incorrectly consume the same inventory.

### Durability

Committed information must survive application or infrastructure failure.

Concurrency control becomes particularly important if inventory becomes shared mutable state.

---

# Larger-Scale Application Considerations

The assessment itself is small. These are future considerations rather than requirements to implement immediately.

A larger deployment could evolve toward:

```text
                    API Gateway
                        │
              ┌─────────┴─────────┐
              │                   │
           Angular            API instances
                                  │
                       ┌──────────┼──────────┐
                       │          │          │
                     API 1      API 2      API 3
                                  │
                       ┌──────────┴──────────┐
                       │                     │
                    Database             Cache/Queue
```

Potential concerns include:

- horizontal scaling
- distributed caching
- database scaling
- concurrency
- asynchronous processing
- rate limiting
- centralized identity
- audit logging
- distributed tracing
- background processing
- resilience
- fault isolation
- disaster recovery
- operational monitoring

These should be introduced only when justified by actual workload and requirements.

---

# Scaling the Optimization Engine

The optimization algorithm is the most likely computational bottleneck if the problem becomes much larger.

Potential future approaches include:

- improved bounded search and pruning
- dynamic programming where applicable
- integer programming
- dedicated optimization solvers
- asynchronous optimization jobs
- job queues
- caching repeated requests
- computational limits per request

The correct approach depends on actual input size and workload.

For the assessment, the solution should remain proportional to the supplied problem.

---

# Resilience

In a larger production system, external dependencies should be treated as failure-prone.

Potential techniques include:

- timeouts
- retries where safe
- circuit breakers
- bulkheads
- graceful degradation
- idempotency

Retries should not be applied blindly. Retrying a read may be safe in some situations, while retrying a state-changing operation can cause duplicate effects unless the operation is idempotent.

---

# Deployment Architecture

A possible production topology is:

```text
                     Internet
                        │
                        ▼
                 Load Balancer /
                  API Gateway
                    /                         ▼        ▼
              Angular     ASP.NET API
                             │
                     ┌───────┴───────┐
                     ▼               ▼
                  Database         Cache
                     │
                     ▼
                Observability
```

Potential deployment platforms include:

- Azure App Service
- Azure Container Apps
- AWS ECS/Fargate
- Google Cloud Run
- Kubernetes where organizational scale actually justifies it

Kubernetes should not be introduced merely because the application is described as enterprise-level.

---

# CI/CD Considerations

A production pipeline should validate the application before deployment.

```text
Pull Request
     │
     ├── Build
     ├── Unit Tests
     ├── Integration Tests
     ├── Frontend Tests
     ├── Static Analysis
     ├── Dependency Scanning
     └── Security Checks
            │
            ▼
          Merge
            │
            ▼
      Build Artifact
            │
            ▼
         Staging
            │
            ▼
       Verification
            │
            ▼
       Production
```

Production should use known, immutable build artifacts.

Environment-specific configuration should be provided during deployment rather than producing different application builds for each environment.

---

# Deployment Strategy at Scale

A larger environment could use:

```text
Development
     ↓
Integration / Test
     ↓
Staging
     ↓
Production
```

Production strategies could include:

- rolling deployment
- blue/green deployment
- canary deployment

The appropriate strategy depends on availability requirements, infrastructure and operational maturity.

---

# Assumptions and Open Questions

The assessment defines the core optimization problem but does not define several application-level requirements.

These are intentionally tracked rather than silently invented.

| ID | Question / Assumption | Current Position |
|---|---|---|
| A-001 | What does "optimal" mean? | Maximize people fed |
| A-002 | Can recipes be fractional? | No; whole recipes |
| A-003 | Can ingredients be fractional? | No; whole quantities |
| A-004 | Can multiple optimal solutions exist? | Yes |
| A-005 | Should all equally optimal solutions be returned? | To be confirmed |
| A-006 | Is persistence required? | Not specified |
| A-007 | Is authentication required? | Not specified |
| A-008 | Are recipes fixed or configurable? | Not specified |
| A-009 | What performance limits are expected? | Not specified |
| A-010 | What deployment platform is required? | Not specified |
| A-011 | Are users or organizations required? | Not specified |
| A-012 | Is audit history required? | Not specified |
| A-013 | Is inventory shared between requests/users? | Not specified |
| A-014 | Are optimization requests independent? | Assumed yes |
| A-015 | Is inventory reservation required? | Not specified |

Any clarification from the assessment owner should supersede the corresponding assumption.

---

# Design Principles

### Keep the Domain Independent

Business rules should not know about frameworks.

### Keep Endpoints Thin

Minimal API endpoints should delegate to application use cases.

### Use MediatR for Application Boundaries

MediatR provides request handling and pipeline behavior; it is not a replacement for domain design.

### Prefer Composition Over Unnecessary Inheritance

Inheritance should only be introduced where a genuine substitutable relationship exists.

### Abstract Where Change Is Expected

Do not create interfaces simply to increase abstraction.

### Validate at the Correct Boundary

User input should be validated before entering business logic, while domain invariants remain protected by the domain.

### Secure by Default

External input, secrets, transport, dependencies and resource consumption should all be treated as security concerns.

### Design for Observability

Production behavior should be diagnosable through logs, metrics and traces.

### Scale Deliberately

Distributed components should be introduced when workload, reliability or organizational requirements justify them.

---
