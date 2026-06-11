# 03. Phân tích yêu cầu

## 3.1 Actor và phạm vi quyền

| Actor | Quyền điển hình | Scope mặc định |
|---|---|---|
| Sale | CRUD lead được giao, visit, check-in, reminder | Self + assigned territory |
| Kỹ thuật | Xem handoff, lịch, cập nhật triển khai | Assigned work orders |
| Manager | Phân công, phê duyệt, dashboard | Team/department |
| Admin | User, role, settings, master data | Tenant/branch được ủy quyền |
| Auditor | Read audit/report | Scope được phê duyệt |
| Integration | Đồng bộ có service account | API contract-specific |

## 3.2 Functional requirements

| ID | Yêu cầu | Priority | Acceptance tóm lược |
|---|---|---|---|
| FR-001 | Đăng nhập, refresh, logout và revoke session | Must | Token rotation; logout vô hiệu refresh token |
| FR-002 | MFA cho Admin/Manager | Must | TOTP recovery có audit |
| FR-003 | Quản lý user/role/permission/scope | Must | Deny by default |
| FR-004 | Tạo/sửa/import lead | Must | Validate và chống trùng |
| FR-005 | Customer 360 timeline | Must | Hiển thị thay đổi theo thời gian |
| FR-006 | Pipeline và trạng thái có rule | Must | Chuyển sai bị chặn |
| FR-007 | Geocode, pin và nearby search | Must | Khoảng cách đúng theo WGS84 |
| FR-008 | Phân công theo người/đội/địa bàn | Must | Có history và notification |
| FR-009 | Visit plan và notes | Must | Offline save và sync |
| FR-010 | Check-in/out có geofence | Must | Accuracy và anti-spoof signal |
| FR-011 | Route tracking theo ca | Must | Start/stop rõ; adaptive sampling |
| FR-012 | Reminder recurring/escalation | Must | Không gửi trùng |
| FR-013 | Notification preferences | Should | In-app bắt buộc với cảnh báo hệ thống |
| FR-014 | Contract metadata và handoff | Must | Checklist hoàn chỉnh trước submit |
| FR-015 | Technical work order | Must | Assignment, SLA, result |
| FR-016 | File/attachment | Must | Scan malware, signed URL |
| FR-017 | Dashboard/filter/drill-down | Must | Scope chính xác |
| FR-018 | Export có audit | Should | Watermark, giới hạn dòng |
| FR-019 | Audit search | Must | Không sửa/xóa qua UI |
| FR-020 | AI summary/suggestion/score | Could | Explainable + human review |
| FR-021 | Settings/feature flags | Should | Typed, versioned |
| FR-022 | Offline outbox/conflict | Must | Idempotent replay |
| FR-023 | SignalR updates | Should | Reconnect và authorization |
| FR-024 | Data retention/anonymization | Must | Job có báo cáo |
| FR-025 | Bulk update có chọn lọc (status, owner, tag) trên tập bản ghi đã lọc | Should | Giới hạn số lượng, preview, chạy background, partial result và audit theo dòng |
| FR-026 | Restore bản ghi đã soft-delete trong restore window | Should | Permission `*.restore`; khôi phục quan hệ; ghi audit before/after |
| FR-027 | Device self-management (xem, đặt tên, thu hồi thiết bị, đăng xuất từ xa) | Should | Hiển thị thiết bị đang hoạt động; revoke vô hiệu session/push tức thì |
| FR-028 | Version history và diff cho customer/contract/work order | Should | Xem thay đổi theo phiên bản, ai đổi, khi nào; khôi phục giá trị cần permission |

## 3.3 Business rules

