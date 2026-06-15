# /api — Dựng REST API enterprise

Đọc `skills/fpt-connect/API_GUIDE.md` + `docs/Project-Bible/08_API.md`.

Mọi endpoint phải có: Validation (DTO allowlist), Pagination (cursor, limit ≤ 100), Search, Filter (allowlist), Sort (allowlist), Versioning (`/api/v1`), Error wrapper (RFC 9457 + code UPPER_SNAKE), Swagger/OpenAPI, Idempotency-Key (command), If-Match/ETag (update), kiểm tra permission + scope.

1. Chạy `hooks/pre-task.md` + `hooks/architecture-check.md` + `hooks/security-check.md`.
2. Command đổi 1 aggregate/transaction; query projection `AsNoTracking`. Contract test cho mọi status/error.

Output: controller/handler + DTO + test + Swagger.
