# LedgerAI.Api: .NET 9 Modular Monolith Backend & AI Web API

[![.NET 9](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16+-blue.svg)](https://www.postgresql.org/)
[![EF Core](https://img.shields.io/badge/EF%20Core-9.0-blueviolet.svg)](https://learn.microsoft.com/en-us/ef/core/)
[![Scalar OpenAPI](https://img.shields.io/badge/API%20Docs-Scalar-green.svg)](https://scalar.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](../LICENSE)

The official backend service for **LedgerAI** — a multi-tenant B2B wholesale Enterprise Resource Planning (ERP), inventory management engine, and digital *Udhaar Khata* (credit ledger). Engineered in C# 13 on `.NET 9`, this project encapsulates deterministic transactional accounting, PostgreSQL 16+ Row-Level Security (RLS), and an isolated multi-agent AI pipeline for natural language order intake.

---

## Table of Contents
1. [Backend Architectural Overview](#1-backend-architectural-overview)
2. [HTTP Middleware & Request Pipeline](#2-http-middleware--request-pipeline)
3. [Source Code Hierarchy & Organization](#3-source-code-hierarchy--organization)
4. [Domain Feature Modules](#4-domain-feature-modules)
5. [Persistence Layer & Strict DTO Separation](#5-persistence-layer--strict-dto-separation)
6. [Multi-Agent AI Orchestration & Tool-Calling Airgap](#6-multi-agent-ai-orchestration--tool-calling-airgap)
7. [External Communication & Background Workers](#7-external-communication--background-workers)
8. [API Endpoints, Webhooks & SignalR Hubs](#8-api-endpoints-webhooks--signalr-hubs)
9. [Configuration & Environment Variables](#9-configuration--environment-variables)
10. [Local Build, Run & Verification Guide](#10-local-build-run--verification-guide)

---

## 1. Backend Architectural Overview

`LedgerAI.Api` is designed as a **Single-Project Modular Monolith**. It groups domain logic, entities, calculators, and services into discrete bounded business contexts (`src/Modules/`), while housing shared persistence abstractions in `src/Data/`, cross-cutting security and global primitives in `src/Core/`, and isolated AI agent workflows in `src/AI/`.

```mermaid
flowchart TD
    subgraph ClientRequests["Inbound Client Requests"]
        HTTP["HTTPS REST Calls\n(React PWA / External)"]
        WS["SignalR WSS Connections\n(/hubs/ledger)"]
        WA["Meta WhatsApp Webhooks\n(HMAC Verified)"]
    end

    subgraph ApiHost["LedgerAI.Api Host (.NET 9)"]
        subgraph Pipeline["Middleware & Routing"]
            Tenant["TenantResolutionMiddleware (ITenantProvider)"]
            Router["Controller Action / SignalR Dispatcher"]
        end

        subgraph CoreModules["Domain Feature Modules"]
            Cat["Catalog Module"]
            Inv["Inventory Module (FEFO)"]
            Acc["Accounts Module (Udhaar Khata)"]
            Ord["Orders Module (State Machine)"]
            Bill["Invoicing Module (00/01 Vouchers)"]
        end

        subgraph AiSubsystem["Isolated AI Subsystem"]
            Sanitize["Text Sanitizer Pipeline"]
            Agents["IntentExtractor & Supervisor Agents"]
            Tools["C# Tool Wrappers (Strict Airgap)"]
        end

        subgraph DataLayer["Persistence Layer"]
            UoW["Unit of Work & Generic Repositories"]
            Interceptor["TenantRlsInterceptor (set_config app.current_tenant)"]
            DbContext["LedgerDbContext (EF Core 9)"]
        end

        subgraph Background["Background Services"]
            OutboxWorker["OutboxDispatcherWorker (BackgroundService)"]
        end
    end

    subgraph Postgres["PostgreSQL 16+ Database"]
        Tables["21 Composite-Tenant Tables"]
        RLS["FORCE ROW LEVEL SECURITY"]
        Locks["Atomic Stored Procedures (FOR UPDATE)"]
    end

    HTTP & WS & WA --> Tenant --> Router
    Router --> CoreModules
    Router -.-> AiSubsystem
    AiSubsystem --> Tools --> CoreModules
    CoreModules --> UoW --> Interceptor --> DbContext --> Postgres
    Postgres --> OutboxWorker
```

---

## 2. HTTP Middleware & Request Pipeline

Every incoming HTTP request passes through tenant resolution before controllers and SignalR endpoints:

```mermaid
sequenceDiagram
    autonumber
    actor Client as Client / Meta Webhook
    participant Host as Kestrel Host
    participant Tenant as TenantContext / ITenantProvider
    participant RLS as TenantRlsInterceptor
    participant Controller as REST Controller / LedgerHub
    participant Service as Domain Service / UnitOfWork
    participant DB as PostgreSQL 16+ Engine

    Client->>Host: HTTP Request (X-Tenant-Id)
    Host->>Tenant: Resolve X-Tenant-Id, then tenant_id / TenantId claims
    Tenant->>Controller: Set scoped TenantContext and route request
    Controller->>Service: Execute business operation (DTO Input)
    Service->>RLS: Open connection inside transaction block
    RLS->>DB: set_config('app.current_tenant', tenantId, false)
    Service->>DB: Execute EF Core query or atomic lock function
    Note over DB: PostgreSQL checks FORCE RLS policies
    DB-->>Service: Return isolated data
    Service-->>Controller: Return Result<T> / Domain Entity
    Controller-->>Client: Return ApiResponseDto<T> JSON (200 OK / 400 Bad Request)
```

`TenantResolutionMiddleware` prefers `X-Tenant-Id`, then checks `tenant_id` and `TenantId` claims. `TenantContext` is scoped per request; `TenantRlsInterceptor` uses its tenant ID to set `app.current_tenant` on an opened database connection. The project currently does not register authentication middleware or provide login/auth endpoints, so those claims and a runtime current-user identity are not presently populated by the API. Do not treat a client-supplied tenant header as proof of tenant membership in a production authentication design.

---

## 3. Source Code Hierarchy & Organization

All source code lives inside `src/` inside the `LedgerAI.Api` project root:

```text
LedgerAI.Api/
├── LedgerAI.Api.csproj                         # .NET 9 Web API project file
├── LedgerAI.Api.sln                            # Visual Studio solution file
├── Program.cs                                  # Kestrel bootstrapping, DI container & middleware wiring
├── appsettings.json                            # Base configuration templates
├── appsettings.Development.json                # Local development settings (ignored by Git)
├── Properties/
│   └── launchSettings.json                     # Port 7274 (HTTPS) & Port 5284 (HTTP)
│
└── src/
    ├── Core/                                   # Cross-Cutting Concerns & Domain Primitives
    │   ├── Common/                             # BaseEntity, Result<T>, PagedResult<T>, DomainException
    │   ├── Interfaces/                         # IRepository<T>, IUnitOfWork, ITenantProvider
    │   ├── Security/                           # JwtSettings, TenantContext, AppRoles
    │   └── Enums/                              # OrderStatus, AccountType, InvoiceType, RiskTier, PaymentTerms, etc.
    │
    ├── Data/                                   # Persistence Layer & Strict DTO Separation
    │   ├── Context/                            # LedgerDbContext.cs (21 DbSets & ModelBuilder configuration)
    │   ├── Configurations/                     # Fluent API configuration folder
    │   ├── Repositories/                       # Generic Repository<T> EF Core implementation
    │   ├── UnitOfWork/                         # UnitOfWork.cs managing EF Core ACID transactions
    │   ├── Interceptors/                       # TenantRlsInterceptor.cs (PostgreSQL RLS connection hook)
    │   ├── Migrations/                         # EF Core Code-First migration output
    │   ├── Seeders/                            # DbSeeder.cs shell for baseline tenant data
    │   └── Dtos/                               # Strictly decoupled Request/Response contracts
    │       ├── Common/                         # ApiResponseDto.cs
    │       ├── Catalog/                        # CategoryDto, UnitOfMeasureDto, ProductDto
    │       ├── Inventory/                      # WarehouseDto, StockBatchDto, StockMovementDto
    │       ├── Accounts/                       # AccountDto, LedgerEntryDto, PaymentAllocationDto
    │       ├── Orders/                         # OrderDto, CreateOrderDto
    │       ├── Invoicing/                      # InvoiceDto, CreateInvoiceDto
    │       └── AI/                             # NormalizedIntentDto, AiDecisionLogDto
    │
    ├── Modules/                                # Bounded Domain Feature Modules
    │   ├── TenantAdmin/                        # Tenant.cs, User.cs, ITenantService, TenantService
    │   ├── Catalog/                            # Category.cs, UnitOfMeasure.cs, Product.cs, ICatalogService, CatalogService
    │   ├── Inventory/                          # Warehouse.cs, StockItem.cs, StockBatch.cs, StockMovement.cs, StockReservation.cs
    │   ├── Accounts/                           # Account.cs, LedgerEntry.cs, LedgerAllocation.cs, IAccountService, AccountService
    │   ├── Orders/                             # Order.cs, OrderItem.cs, IOrderService, OrderService
    │   ├── Invoicing/                          # Invoice.cs, InvoiceItem.cs, InvoiceCharge.cs, IInvoiceService, InvoiceService
    │   └── Operations/                         # InboundMessage.cs, OutboxEvent.cs, AuditLog.cs
    │
    ├── AI/                                     # Multi-Agent AI Orchestration Subsystem
    │   ├── Agents/                             # IntentExtractorAgent, SupervisorAgent, OrderAgent, LedgerAgent
    │   ├── Tools/                              # OrderTools, LedgerTools, CatalogTools (Strict Airgap C# Wrappers)
    │   ├── Pipelines/                          # TextSanitizerPipeline, IntentValidationPipeline
    │   ├── Monitoring/                         # IAiTelemetryService, AiDecisionLogger (SignalR live trace)
    │   ├── Prompts/                            # IntentExtractionPrompt.txt (bilingual prompt templates)
    │   └── Routers/                            # LlmFailoverRouter (Groq LPU -> HuggingFace fallback)
    │
    ├── Communication/                          # External Messaging & Asynchronous Workers
    │   ├── WhatsApp/                           # WhatsAppClient (HttpClient), WebhookSignatureVerifier
    │   └── Workers/                            # OutboxDispatcherWorker (BackgroundService)
    │
    └── Api/                                    # Presentation Layer
        ├── Controllers/                        # SystemMetadataController, Catalog, Inventory, Accounts, Orders, Invoices, AiChat
        ├── Webhooks/                           # WhatsAppWebhookController (Meta ingress & verification)
        └── Hubs/                               # LedgerHub (SignalR WebSocket hub for UI telemetry & alerts)
```

---

## 4. Domain Feature Modules

| Module | Core Entities | Responsibilities & Capabilities |
| :--- | :--- | :--- |
| **`TenantAdmin`** | `Tenant`, `User` | Tenant isolation root, onboarding, user provisioning, and 5-tier RBAC (`Owner`, `Admin`, `Manager`, `Operator`, `Viewer`). |
| **`Catalog`** | `Category`, `UnitOfMeasure`, `Product` | Master catalog, two-tier UOM conversions (`Base` + `Secondary` pack), fractional constraints, and JSONB Roman Urdu aliases. |
| **`Inventory`** | `Warehouse`, `StockItem`, `StockBatch`, `StockMovement`, `StockReservation` | Multi-warehouse FEFO batch tracking, atomic order reservations (`fn_reserve_batch_stock`), and trigger-maintained aggregate stock. |
| **`Accounts`** | `Account`, `LedgerEntry`, `LedgerAllocation` | Unified party register (`CUSTOMER`, `SUPPLIER`, `TRANSPORTER`), *Udhaar Khata* running balances, single-reversal entries, and automated FIFO payment allocation. |
| **`Orders`** | `Order`, `OrderItem` | B2B sales order state machine (`PENDING_APPROVAL`, `VALIDATED`, `COMMITTED`, `REJECTED`, `CANCELLED`), confidence scoring, and credit limit exposure checks. |
| **`Invoicing`** | `Invoice`, `InvoiceItem`, `InvoiceCharge` | Dual-series vouchers (`00` Purchase / `01` Sale), landed cost proration into WAC, stock commits, and localized wholesale charges (`Bilty`, `Mazdoori`, `WHT`, `Kasar`). |
| **`Operations`** | `InboundMessage`, `OutboxEvent`, `AuditLog` | Inbound WhatsApp payload logging with `wamid` deduplication, Transactional Outbox event queue, and immutable audit trails. |

---

## 5. Persistence Layer & Strict DTO Separation

### Database Setup & Migration Workflow

LedgerAI uses a **hybrid Code-First + SQL Hardening** workflow.

1. Run EF Core migrations to create/update the baseline 21 `snake_case` tables, computed columns, and indexes:
   ```bash
   dotnet ef database update
   ```
2. When creating a fresh database or resetting the schema through the EF Core Code-First migration path, run the hardening patch immediately afterward:
   ```bash
   psql "$CONNECTION_STRING" -f docs/schemas/patch-update-efcore.sql
   ```

`docs/schemas/patch-update-efcore.sql` is needed **only for the EF Core Code-First migration path**. It is not needed when bootstrapping directly from the complete standalone SQL schema (`Docs/Schemas/LedgerAI_Schema_v2.sql`). The patch installs `FORCE ROW LEVEL SECURITY` policies, `stock_items` synchronization and direct-write guards, immutable `audit_logs`, ledger allocation validation, security-invoker views, composite tenant/batch-tuple foreign keys, `CHECK` constraints, and atomic `fn_*` stock/credit lock functions.

Verify RLS and FORCE RLS on the 21 tenant tables:

```sql
SELECT c.relname AS table_name,
       c.relrowsecurity AS rls_enabled,
       c.relforcerowsecurity AS rls_forced
FROM pg_class c
JOIN pg_namespace n ON n.oid = c.relnamespace
WHERE n.nspname = 'public'
  AND c.relkind = 'r'
  AND c.relname IN (
    'tenants','users','categories','units_of_measure','products','warehouses',
    'stock_items','stock_batches','stock_movements','orders','order_items',
    'stock_reservations','accounts','invoices','invoice_items','invoice_charges',
    'ledger_entries','ledger_allocations','inbound_messages','outbox_events','audit_logs'
  )
ORDER BY c.relname;
```

Before seeding or manually inserting tenant-scoped data, set the session context:
`SELECT set_config('app.current_tenant', '<tenant-id>', false);`

### Per-Module Entities & EF Configuration

Domain entities live inside bounded modules under `src/Modules/*/`. Each module may own entity-specific `IEntityTypeConfiguration<T>` classes beside the module or shared mappings may live in `src/Data/Configurations/`. All mappings are discovered through:

```csharp
modelBuilder.ApplyConfigurationsFromAssembly(typeof(LedgerDbContext).Assembly);
```

Keep existing table names, column names, precision, conversions, and computed columns unchanged; configuration moves must not introduce migration drift. The convention is documented in `src/Data/Configurations/README.md`.

### Database & Connection Logging Layer

`src/Data/Interceptors/` contains the tenant RLS connection interceptor, while `src/Data/Configuration/` contains the startup readiness check. The layer enforces tenant context, logs connection open/close/failure events and elapsed database target information, emits warnings for missing tenant context, and reports pending EF Core migrations at startup through `ILogger<T>`. Secrets and connection-string credentials are never logged.

### Master Tenant & Mock Data Seed

With `SeedData:Enabled=true` and `SeedData:IncludeMockData=true` in `appsettings.Development.json`, development startup runs `DbSeeder` automatically. On a fresh database it creates tenant `MASTER-PKR` (`11111111-1111-4111-8111-111111111111`) and these users:

| Role | User code | Email | Deterministic ID |
| :--- | :--- | :--- | :--- |
| Owner | `OWNER-001` | `owner@ledgerai.local` | `8a47cb3a-79f8-2abd-5636-20ed11d64497` |
| Admin | `ADMIN-001` | `admin@ledgerai.local` | `07a77b60-b2ff-8ded-bd34-529f131d10e2` |
| Operator | `OPERATOR-001` | `operator@ledgerai.local` | `1a804678-0fcc-faec-7d0c-c395d49209be` |

User IDs are `md5('user:<USER-CODE>')::uuid`, with the Owner key `user:owner`. The default local seed password is `LedgerAI@2026!`; the seeder stores it as a salted PBKDF2-HMAC-SHA256 hash (100,000 iterations, 32-byte output, 16-byte random salt, encoded as `base64(salt):base64(hash)`). That is the only password-hashing implementation currently in the codebase; there is no login handler or password-verification service yet. New seed records use the specified deterministic IDs; existing tenant/user rows found by their natural keys are retained rather than having primary keys rewritten.

The seeder also inserts interlinked catalog, inventory, account, order, invoice, ledger, inbound-message, outbox, and audit records. It sets `app.current_tenant` transaction-locally before tenant-scoped inserts.

### Entity-DTO Decoupling Standard
Internal EF Core entity models (`src/Modules/*/Entities`) are strictly decoupled from external consumers:
* **Controllers & Hubs** only accept and return strongly typed DTOs (`src/Data/Dtos/*`).
* **Entities** are never serialized directly into HTTP responses, preventing accidental leakage of internal keys, password hashes, or sensitive financial state.

### PostgreSQL Row-Level Security Connection Interceptor
The `TenantRlsInterceptor` hooks into EF Core's connection lifecycle:
```csharp
public override async Task ConnectionOpenedAsync(
    DbConnection connection,
    ConnectionEndEventData eventData,
    CancellationToken cancellationToken = default)
{
    var tenantId = _tenantProvider.GetCurrentTenantId();
    if (tenantId.HasValue && tenantId.Value != Guid.Empty)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT set_config('app.current_tenant', @tenantId, false)";
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@tenantId";
        parameter.Value = tenantId.Value.ToString();
        command.Parameters.Add(parameter);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
    await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
}
```

---

## 6. Multi-Agent AI Orchestration & Tool-Calling Airgap

The AI subsystem operates in a secure sandbox isolated from direct database access:

```mermaid
flowchart LR
    subgraph Inbound["1. Ingress"]
        Msg["Raw Roman Urdu / Voice Note"] --> STT["Whisper STT"]
    end

    subgraph Pipeline["2. Pipeline Sanitization & Parsing"]
        STT --> Clean["TextSanitizerPipeline\n(Injection Filter)"]
        Clean --> LLM["IntentExtractorAgent\n(Groq LPU llama-3.3-70b)"]
        LLM --> Gate["IntentValidationPipeline\n(Confidence >= 0.85)"]
    end

    subgraph Airgap["3. Strict Airgap Tool Dispatch"]
        Gate --> Supervisor["SupervisorAgent"]
        Supervisor --> Tools["Typed C# Tools\n(OrderTools, LedgerTools, CatalogTools)"]
    end

    subgraph Core["4. Deterministic Business Execution"]
        Tools --> Services["OrderService / AccountService"]
        Services --> DB["PostgreSQL 16+ Engine\n(Atomic Locks & Force RLS)"]
    end

    subgraph Observability["5. Observability"]
        Supervisor -.-> Telemetry["AiDecisionLogger"]
        Telemetry -.-> Hub["SignalR LedgerHub (Live Trace)"]
    end
```

* **No Raw SQL / No DbContext:** Agents invoke typed methods in `src/AI/Tools/`, which invoke domain services (`IOrderService`, `IAccountService`).
* **Resilience & Key Rotation:** `LlmFailoverRouter` (backed by Polly v8) rotates Groq API keys on HTTP `429` rate limits and falls back to HuggingFace on timeouts.
* **Human-in-the-Loop Gating:** If `ai_confidence_score < 0.85` or the customer breaches their `credit_limit`, the order is flagged `requires_approval = true` with status `PENDING_APPROVAL`.

---

## 7. External Communication & Background Workers

### Meta WhatsApp Cloud API Client
* **Inbound:** Handled via `WhatsAppWebhookController` at `/api/webhooks/whatsapp`. Inbound payloads are verified using HMAC-SHA256 (`WebhookSignatureVerifier`) and deduplicated via unique `wamid` idempotency keys.
* **Outbound:** `WhatsAppClient` handles outbound template notifications and interactive order confirmation messages.

### Transactional Outbox Background Worker
`OutboxDispatcherWorker` runs as an ASP.NET Core `BackgroundService`:
1. Periodically polls the `outbox_events` table for unprocessed events (`WHERE processed_at IS NULL AND available_at <= now()`).
2. Dispatches notifications (SignalR alerts, WhatsApp confirmation messages).
3. Marks events as processed or applies exponential backoff on transient failure.
4. Ensures database transactions commit reliably without distributed 2-phase commit overhead.

---

## 8. API Endpoints, Webhooks & SignalR Hubs

### Summary of REST Endpoints

| Area | HTTP Method | Endpoint | Description |
| :--- | :---: | :--- | :--- |
| **System** | `GET` | `/api/metadata` | Health, version, environment, and database engine status. |
| **System** | `GET` | `/health` | ASP.NET Core health check endpoint. |
| **Catalog** | `GET` | `/api/catalog/products` | Retrieve active products with pricing and UOM details. |
| **Inventory**| `GET` | `/api/inventory/warehouses` | List active godowns/warehouses. |
| **Accounts** | `GET` | `/api/accounts` | Query party registers, current balances, and risk tiers. |
| **Orders** | `GET` | `/api/orders` | Retrieve wholesale sales orders and approval states. |
| **Invoicing**| `GET` | `/api/invoices` | List dual-series purchase (`00`) and sale (`01`) invoices. |
| **AI Chat** | `POST` | `/api/aichat/message` | Ingest informal natural language text for intent extraction. |
| **Webhooks** | `GET` | `/api/webhooks/whatsapp` | Meta Cloud API webhook subscription challenge verification. |
| **Webhooks** | `POST` | `/api/webhooks/whatsapp` | Inbound WhatsApp messages, voice notes, and delivery receipts. |
| **SignalR** | `WSS` | `/hubs/ledger` | Live WebSocket hub for ERP stock changes and AI decision logs. |

### Interactive Documentation
* **Scalar Interactive UI:** `http://localhost:5284/scalar/v1` (or `https://localhost:7274/scalar/v1`)
* **OpenAPI 3.x Document:** `http://localhost:5284/openapi/v1.json`

---

## 9. Configuration & Environment Variables

Configure application settings in `appsettings.json` or `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ledgerai_db;Username=postgres;Password=YourPassword;Include Error Detail=true;"
  },
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyWithAtLeast32BytesLengthPlaceholder!",
    "Issuer": "LedgerAI.Api",
    "Audience": "LedgerAI.Clients",
    "ExpiryMinutes": 120
  },
  "GroqSettings": {
    "ApiKey": "gsk_YourGroqApiKeyPlaceholder",
    "BaseUrl": "https://api.groq.com/openai/v1",
    "PrimaryModel": "llama-3.3-70b-versatile",
    "FallbackModel": "mixtral-8x7b-32768",
    "TimeoutSeconds": 30
  },
  "WhatsAppSettings": {
    "ApiVersion": "v20.0",
    "PhoneNumberId": "YOUR_PHONE_NUMBER_ID",
    "AccessToken": "YOUR_WHATSAPP_PERMANENT_ACCESS_TOKEN",
    "WebhookVerifyToken": "YOUR_CUSTOM_VERIFY_TOKEN",
    "AppSecret": "YOUR_APP_SECRET"
  }
}
```

Serilog writes structured console and rolling file logs (`logs/ledgerai-.log`). Request logging includes `TenantId` and `CorrelationId`; secrets, API keys, raw tokens, passwords, and connection-string credentials must not be written to logs.

---

## 10. Local Build, Run & Verification Guide

### Prerequisites
* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* [PostgreSQL 16+](https://www.postgresql.org/) (Local instance or Supabase)

### Build & Execution Commands

```bash
# 1. Navigate to the API root
cd LedgerAI.Api

# 2. Restore NuGet dependencies
dotnet restore

# 3. Compile the solution (0 Warnings, 0 Errors)
dotnet build

# 4. Run the API under the local Kestrel profile
dotnet run --launch-profile http
```

### Database Setup, Seeding & Verification

LedgerAI uses the hybrid **EF Core Code-First + SQL Hardening** workflow:

```bash
cd LedgerAI.Api
dotnet ef database update
psql "$CONNECTION_STRING" -f ../Docs/Schemas/patch-update-efcore.sql
```

The SQL hardening patch is required only after an EF Core migration-created/fresh/reset database. It is not required if the database was bootstrapped directly from `../Docs/Schemas/LedgerAI_Schema_v2.sql`.

Enable local seed data with `appsettings.Development.json`:

```json
{
  "SeedData": {
    "Enabled": true,
    "IncludeMockData": true
  }
}
```

Starting the API in Development runs the idempotent `DbSeeder` when enabled: it creates the `MASTER-PKR` tenant and core users, then interlinks categories, UOMs, products, warehouses, stock batches/movements, accounts, orders/items/reservations, invoices/items/charges, ledger entries/allocations, inbound messages, outbox events, and audit records. Seeded `created_by`, `issued_by`, and audit `user_id` fields use the Master Owner's actual seeded ID. Runtime endpoints currently do not resolve an authenticated current user, so actor IDs for future write endpoints should be derived from authenticated identity rather than trusted from request data. `app.current_tenant` is set before tenant-scoped writes and must be set for manual seed/administrative inserts too.

### Verification Endpoints
Once running, verify the backend via curl or browser:

```bash
# 1. System Metadata Check
curl.exe -s -H "X-Tenant-Id: 11111111-1111-4111-8111-111111111111" -H "X-Correlation-ID: dev-master-metadata" http://localhost:5284/api/metadata

# 2. ASP.NET Core Health Probe
curl.exe -s -H "X-Tenant-Id: 11111111-1111-4111-8111-111111111111" -H "X-Correlation-ID: dev-master-health" http://localhost:5284/health

# 3. Tenant-scoped API request
curl.exe -s -H "X-Tenant-Id: 11111111-1111-4111-8111-111111111111" -H "X-Correlation-ID: dev-master-catalog" http://localhost:5284/api/catalog/products

# 4. Scalar API Interactive Documentation
open http://localhost:5284/scalar/v1
```

When sending requests from Scalar, include `X-Tenant-Id: 11111111-1111-4111-8111-111111111111` in the request headers (and optionally an `X-Correlation-ID`). The same tenant header is included in the repository's `LedgerAI.Api.http` requests.

---

## License

This project is licensed under the [MIT License](../LICENSE).