| ID | Rule |
|---|---|
| BR-001 | Số điện thoại được chuẩn hóa về E.164; hash dùng dò trùng, bản rõ mã hóa ở mức ứng dụng hoặc TDE + field protection. |
| BR-002 | Lead trùng khi cùng normalized phone, hoặc similarity tên + địa chỉ vượt ngưỡng; merge cần permission. |
| BR-003 | Một lead chỉ có một owner chính tại một thời điểm; mọi lần đổi owner có history. |
| BR-004 | Trạng thái hợp lệ: `New -> Contacted -> Qualified -> VisitPlanned -> Proposal -> Contracted`; có nhánh `Nurturing/Lost/Duplicate`. |
| BR-005 | `Lost` bắt buộc reason; mở lại cần ghi lý do. |
| BR-006 | Check-in hợp lệ khi distance <= geofence radius, accuracy <= 100 m và thời gian server lệch thiết bị <= 5 phút; ngoại lệ cần manager review. |
| BR-007 | Tracking chỉ hoạt động trong ca được bắt đầu rõ ràng và người dùng thấy chỉ báo đang theo dõi. |
| BR-008 | Route point có accuracy > 200 m không dùng tính KPI nhưng vẫn giữ để chẩn đoán theo retention ngắn. |
| BR-009 | Handoff yêu cầu địa chỉ, pin, contact, gói dịch vụ, thời gian mong muốn và ghi chú hạ tầng. |
| BR-010 | Reminder quá hạn 24 giờ escalates cho manager, trừ reminder cá nhân được đánh dấu private. |
| BR-011 | File ảnh tối đa 10 MB, tài liệu 25 MB; loại file theo allowlist. |
| BR-012 | AI output không được tự gửi khách hoặc thay đổi system of record. |
| BR-013 | Export > 5.000 dòng chạy background; link hết hạn sau 24 giờ. |
| BR-014 | Người dùng không xem GPS đồng nghiệp nếu không có `tracking.view.team`. |
| BR-015 | Xóa khách hàng thực hiện soft-delete; yêu cầu data subject được xử lý theo workflow phê duyệt. |
| BR-016 | Bulk update tối đa 500 bản ghi/lệnh (mặc định), chạy preview trước khi áp dụng; mỗi dòng thay đổi sinh một audit entry và tuân thủ scope/permission của từng bản ghi; bản ghi vi phạm rule bị bỏ qua và liệt kê trong báo cáo. |
| BR-017 | Restore chỉ áp dụng trong restore window (mặc định 30 ngày từ `DeletedAtUtc`), cần permission `*.restore`; nếu định danh duy nhất (vd. `PhoneHash`) đã bị bản ghi khác chiếm thì restore bị chặn và yêu cầu xử lý trùng. |
| BR-018 | Mỗi user tối đa 10 thiết bị hoạt động; thu hồi thiết bị revoke toàn bộ session-family và push token của thiết bị đó; thiết bị có `RiskStatus` cao bị buộc reauth/MFA. |

## 3.4 Non-functional requirements

| ID | Nhóm | Chỉ tiêu |
|---|---|---|
| NFR-001 | Availability | 99.9%/tháng, loại trừ bảo trì báo trước |
| NFR-002 | API latency | P95 read < 500 ms; write < 800 ms ở 300 RPS |
| NFR-003 | Map | 95% marker viewport hiển thị < 2 giây với <= 2.000 điểm cluster |
| NFR-004 | Scale | 20.000 user, 5.000 concurrent, 10 triệu route points/ngày |
| NFR-005 | RPO/RTO | RPO <= 15 phút, RTO <= 2 giờ |
| NFR-006 | Security | OWASP ASVS L2; Admin functions tiệm cận L3 |
| NFR-007 | Accessibility | WCAG 2.2 AA cho luồng cốt lõi |
| NFR-008 | Compatibility | Hai phiên bản mới nhất Chrome/Edge/Safari; Android Chrome |
| NFR-009 | Observability | 100% request có correlation ID; golden signals |
| NFR-010 | Localization | Không hard-code text/timezone/currency |
| NFR-011 | Maintainability | Domain coverage >= 80%; cyclomatic complexity cảnh báo > 15 |
| NFR-012 | Privacy | GPS raw retention mặc định 90 ngày; aggregate 24 tháng |

