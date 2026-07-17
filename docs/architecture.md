# RetailSphere — Enterprise Retail Management System
## Phase 1: Solution Architecture & Design

Target stack: ASP.NET Core 9 (Clean Architecture, DDD, CQRS/MediatR, EF Core, MySQL 8.0, Redis, Hangfire) + Blazor (.NET 9, MudBlazor), styled after the existing WMC Platform.UI (MudThemeProvider theme, MudDrawer/Sidebar, JWT `AuthenticationStateProvider`, typed `HttpClient` API clients, Blazored.LocalStorage).

No code in this phase — design only. Implementation begins module-by-module after this is approved.

---

## 1. Business Domain Analysis

### 1.1 Business Model
A multi-branch, multi-category retail chain selling shoes, watches, clothing, and accessories, through both physical POS terminals and (future) online storefronts. Products are heavily variant-driven (size × color × material), seasonal, and brand-dependent, which is the central modeling challenge — a "product" is really a template that expands into many sellable SKUs.

### 1.2 Core Business Processes
- **Merchandising**: category/brand management, product creation, variant/SKU generation, pricing, seasonal catalogs.
- **Procurement**: supplier management, purchase orders, goods receipt (GRN), purchase returns, supplier payment/reconciliation.
- **Inventory**: multi-branch and warehouse stock tracking, transfers between branches, adjustments/write-offs, damage/shrinkage tracking, reorder automation.
- **Sales**: in-store POS, sales orders, quotations, returns/exchanges, discounts/promotions/coupons, gift cards.
- **Customer relationship**: profiles, loyalty points, wallet balance, purchase history, multi-address support (billing/shipping).
- **Finance**: cash register reconciliation per shift/branch, expenses/income tracking, tax computation (jurisdiction-aware), P&L reporting, multi-payment-method settlement.
- **Reporting & analytics**: operational dashboards for owners/branch managers, and a forecasting/insights layer for future AI features.

### 1.3 Business Rules (representative — to be ratified with stakeholders)
- Every sellable unit is a **SKU** (Product Variant), never a bare Product; a Product is a non-transactable template.
- Stock is tracked per **branch/warehouse**, never globally — a single "quantity on hand" field at the product level is not sufficient in a multi-branch system.
- Stock adjustments, transfers, and damage write-offs must be **auditable and reversible only through compensating transactions**, never by editing history.
- A sale cannot be finalized if inventory reservation fails (no oversell), except for explicitly flagged backorder/pre-order SKUs.
- Discounts, coupons, and loyalty redemptions are evaluated centrally by a **pricing/promotion engine** so POS, online, and quotes always apply the same rules.
- Returns/exchanges must reference an original sale line; refund method is constrained by original payment method and store policy (e.g. cash refunds capped, remainder to wallet/gift card).
- Every price-affecting or stock-affecting mutation produces a domain event for audit logging — no silent updates.
- Multi-currency and multi-tax-jurisdiction support should be modeled as configuration per branch/region even if only one currency is used at launch (cheap now, expensive to retrofit).

### 1.4 User Roles (initial RBAC set)
| Role | Typical scope |
|---|---|
| Super Admin | Cross-branch, all modules, system configuration |
| Regional/Area Manager | Multiple branches within a region |
| Branch Manager | Single branch — inventory, staff, cash register, local reporting |
| Cashier / POS Operator | Sales module only, own register session |
| Warehouse Staff | Inventory, GRN, transfers |
| Purchasing Officer | Suppliers, purchase orders, supplier payments |
| Accountant / Finance | Financial module, reporting, tax |
| Customer Service | Customer profiles, returns/exchanges, loyalty adjustments |
| Auditor (read-only) | Audit logs, reports, no write access |

Roles should be **permission-based underneath**, not hardcoded — roles are named bundles of granular permissions (`sales.pos.create`, `inventory.transfer.approve`, etc.) so new roles can be composed without code changes.

### 1.5 Operational Workflows (examples)
- **Purchase-to-stock**: Purchase Order → Supplier confirmation → Goods Received Note → QC/damage check → Stock posted to warehouse → optional transfer to branch.
- **Sale-to-cash**: Cart build (POS or online) → promotion/discount evaluation → payment capture (possibly split across methods) → stock decrement → receipt/invoice → loyalty points accrual.
- **Return-to-refund**: Return request against original sale → condition check → refund method decision → stock re-entry (if resalable) or damage tracking (if not) → loyalty point reversal if applicable.
- **Stock transfer**: Source branch requests/dispatches → in-transit state → destination branch receives and confirms → discrepancy handling.
- **Low-stock reorder**: Reorder level breached → automated PO draft suggestion → purchasing officer approval → standard purchase flow.

