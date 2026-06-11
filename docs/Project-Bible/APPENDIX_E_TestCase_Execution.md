# Phụ lục E. Phiếu thực thi 300 Test Case

> Sinh tự động từ catalog `12_Testing.md`. Mỗi mục là test specification có thể chuyển sang TestRail/Xray/Azure Test Plans; scenario và expected trong chương 12 là nguồn chuẩn.

## E.1 Quy ước bằng chứng

- API: request/response redacted, trace ID, database/outbox/audit assertions.
- UI: screenshot/video chỉ khi lỗi, DOM/accessibility assertion ưu tiên hơn pixel-only.
- Performance: script, dataset version, topology, raw result và percentile.
- Security: payload, affected control và evidence đã loại secret/PII.
- Recovery: backup ID, restore timeline, integrity check và actual RPO/RTO.

## TC-001 - Login đúng Sale active

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Authentication |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-001 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Login đúng Sale active**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **200`, token 10 phút, session/audit tạo**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-002 - Sai password 1 lần

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Authentication |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-002 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Sai password 1 lần**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **401` chung, không tiết lộ account tồn tại**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-003 - Sai 5 lần liên tiếp

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Authentication |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-003 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Sai 5 lần liên tiếp**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Delay/lock theo policy, security metric tăng**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-004 - User locked login đúng password

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Authentication |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-004 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **User locked login đúng password**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **403 ACCOUNT_LOCKED`, không tạo session**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-005 - Tenant disabled

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Authentication |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-005 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Tenant disabled**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **403 TENANT_DISABLED**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-006 - Admin chưa MFA

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Authentication |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-006 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Admin chưa MFA**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Trả MFA challenge, chưa trả access token**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-007 - OTP đúng trong time window

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Authentication |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-007 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **OTP đúng trong time window**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Token pair phát một lần**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-008 - OTP hết hạn/replay

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Authentication |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-008 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **OTP hết hạn/replay**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **401 MFA_INVALID`, challenge không tái dùng**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-009 - Login payload có SQL/XSS

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Authentication |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-009 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Login payload có SQL/XSS**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không execute/reflect; validation an toàn**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-010 - 100 login/phút cùng IP

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Authentication |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-010 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **100 login/phút cùng IP**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **429`, `Retry-After`, user hợp lệ không bị lock vĩnh viễn**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-011 - Refresh token hợp lệ

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Token/session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-011 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Refresh token hợp lệ**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Token cũ revoke, pair mới trả**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-012 - Reuse refresh token cũ

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Token/session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-012 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Reuse refresh token cũ**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Revoke family, security event**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-013 - Hai refresh đồng thời

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Token/session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-013 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Hai refresh đồng thời**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Một `200`, một reuse/conflict**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-014 - Refresh hết absolute lifetime

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Token/session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-014 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Refresh hết absolute lifetime**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **401`, yêu cầu login**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-015 - Logout current session

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Token/session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-015 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Logout current session**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Refresh fail <= 5 giây**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-016 - Logout all devices

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Token/session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-016 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Logout all devices**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Mọi family của user revoke**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-017 - Admin lock user

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Token/session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-017 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Admin lock user**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Session revoke và SignalR disconnect**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-018 - JWT sai signature

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Token/session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-018 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **JWT sai signature**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **401`, không gọi handler**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-019 - JWT đúng nhưng sai audience

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Token/session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-019 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **JWT đúng nhưng sai audience**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **401**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-020 - Access token hết hạn 1 giây

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Token/session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-020 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Access token hết hạn 1 giây**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **401` với clock skew theo policy**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-021 - Branch Admin tạo user cùng branch

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | IAM |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-021 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Branch Admin tạo user cùng branch**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Thành công, activation event**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-022 - Branch Admin tạo user branch khác

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | IAM |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-022 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Branch Admin tạo user branch khác**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **403**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-023 - Employee code trùng tenant

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | IAM |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-023 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Employee code trùng tenant**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **409 USER_DUPLICATE**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-024 - Cùng code khác tenant

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | IAM |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-024 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Cùng code khác tenant**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Cho phép**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-025 - Role mới với permission hợp lệ

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | IAM |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-025 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Role mới với permission hợp lệ**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Version 1 và audit diff**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-026 - Gán permission vượt quyền admin

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | IAM |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-026 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Gán permission vượt quyền admin**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **403**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-027 - Xóa role đang gán

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | IAM |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-027 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Xóa role đang gán**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Chặn hoặc require reassignment**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-028 - Role change khi user online

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | IAM |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-028 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Role change khi user online**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Quyền mới hiệu lực <= 60 giây**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-029 - User tự nâng role qua mass assignment

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | IAM |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-029 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **User tự nâng role qua mass assignment**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Field bị bỏ/chặn**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-030 - Manager query users

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | IAM |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-030 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Manager query users**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Chỉ trả team scope**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-031 - Tạo department con hợp lệ

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Organization/territory |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-031 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed cây phòng ban và polygon WGS84 đã biết; actor có scope cụ thể để kiểm tra boundary.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Tạo department con hợp lệ**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Path đúng, tree hiển thị**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-032 - Parent là descendant

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Organization/territory |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-032 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed cây phòng ban và polygon WGS84 đã biết; actor có scope cụ thể để kiểm tra boundary.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Parent là descendant**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **422 ORG_CYCLE**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-033 - Xóa department có active users

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Organization/territory |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-033 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed cây phòng ban và polygon WGS84 đã biết; actor có scope cụ thể để kiểm tra boundary.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Xóa department có active users**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Chặn và trả dependency count**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-034 - Import GeoJSON polygon hợp lệ

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Organization/territory |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-034 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed cây phòng ban và polygon WGS84 đã biết; actor có scope cụ thể để kiểm tra boundary.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Import GeoJSON polygon hợp lệ**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Preview area và publish version**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-035 - Polygon tự cắt

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Organization/territory |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-035 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed cây phòng ban và polygon WGS84 đã biết; actor có scope cụ thể để kiểm tra boundary.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Polygon tự cắt**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **422 GEOMETRY_INVALID**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-036 - Polygon chưa đóng

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Organization/territory |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-036 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed cây phòng ban và polygon WGS84 đã biết; actor có scope cụ thể để kiểm tra boundary.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Polygon chưa đóng**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Server normalize hoặc reject rõ**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-037 - Hai territory overlap cấm

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Organization/territory |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-037 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed cây phòng ban và polygon WGS84 đã biết; actor có scope cụ thể để kiểm tra boundary.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Hai territory overlap cấm**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Cảnh báo/chặn theo policy**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-038 - Lead nằm trên boundary

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Organization/territory |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-038 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed cây phòng ban và polygon WGS84 đã biết; actor có scope cụ thể để kiểm tra boundary.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Lead nằm trên boundary**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Rule tie-break deterministic**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-039 - Version territory tương lai

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Organization/territory |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-039 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed cây phòng ban và polygon WGS84 đã biết; actor có scope cụ thể để kiểm tra boundary.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Version territory tương lai**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Chỉ active từ effective time**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-040 - Manager xem polygon ngoài scope

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Organization/territory |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-040 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed cây phòng ban và polygon WGS84 đã biết; actor có scope cụ thể để kiểm tra boundary.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Manager xem polygon ngoài scope**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không trả geometry**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-041 - Tạo lead đủ dữ liệu

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer create/dedupe |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-041 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Tạo lead đủ dữ liệu**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **201`, status New, timeline/SLA**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-042 - Phone `0901234567

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer create/dedupe |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-042 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Phone `0901234567**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Normalize `+84901234567**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-043 - Phone sai độ dài

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer create/dedupe |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-043 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Phone sai độ dài**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **422 PHONE_INVALID**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-044 - Exact phone duplicate

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer create/dedupe |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-044 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Exact phone duplicate**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **409`, không tạo row**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-045 - Tên+địa chỉ fuzzy match

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer create/dedupe |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-045 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Tên+địa chỉ fuzzy match**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Cảnh báo candidate, cho quyết định**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-046 - Offline retry cùng idempotency key

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer create/dedupe |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-046 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Offline retry cùng idempotency key**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Một customer duy nhất**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-047 - Không geocode được

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer create/dedupe |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-047 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Không geocode được**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Lưu với needsLocation flag**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-048 - Pin ngoài Việt Nam theo policy

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer create/dedupe |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-048 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Pin ngoài Việt Nam theo policy**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **422 LOCATION_OUT_OF_SCOPE**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-049 - Source không tồn tại

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer create/dedupe |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-049 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Source không tồn tại**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **422 SOURCE_INVALID**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-050 - User không có create permission

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer create/dedupe |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-050 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **User không có create permission**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **403`, DB không đổi**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-051 - Owner mở Customer 360

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer read/update |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-051 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Owner mở Customer 360**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Đủ tab theo permission**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-052 - Kỹ thuật mở customer được handoff

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer read/update |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-052 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Kỹ thuật mở customer được handoff**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Chỉ field cần triển khai**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-053 - Sale khác team đoán UUID

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer read/update |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-053 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Sale khác team đoán UUID**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **404/403` theo anti-enumeration policy**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-054 - Update đúng ETag

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer read/update |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-054 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Update đúng ETag**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **200`, version tăng, history**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-055 - Update ETag cũ

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer read/update |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-055 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Update ETag cũ**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **412/409`, không overwrite**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-056 - Patch ownerId không allowlist

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer read/update |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-056 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Patch ownerId không allowlist**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **422` hoặc field ignored có log**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-057 - Soft-deleted customer trong list

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer read/update |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-057 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Soft-deleted customer trong list**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không xuất hiện mặc định**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-058 - Auditor xem deleted tombstone

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer read/update |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-058 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Auditor xem deleted tombstone**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Thấy metadata, PII theo permission**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-059 - Search không dấu tên Việt

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer read/update |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-059 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Search không dấu tên Việt**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Kết quả theo normalized search**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-060 - Sort field injection

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Customer read/update |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-060 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Sort field injection**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **400 SORT_INVALID**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-061 - New -> Contacted

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Pipeline/assignment/merge |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-061 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **New -> Contacted**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **History và automation một lần**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-062 - New -> Contracted trực tiếp

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Pipeline/assignment/merge |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-062 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **New -> Contracted trực tiếp**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **422 TRANSITION_INVALID**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-063 - Qualified -> Lost thiếu reason

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Pipeline/assignment/merge |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-063 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Qualified -> Lost thiếu reason**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **422 LOSS_REASON_REQUIRED**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-064 - Manager override transition

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Pipeline/assignment/merge |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-064 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Manager override transition**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Thành công với reason/audit**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-065 - Assign active in-scope Sale

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Pipeline/assignment/merge |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-065 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Assign active in-scope Sale**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Một primary owner**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-066 - Assign locked user

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Pipeline/assignment/merge |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-066 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Assign locked user**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **422 ASSIGNEE_INACTIVE**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-067 - Hai assignment đồng thời

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Pipeline/assignment/merge |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-067 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Hai assignment đồng thời**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Một thắng, một conflict**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-068 - Merge giữ survivor

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Pipeline/assignment/merge |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-068 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Merge giữ survivor**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Child records chuyển, redirect tồn tại**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-069 - Merge contract conflict

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Pipeline/assignment/merge |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-069 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Merge contract conflict**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Chặn và yêu cầu resolution**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-070 - Mark not-duplicate

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Pipeline/assignment/merge |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-070 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Mark not-duplicate**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Pair suppression lưu, không merge**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-071 - CSV UTF-8 template đúng

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Import |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-071 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **CSV UTF-8 template đúng**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Dry-run counts chính xác**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-072 - XLSX 10.000 dòng

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Import |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-072 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **XLSX 10.000 dòng**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Job async, progress**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-073 - File > 20 MB

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Import |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-073 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **File > 20 MB**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **413**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-074 - MIME giả `.xlsx

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Import |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-074 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **MIME giả `.xlsx**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Quarantine/reject**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-075 - Header bắt buộc thiếu

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Import |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-075 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Header bắt buộc thiếu**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Mapping error trước import**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-076 - 5% dòng lỗi

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Import |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-076 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **5% dòng lỗi**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Import dòng hợp lệ, report lỗi**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-077 - >20% lỗi

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Import |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-077 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **>20% lỗi**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không commit theo policy**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-078 - Exact duplicate rows

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Import |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-078 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Exact duplicate rows**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Skip/update theo lựa chọn**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-079 - Dòng ngoài branch scope

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Import |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-079 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Dòng ngoài branch scope**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Reject từng dòng**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-080 - Retry confirm import

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Import |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-080 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Retry confirm import**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không tạo batch/customer trùng**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-081 - Ghi call outcome Callback

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Interaction/notes/files |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-081 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Ghi call outcome Callback**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Interaction + reminder**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-082 - Note 10.001 ký tự

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Interaction/notes/files |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-082 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Note 10.001 ký tự**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **422 MAX_LENGTH**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-083 - Offline note replay

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Interaction/notes/files |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-083 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Offline note replay**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Giữ occurredAt, một record**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-084 - Voice draft chưa confirm

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Interaction/notes/files |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-084 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Voice draft chưa confirm**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không lưu system of record**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-085 - Upload JPEG hợp lệ 2 MB

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Interaction/notes/files |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-085 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Upload JPEG hợp lệ 2 MB**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Scan clean, attachment available**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-086 - Executable đổi đuôi JPEG

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Interaction/notes/files |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-086 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Executable đổi đuôi JPEG**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Magic-byte reject/quarantine**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-087 - File chứa malware test signature

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Interaction/notes/files |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-087 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **File chứa malware test signature**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Quarantine, alert, không download**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-088 - Signed URL hết 5 phút

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Interaction/notes/files |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-088 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Signed URL hết 5 phút**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Access denied**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-089 - User ngoài resource tải file

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Interaction/notes/files |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-089 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **User ngoài resource tải file**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **403/404**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-090 - Delete attachment

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Interaction/notes/files |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-090 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Delete attachment**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Unlink/audit; blob lifecycle đúng**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-091 - Tạo visit tương lai hợp lệ

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Visit scheduling |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-091 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Tạo visit tương lai hợp lệ**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Scheduled + reminder**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-092 - End <= start

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Visit scheduling |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-092 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **End <= start**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **422 TIME_RANGE_INVALID**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-093 - Time nhập Asia/Ho_Chi_Minh

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Visit scheduling |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-093 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Time nhập Asia/Ho_Chi_Minh**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **UTC lưu đúng**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-094 - Assignee có lịch overlap

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Visit scheduling |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-094 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Assignee có lịch overlap**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Warning/chặn theo policy**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-095 - Manager override overlap

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Visit scheduling |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-095 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Manager override overlap**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Lưu reason**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-096 - Customer thiếu location

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Visit scheduling |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-096 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Customer thiếu location**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Yêu cầu pin/address trước submit**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-097 - Reschedule visit

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Visit scheduling |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-097 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Reschedule visit**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Version tăng, notify liên quan**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-098 - Cancel visit có reason

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Visit scheduling |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-098 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Cancel visit có reason**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Reminder cancel, history giữ**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-099 - Recurring visit DST timezone khác

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Visit scheduling |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-099 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Recurring visit DST timezone khác**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Occurrence đúng timezone user**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-100 - Sale xem visit người khác

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Visit scheduling |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-100 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Sale xem visit người khác**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Chỉ shared/in-scope**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-101 - 50 m, accuracy 20 m

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in/out |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-101 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **50 m, accuracy 20 m**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Valid**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-102 - 150 m với radius 100

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in/out |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-102 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **150 m với radius 100**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Review**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-103 - 250 m với radius 100

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in/out |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-103 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **250 m với radius 100**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Reject**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-104 - Accuracy 150 m trong geofence

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in/out |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-104 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Accuracy 150 m trong geofence**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Review**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-105 - Accuracy 250 m

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in/out |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-105 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Accuracy 250 m**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Reject**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-106 - Device time lệch 10 phút

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in/out |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-106 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Device time lệch 10 phút**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Review**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-107 - Mock location true

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in/out |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-107 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Mock location true**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Reject/security flag**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-108 - Offline valid check-in

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in/out |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-108 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Offline valid check-in**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Pending rồi sync, server tính distance**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-109 - Check-out thiếu required outcome

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in/out |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-109 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Check-out thiếu required outcome**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **422**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-110 - Checkout trước checkin

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in/out |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-110 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Checkout trước checkin**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Exception path/override, không silently complete**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-111 - Manager approve Review

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in review |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-111 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Manager approve Review**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Final Approved + audit**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-112 - Manager reject với reason

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in review |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-112 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Manager reject với reason**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Final Rejected + notify**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-113 - Reason <10 ký tự

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in review |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-113 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Reason <10 ký tự**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Validation fail**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-114 - User tự duyệt check-in

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in review |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-114 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **User tự duyệt check-in**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **403**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-115 - Manager khác branch duyệt

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in review |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-115 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Manager khác branch duyệt**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **403**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-116 - Hai reviewer cùng quyết định

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in review |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-116 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Hai reviewer cùng quyết định**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Một success, một conflict**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-117 - Request more info

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in review |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-117 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Request more info**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **State AwaitingEvidence**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-118 - Raw coordinate sau approve

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in review |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-118 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Raw coordinate sau approve**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không thay đổi**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-119 - Evidence quarantined

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in review |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-119 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Evidence quarantined**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không approve cho tới resolved**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-120 - Review queue 10.000 item

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Check-in review |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-120 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Review queue 10.000 item**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Cursor/page P95 đạt SLO**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-121 - Start sau consent

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Tracking session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-121 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Start sau consent**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Active session + indicator**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-122 - Start không consent

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Tracking session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-122 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Start không consent**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không thu/lưu điểm**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-123 - Start khi session active

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Tracking session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-123 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Start khi session active**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **409 SESSION_ACTIVE**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-124 - GPS permission denied

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Tracking session |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-124 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **GPS permission denied**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Reason path, không tracking ngầm**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-125 - Stop online

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Tracking session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-125 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Stop online**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Flush, close, aggregate job**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-126 - Stop offline

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Tracking session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-126 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Stop offline**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Local stop ngay, command queued**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-127 - Auto-stop end-of-day

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Tracking session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-127 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Auto-stop end-of-day**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Close reason System, notify**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-128 - App restart giữa ca

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Tracking session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-128 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **App restart giữa ca**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Khôi phục state/session**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-129 - Token refresh giữa tracking

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Tracking session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-129 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Token refresh giữa tracking**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Ingest tiếp, không mất sequence**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-130 - User locked giữa ca

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Tracking session |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-130 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **User locked giữa ca**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Stop/reject ingest sau grace**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-131 - Batch 100 điểm đúng sequence

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | GPS ingest/quality |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-131 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Batch 100 điểm đúng sequence**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **202`, accepted through 100**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-132 - Batch 101 điểm

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | GPS ingest/quality |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-132 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Batch 101 điểm**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **413/422**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-133 - Retry cùng key

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | GPS ingest/quality |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-133 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Retry cùng key**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Point count không tăng**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-134 - Sequence duplicate khác payload

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | GPS ingest/quality |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-134 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Sequence duplicate khác payload**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **409 IDEMPOTENCY_CONFLICT**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-135 - Latitude 91

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | GPS ingest/quality |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-135 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Latitude 91**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Reject điểm cụ thể**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-136 - Timestamp 1 giờ tương lai

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | GPS ingest/quality |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-136 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Timestamp 1 giờ tương lai**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Reject/flag**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-137 - Accuracy 300 m

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | GPS ingest/quality |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-137 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Accuracy 300 m**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Lưu quality Bad, loại KPI**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-138 - Impossible speed 300 km/h

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | GPS ingest/quality |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-138 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Impossible speed 300 km/h**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Flag anomaly, không tự kết tội**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-139 - Late points trong 15 phút grace

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | GPS ingest/quality |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-139 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Late points trong 15 phút grace**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Accept**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-140 - Late points sau grace

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | GPS ingest/quality |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-140 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Late points sau grace**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Reject với code**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-141 - Route có GPS gap 20 phút

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Route/history/map |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-141 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Route có GPS gap 20 phút**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Polyline ngắt tại gap**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-142 - Simplification zoom thấp

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Route/history/map |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-142 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Simplification zoom thấp**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Ít điểm, shape trong tolerance**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-143 - Distance aggregate known fixture

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Route/history/map |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-143 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Distance aggregate known fixture**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Sai số <= 2%**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-144 - Manager xem team route

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Route/history/map |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-144 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Manager xem team route**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Thành công + GPS access audit**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-145 - Manager xem ngoài team

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Route/history/map |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-145 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Manager xem ngoài team**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **403**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-146 - Raw points quá retention

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Route/history/map |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-146 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Raw points quá retention**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Chỉ summary/anonymized**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-147 - 2.000 customer markers

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Route/history/map |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-147 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **2.000 customer markers**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Cluster render <2 giây target**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-148 - Nearby radius 5 km

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Route/history/map |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-148 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Nearby radius 5 km**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Đúng thứ tự distance**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-149 - Map provider timeout

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Route/history/map |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-149 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Map provider timeout**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **List/address fallback**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-150 - Navigation URL special chars

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Route/history/map |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-150 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Navigation URL special chars**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **URL encode, đúng destination**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-151 - Reminder một lần

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Reminder |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-151 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Reminder một lần**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Job đúng due UTC**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-152 - Due quá khứ

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Reminder |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-152 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Due quá khứ**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Reject hoặc explicit immediate**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-153 - Weekly recurrence

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Reminder |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-153 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Weekly recurrence**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Tạo đúng occurrence kế**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-154 - Complete recurring

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Reminder |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-154 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Complete recurring**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không sửa history cũ**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-155 - Snooze 30 phút

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Reminder |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-155 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Snooze 30 phút**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **OriginalDue giữ nguyên**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-156 - Automation retry

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Reminder |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-156 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Automation retry**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Một reminder theo automation key**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-157 - Overdue 24 giờ

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Reminder |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-157 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Overdue 24 giờ**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Escalation manager một lần**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-158 - Private reminder overdue

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Reminder |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-158 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Private reminder overdue**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không escalate**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-159 - Assignee locked

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Reminder |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-159 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Assignee locked**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Reassignment/exception queue**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-160 - 100k reminder cùng phút

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Reminder |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-160 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **100k reminder cùng phút**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Job throughput, không duplicate**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-161 - In-app event mới

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Notification |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-161 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **In-app event mới**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Unread count +1 <=5 giây**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-162 - Duplicate event key

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Notification |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-162 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Duplicate event key**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Một notification**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-163 - Mark read

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Notification |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-163 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Mark read**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **readAt` set, count giảm**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-164 - Read-all before timestamp

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Notification |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-164 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Read-all before timestamp**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Chỉ item phù hợp**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-165 - Quiet hours push

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Notification |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-165 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Quiet hours push**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Delay đúng timezone**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-166 - Critical security event

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Notification |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-166 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Critical security event**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Bỏ quiet hours có nhãn**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-167 - Provider 500

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Notification |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-167 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Provider 500**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Retry exponential**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-168 - Retry max

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Notification |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-168 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Retry max**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Dead-letter + alert**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-169 - Deep link resource deleted

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Notification |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-169 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Deep link resource deleted**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Safe not-found screen**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-170 - SignalR reconnect

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Notification |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-170 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **SignalR reconnect**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không mất inbox; không gửi duplicate**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-171 - Contract draft valid

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Contract/handoff |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-171 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Contract draft valid**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Tạo metadata**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-172 - External reference trùng

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Contract/handoff |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-172 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **External reference trùng**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **409**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-173 - Value âm

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Contract/handoff |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-173 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Value âm**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **422**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-174 - Mark Signed thiếu document/reference

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Contract/handoff |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-174 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Mark Signed thiếu document/reference**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Chặn**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-175 - Handoff đủ checklist

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Contract/handoff |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-175 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Handoff đủ checklist**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Submit và auto-create/approval**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-176 - Handoff thiếu pin

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Contract/handoff |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-176 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Handoff thiếu pin**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **422 HANDOFF_INCOMPLETE**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-177 - Duplicate active handoff

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Contract/handoff |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-177 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Duplicate active handoff**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **409**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-178 - Four-eyes submitter tự approve

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Contract/handoff |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-178 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Four-eyes submitter tự approve**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **403**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-179 - Reject handoff

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Contract/handoff |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-179 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Reject handoff**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Sale notified, reason stored**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-180 - Contract canceled khi pending

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Contract/handoff |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-180 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Contract canceled khi pending**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Handoff invalidated**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-181 - Assign technician đúng skill

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Work order |
| Priority | P1/High |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-181 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Assign technician đúng skill**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Assigned + SLA**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-182 - Assign locked technician

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Work order |
| Priority | P1/High |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-182 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Assign locked technician**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Reject**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-183 - Technician accept own order

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Work order |
| Priority | P1/High |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-183 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Technician accept own order**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **InProgress/Accepted**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-184 - Technician access order khác

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Work order |
| Priority | P1/High |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-184 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Technician access order khác**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **403**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-185 - Complete đủ checklist/evidence

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Work order |
| Priority | P1/High |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-185 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Complete đủ checklist/evidence**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Completed**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-186 - Complete thiếu checkout

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Work order |
| Priority | P1/High |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-186 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Complete thiếu checkout**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **422**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-187 - Fail CustomerAbsent

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Work order |
| Priority | P1/High |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-187 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Fail CustomerAbsent**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Failure reason + revisit option**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-188 - Revisit tạo child

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Work order |
| Priority | P1/High |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-188 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Revisit tạo child**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Parent immutable, child linked**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-189 - Vượt max attempts

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Work order |
| Priority | P1/High |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-189 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Vượt max attempts**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Approval required**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-190 - SLA overdue

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Work order |
| Priority | P1/High |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-190 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **SLA overdue**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Alert/metric một lần**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-191 - Personal dashboard today

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Dashboard/export |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-191 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Personal dashboard today**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Chỉ KPI user**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-192 - Team funnel

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Dashboard/export |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-192 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Team funnel**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Tổng bằng drill-down cùng cutoff**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-193 - Previous-period compare

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Dashboard/export |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-193 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Previous-period compare**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Date boundaries đúng**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-194 - Read model trễ

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Dashboard/export |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-194 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Read model trễ**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Hiển thị freshness timestamp**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-195 - Một widget lỗi

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Dashboard/export |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-195 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Một widget lỗi**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Các widget khác vẫn hiển thị**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-196 - Export 4.000 rows

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Dashboard/export |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-196 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Export 4.000 rows**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Sync/stream theo policy**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-197 - Export 100.000 rows

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Dashboard/export |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-197 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Export 100.000 rows**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **202` async**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-198 - Export restricted field thiếu quyền

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Dashboard/export |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-198 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Export restricted field thiếu quyền**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Field loại/mask**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-199 - Download sau 24 giờ

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Dashboard/export |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-199 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Download sau 24 giờ**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Link expired**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-200 - Export/download

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Dashboard/export |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-200 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Export/download**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Hai audit events có purpose**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-201 - Customer update

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Audit/settings |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-201 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Customer update**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Audit before/after redacted**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-202 - Audit mutation qua API

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Audit/settings |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-202 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Audit mutation qua API**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không endpoint/`405**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-203 - Hash chain tamper fixture

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Audit/settings |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-203 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Hash chain tamper fixture**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Verification phát hiện**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-204 - Audit query ngoài scope

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Audit/settings |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-204 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Audit query ngoài scope**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không trả record**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-205 - GPS access

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Audit/settings |
| Priority | P0/Critical |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-205 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **GPS access**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Restricted access audit**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-206 - Update setting đúng schema

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Audit/settings |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-206 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Update setting đúng schema**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Version mới/effective**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-207 - Geofence radius 5 m

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Audit/settings |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-207 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Geofence radius 5 m**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Validation reject**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-208 - Generic setting chứa secret

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Audit/settings |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-208 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Generic setting chứa secret**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Reject; hướng dẫn vault**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-209 - Rollback setting

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Audit/settings |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-209 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Rollback setting**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Tạo version mới, không xóa cũ**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-210 - Concurrent setting publish

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Audit/settings |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Unit + API/integration; E2E khi có UI |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-210 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Concurrent setting publish**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Optimistic conflict**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-211 - Summary grounded fixture

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI summary/note |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-211 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Summary grounded fixture**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Facts có citation đúng**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-212 - Timeline mâu thuẫn

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI summary/note |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-212 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Timeline mâu thuẫn**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Nêu mâu thuẫn, không tự chọn**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-213 - Thiếu dữ liệu

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI summary/note |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-213 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Thiếu dữ liệu**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **missingInformation`, không bịa**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-214 - Note chứa prompt injection

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI summary/note |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-214 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Note chứa prompt injection**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Instruction bị coi là data**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-215 - Note yêu cầu system prompt

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI summary/note |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-215 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Note yêu cầu system prompt**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không tiết lộ**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-216 - Restricted event trong context

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI summary/note |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-216 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Restricted event trong context**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không retrieve/output**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-217 - Provider timeout

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI summary/note |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-217 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Provider timeout**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Fallback/cache, CRM không lỗi**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-218 - Output JSON sai schema

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI summary/note |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-218 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Output JSON sai schema**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Reject/retry bounded**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-219 - User feedback negative

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI summary/note |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-219 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **User feedback negative**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **AiFeedback lưu**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-220 - Cùng input hash/model

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI summary/note |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-220 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Cùng input hash/model**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Cache theo policy**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-221 - Eligible customer

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI score/suggestion/route |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-221 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Eligible customer**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Score/band/factors/version**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-222 - Insufficient features

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI score/suggestion/route |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-222 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Insufficient features**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không tạo điểm giả**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-223 - Protected attribute injected

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI score/suggestion/route |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-223 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Protected attribute injected**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Feature pipeline loại**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-224 - Model drift vượt threshold

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI score/suggestion/route |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-224 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Model drift vượt threshold**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Kill switch/fallback**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-225 - Next action accepted

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI score/suggestion/route |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-225 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Next action accepted**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Chỉ tạo draft/reminder sau confirm**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-226 - Suggestion rejected

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI score/suggestion/route |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-226 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Suggestion rejected**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không thay dữ liệu, feedback**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-227 - Suggestion gửi khách tự động

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI score/suggestion/route |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-227 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Suggestion gửi khách tự động**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Bị policy chặn**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-228 - Route 10 visits khả thi

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI score/suggestion/route |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-228 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Route 10 visits khả thi**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Tôn trọng time windows**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-229 - Constraint bất khả thi

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI score/suggestion/route |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-229 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Constraint bất khả thi**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Nêu violations, không giả route**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-230 - Maps route API down

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | AI score/suggestion/route |
| Priority | P1/High |
| Automation | AI evaluation + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-230 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Maps route API down**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Deterministic heuristic fallback**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-231 - Tạo lead offline, restart app

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Offline/sync |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | E2E offline + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-231 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Client local store mã hóa được reset; network có thể chuyển online/offline và server có fixture conflict.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Tạo lead offline, restart app**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Draft/outbox còn nguyên**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-232 - Sync lead khi online

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Offline/sync |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | E2E offline + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-232 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Client local store mã hóa được reset; network có thể chuyển online/offline và server có fixture conflict.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Sync lead khi online**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Mapping local-server ID**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-233 - Note phụ thuộc lead local

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Offline/sync |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | E2E offline + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-233 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Client local store mã hóa được reset; network có thể chuyển online/offline và server có fixture conflict.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Note phụ thuộc lead local**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Gửi đúng thứ tự**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-234 - Partial batch failure

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Offline/sync |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | E2E offline + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-234 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Client local store mã hóa được reset; network có thể chuyển online/offline và server có fixture conflict.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Partial batch failure**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Item độc lập vẫn sync**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-235 - Auth hết hạn

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Offline/sync |
| Priority | P0/Critical |
| Automation | E2E offline + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-235 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Client local store mã hóa được reset; network có thể chuyển online/offline và server có fixture conflict.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Auth hết hạn**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Refresh rồi tiếp tục**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-236 - Account revoked offline

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Offline/sync |
| Priority | P0/Critical |
| Automation | E2E offline + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-236 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Client local store mã hóa được reset; network có thể chuyển online/offline và server có fixture conflict.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Account revoked offline**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Dừng sync, bảo vệ local data**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-237 - Server record đổi cùng field

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Offline/sync |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | E2E offline + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-237 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Client local store mã hóa được reset; network có thể chuyển online/offline và server có fixture conflict.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Server record đổi cùng field**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **NeedsReview conflict**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-238 - Server/local đổi field khác

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Offline/sync |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | E2E offline + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-238 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Client local store mã hóa được reset; network có thể chuyển online/offline và server có fixture conflict.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Server/local đổi field khác**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Merge an toàn theo rule**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-239 - Retry sau timeout không rõ kết quả

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Offline/sync |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | E2E offline + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-239 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Client local store mã hóa được reset; network có thể chuyển online/offline và server có fixture conflict.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Retry sau timeout không rõ kết quả**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Idempotency tránh trùng**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-240 - Local DB schema cũ

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Offline/sync |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | E2E offline + integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-240 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Client local store mã hóa được reset; network có thể chuyển online/offline và server có fixture conflict.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Local DB schema cũ**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Migration hoặc safe block**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-241 - IDOR customer UUID

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Security |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-241 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **IDOR customer UUID**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Scope chặn**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-242 - Horizontal privilege check-in

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Security |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-242 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Horizontal privilege check-in**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Scope chặn**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-243 - Stored XSS trong note

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Security |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-243 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Stored XSS trong note**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Encode khi render**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-244 - SQL injection search

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Security |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-244 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **SQL injection search**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Parameterized, không delay/leak**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-245 - CSRF khi cookie mode

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Security |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-245 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **CSRF khi cookie mode**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Token/origin chặn**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-246 - CORS unknown origin

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Security |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-246 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **CORS unknown origin**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không ACAO**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-247 - Path traversal filename

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Security |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-247 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Path traversal filename**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Storage key random**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-248 - SSRF import URL nội bộ

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Security |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-248 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **SSRF import URL nội bộ**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Block private/metadata IP**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-249 - Secret scan repository/image

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Security |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-249 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Secret scan repository/image**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Build fail**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-250 - Dependency Critical CVE

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Security |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-250 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Dependency Critical CVE**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Release gate fail/approved exception**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-251 - Customer list 300 RPS

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Performance |
| Priority | P1/High |
| Automation | k6 performance |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-251 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dataset production-like, warm-up tách khỏi measurement, threshold/SLO cấu hình trong test.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Customer list 300 RPS**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **P95 <500 ms, error <1%**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-252 - Customer write 100 RPS

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Performance |
| Priority | P1/High |
| Automation | k6 performance |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-252 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dataset production-like, warm-up tách khỏi measurement, threshold/SLO cấu hình trong test.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Customer write 100 RPS**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **P95 <800 ms**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-253 - Nearby 2.000 candidates

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Performance |
| Priority | P1/High |
| Automation | k6 performance |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-253 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dataset production-like, warm-up tách khỏi measurement, threshold/SLO cấu hình trong test.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Nearby 2.000 candidates**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **P95 <500 ms**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-254 - GPS 10M points/day profile

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Performance |
| Priority | P0/Critical |
| Automation | k6 performance |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-254 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dataset production-like, warm-up tách khỏi measurement, threshold/SLO cấu hình trong test.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **GPS 10M points/day profile**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không backlog vượt 5 phút**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-255 - 5.000 SignalR concurrent

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Performance |
| Priority | P1/High |
| Automation | k6 performance |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-255 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dataset production-like, warm-up tách khỏi measurement, threshold/SLO cấu hình trong test.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **5.000 SignalR concurrent**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Stable reconnect/broadcast**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-256 - Dashboard 200 concurrent

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Performance |
| Priority | P1/High |
| Automation | k6 performance |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-256 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dataset production-like, warm-up tách khỏi measurement, threshold/SLO cấu hình trong test.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Dashboard 200 concurrent**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **P95 <800 ms từ read model**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-257 - Export load song song

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Performance |
| Priority | P1/High |
| Automation | k6 performance |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-257 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dataset production-like, warm-up tách khỏi measurement, threshold/SLO cấu hình trong test.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Export load song song**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **API interactive không suy giảm >20%**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-258 - 4-hour soak

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Performance |
| Priority | P1/High |
| Automation | k6 performance |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-258 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dataset production-like, warm-up tách khỏi measurement, threshold/SLO cấu hình trong test.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **4-hour soak**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không memory/connection leak**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-259 - Stress tới saturation

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Performance |
| Priority | P1/High |
| Automation | k6 performance |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-259 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dataset production-like, warm-up tách khỏi measurement, threshold/SLO cấu hình trong test.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Stress tới saturation**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Graceful 429, phục hồi**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-260 - DB index regression dataset

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Performance |
| Priority | P1/High |
| Automation | k6 performance |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-260 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Dataset production-like, warm-up tách khỏi measurement, threshold/SLO cấu hình trong test.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **DB index regression dataset**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Query plan không scan bảng lớn bất ngờ**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-261 - Google Maps timeout

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Resilience/recovery |
| Priority | P1/High |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-261 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Google Maps timeout**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Circuit breaker/list fallback**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-262 - Notification provider down 30m

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Resilience/recovery |
| Priority | P1/High |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-262 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Notification provider down 30m**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Queue giữ/retry**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-263 - AI provider down

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Resilience/recovery |
| Priority | P1/High |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-263 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **AI provider down**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Core CRM bình thường**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-264 - Redis unavailable

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Resilience/recovery |
| Priority | P1/High |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-264 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Redis unavailable**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **DB fallback có rate protection**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-265 - One API instance killed

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Resilience/recovery |
| Priority | P1/High |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-265 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **One API instance killed**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **LB chuyển, no session loss**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-266 - Worker crash giữa job

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Resilience/recovery |
| Priority | P1/High |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-266 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Worker crash giữa job**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Lock expiry/retry idempotent**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-267 - SQL transient disconnect

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Resilience/recovery |
| Priority | P1/High |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-267 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **SQL transient disconnect**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Retry safe, không double commit**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-268 - Restore latest backup

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Resilience/recovery |
| Priority | P0/Critical |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-268 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Restore latest backup**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **RPO <=15m, integrity pass**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-269 - Full region/site fail drill

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Resilience/recovery |
| Priority | P1/High |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-269 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Full region/site fail drill**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **RTO <=2h**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-270 - Corrupt backup sample

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Resilience/recovery |
| Priority | P0/Critical |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-270 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Corrupt backup sample**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Phát hiện và dùng restore point khác**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-271 - Keyboard login/customer flow

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Frontend/accessibility |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Playwright + accessibility |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-271 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Keyboard login/customer flow**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không focus trap/lost focus**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-272 - Screen reader form error

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Frontend/accessibility |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Playwright + accessibility |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-272 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Screen reader form error**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Label/error được đọc**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-273 - Contrast light/dark

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Frontend/accessibility |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Playwright + accessibility |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-273 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Contrast light/dark**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **WCAG AA**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-274 - Zoom 200%

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Frontend/accessibility |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Playwright + accessibility |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-274 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Zoom 200%**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không mất action/content**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-275 - Viewport 360 px

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Frontend/accessibility |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Playwright + accessibility |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-275 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Viewport 360 px**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không horizontal scroll core flow**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-276 - Reduced motion

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Frontend/accessibility |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Playwright + accessibility |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-276 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Reduced motion**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Animation không thiết yếu tắt**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-277 - Map unavailable keyboard

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Frontend/accessibility |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Playwright + accessibility |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-277 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Map unavailable keyboard**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **List alternative đủ chức năng**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-278 - Vietnamese text dài

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Frontend/accessibility |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Playwright + accessibility |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-278 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Vietnamese text dài**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không truncate action quan trọng**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-279 - Slow 3G loading

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Frontend/accessibility |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Playwright + accessibility |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-279 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Slow 3G loading**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Skeleton, không duplicate submit**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-280 - Browser back dirty form

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Frontend/accessibility |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Playwright + accessibility |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-280 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Browser back dirty form**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Confirm/restore draft**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-281 - Tracking trước consent

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Privacy/retention |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-281 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Tracking trước consent**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Zero point stored**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-282 - User stop consent/session

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Privacy/retention |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-282 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **User stop consent/session**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Thu thập dừng ngay**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-283 - Route raw quá 90 ngày

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Privacy/retention |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-283 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Route raw quá 90 ngày**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Deleted/anonymized theo policy**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-284 - Aggregate sau raw delete

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Privacy/retention |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-284 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Aggregate sau raw delete**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không tái nhận diện cá nhân ngoài scope**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-285 - Data access request

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Privacy/retention |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-285 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Data access request**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Export đủ resource được phép**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-286 - Anonymize có dual approval

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Privacy/retention |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-286 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Anonymize có dual approval**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **PII thay thế, evidence giữ**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-287 - Legal hold active

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Privacy/retention |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-287 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Legal hold active**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Delete bị chặn có reason**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-288 - PII trong application log

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Privacy/retention |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-288 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **PII trong application log**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Scanner không tìm phone/token**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-289 - AI provider payload

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Privacy/retention |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-289 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **AI provider payload**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Không chứa raw GPS/full phone**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-290 - User xem privacy notice

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Privacy/retention |
| Priority | P0/Critical |
| Automation | Security/API/Integration |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-290 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **User xem privacy notice**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Version/acceptedAt lưu**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-291 - /health/live` không dependency ngoài

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Deployment/observability |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-291 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **/health/live` không dependency ngoài**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **200` khi process healthy**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-292 - DB down `/health/ready

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Deployment/observability |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-292 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **DB down `/health/ready**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **503`, instance khỏi LB**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-293 - Request API

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Deployment/observability |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-293 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Request API**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Log/trace/metric cùng traceId**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-294 - HTTP 5xx >2% 5m

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Deployment/observability |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-294 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **HTTP 5xx >2% 5m**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Alert đúng route/on-call**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-295 - GPS lag >5m

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Deployment/observability |
| Priority | P0/Critical |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-295 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **GPS lag >5m**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Alert và dashboard**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-296 - Deploy migration compatible

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Deployment/observability |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-296 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Deploy migration compatible**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Old/new app cùng chạy rolling**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-297 - Container chạy non-root

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Deployment/observability |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-297 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Container chạy non-root**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Pass policy**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-298 - TLS/headers scan

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Deployment/observability |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-298 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **TLS/headers scan**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **TLS1.2+, HSTS/CSP đúng**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-299 - Rollback app

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Deployment/observability |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-299 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Rollback app**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Schema vẫn tương thích**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

## TC-300 - Production smoke sau deploy

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | Deployment/observability |
| Priority | P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt. |
| Automation | Resilience/operations |
| Requirement oracle | Chương 3, use case/API liên quan và expected của TC-300 |

### Preconditions

Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic. Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết.

### Steps

1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **Production smoke sau deploy**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.

### Expected assertions

- Oracle chính: **Login, customer read, check-in sandbox, health pass**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.

