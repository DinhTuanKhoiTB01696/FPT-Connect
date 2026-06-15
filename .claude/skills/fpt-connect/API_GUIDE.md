# API_GUIDE.md — Quy ước API

> Chi tiết: `docs/Project-Bible/08_API.md` + `APPENDIX_C_API_Detailed.md`.

## REST conventions

- Base: `/api/v1`. Resource số nhiều, kebab/lowercase: `/customers`, `/work-orders`.
- Method: GET (đọc), POST (tạo/hành động), PATCH (sửa một phần), DELETE (soft delete). Tránh verb trong path; hành động đặc thù dùng sub-resource: `POST /customers/{id}/merge`.
- JSON camelCase; UTC ISO 8601; charset UTF-8.

## Response wrapper

Success:
```json
{ "data": { }, "meta": { "traceId": "00-abcd", "timestampUtc": "2026-06-11T08:00:00Z" } }
```
List trả `data: []` + `meta: { nextCursor, count }`.

## Error (RFC 9457 Problem Details)

```json
{ "type": "https://fptconnect/errors/validation", "title": "Validation failed",
  "status": 422, "code": "VALIDATION_FAILED", "traceId": "00-abcd",
  "errors": { "phone": ["PHONE_INVALID"] } }
```
Status dùng: 200/201/202/204, 400, 401, 403, 404, 409, 412 (ETag), 422, 429, 503.
Error `code` là hằng UPPER_SNAKE ổn định (vd `CUSTOMER_DUPLICATE`, `RESTORE_CONFLICT`).

## Pagination / Search / Filter / Sort

- **Pagination**: cursor-based, `limit` ≤ 100 + `cursor`. Không offset cho bảng lớn.
- **Search**: `?q=` trên normalized columns (tìm tiếng Việt không dấu).
- **Filter**: query param theo allowlist (`?status=New&ownerId=`). Field ngoài allowlist → 400.
- **Sort**: `?sort=createdAt:desc` theo allowlist; field lạ → `400 SORT_INVALID`.

## Versioning

- Breaking change → `/api/v2`. Non-breaking (thêm field optional) giữ v1.
- OpenAPI là contract executable; mọi endpoint có swagger + contract test.

## Validation

- Validate ở Application (FluentValidation hoặc validator) trước domain.
- DTO request allowlist (chống mass assignment). Boundary rõ ràng (độ dài, range, enum).
- Idempotency: command offline/retry gửi `Idempotency-Key`; update gửi `If-Match: "<rowVersion>"`.

## Bảo mật API (checklist)

- [ ] Object ID kiểm tra scope, không chỉ permission (chống IDOR).
- [ ] Rate limit riêng cho login, geocode, export, AI, GPS ingest.
- [ ] Bulk operation: dryRun-able, giới hạn dòng, audit theo dòng.
- [ ] OpenAPI không chứa secret/PII thật.