### 1.6 Future Scalability Requirements to Design For Now
- E-commerce storefront and marketplace channel integration (Shopify/Amazon-style) sharing the same product/inventory core — implies an **omnichannel inventory model** from day one (reserved vs. available stock).
- Multi-currency, multi-tax-region expansion (international branches).
- AI-driven forecasting/recommendations layered on top of clean, event-sourced sales/inventory data — this argues for **domain events being persisted and published**, not just fired in-process, so a future analytics/ML pipeline can consume them without re-architecting.
- Franchise/multi-tenant mode (each mega-store brand or franchisee as a tenant) — worth confirming now whether RetailSphere stays single-tenant-multi-branch or must become multi-tenant, since retrofitting tenant isolation into a live schema is expensive.
- Marketplace-style supplier self-service portal.
- Mobile POS / handheld scanning devices, offline-first POS mode with sync.

**Decisions confirmed:**
1. **Tenancy**: single-tenant, multi-branch. No `TenantId`/tenant isolation layer — simplifies the schema considerably. If franchising/multi-tenant is ever revisited, it becomes its own future initiative, not a v1 concern.
2. **E-commerce**: deferred. v1 ships POS + back-office only. The Inventory model is still built with `QuantityOnHand`/`QuantityReserved`/`QuantityAvailable` from day one (§4.2) so a future storefront can reserve stock without a breaking migration.
3. **POS connectivity**: both offline and online required. This is a first-class architectural driver — see the dedicated Offline POS Architecture in §8.1.
4. **Currency/tax**: multi-region-capable architecture, but only the **Pakistan** region is configured at launch — PKR currency, FBR sales tax + provincial revenue authority (PRA/SRA/KPRA/BRA) service-tax rules modeled as data (`TaxJurisdictions`/`TaxRules`), not hardcoded, so additional regions/currencies are configuration, not code changes.
5. **Finance depth**: lighter finance module (expenses/income, cash register reconciliation, P&L, tax) — not a full double-entry general ledger. Designed with clean export/integration points (e.g. a `LedgerExportService` abstraction) so it can hand off to dedicated accounting software (QuickBooks or a local Pakistani equivalent) later without re-architecture.

---

## 2. Functional Modules

Organized by bounded-context-aligned module groups (detailed mapping in §3).

**Administration** — User Management, Role & Permission Management (RBAC), Branch Management, Store Settings/Configuration, Audit Logs, Activity Logs, System Health/Feature Flags.

**Product Management** — Product Catalog, Categories, Subcategories, Brands, Variants (size/color/material/etc.), Attributes, Tags, Barcodes, SKU Generation, Product Images, Product Videos, Price Lists.

**Inventory Management** — Stock Management, Warehouses, Branch Inventory, Stock Transfers, Stock Adjustments, Purchase Receipts (GRN), Damage/Shrinkage Tracking, Low Stock Alerts, Reorder Levels, Stock Reservations (for omnichannel).

**Purchasing** — Suppliers, Purchase Orders, Purchase Returns, Goods Received Notes, Supplier Payments, Supplier Ledger/Statement.

**Sales** — Point of Sale (POS), Sales Orders, Quotations, Returns, Exchanges, Discounts, Promotions, Coupons, Gift Cards, Cash Register / Shift Management.

**Customer Management** — Customer Profiles, Loyalty Program, Reward Points, Customer Wallet, Purchase History, Addresses, Customer Segmentation (future).

**Financial Management** (lightweight, not full GL — see decision below) — Expenses, Income, Cash Register Reconciliation, Profit & Loss, Tax Management (Pakistan tax rules per jurisdiction), Payment Methods, Supplier/Customer Ledgers, and an export abstraction for handing data to dedicated accounting software later.

**Reporting** — Sales Reports, Inventory Reports, Customer Reports, Purchase Reports, Financial Reports, Dashboard Analytics (per-role dashboards).

**Notifications** — Email, SMS, Push, In-App Notifications, Notification Preferences.

**AI Features (future scope, designed for, not built in v1)** — Product Recommendations, Sales Forecasting, Inventory Forecasting, Customer Insights, Demand Prediction. These will consume the domain event stream and reporting data marts rather than being bolted directly onto transactional tables.

---

## 3. Domain-Driven Design

### 3.1 Bounded Contexts
| Bounded Context | Responsibility | Owns |
|---|---|---|
| **Identity & Access** | Users, roles, permissions, auth | User, Role, Permission |
| **Catalog** | Product definition & merchandising | Product, Category, Brand, Variant, Attribute |
| **Inventory** | Stock truth per branch/warehouse | StockItem, Warehouse, StockTransfer, StockAdjustment |
| **Procurement** | Supplier-side purchasing | Supplier, PurchaseOrder, GoodsReceivedNote, PurchaseReturn |
| **Sales** | Point of sale & order lifecycle | SalesOrder, POSTransaction, Quotation, Return, Promotion, Coupon, GiftCard |
| **Customers** | Customer relationship | Customer, LoyaltyAccount, Wallet, Address |
| **Finance** | Money movement & reporting truth | CashRegisterSession, Expense, Income, TaxRule, PaymentRecord |
| **Notifications** | Outbound communication | NotificationTemplate, NotificationLog |
| **Reporting/Analytics** | Read-side aggregation (CQRS read models) | Materialized report projections |

