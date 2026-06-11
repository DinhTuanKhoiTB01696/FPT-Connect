# Phụ lục C. API Contract chi tiết

> Sinh tự động từ catalog chuẩn trong `08_API.md`. Khi thay endpoint, cập nhật catalog và chạy `node tools/generate-project-bible-appendices.mjs`.

## C.1 Quy tắc áp dụng

Mỗi endpoint bên dưới kế thừa error envelope RFC 9457, UTC, correlation ID, cursor pagination, idempotency và security baseline của chương 8. Phụ lục làm rõ Definition of Done cho implementer và contract tester; OpenAPI sinh từ code vẫn là schema executable.

## API-001 - `POST /auth/login`

**Mục đích.** Thực thi lệnh hoặc tạo auth / login theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Public |
| Request và validation đặc thù | identifier, password, device; rate limit |
| Success response | token pair/MFA challenge |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Theo state/challenge/token; không áp dụng ETag aggregate. |

### Request contract

- Header/auth: Không yêu cầu `Authorization`; vẫn gửi `X-Correlation-ID` và device context allowlisted khi contract cần. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **identifier, password, device; rate limit**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **token pair/MFA challenge**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Credential không hợp lệ; response không tiết lộ identifier có tồn tại. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Endpoint public phải rate-limit theo IP/device fingerprint, dùng lỗi không tiết lộ account và không ghi credential vào log.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Không dùng response cache. Replay bị kiểm soát bằng challenge/token state, transaction và rate limit; refresh rotation phải chống race.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-002 - `POST /auth/mfa/verify`

**Mục đích.** Thực thi lệnh hoặc tạo auth / mfa / verify theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Challenge |
| Request và validation đặc thù | challengeId, 6-digit OTP |
| Success response | token pair |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Theo state/challenge/token; không áp dụng ETag aggregate. |

### Request contract

- Header/auth: Gửi challenge ID/token ngắn hạn theo request contract; không yêu cầu Bearer access token. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **challengeId, 6-digit OTP**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **token pair**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Challenge/OTP hết hạn, sai hoặc đã dùng; chưa tạo access session. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Xác thực bằng challenge ngắn hạn thay cho Bearer access token. Challenge phải bind với login attempt/device, hết hạn nhanh và dùng một lần.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Không dùng response cache. Replay bị kiểm soát bằng challenge/token state, transaction và rate limit; refresh rotation phải chống race.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-003 - `POST /auth/refresh`

**Mục đích.** Thực thi lệnh hoặc tạo auth / refresh theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Refresh |
| Request và validation đặc thù | refreshToken; rotation |
| Success response | token pair |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Theo state/challenge/token; không áp dụng ETag aggregate. |

### Request contract

- Header/auth: Gửi refresh token trong body/cookie bảo vệ theo auth design; không dùng access token hết hạn để xác thực refresh. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **refreshToken; rotation**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **token pair**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Refresh token hết hạn/revoke/reuse; reuse làm revoke token family. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Xác thực bằng refresh token; server chỉ lưu hash, rotation trong transaction và revoke toàn family khi phát hiện reuse.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Không dùng response cache. Replay bị kiểm soát bằng challenge/token state, transaction và rate limit; refresh rotation phải chống race.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-004 - `POST /auth/logout`

**Mục đích.** Thực thi lệnh hoặc tạo auth / logout theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | User |
| Request và validation đặc thù | session/current/all flag |
| Success response | 204 |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Theo state/challenge/token; không áp dụng ETag aggregate. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **session/current/all flag**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **204**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `User`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-005 - `GET /auth/me`

**Mục đích.** Truy vấn auth / me theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | User |
| Request và validation đặc thù | - |
| Success response | profile + effective scopes |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **-**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **profile + effective scopes**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `User`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-006 - `POST /auth/mfa/enroll`

**Mục đích.** Thực thi lệnh hoặc tạo auth / mfa / enroll theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | User |
| Request và validation đặc thù | reauth |
| Success response | secret QR one-time |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Theo state/challenge/token; không áp dụng ETag aggregate. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **reauth**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **secret QR one-time**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `User`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-007 - `POST /auth/mfa/confirm`