## 3.5 Security requirements

- OIDC-ready; JWT access token 10 phút, refresh token tối đa 30 ngày và rotation mỗi lần dùng.
- Refresh token chỉ lưu hash; reuse detection revoke token family.
- Password tối thiểu 12 ký tự nếu dùng local identity; kiểm tra breached-password list.
- TLS 1.2+; HSTS; secrets trong vault/environment protected, không commit.
- RBAC + permission + resource scope; kiểm tra ở Application layer và query filter.
- CSRF áp dụng nếu token qua cookie; nếu Authorization header thì CORS allowlist nghiêm ngặt.
- Rate limit theo IP/user/client; login dùng progressive delay và lock có thời hạn.
- File upload kiểm tra MIME, magic bytes, tên ngẫu nhiên, antivirus, không execute.
- PII masked trong log; không log token, password, phone đầy đủ, nội dung AI nhạy cảm.
- Audit: login, export, permission, customer sensitive update, GPS access, AI action.
- Threat model STRIDE và penetration test trước production.

## 3.6 Performance và scalability

| Khu vực | Chiến lược |
|---|---|
| Customer search | Full-text/normalized columns, seek pagination |
| Nearby | SQL Server `geography` + spatial index |
| Route ingest | Batch 20-100 points, partition theo ngày/tháng, async processing |
| Dashboard | Read model/materialized aggregate, cache 1-5 phút |
| SignalR | Redis backplane khi scale-out |
| File | Direct-to-object-storage signed upload |
| AI | Queue, timeout, circuit breaker, token budget |
| Export | Background job + streaming |

## 3.7 Availability, backup và recovery

- Production tối thiểu hai application instances sau load balancer.
- SQL Server HA theo hạ tầng: Always On hoặc managed equivalent.
- Full backup hàng ngày, differential mỗi 6 giờ, log backup mỗi 15 phút.
- Backup mã hóa, immutable/offsite; restore test hàng quý.
- Runbook failover, corruption, credential leak, Maps outage và AI provider outage.
- Chế độ degraded: CRM core vẫn hoạt động khi AI/Maps/notification provider lỗi.

## 3.8 Logging và monitoring

| Signal | Metric/Log | Alert |
|---|---|---|
| Traffic | requests/sec, active users | Bất thường 3 sigma |
| Errors | HTTP 5xx, domain failure | > 2% trong 5 phút |
| Latency | P50/P95/P99 | P95 > SLO 10 phút |
| Saturation | CPU, memory, DB pool, queue | > 80% có xu hướng |
| GPS | ingest lag, invalid accuracy | Lag > 5 phút |
| Jobs | success/failure/retry/dead-letter | 3 lần lỗi |
| Security | failed login, denied access, export | Spike hoặc policy match |

Log JSON gồm `timestamp`, `level`, `traceId`, `userId` dạng pseudonym, `tenantId`, `endpoint`, `status`, `durationMs`; retention log ứng dụng 30-90 ngày theo môi trường.

## 3.9 Data classification

| Class | Ví dụ | Kiểm soát |
|---|---|---|
| Public | Nội dung hướng dẫn công khai | Integrity |
| Internal | Cấu hình không nhạy cảm | Authenticated access |
| Confidential | Lead, phone, contract | Encryption, scoped access |
| Restricted | GPS, token, audit security | Least privilege, audit, retention ngắn |

## 3.10 Acceptance cấp hệ thống

- [ ] 100% FR Must truy vết tới UC, API và test.
- [ ] Không có lỗ hổng Critical/High chưa xử lý.
- [ ] Load test đạt NFR-002/NFR-004 với dữ liệu tương đương production.
- [ ] Restore drill đạt RPO/RTO.
- [ ] Privacy review xác nhận consent, purpose và retention GPS.
- [ ] Pilot KPI đạt ngưỡng trong `02_Khao_Sat.md`.

