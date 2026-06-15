# /testing — QA nội bộ

Đọc `skills/fpt-connect/TESTING.md` + `docs/Project-Bible/12_Testing.md`.

Cổng theo thứ tự: Compile → Build → Lint → TypeScript → Unit → Integration → Regression.

1. Backend: `dotnet build -c Release` + `dotnet test`. Frontend: `npm run typecheck` + `npm test` + `npm run build`.
2. Bổ sung test cho code mới; domain coverage ≥ 80%; phủ mọi error code; idempotency.
3. Manual QA checklist UI (loading/empty/error/offline, keyboard, dark mode, 360px).

Output: `TEST_REPORT.md`.