**Mục đích.** Thực thi lệnh hoặc tạo auth / mfa / confirm theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | User |
| Request và validation đặc thù | OTP |
| Success response | recovery codes one-time |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Theo state/challenge/token; không áp dụng ETag aggregate. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **OTP**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **recovery codes one-time**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `User`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-008 - `GET /sessions`

**Mục đích.** Truy vấn sessions theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | User |
| Request và validation đặc thù | cursor |
| Success response | sessions masked |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **cursor**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **sessions masked**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `User`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-009 - `DELETE /sessions/{id}`

**Mục đích.** Thu hồi hoặc xóa sessions / tài nguyên xác định theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Owner/Admin |
| Request và validation đặc thù | id in scope |
| Success response | 204 |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **id in scope**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **204**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 204 | Kết quả idempotent; resource không còn active/accessible. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Owner/Admin`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-010 - `GET /users`

**Mục đích.** Truy vấn users theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | users.read |
| Request và validation đặc thù | department/status/search |
| Success response | page |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **department/status/search**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **page**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `users.read`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-011 - `POST /users`

**Mục đích.** Thực thi lệnh hoặc tạo users theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | users.manage |
| Request và validation đặc thù | unique employee/email |
| Success response | 201` user |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **unique employee/email**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **201` user**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `users.manage`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-012 - `PATCH /users/{id}`

**Mục đích.** Cập nhật một phần users / tài nguyên xác định theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | users.manage |
| Request và validation đặc thù | ETag, scoped fields |
| Success response | user |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **ETag, scoped fields**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **user**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/204 | Cập nhật thành công; trả version mới khi có aggregate. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `users.manage`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-013 - `POST /users/{id}/lock`

**Mục đích.** Thực thi lệnh hoặc tạo users / tài nguyên xác định / lock theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | users.manage |
| Request và validation đặc thù | reason |
| Success response | 204 |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **reason**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **204**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `users.manage`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-014 - `GET /roles`

**Mục đích.** Truy vấn roles theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | roles.read |
| Request và validation đặc thù | - |
| Success response | roles |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **-**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **roles**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `roles.read`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-015 - `POST /roles`

**Mục đích.** Thực thi lệnh hoặc tạo roles theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | roles.manage |
| Request và validation đặc thù | code/name/permissions |
| Success response | role |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **code/name/permissions**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **role**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `roles.manage`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-016 - `PUT /roles/{id}/permissions`

**Mục đích.** Thay thế/công bố roles / tài nguyên xác định / permissions theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | roles.manage |
| Request và validation đặc thù | permissionIds/version |
| Success response | role version |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **permissionIds/version**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **role version**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/204 | Cập nhật thành công; trả version mới khi có aggregate. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `roles.manage`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-017 - `PUT /users/{id}/roles`

**Mục đích.** Thay thế/công bố users / tài nguyên xác định / roles theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | roles.assign |
| Request và validation đặc thù | roles + scopes |
| Success response | assignments |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **roles + scopes**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **assignments**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/204 | Cập nhật thành công; trả version mới khi có aggregate. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `roles.assign`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-018 - `GET /departments/tree`

**Mục đích.** Truy vấn departments / tree theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | org.read |
| Request và validation đặc thù | rootId |
| Success response | tree |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **rootId**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **tree**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `org.read`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-019 - `POST /departments`

**Mục đích.** Thực thi lệnh hoặc tạo departments theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | org.manage |
| Request và validation đặc thù | parent/code/name |
| Success response | department |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **parent/code/name**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **department**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `org.manage`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-020 - `GET /territories`

**Mục đích.** Truy vấn territories theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | territories.read |
| Request và validation đặc thù | effectiveAt |
| Success response | GeoJSON list |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **effectiveAt**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **GeoJSON list**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `territories.read`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-021 - `POST /territories`

**Mục đích.** Thực thi lệnh hoặc tạo territories theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | territories.manage |
| Request và validation đặc thù | valid GeoJSON polygon |
| Success response | territory |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **valid GeoJSON polygon**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **territory**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `territories.manage`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-022 - `POST /territories/import`

**Mục đích.** Thực thi lệnh hoặc tạo territories / import theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | territories.manage |
| Request và validation đặc thù | GeoJSON file |
| Success response | job |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **GeoJSON file**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **job**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `territories.manage`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-023 - `GET /customers`

**Mục đích.** Truy vấn customers theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | customers.read |
| Request và validation đặc thù | filter/sort/cursor |
| Success response | scoped page |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **filter/sort/cursor**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **scoped page**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `customers.read`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-024 - `POST /customers`

**Mục đích.** Thực thi lệnh hoặc tạo customers theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | customers.create |
| Request và validation đặc thù | name, phone, source |
| Success response | 201` + duplicates |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **name, phone, source**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **201` + duplicates**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `customers.create`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-025 - `GET /customers/{id}`

**Mục đích.** Truy vấn customers / tài nguyên xác định theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | customers.read |
| Request và validation đặc thù | include allowlist |
| Success response | 360 summary |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **include allowlist**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **360 summary**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `customers.read`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-026 - `PATCH /customers/{id}`

**Mục đích.** Cập nhật một phần customers / tài nguyên xác định theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | customers.update |
| Request và validation đặc thù | ETag, patch allowlist |
| Success response | customer |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **ETag, patch allowlist**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **customer**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/204 | Cập nhật thành công; trả version mới khi có aggregate. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `customers.update`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-027 - `DELETE /customers/{id}`

**Mục đích.** Thu hồi hoặc xóa customers / tài nguyên xác định theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | customers.delete |
| Request và validation đặc thù | reason/reauth |
| Success response | 204` soft delete |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **reason/reauth**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **204` soft delete**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 204 | Kết quả idempotent; resource không còn active/accessible. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `customers.delete`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-028 - `POST /customers/imports`

**Mục đích.** Thực thi lệnh hoặc tạo customers / imports theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | customers.import |
| Request và validation đặc thù | fileId/mapping/dryRun |
| Success response | import job |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **fileId/mapping/dryRun**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **import job**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `customers.import`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-029 - `GET /customers/imports/{id}`

**Mục đích.** Truy vấn customers / imports / tài nguyên xác định theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | customers.import |
| Request và validation đặc thù | - |
| Success response | progress/report |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **-**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **progress/report**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `customers.import`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-030 - `POST /customers/duplicates/search`

**Mục đích.** Thực thi lệnh hoặc tạo customers / duplicates / search theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | customers.read |
| Request và validation đặc thù | normalized fields |
| Success response | candidates |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **normalized fields**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **candidates**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `customers.read`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-031 - `POST /customers/{id}/merge`

**Mục đích.** Thực thi lệnh hoặc tạo customers / tài nguyên xác định / merge theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | customers.merge |
| Request và validation đặc thù | duplicateId/field decisions |
| Success response | survivor |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **duplicateId/field decisions**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **survivor**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `customers.merge`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-032 - `POST /customers/{id}/status-transitions`

**Mục đích.** Thực thi lệnh hoặc tạo customers / tài nguyên xác định / status-transitions theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | customers.update |
| Request và validation đặc thù | target/reason/version |
| Success response | customer |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **target/reason/version**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **customer**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `customers.update`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-033 - `POST /customers/{id}/assignments`

**Mục đích.** Thực thi lệnh hoặc tạo customers / tài nguyên xác định / assignments theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | customers.assign |
| Request và validation đặc thù | userId/reason |
| Success response | assignment |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **userId/reason**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **assignment**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `customers.assign`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-034 - `GET /customers/{id}/timeline`

**Mục đích.** Truy vấn customers / tài nguyên xác định / timeline theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | customers.read |
| Request và validation đặc thù | types/cursor |
| Success response | events |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **types/cursor**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **events**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `customers.read`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-035 - `POST /customers/{id}/interactions`

**Mục đích.** Thực thi lệnh hoặc tạo customers / tài nguyên xác định / interactions theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | interactions.create |
| Request và validation đặc thù | type/outcome/content |
| Success response | interaction |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **type/outcome/content**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **interaction**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `interactions.create`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-036 - `GET /customers/nearby`

**Mục đích.** Truy vấn customers / nearby theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | customers.read |
| Request và validation đặc thù | lat/lng/radius<=20km |
| Success response | distance page |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **lat/lng/radius<=20km**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **distance page**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `customers.read`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-037 - `POST /geocoding/resolve`

**Mục đích.** Thực thi lệnh hoặc tạo geocoding / resolve theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | maps.use |
| Request và validation đặc thù | address/country=VN |
| Success response | candidates |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **address/country=VN**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **candidates**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `maps.use`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-038 - `POST /geocoding/reverse`

**Mục đích.** Thực thi lệnh hoặc tạo geocoding / reverse theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | maps.use |
| Request và validation đặc thù | valid lat/lng |
| Success response | address |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **valid lat/lng**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **address**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `maps.use`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-039 - `GET /visits`

**Mục đích.** Truy vấn visits theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | visits.read |
| Request và validation đặc thù | assignee/date/status |
| Success response | page/calendar |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **assignee/date/status**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **page/calendar**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `visits.read`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-040 - `POST /visits`

**Mục đích.** Thực thi lệnh hoặc tạo visits theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | visits.create |
| Request và validation đặc thù | customer/time/purpose |
| Success response | visit |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **customer/time/purpose**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **visit**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `visits.create`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-041 - `GET /visits/{id}`

**Mục đích.** Truy vấn visits / tài nguyên xác định theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | visits.read |
| Request và validation đặc thù | - |
| Success response | detail |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **-**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **detail**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `visits.read`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-042 - `PATCH /visits/{id}`

**Mục đích.** Cập nhật một phần visits / tài nguyên xác định theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | visits.update |
| Request và validation đặc thù | ETag |
| Success response | visit |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **ETag**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **visit**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/204 | Cập nhật thành công; trả version mới khi có aggregate. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `visits.update`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-043 - `POST /visits/{id}/cancel`

**Mục đích.** Thực thi lệnh hoặc tạo visits / tài nguyên xác định / cancel theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | visits.update |
| Request và validation đặc thù | reason |
| Success response | visit |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **reason**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **visit**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `visits.update`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-044 - `POST /visits/{id}/check-ins`

**Mục đích.** Thực thi lệnh hoặc tạo visits / tài nguyên xác định / check-ins theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | checkins.create |
| Request và validation đặc thù | coordinates/accuracy/time |
| Success response | validation result |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **coordinates/accuracy/time**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **validation result**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `checkins.create`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-045 - `POST /visits/{id}/check-outs`

**Mục đích.** Thực thi lệnh hoặc tạo visits / tài nguyên xác định / check-outs theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | checkins.create |
| Request và validation đặc thù | location/outcome/checklist |
| Success response | result |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **location/outcome/checklist**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **result**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `checkins.create`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-046 - `GET /check-ins/review-queue`

**Mục đích.** Truy vấn check-ins / review-queue theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | checkins.review |
| Request và validation đặc thù | status/team |
| Success response | page |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **status/team**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **page**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `checkins.review`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-047 - `POST /check-ins/{id}/decisions`

**Mục đích.** Thực thi lệnh hoặc tạo check-ins / tài nguyên xác định / decisions theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | checkins.review |
| Request và validation đặc thù | approve/reject/reason |
| Success response | decision |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **approve/reject/reason**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **decision**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `checkins.review`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-048 - `POST /tracking/sessions`

**Mục đích.** Thực thi lệnh hoặc tạo tracking / sessions theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | tracking.create |
| Request và validation đặc thù | device/consentVersion |
| Success response | session |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **device/consentVersion**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **session**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `tracking.create`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-049 - `POST /tracking/sessions/{id}/points:batch`

**Mục đích.** Thực thi lệnh hoặc tạo tracking / sessions / tài nguyên xác định / points:batch theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Owner |
| Request và validation đặc thù | 1..100 ordered points |
| Success response | accepted sequence |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **1..100 ordered points**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **accepted sequence**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Owner`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-050 - `POST /tracking/sessions/{id}/stop`

