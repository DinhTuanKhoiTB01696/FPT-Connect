# ARCHITECTURE.md — Kiến trúc FPT Connect

> Chi tiết đầy đủ: `docs/Project-Bible/07_Architecture.md`. File này là bản tóm tắt ràng buộc bắt buộc cho agent.

## Stack & nguyên tắc

- **Backend**: .NET 10, ASP.NET Core, EF Core. **Frontend**: Vue 3 (latest) + Vite + TS + Tailwind + shadcn-vue + Pinia. **Maps**: Google Maps API. **DB**: SQL Server.
- SOLID · DRY · KISS. Code rõ ràng hơn là "thông minh". Tránh trừu tượng hoá sớm.

## Clean Architecture (chiều phụ thuộc)

```text
Domain        <- Application       <- Infrastructure
                                   <- Api
                                   <- Worker
```

- **Domain**: entity, value object, domain event, policy. KHÔNG tham chiếu EF/HTTP/JWT/Serilog/provider.
- **Application**: use case (CQRS handler), DTO, port (interface), validator. Không biết EF cụ thể.
- **Infrastructure**: EF Core, repository, security, cache, maps, notification, AI, storage — hiện thực các port.
- **Api**: HTTP, middleware, auth, SignalR, composition root.
- **Worker**: outbox, reminder, route aggregation, retention.

## Pattern

- **Repository**: chỉ cho aggregate có hành vi (`ICustomerRepository`, `IRouteSessionRepository`). KHÔNG tạo `IRepository<T>` lộ `IQueryable`.
- **Unit of Work**: `IUnitOfWork.SaveChangesAsync` dispatch domain event vào **transactional outbox** trước commit.
- **CQRS (optional nhưng khuyến nghị)**: Command đổi đúng một aggregate trong transaction; Query dùng projection `AsNoTracking`. Cross-aggregate qua domain event + outbox, consumer idempotent (inbox/dedup key).
- **DI**: đăng ký theo layer (`AddApplication`, `AddInfrastructure`); không new() service thủ công.
- **SignalR**: authenticate khi connect; mỗi group join kiểm tra scope, không tin group name client.
- **Auth**: JWT access 10 phút + refresh rotation (lưu hash, reuse detection revoke family) + MFA TOTP cho Admin/Manager.

## Middleware order (Api)

forwarded headers → correlation/trace ID → exception→ProblemDetails → security headers/HSTS → rate limit → authentication → tenant/user context → authorization → idempotency → request logging/metrics → endpoint.

## Resilience

Timeout ngắn theo dependency; retry chỉ thao tác idempotent; circuit breaker cho Maps/AI/notification; optimistic concurrency bằng RowVersion (trả 409/412). Degraded mode: CRM core vẫn chạy khi AI/Maps/notify lỗi.

## Quy ước đặt tên

- Project: `FptConnect.<Layer>`. Namespace theo folder.
- Class C#: PascalCase; interface `I...`; async method hậu tố `Async`.
- Endpoint: `/api/v1/<resource>` số nhiều, kebab/lowercase.
- Bảng: schema theo context (`crm.Customers`). Vue component PascalCase; composable `useXxx`; store `useXxxStore`.

## ADR

Bắt buộc ADR khi đổi: DB engine, auth model, map provider, queue, storage, multi-tenancy, AI provider, retention, breaking API. Template: Context, Decision, Options, Consequences, Security, Migration, Rollback, Owner, Date.
