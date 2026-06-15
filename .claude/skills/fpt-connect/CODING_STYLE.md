# CODING_STYLE.md — Quy ước viết code

> Format theo `.editorconfig` (C# 4 spaces, TS/Vue 2 spaces, CRLF, final newline).

## Backend (.NET 10 / C#)

- File-scoped namespace; `var` khi kiểu rõ; nullable reference types bật.
- Async suffix `Async`; trả `Task`/`Task<T>`; truyền `CancellationToken` qua API/handler.
- Một class một file; tên file = tên type. Folder phản ánh namespace.
- **Entity** (Domain): thuần POCO + hành vi domain; kế thừa `BaseEntity`; không attribute EF, cấu hình ở `OnModelCreating`/`IEntityTypeConfiguration`.
- **DTO** (Application/Api): record bất biến, hậu tố `Request`/`Response`/`Dto`; KHÔNG lộ entity ra ngoài API.
- **Repository** (Infrastructure): interface ở Application (`ICustomerRepository`), impl ở Infrastructure; không trả `IQueryable` ra ngoài.
- **Service/Handler** (Application): mỏng, điều phối; business rule nằm trong Domain. Validate ở validator.
- DI qua constructor; không service locator. Exception nghiệp vụ là type rõ ràng, map sang ProblemDetails ở middleware.

```csharp
public sealed record CreateCustomerRequest(string FullName, string? Phone, string? SourceCode);

public async Task<Result<Guid>> Handle(CreateCustomerCommand cmd, CancellationToken ct)
{
    var customer = Customer.Create(cmd.FullName, cmd.Phone);   // hành vi ở Domain
    await _repo.AddAsync(customer, ct);
    await _uow.SaveChangesAsync(ct);
    return Result.Ok(customer.PublicId);
}
```

## Frontend (Vue 3 + TS)

- `<script setup lang="ts">`; Composition API; Pinia store `useXxxStore`.
- Component PascalCase (`CustomerTable.vue`); composable `useXxx.ts`; type/interface PascalCase.
- Gọi API qua client tập trung (`src/lib/api.ts`); không gọi axios rải rác.
- Không hard-code màu/spacing — dùng class Tailwind ánh xạ token.
- Props có type rõ; tránh `any`. State cục bộ dùng `ref`/`reactive`, state chia sẻ dùng store.

```ts
const auth = useAuthStore()
async function submit() {
  await auth.login(identifier.value, password.value)
}
```

## Comments

- Comment giải thích "tại sao", không mô tả lại "cái gì". Tiếng Việt hoặc Anh nhất quán trong một file.
- Không để code chết / comment-out code. Không TODO bỏ ngỏ (tạo task thay vì TODO).

## Imports & formatting

- Nhóm import: framework → bên thứ ba → nội bộ. Bỏ import thừa.
- Dòng ≤ ~120 ký tự; một câu lệnh một dòng; tránh nested sâu (early return).