**Mục đích.** Thực thi lệnh hoặc tạo tracking / sessions / tài nguyên xác định / stop theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Owner/System |
| Request và validation đặc thù | stoppedAt/reason |
| Success response | summary pending |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **stoppedAt/reason**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **summary pending**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Owner/System`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-051 - `GET /tracking/sessions`

**Mục đích.** Truy vấn tracking / sessions theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | tracking.read.* |
| Request và validation đặc thù | user/date |
| Success response | page |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **user/date**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **page**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `tracking.read.*`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-052 - `GET /tracking/sessions/{id}/route`

**Mục đích.** Truy vấn tracking / sessions / tài nguyên xác định / route theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | tracking.read.* |
| Request và validation đặc thù | simplification level |
| Success response | polyline/stops/gaps |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **simplification level**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **polyline/stops/gaps**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `tracking.read.*`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-053 - `GET /reminders`

**Mục đích.** Truy vấn reminders theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Owner/Manager |
| Request và validation đặc thù | status/due/customer |
| Success response | page |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **status/due/customer**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **page**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Owner/Manager`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-054 - `POST /reminders`

**Mục đích.** Thực thi lệnh hoặc tạo reminders theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | reminders.create |
| Request và validation đặc thù | title/due/recurrence |
| Success response | reminder |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **title/due/recurrence**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **reminder**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `reminders.create`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-055 - `PATCH /reminders/{id}`

