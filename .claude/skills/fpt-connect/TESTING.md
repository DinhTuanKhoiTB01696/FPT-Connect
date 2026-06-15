# TESTING.md — Cách verify trước khi hoàn thành

> Chiến lược đầy đủ + 312 test case: `docs/Project-Bible/12_Testing.md`.

## Thứ tự verify bắt buộc (gate)

1. **Compile** — backend `dotnet build Backend/FptConnect.sln -c Release` không lỗi/cảnh báo mới.
2. **TypeScript** — `cd Frontend && npm run typecheck` sạch.
3. **Lint/format** — theo `.editorconfig`; không cảnh báo lint mới.
4. **Build** — `npm run build` (frontend) thành công.
5. **Unit test** — `dotnet test` + `npm test` pass; thêm test cho code mới.
6. **Integration/API** — chạy test integration liên quan (Testcontainers/WebApplicationFactory) khi đụng DB/endpoint.

## Tầng test (tham chiếu ch.12.1)

| Tầng | Công cụ | Khi nào |
|---|---|---|
| Unit (domain/validator) | xUnit + FluentAssertions | luôn, cho logic mới |
| Architecture | NetArchTest | giữ chiều phụ thuộc Clean Architecture |
| Integration | Testcontainers (SQL/Redis) | đụng persistence/cache |
| API contract | WebApplicationFactory + OpenAPI | mọi endpoint: status/schema/auth/idempotency |
| Frontend | Vitest + Vue Test Utils | store/composable/component lõi |
| E2E | Playwright | smoke + regression luồng chính |

## Coverage & chất lượng

- Domain coverage ≥ 80%. Mỗi command có test happy-path + negative (validation, scope, conflict).
- Bao phủ mọi error code đã khai báo; test idempotency cho command có Idempotency-Key.

## Manual QA checklist (trước PR màn UI)

- [ ] Loading / empty / error / offline / permission-denied hiển thị đúng.
- [ ] Keyboard navigation + focus visible; screen reader đọc được label/lỗi.
- [ ] Dark mode + light mode; tương phản AA.
- [ ] 360px (mobile) không vỡ layout / không scroll ngang ở luồng lõi.
- [ ] Không hard-code màu; dùng token.

## Regression

- Trước release: chạy full regression + security (SAST/DAST) + performance gate (k6) theo ch.12.
- Không merge nếu có lỗi Critical/High hoặc giảm coverage dưới ngưỡng.
