# 14. Coding Convention

## 14.1 Repository

```text
/
  src/backend/
    FptConnect.sln
    src/{Domain,Application,Infrastructure,Api,Worker}/
    tests/{Unit,Integration,Architecture,Api}/
  src/frontend/
    src/{app,components,features,layouts,router,stores,lib,types}/
    tests/{unit,e2e}/
  deploy/{docker,iis,nginx}/
  docs/Project-Bible/
```

Feature frontend có `api`, `components`, `composables`, `pages`, `schemas`, `stores`, `types`; shared chỉ chứa thứ thực sự dùng từ hai feature.

## 14.2 C# naming và style

- `PascalCase`: type, method, property; `camelCase`: parameter/local; `_camelCase`: private field.
- Interface có `I`; async method hậu tố `Async`; cancellation token là parameter cuối.
- Nullable reference types bật; warning as error trong CI.
- Dùng record cho immutable DTO/value object, class/entity cho aggregate có identity.
- Không dùng exception cho validation/business flow dự kiến; trả typed Result.
- Không `.Result/.Wait()`, không `async void`, không `DateTime.Now`; dùng `TimeProvider` và UTC.

```csharp
public sealed record CreateCustomerCommand(
    string FullName,
    PhoneNumber Phone,
    string SourceCode,
    GeoPoint? Location) : IRequest<Result<CustomerDto>>;
```

## 14.3 Entity/DTO/validation

- Entity không public setter tùy ý; hành vi qua method (`AssignTo`, `TransitionTo`).
- API request/response không expose EF entity.
- DTO suffix theo intent: `CreateCustomerRequest`, `CustomerSummaryResponse`; không dùng `CustomerDto` cho mọi context.
- FluentValidation cho shape/range; domain invariant vẫn nằm Domain.
- Mapping explicit cho PII và permission-dependent fields; tránh auto-map mass assignment.

## 14.4 Repository/service/controller

- Controller mỏng: bind, dispatch, map HTTP.
- Handler một use case; domain service chỉ khi logic không thuộc một entity/value object.
- Repository không chứa business rule và không trả `IQueryable` ra Application.
- Query handler projection trực tiếp; tránh N+1, include graph tùy tiện.
- External provider qua port: `IMapService`, `INotificationSender`, `IAiGateway`.

## 14.5 EF Core

- Fluent configuration riêng từng entity; migration có tên mô tả.
- Read query dùng `AsNoTracking`; list luôn bounded/paginated.
- Index tương ứng query thật; kiểm tra execution plan.
- Concurrency token rowversion; transaction ngắn.
- Không lazy loading; không client evaluation; raw SQL parameterized.

## 14.6 Vue/TypeScript

- TypeScript strict; Composition API `<script setup lang="ts">`.
- Component `PascalCase.vue`; composable `useCustomerSearch.ts`; Pinia store `useCustomerStore`.
- Server state ưu tiên query/cache layer; Pinia giữ app/client state, không sao chép mọi API data.
- Zod/Valibot validate boundary API/form khi phù hợp.
- Không gọi API trực tiếp trong presentational component.
- Text qua i18n key; semantic token qua Tailwind config.

```ts
export interface CustomerSummary {
  id: string
  fullName: string
  maskedPhone: string
  status: CustomerStatus
  rowVersion: string
}
```

## 14.7 API và error conventions

- Resource plural, action endpoint chỉ khi không biểu diễn tốt bằng CRUD.
- `POST` create/command, `PATCH` partial, `PUT` replace configuration.
- Domain error code ổn định uppercase snake case.
- Không trả stack trace/SQL/provider error.
- OpenAPI examples dùng synthetic data; generated client được build trong CI.

## 14.8 Logging/comment

- Structured log template: `"Customer {CustomerId} assigned to {AssigneeId}"`.
- ID có thể log; phone/email/content/token không log.
- Comment giải thích “why/constraint”, không kể lại code.
- XML docs cho public library/contract; ADR cho quyết định lớn.

## 14.9 Test conventions

- Tên: `Method_State_ExpectedBehavior` hoặc Given/When/Then rõ.
- Arrange-Act-Assert; một hành vi chính; clock/UUID/provider deterministic.
- Không mock EF DbSet; integration test dùng SQL Server Testcontainer.
- Fixture không dùng PII thật; test parallel-safe; flaky test là defect.

## 14.10 Git

Branch: `main` protected, `develop` chỉ nếu release model cần; ưu tiên trunk-based với short-lived `feature/FC-123-customer-import`, `fix/FC-456-refresh-reuse`.

Commit Conventional Commits:

```text
feat(customers): add scoped nearby search
fix(auth): revoke token family on refresh reuse
test(checkin): cover accuracy review threshold
docs(api): define idempotency conflict response
```

PR nhỏ, một mục tiêu, liên kết ticket/UC/TC, mô tả migration/security/rollback. Không commit generated secret, `.env`, build output.

## 14.11 Review checklist

- [ ] Rule và authorization ở đúng layer.
- [ ] Query scoped trước pagination; không IDOR/N+1.
- [ ] Command idempotent/concurrent-safe.
- [ ] PII/log/file/input được bảo vệ.
- [ ] Error/observability/audit đầy đủ.
- [ ] Test happy, boundary, unauthorized, conflict và dependency failure.
- [ ] Docs/OpenAPI/migration/runbook cập nhật.