**Mục đích.** Cập nhật một phần reminders / tài nguyên xác định theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Owner |
| Request và validation đặc thù | ETag |
| Success response | reminder |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **ETag**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **reminder**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/204 | Cập nhật thành công; trả version mới khi có aggregate. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Owner`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-056 - `POST /reminders/{id}/complete`

**Mục đích.** Thực thi lệnh hoặc tạo reminders / tài nguyên xác định / complete theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Owner |
| Request và validation đặc thù | outcome/interactionId |
| Success response | reminder |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **outcome/interactionId**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **reminder**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Owner`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-057 - `POST /reminders/{id}/snooze`

**Mục đích.** Thực thi lệnh hoặc tạo reminders / tài nguyên xác định / snooze theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Owner |
| Request và validation đặc thù | until/reason |
| Success response | reminder |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **until/reason**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **reminder**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Owner`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-058 - `GET /notifications`

**Mục đích.** Truy vấn notifications theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Owner |
| Request và validation đặc thù | unread/cursor |
| Success response | inbox |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **unread/cursor**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **inbox**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Owner`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-059 - `POST /notifications/{id}/read`

**Mục đích.** Thực thi lệnh hoặc tạo notifications / tài nguyên xác định / read theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Owner |
| Request và validation đặc thù | - |
| Success response | 204 |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **-**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **204**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Owner`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-060 - `POST /notifications/read-all`

**Mục đích.** Thực thi lệnh hoặc tạo notifications / read-all theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Owner |
| Request và validation đặc thù | beforeUtc |
| Success response | count |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **beforeUtc**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **count**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Owner`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-061 - `GET /notification-preferences`