Contexts communicate via **domain events** (in-process via MediatR notifications; cross-service in future via an outbox + message broker) rather than direct entity references across context boundaries. E.g. Sales does not reach into Inventory's tables directly — it raises `SaleCompletedEvent`, which Inventory subscribes to and decrements stock.

### 3.2 Representative Aggregates, Entities, Value Objects

**Catalog context**
- Aggregate root: `Product` — Entities: `ProductVariant` (the sellable SKU), `ProductImage`, `ProductAttributeValue` — Value Objects: `SKU`, `Barcode`, `Money`, `Dimension`.
- Aggregate root: `Category` (self-referencing tree for category/subcategory) — Value Object: `CategoryPath`.
- Aggregate root: `Brand`.

**Inventory context**
- Aggregate root: `StockItem` (per Warehouse/Branch + Variant) — Value Objects: `Quantity` (on-hand, reserved, available — never a single number), `ReorderThreshold`.
- Aggregate root: `StockTransfer` — Entities: `StockTransferLine` — enforces state machine (Draft → Dispatched → InTransit → Received/Discrepancy).
- Aggregate root: `StockAdjustment` — Value Object: `AdjustmentReason` (damage, theft, count correction, etc.).

**Procurement context**
- Aggregate root: `PurchaseOrder` — Entities: `PurchaseOrderLine` — Value Objects: `Money`, `POStatus`.
- Aggregate root: `GoodsReceivedNote` — Entities: `GRNLine`.
- Aggregate root: `Supplier` — Value Object: `SupplierContact`, `PaymentTerms`.

**Sales context**
- Aggregate root: `SalesOrder` (covers both POS and future online orders) — Entities: `SalesOrderLine`, `PaymentSplit` — Value Objects: `Money`, `DiscountApplication`, `OrderStatus`.
- Aggregate root: `Promotion`/`Coupon` — Value Object: `PromotionRule` (strategy pattern: percentage, fixed amount, buy-X-get-Y, tiered).
- Aggregate root: `GiftCard` — Value Object: `GiftCardCode`, `Balance`.
- Aggregate root: `CashRegisterSession` — Entities: `CashMovement`.

**Customers context**
- Aggregate root: `Customer` — Entities: `Address`, `LoyaltyLedgerEntry` — Value Objects: `Email`, `PhoneNumber`, `LoyaltyTier`.
- Aggregate root: `Wallet` — Entities: `WalletTransaction`.

**Identity & Access context**
- Aggregate root: `User` — Entities: `RefreshToken`, `UserRoleAssignment` — Value Objects: `HashedPassword`, `PermissionCode`.

### 3.3 Domain Events (representative)
`ProductVariantCreated`, `PriceChanged`, `StockReceived`, `StockTransferred`, `StockAdjusted`, `LowStockThresholdBreached`, `PurchaseOrderApproved`, `GoodsReceived`, `SaleCompleted`, `SaleReturned`, `PaymentCaptured`, `LoyaltyPointsAccrued`, `LoyaltyPointsRedeemed`, `GiftCardIssued`, `GiftCardRedeemed`, `CashRegisterOpened`, `CashRegisterClosed`, `LowStockAlertRaised`.

Each event is persisted (event log table, or transactional outbox) so it can drive: audit logging, notifications, cache invalidation, and — later — the forecasting/analytics pipeline. This is the cheapest insurance for the "AI Features" future scope: build the event trail now, bolt on ML later.

### 3.4 Repositories, Domain Services, Application Services
- **Repositories**: one per aggregate root only (`IProductRepository`, `IStockItemRepository`, `ISalesOrderRepository`, …), never per entity. Repositories return/persist whole aggregates to protect invariants.
- **Domain Services**: used when logic doesn't naturally belong to a single aggregate — e.g. `PricingEngine` (applies promotions/coupons across order lines), `StockAvailabilityService` (checks availability across warehouses for a sale), `SkuGenerationService`, `TaxCalculationService`.
- **Application Services**: implemented as MediatR command/query handlers in the Application layer — orchestrate repositories + domain services + unit of work, and are the only entry point the API layer calls into. No business logic lives here beyond orchestration.

---

## 4. Database Design (MySQL 8.0)