**Mục đích.** Truy vấn notification-preferences theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Owner |
| Request và validation đặc thù | - |
| Success response | matrix |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **-**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **matrix**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Owner`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-062 - `PUT /notification-preferences`

**Mục đích.** Thay thế/công bố notification-preferences theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Owner |
| Request và validation đặc thù | channel/event settings |
| Success response | matrix |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **channel/event settings**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **matrix**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/204 | Cập nhật thành công; trả version mới khi có aggregate. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Owner`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-063 - `POST /files/upload-requests`

**Mục đích.** Thực thi lệnh hoặc tạo files / upload-requests theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Resource writer |
| Request và validation đặc thù | name/type/size/hash |
| Success response | signed upload |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **name/type/size/hash**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **signed upload**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Resource writer`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-064 - `POST /files/{id}/complete`

**Mục đích.** Thực thi lệnh hoặc tạo files / tài nguyên xác định / complete theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Uploader |
| Request và validation đặc thù | etag/storage proof |
| Success response | scan pending |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **etag/storage proof**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **scan pending**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Uploader`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-065 - `GET /files/{id}/download`

**Mục đích.** Truy vấn files / tài nguyên xác định / download theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Resource reader |
| Request và validation đặc thù | - |
| Success response | short signed URL |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **-**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **short signed URL**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Resource reader`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-066 - `POST /customers/{id}/contracts`

**Mục đích.** Thực thi lệnh hoặc tạo customers / tài nguyên xác định / contracts theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | contracts.create |
| Request và validation đặc thù | package/value/reference |
| Success response | contract |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **package/value/reference**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **contract**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `contracts.create`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-067 - `PATCH /contracts/{id}`

**Mục đích.** Cập nhật một phần contracts / tài nguyên xác định theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | contracts.update |
| Request và validation đặc thù | ETag |
| Success response | contract |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **ETag**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **contract**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/204 | Cập nhật thành công; trả version mới khi có aggregate. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `contracts.update`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-068 - `POST /contracts/{id}/handoffs`

**Mục đích.** Thực thi lệnh hoặc tạo contracts / tài nguyên xác định / handoffs theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | handoffs.create |
| Request và validation đặc thù | checklist/window |
| Success response | handoff |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **checklist/window**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **handoff**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `handoffs.create`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-069 - `POST /handoffs/{id}/decision`

**Mục đích.** Thực thi lệnh hoặc tạo handoffs / tài nguyên xác định / decision theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | handoffs.approve |
| Request và validation đặc thù | decision/reason |
| Success response | handoff/work order |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **decision/reason**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **handoff/work order**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `handoffs.approve`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-070 - `GET /work-orders`

**Mục đích.** Truy vấn work-orders theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | workorders.read |
| Request và validation đặc thù | assignee/status/date |
| Success response | page |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **assignee/status/date**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **page**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `workorders.read`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-071 - `POST /work-orders/{id}/assign`

**Mục đích.** Thực thi lệnh hoặc tạo work-orders / tài nguyên xác định / assign theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | workorders.assign |
| Request và validation đặc thù | assignee/schedule |
| Success response | work order |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **assignee/schedule**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **work order**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `workorders.assign`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-072 - `POST /work-orders/{id}/accept`

**Mục đích.** Thực thi lệnh hoặc tạo work-orders / tài nguyên xác định / accept theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Assignee |
| Request và validation đặc thù | - |
| Success response | work order |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **-**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **work order**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Assignee`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-073 - `POST /work-orders/{id}/progress`

**Mục đích.** Thực thi lệnh hoặc tạo work-orders / tài nguyên xác định / progress theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Assignee |
| Request và validation đặc thù | state/checklist/note |
| Success response | work order |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **state/checklist/note**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **work order**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Assignee`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-074 - `POST /work-orders/{id}/complete`

**Mục đích.** Thực thi lệnh hoặc tạo work-orders / tài nguyên xác định / complete theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Assignee |
| Request và validation đặc thù | result/evidence |
| Success response | work order |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **result/evidence**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **work order**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Assignee`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-075 - `POST /work-orders/{id}/revisit`

**Mục đích.** Thực thi lệnh hoặc tạo work-orders / tài nguyên xác định / revisit theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Tech/Manager |
| Request và validation đặc thù | reason/schedule |
| Success response | child work order |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **reason/schedule**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **child work order**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Tech/Manager`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-076 - `GET /dashboard/me`

**Mục đích.** Truy vấn dashboard / me theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | User |
| Request và validation đặc thù | period |
| Success response | personal widgets |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **period**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **personal widgets**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `User`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-077 - `GET /dashboard/team`

**Mục đích.** Truy vấn dashboard / team theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | analytics.read.team |
| Request và validation đặc thù | team/period |
| Success response | team widgets |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **team/period**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **team widgets**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `analytics.read.team`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-078 - `POST /exports`

**Mục đích.** Thực thi lệnh hoặc tạo exports theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | exports.create |
| Request và validation đặc thù | template/filter/purpose |
| Success response | job |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **template/filter/purpose**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **job**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `exports.create`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-079 - `GET /exports/{id}`

**Mục đích.** Truy vấn exports / tài nguyên xác định theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Owner/Auditor |
| Request và validation đặc thù | - |
| Success response | status/download |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **-**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **status/download**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `Owner/Auditor`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-080 - `GET /audit-logs`

**Mục đích.** Truy vấn audit-logs theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | audit.read |
| Request và validation đặc thù | actor/action/resource/time |
| Success response | page |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **actor/action/resource/time**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **page**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `audit.read`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-081 - `GET /settings`

**Mục đích.** Truy vấn settings theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | settings.read |
| Request và validation đặc thù | namespace |
| Success response | typed values |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **namespace**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **typed values**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `settings.read`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-082 - `PUT /settings/{key}`

**Mục đích.** Thay thế/công bố settings / tài nguyên xác định theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | settings.manage |
| Request và validation đặc thù | schema-valid value |
| Success response | version |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **schema-valid value**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **version**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/204 | Cập nhật thành công; trả version mới khi có aggregate. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `settings.manage`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-083 - `POST /ai/customers/{id}/summary`

**Mục đích.** Thực thi lệnh hoặc tạo ai / customers / tài nguyên xác định / summary theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | ai.summary |
| Request và validation đặc thù | promptVersion optional |
| Success response | AI run/result |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **promptVersion optional**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **AI run/result**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `ai.summary`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-084 - `POST /ai/customers/{id}/next-actions`

**Mục đích.** Thực thi lệnh hoặc tạo ai / customers / tài nguyên xác định / next-actions theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | ai.suggest |
| Request và validation đặc thù | constraints |
| Success response | suggestions |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **constraints**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **suggestions**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `ai.suggest`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-085 - `GET /ai/customers/{id}/score`

**Mục đích.** Truy vấn ai / customers / tài nguyên xác định / score theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | ai.score |
| Request và validation đặc thù | - |
| Success response | score/factors/version |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **-**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **score/factors/version**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `ai.score`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-086 - `POST /ai/routes/optimize`

**Mục đích.** Thực thi lệnh hoặc tạo ai / routes / optimize theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | ai.route |
| Request và validation đặc thù | visits/start/constraints |
| Success response | route plan |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **visits/start/constraints**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **route plan**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `ai.route`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-087 - `POST /ai/runs/{id}/feedback`

**Mục đích.** Thực thi lệnh hoặc tạo ai / runs / tài nguyên xác định / feedback theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | User |
| Request và validation đặc thù | rating/reason/comment |
| Success response | feedback |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới. |

### Request contract

- Header/auth: Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **rating/reason/comment**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **feedback**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |
| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |
| 403 | Thiếu permission hoặc resource ngoài organizational scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Authorization bắt buộc: `User`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.
- Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.

### Idempotency, consistency và cache

Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-088 - `GET /health/live`

**Mục đích.** Truy vấn health / live theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Infrastructure |
| Request và validation đặc thù | - |
| Success response | liveness |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Probe qua network/identity hạ tầng được kiểm soát; không dùng user Bearer token. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **-**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **liveness**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Chỉ expose qua network/probe được kiểm soát; response không chứa connection string, version nhạy cảm hoặc chi tiết dependency.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

## API-089 - `GET /health/ready`

**Mục đích.** Truy vấn health / ready theo đúng organizational scope và business rule của use case liên quan.

| Thuộc tính | Contract |
|---|---|
| Permission/scope | Infrastructure |
| Request và validation đặc thù | - |
| Success response | dependency readiness |
| Content type | `application/json; charset=utf-8`, trừ upload/download đã ký |
| Version/concurrency | Read-only; dùng cursor/cutoff khi phân trang. |

### Request contract

- Header/auth: Probe qua network/identity hạ tầng được kiểm soát; không dùng user Bearer token. Command retryable thêm `Idempotency-Key` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **-**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **dependency readiness**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả `data[]` và `meta.nextCursor`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |
| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |
| 429 | Vượt rate limit/quota của endpoint. |
| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |

### Security notes

- Chỉ expose qua network/probe được kiểm soát; response không chứa connection string, version nhạy cảm hoặc chi tiết dependency.

### Idempotency, consistency và cache

Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.

### Observability và test bắt buộc

- Log event có `traceId`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.