### 4.1 Cross-Cutting Conventions
- **Primary keys**: `BIGINT UNSIGNED AUTO_INCREMENT` for internal PKs (cheap joins/indexes); a separate `CHAR(36) BINARY` `PublicId` (UUID) column exposed externally via the API, so internal IDs are never leaked and IDs aren't guessable/enumerable.
- **Audit columns** on every table: `CreatedAtUtc DATETIME(6)`, `CreatedBy BIGINT UNSIGNED`, `ModifiedAtUtc DATETIME(6) NULL`, `ModifiedBy BIGINT UNSIGNED NULL`.
- **Soft delete**: `IsDeleted TINYINT(1) NOT NULL DEFAULT 0`, `DeletedAtUtc DATETIME(6) NULL`, `DeletedBy BIGINT UNSIGNED NULL`. EF Core global query filter (`HasQueryFilter`) excludes soft-deleted rows automatically; a `withDeleted` escape hatch exists for admin/audit views.
- **Multi-branch support**: every operational table (stock, sales, cash register, purchase) carries `BranchId BIGINT UNSIGNED NOT NULL` with an FK to `Branches`. Reference/catalog tables (Product, Category, Brand) are branch-agnostic; Inventory-level tables are branch-scoped.
- **Concurrency**: `RowVersion` (EF Core `[Timestamp]`-style, implemented via a `BIGINT` version counter since MySQL lacks native rowversion) on aggregates subject to concurrent writes (StockItem, CashRegisterSession).
- **Money**: stored as `DECIMAL(18,4)` — never float/double. Currency stored alongside as `CHAR(3)` (ISO 4217); v1 operates in `PKR` but every money-bearing table carries the currency code so additional regions are configuration, not migration.
- **Charset**: `utf8mb4` throughout for full Unicode (Urdu/English product names, receipts).
- **Tax model (Pakistan, v1)**: `TaxJurisdictions` (`Country`, `Province` — Punjab/Sindh/KPK/Balochistan/ICT/GB/AJK — `TaxAuthority` e.g. FBR/PRA/SRA/KPRA/BRA) and `TaxRules` (`Rate`, `AppliesTo` — goods vs. services, since Pakistan splits sales tax on goods (FBR, federal) from sales tax on services (provincial revenue authorities)). `Branches.TaxJurisdictionId` determines which rules apply at that branch. This structure is the extension point for additional countries/regions later — no schema change needed, only new rows.

### 4.2 Core Table Groups

**Identity & Access**: `Users`, `Roles`, `Permissions`, `RolePermissions`, `UserRoles`, `RefreshTokens`, `Branches`, `AuditLogs`, `ActivityLogs`.

**Catalog**: `Categories` (self-FK `ParentCategoryId`), `Brands`, `Products`, `ProductAttributes`, `AttributeValues`, `ProductVariants` (FK `ProductId`, composite uniqueness on `ProductId + Size + Color + Material` combination via a generated `VariantHash` or explicit `AttributeSetId`), `ProductImages`, `ProductVideos`, `Barcodes` (1:1 or 1:many with `ProductVariants`), `Tags`, `ProductTags`.

**Inventory**: `Warehouses`, `StockItems` (composite unique `WarehouseId/BranchId + ProductVariantId`, columns `QuantityOnHand`, `QuantityReserved`, computed/derived `QuantityAvailable`), `StockTransfers`, `StockTransferLines`, `StockAdjustments`, `StockAdjustmentLines`, `ReorderRules` (`ReorderLevel`, `ReorderQuantity` per `ProductVariantId + BranchId`).

**Procurement**: `Suppliers`, `SupplierContacts`, `PurchaseOrders`, `PurchaseOrderLines`, `GoodsReceivedNotes`, `GRNLines`, `PurchaseReturns`, `PurchaseReturnLines`, `SupplierPayments`.

**Sales**: `SalesOrders`, `SalesOrderLines`, `Quotations`, `QuotationLines`, `Returns`, `ReturnLines`, `Exchanges`, `Discounts`, `Promotions`, `PromotionRules`, `Coupons`, `CouponRedemptions`, `GiftCards`, `GiftCardTransactions`, `CashRegisterSessions`, `CashMovements`, `PaymentMethods`, `Payments`.

**Customers**: `Customers`, `CustomerAddresses`, `LoyaltyAccounts`, `LoyaltyLedgerEntries`, `Wallets`, `WalletTransactions`.

**Finance**: `Expenses`, `Income`, `TaxRules`, `TaxJurisdictions`, `SupplierLedger`, `CustomerLedger`.

**Notifications**: `NotificationTemplates`, `NotificationLogs`, `NotificationPreferences`.

**Settings**: `StoreSettings` (key/value, scoped by `BranchId NULL` = global default vs. branch override), `FeatureFlags`.

### 4.3 Key Relationships & Constraints
- `ProductVariants.ProductId` → `Products.Id` (cascade restrict — a Product can't be deleted while variants exist; soft-delete instead).
- `StockItems` FK to both `ProductVariants` and `Warehouses`/`Branches`; unique composite index prevents duplicate stock rows per SKU per location.
- `SalesOrderLines.ProductVariantId` → `ProductVariants.Id` (restrict — historical order lines must never be orphaned even if a variant is discontinued; discontinue via `IsActive` flag, not deletion).
- `PurchaseOrderLines`/`GRNLines` reference `SalesOrders`-style line pattern; `GRNLines.PurchaseOrderLineId` nullable to support unplanned/direct receipts.
- `Returns.OriginalSalesOrderLineId` → `SalesOrderLines.Id` (enforces the business rule that returns must trace to an original sale).
- Composite keys used for pure join/mapping tables (`UserRoles`, `RolePermissions`, `ProductTags`) — surrogate PK avoided there since the pair itself is the natural key.
- `CHECK` constraints (MySQL 8.0.16+) for invariants like `QuantityOnHand >= 0`, `QuantityReserved <= QuantityOnHand`, `UnitPrice >= 0`.

### 4.4 Indexing Strategy
- Every FK column indexed explicitly (MySQL doesn't always auto-index FKs the way you'd want for query patterns).
- Composite covering indexes for hot paths: `StockItems(BranchId, ProductVariantId)`, `SalesOrders(BranchId, CreatedAtUtc)` for date-ranged branch reports, `SalesOrderLines(SalesOrderId)`.
- Full-text index (`FULLTEXT`) on `Products(Name, Description)` for catalog search, or delegate search to a dedicated search index (Meilisearch/Elasticsearch) once catalog size justifies it.
- `Barcodes.Code` unique index — barcode scans are a hot lookup path in POS and must be O(1).
- Partial/functional consideration: since MySQL lacks partial indexes, soft-delete filtering relies on `IsDeleted` being the leading column in composite indexes where deleted-row volume could be high.

### 4.5 ER Diagram (textual)

```
Branches ──< Warehouses ──< StockItems >── ProductVariants >── Products >── Categories (self-ref)
   │                              │                                 │
   │                              │                                 └──< Brands
   │                              ├──< StockTransferLines >── StockTransfers
   │                              └──< StockAdjustmentLines >── StockAdjustments
   │
   ├──< CashRegisterSessions ──< CashMovements
   ├──< SalesOrders ──< SalesOrderLines >── ProductVariants
   │        │                 └──< Payments >── PaymentMethods
   │        └──< Returns ──< ReturnLines
   │
   └──< PurchaseOrders ──< PurchaseOrderLines >── ProductVariants
                │                └──< GRNLines >── GoodsReceivedNotes
                └── Suppliers

Customers ──< CustomerAddresses
   ├──< LoyaltyAccounts ──< LoyaltyLedgerEntries
   ├──< Wallets ──< WalletTransactions
   └──< SalesOrders (CustomerId FK)

Users ──< UserRoles >── Roles ──< RolePermissions >── Permissions
```

A full diagram (draw.io/dbdiagram.io) should be generated once field-level review is done — happy to produce a `.dbml` or Mermaid ER file for that as a next step if useful.

---

## 5. Clean Architecture — Solution Structure

```
RetailSphere.sln
├── src/
│   ├── RetailSphere.Domain              (Entities, Aggregates, Value Objects, Domain Events, Domain Exceptions, Repository interfaces)
│   ├── RetailSphere.Application         (CQRS commands/queries, MediatR handlers, DTOs, FluentValidation validators, AutoMapper profiles, Application service interfaces)
│   ├── RetailSphere.Infrastructure       (External integrations: SendGrid/SMS, Redis, Hangfire job definitions, file storage, OpenTelemetry setup)
│   ├── RetailSphere.Persistence         (EF Core DbContext, entity configurations, migrations, repository implementations, MySQL-specific concerns)
│   ├── RetailSphere.API                 (ASP.NET Core Web API — controllers/minimal APIs, middleware, Swagger, JWT auth wiring, DI composition root)
│   ├── RetailSphere.SharedKernel         (Base entity/aggregate/value-object types, Result<T> pattern, common interfaces used across all layers)
│   ├── RetailSphere.Contracts           (Public API request/response contracts shared with the Blazor client — versionable, no domain leakage)
│   └── RetailSphere.Common               (Cross-cutting non-domain utilities: date/time provider, current-user accessor, pagination helpers)
├── ui/
│   └── RetailSphere.UI                  (Blazor WASM/Server + MudBlazor — mirrors WMC Platform.UI structure: Layout/, Components/, Pages/, Clients/, Configuration/Authentication/)
└── tests/
    ├── RetailSphere.UnitTests            (Domain + Application logic, no I/O)
    └── RetailSphere.IntegrationTests      (API + Persistence, Testcontainers-backed MySQL/Redis)
```

**Dependency rule**: `Domain` depends on nothing. `Application` depends only on `Domain` + `SharedKernel`. `Infrastructure` and `Persistence` depend on `Application` (implementing its interfaces) and `Domain`. `API` composes everything at the edge. `Contracts` is referenced by both `API` and `UI` so request/response shapes are shared without either side depending on `Domain`.

| Project | Responsibility |
|---|---|
| Domain | Business rules and invariants. Zero framework dependencies (no EF, no ASP.NET). |
| Application | Use cases (CQRS handlers), orchestration, validation rules, mapping. Depends on abstractions only. |
| Infrastructure | Implementations of cross-cutting concerns Application depends on abstractly: email/SMS senders, caching, background jobs, telemetry. |
| Persistence | EF Core `DbContext`, fluent configurations, migrations, concrete repository implementations, MySQL provider (Pomelo). |
| API | HTTP surface: controllers, middleware pipeline, auth, Swagger, global exception handling, DI wiring (composition root — the only project allowed to reference everything). |
| SharedKernel | Truly cross-cutting base types (`Entity`, `AggregateRoot`, `ValueObject`, `Result<T>`, `IDomainEvent`) reused by every bounded context. |
| Contracts | DTOs for the wire — shared between API and Blazor client so the frontend never needs to guess response shape. |
| Common | Small infrastructure-agnostic utilities (clock abstraction, current-user context, pagination/sorting primitives). |
| UnitTests / IntegrationTests | Test pyramid — fast domain/application tests vs. slower API+DB tests. |

Each bounded context (Catalog, Inventory, Procurement, Sales, Customers, Finance) is a **folder-per-context inside Domain/Application/Persistence**, not a separate project, until/unless a context needs to be physically separated into its own service — keeps the "modular monolith" option open without premature microservice complexity.

---

## 6. API Design

- **Versioning**: URL-segment versioning (`/api/v1/...`) via `Asp.Versioning.Http`; new breaking versions get a new segment, old versions deprecated with a sunset header, never mutated in place.
- **Resource-oriented REST**: `/api/v1/products`, `/api/v1/products/{id}/variants`, `/api/v1/branches/{branchId}/stock-items`, `/api/v1/sales-orders`, etc. Nest only one level deep; deeper relationships go through query parameters.
- **Pagination**: cursor-based for high-volume feeds (sales transactions), offset-based (`page`, `pageSize`, capped max) for admin lists — response envelope includes `totalCount`, `page`, `pageSize`, `hasNextPage`.
- **Filtering/Sorting**: standardized query string convention, e.g. `?filter[categoryId]=3&filter[branchId]=1&sort=-createdAtUtc`, parsed into a shared `QueryOptions` object in the `Common` project so every list endpoint behaves identically.
- **Response envelope**: consistent shape — `{ data, meta, errors }` — success and error responses always follow the same wrapper so the Blazor client has one deserialization path.
- **Error handling**: global exception-handling middleware maps domain exceptions → `ProblemDetails` (RFC 7807) with correlation IDs; validation errors (FluentValidation) return 422 with field-level error maps; unhandled exceptions are logged with Serilog + OpenTelemetry trace ID surfaced to the client for support correlation.
- **Validation**: FluentValidation validators run as a MediatR pipeline behavior before handlers execute — handlers never re-validate.
- **Auth**: every endpoint requires JWT bearer auth by default (`[Authorize]` at a base controller/convention level); anonymous endpoints are explicitly opt-in, not opt-out.
- **API docs**: Swagger/OpenAPI with JWT bearer scheme configured, grouped by tag per bounded context, examples generated from real DTOs.

---

## 7. Security

- **JWT Authentication**: short-lived access tokens (10–15 min), signed with an asymmetric key (RS256) so the Blazor client and any future services can verify without holding the signing secret.
- **Refresh Tokens**: long-lived, opaque, stored server-side hashed (never the raw token), rotated on every use (rotation + reuse detection — if a used-and-rotated refresh token is replayed, revoke the whole token family and force re-login).
- **RBAC + permission-based authorization**: roles are convenience groupings; actual authorization checks are against fine-grained permission codes via policy-based authorization (`[Authorize(Policy = "inventory.transfer.approve")]`), so permissions can be reassigned without redeploying.
- **Password hashing**: ASP.NET Core Identity's default (PBKDF2/Argon2id via `PasswordHasher<T>` or explicit Argon2id) — never custom hashing.
- **Secure file uploads**: content-type allow-listing, magic-byte verification (not just extension), size limits, virus-scan hook point, storage outside the web root (or blob storage), randomized/opaque file names.
- **Rate limiting**: ASP.NET Core's built-in rate limiter middleware backed by Redis for multi-instance consistency — tighter limits on auth endpoints (login/refresh) to blunt credential-stuffing.
- **Data protection**: EF Core column-level encryption or ASP.NET Data Protection API for sensitive fields (customer PII where required by policy); TLS everywhere; secrets in a vault (Azure Key Vault/AWS Secrets Manager/GCP Secret Manager), never in appsettings.
- **Audit logging**: every state-changing command handler emits an audit record (who/what/when/before-after) to the `AuditLogs` table, separate from application logs, queryable by auditors independent of Serilog/OTel sinks.
- **CORS**: locked to known frontend origins; no wildcard in production.

---

## 8. Frontend Architecture (Blazor + MudBlazor)

Modeled directly on the existing **Platform.UI** project so RetailSphere's UI feels consistent with your other WMC products:

- **Shell/theming**: `MudThemeProvider` with a single centralized `MudTheme` (light + dark `PaletteLight`/`PaletteDark`, shared `Typography`, `LayoutProperties` for drawer widths) — same pattern as `MainLayout.razor`'s `CustomTheme`. Theme toggle stored in `Blazored.LocalStorage`.
- **Layout**: `MudLayout` with a top `AppBar`-style header component (equivalent to `WmcHeader`) + `MudDrawer` containing a `Sidebar` component that supports pin/hover-to-expand (`OpenMiniOnHover`, `_sidebarPinned` persisted to local storage) — reused verbatim as a pattern for RetailSphere's `Sidebar.razor`.
- **Navigation**: `MudNavMenu`/`MudNavGroup`/`MudNavLink` tree, with per-link visibility driven by the current user's **permissions** (not roles directly) — extending the existing `IsLinkVisible` search-filter pattern to also gate on `AuthorizeView`/permission claims so nav items disappear entirely for unauthorized roles rather than just being disabled.
- **Auth flow**: custom `AuthenticationStateProvider` (JWT-based, same shape as `JwtAuthenticationStateProvider`) + a `DelegatingHandler` (`JwtTokenMessageHandler` equivalent) attached to typed `HttpClient`s so every API call automatically carries the bearer token and triggers silent refresh on 401.
- **API integration**: typed `HttpClient` wrapper per bounded context, following the `ServerApiClient.<Feature>.cs` partial-class pattern already used (`ServerApiClient.Sales.cs`, `ServerApiClient.Inventory.cs`, etc.) — one client, partitioned by feature, not one client per module, to avoid DI sprawl.
- **State management**: scoped services for cross-component session state (current branch selection, current cash register session, cart-in-progress) — same idiom as `IGlobalDateRangeService`/`IUserOptionService` in Platform.UI — plus `Blazored.LocalStorage` for durable UI preferences (sidebar pin state, theme, last-used branch).
- **Reusable components**: shared `Components/Shared` library for data grids (`MudDataGrid` wrappers with built-in server-side paging/filter/sort matching the API's `QueryOptions` contract), confirmation dialogs, snackbar notification helpers, form field wrappers with FluentValidation-driven client-side messages.
- **Dashboards**: role-specific landing dashboards (owner/branch manager/cashier see different widgets) built on `ApexCharts` (already used in Platform.UI) for consistency — sales trend, low-stock alerts, today's cash register status, top-selling SKUs.
- **Responsive design**: MudBlazor's breakpoint system (`Breakpoint.Md` drawer collapse, as already used) plus a POS-specific simplified layout for tablet/touch use at checkout.
- **Rendering mode**: back-office/admin uses Blazor **WebAssembly hosted** (matches Platform.UI). POS requires offline support (§1.6), which rules out Blazor **Server** for the POS screen entirely (Server mode has zero function without a live SignalR connection to the host) — POS is therefore also **WASM**, but with an explicit offline data/sync layer (§8.1) rather than relying on live API calls per action.

### 8.1 Offline-Capable POS Architecture

Since POS must work both online and offline, it is designed as a **local-first client with background sync**, not a thin API client:

- **Local store**: browser `IndexedDB` (via a thin JS interop or a library like `Blazored.IndexedDb`) holds a working copy of the data POS needs to operate standalone — active branch's catalog/prices/promotions/tax rules (synced down periodically) and a **pending transaction queue** for sales made while offline.
- **Sale capture**: every POS sale is built locally first (cart, discounts, tax, payment) using the locally cached catalog/pricing/tax data, and assigned a **client-generated idempotency key** (GUID) at creation time — this key travels with the transaction regardless of when it syncs, so replays never double-post.
- **Sync engine**: a background service (runs in the Blazor WASM client) attempts to flush the pending queue whenever connectivity is detected (`navigator.onLine` + periodic ping), posting queued `SalesOrder`s to `/api/v1/sales-orders` in order. The API's command handler treats the idempotency key as a natural key — a repeated submission is a no-op, not a duplicate sale.
- **Stock reservation semantics while offline**: offline sales optimistically decrement the **locally cached** available quantity so the cashier doesn't oversell against what the device last knew; on sync, the server is the source of truth — if the server-side stock can't cover the sale (rare, e.g. two offline registers sold the last pair of shoes), the transaction is flagged `NeedsReview` rather than silently failing, and surfaces in a manager reconciliation queue rather than blocking the cashier's shift.
- **Cash register session**: session open/close and cash movements also queue offline and reconcile the same way — a register can open, sell, and close entirely offline for a full shift if needed.
- **Conflict/reconciliation UI**: a manager-facing "Sync & Exceptions" screen (part of Sales module, Phase 5) shows any transactions that synced with a discrepancy (stock shortfall, price drift if catalog updated while offline) for manual resolution.
- **What stays online-only**: purchasing, transfers, admin/user management, and reporting are not designed for offline use — only the POS sale-capture path needs it, keeping the offline surface area deliberately small.

---

## 9. Development Roadmap

| Phase | Objectives | Deliverables | Dependencies | Complexity |
|---|---|---|---|---|
| 0. Foundations | Solution scaffold, CI/CD, auth skeleton, DB baseline | Clean Architecture solution, EF Core + MySQL wired, JWT+refresh auth, Docker Compose (API+MySQL+Redis), base Blazor shell/theme/layout | None | Medium |
| 1. Admin & Identity | Users, roles, permissions, branches, audit/activity logs | Full RBAC, branch CRUD, audit log viewer | Phase 0 | Medium |
| 2. Catalog | Categories, brands, products, variants, attributes, images, barcodes/SKU generation | Catalog module end-to-end (API+UI) | Phase 1 | High (variant modeling is the trickiest part) |
| 3. Inventory | Warehouses, branch stock, transfers, adjustments, reorder rules, low-stock alerts | Inventory module, background job for reorder alerts (Hangfire) | Phase 2 | High |
| 4. Procurement | Suppliers, POs, GRN, purchase returns, supplier payments | Procurement module | Phase 2, 3 | Medium |
| 5. Sales & POS | POS terminal UI (online + offline/sync per §8.1), sales orders, quotations, returns/exchanges, promotions/coupons/gift cards, cash register | Sales module — the core revenue path, including offline sync engine and exception-reconciliation screen | Phase 3 (stock decrement), Phase 2 | Very High (offline sync adds real complexity — recommend building online-only POS first, then layering offline capability once the sale flow is stable) |
| 6. Customers | Profiles, loyalty, wallet, addresses, purchase history | Customer module, loyalty accrual/redemption tied into Sales | Phase 5 | Medium |
| 7. Finance & Reporting | Expenses/income, tax rules, cash reconciliation, P&L, all reporting dashboards | Finance module, reporting/dashboard suite | Phases 3–6 | High |
| 8. Notifications | Email/SMS/push/in-app, template management | Notification module (SendGrid/Twilio integration, Hangfire-scheduled) | Phase 5, 6 | Low–Medium |
| 9. Hardening & Launch | Load testing, security review, observability (OTel/Grafana dashboards), UAT | Production readiness sign-off | All prior | Medium |
| 10. AI/Forecasting (future) | Recommendations, demand/sales forecasting, insights | ML pipeline off the event/reporting store built in earlier phases | Phase 7, event log maturity | High (separate workstream) |

Recommended sequencing rationale: Catalog and Inventory must be solid before Sales, since POS is the highest-complexity, highest-risk module and depends on correct stock semantics existing first. Financial reporting is sequenced after Sales/Customers since it aggregates their data.

---

## 10. Best Practices

- **Logging**: Serilog structured logging, enriched with `BranchId`, `UserId`, `CorrelationId`; sinks to console (dev) and a centralized store (Seq/Elasticsearch/Grafana Loki) in production; never log PII or raw payment data.
- **Monitoring/Observability**: OpenTelemetry traces + metrics exported to Grafana/Tempo/Prometheus (or your existing WMC observability stack, since Platform already uses Grafana) — instrument MediatR pipeline, EF Core queries, and HTTP clients for end-to-end trace correlation.
- **Testing**: unit tests for Domain/Application (no I/O, fast, high coverage on business rules — pricing, stock invariants, loyalty math); integration tests against real MySQL/Redis via Testcontainers for Persistence/API; contract tests for the Blazor↔API `Contracts` boundary; a thin layer of E2E tests (Playwright) for critical POS flows only.
- **CI/CD**: build → unit tests → integration tests → containerize → deploy to staging → smoke tests → manual/automated promotion to production; migrations applied via a controlled release step, never on app startup in production.
- **Performance**: Redis caching for read-heavy, low-volatility data (category trees, product catalog, store settings); pagination enforced everywhere; EF Core `AsNoTracking()` for read paths; database indexing reviewed against actual query plans, not guessed.
- **Scalability**: stateless API instances behind a load balancer; Redis for distributed cache/session/rate-limiting so horizontal scaling doesn't break sticky-session assumptions; background jobs (Hangfire) on a separate worker process from the web tier once volume justifies it.
- **Maintainability**: strict adherence to the dependency rule (§5); one bounded context = one folder set across layers, so a context can be extracted into its own service later without a full rewrite; ADRs (architecture decision records) for major decisions like the multi-tenant question in §1.6.
- **Security**: dependency scanning (Dependabot/Snyk) in CI, secrets never committed, periodic access review of roles/permissions, rate limiting and audit logging as described in §7.
- **Coding standards**: EditorConfig + Roslyn analyzers enforced in CI (build fails on violation, not just warns), consistent naming per Microsoft's .NET conventions, nullable reference types enabled solution-wide.

---

## Decisions Log

| # | Decision | Resolution |
|---|---|---|
| 1 | Tenancy | Single-tenant, multi-branch. No tenant isolation layer. |
| 2 | E-commerce/omnichannel | Deferred — v1 is POS + back-office only. Inventory modeled with available/reserved quantities now to avoid a future breaking migration. |
| 3 | POS connectivity | Both offline and online. POS built as a local-first Blazor WASM client with an IndexedDB queue + idempotent sync engine (§8.1). |
| 4 | Currency/tax | Multi-region-capable data model; only Pakistan configured at launch (PKR, FBR + provincial revenue authority tax rules). |
| 5 | Finance depth | Lightweight finance module (expenses/income, cash reconciliation, P&L, tax) — not full GL. Built with an export/integration point for future accounting software. |

All five open questions from the initial draft were resolved before Phase 0 (this codebase) began.

## Phase 0 status

The solution scaffold described in §5, the Identity & Access auth skeleton (§7's JWT/refresh-token design), EF Core + MySQL wiring, and the MudBlazor UI shell (§8) are implemented in this repository. See the root `README.md` for how to run it. Business modules (Phases 1–10 above) are still to be built out.
