# Phụ lục B. Master Data và Glossary

## B.1 Customer status

| Code | Tên hiển thị | Terminal | Required fields khi vào | Allowed next |
|---|---|---:|---|---|
| `New` | Mới | Không | source, contact | Contacted, Duplicate |
| `Contacted` | Đã liên hệ | Không | interaction outcome | Qualified, Nurturing, Lost |
| `Qualified` | Đủ điều kiện | Không | need, serviceability sơ bộ | VisitPlanned, Lost |
| `Nurturing` | Nuôi dưỡng | Không | next follow-up | Contacted, Lost |
| `VisitPlanned` | Đã lên lịch gặp | Không | active visit | Proposal, Qualified, Lost |
| `Proposal` | Đã đề xuất | Không | package/proposal date | Contracted, Nurturing, Lost |
| `Contracted` | Đã ký | Không | signed contract reference | InstallationPending |
| `InstallationPending` | Chờ triển khai | Không | approved handoff | Active, InstallationFailed |
| `InstallationFailed` | Triển khai lỗi | Không | failure reason | InstallationPending, Lost |
| `Active` | Đang sử dụng | Có | completed work order | - |
| `Lost` | Không chuyển đổi | Có điều kiện | loss reason | Contacted qua reopen |
| `Duplicate` | Trùng | Có | survivor ID | - |

## B.2 Visit/work order/check-in status

| Entity | Codes |
|---|---|
| Visit | `Draft`, `Scheduled`, `InProgress`, `Completed`, `Canceled`, `Exception` |
| Check-in | `Valid`, `Review`, `AwaitingEvidence`, `Approved`, `Rejected` |
| Route session | `Active`, `Stopping`, `Stopped`, `AutoStopped`, `Invalidated` |
| Handoff | `Draft`, `Submitted`, `PendingApproval`, `Approved`, `Rejected`, `Canceled` |
| Work order | `Open`, `Assigned`, `Accepted`, `Traveling`, `InProgress`, `Paused`, `Completed`, `Failed`, `RevisitRequired`, `Canceled` |
| Reminder | `Scheduled`, `Snoozed`, `Completed`, `Canceled`, `Overdue` |

## B.3 Standard reason codes

| Domain | Code | Dùng khi |
|---|---|---|
| Customer loss | `NO_NEED`, `PRICE`, `COMPETITOR`, `UNREACHABLE`, `OUT_OF_COVERAGE`, `OTHER` | Chuyển Lost |
| Check-in review | `OUTSIDE_GEOFENCE`, `LOW_ACCURACY`, `TIME_SKEW`, `MOCK_SIGNAL`, `MISSING_EVIDENCE` | Validation/review |
| Work failure | `CUSTOMER_ABSENT`, `INFRASTRUCTURE`, `EQUIPMENT`, `SAFETY`, `CUSTOMER_RESCHEDULE`, `OTHER` | Failed/revisit |
| Assignment | `WORKLOAD_BALANCE`, `TERRITORY`, `SKILL`, `LEAVE`, `ESCALATION`, `MANUAL_OVERRIDE` | Đổi owner/assignee |
| Reminder | `ACTION_COMPLETED`, `NO_LONGER_REQUIRED`, `DUPLICATE`, `RESCHEDULED` | Complete/cancel |

`OTHER` luôn bắt buộc `reasonText` 10-500 ký tự.

## B.4 Permission catalog

| Namespace | Permissions |
|---|---|
| IAM | `users.read`, `users.manage`, `roles.read`, `roles.manage`, `roles.assign`, `sessions.revoke` |
| Organization | `org.read`, `org.manage`, `territories.read`, `territories.manage` |
| Customer | `customers.read`, `customers.create`, `customers.update`, `customers.delete`, `customers.assign`, `customers.merge`, `customers.import` |
| Field | `visits.read`, `visits.create`, `visits.update`, `checkins.create`, `checkins.review`, `tracking.create`, `tracking.read.self`, `tracking.read.team` |
| Operations | `contracts.create`, `contracts.update`, `handoffs.create`, `handoffs.approve`, `workorders.read`, `workorders.assign`, `workorders.execute` |
| Engagement | `reminders.create`, `reminders.manage.team`, `notifications.manage` |
| Governance | `analytics.read.team`, `exports.create`, `audit.read`, `settings.read`, `settings.manage` |
| AI | `ai.summary`, `ai.suggest`, `ai.score`, `ai.route` |

## B.5 Notification events

| Event code | Recipient | Severity | Default channels |
|---|---|---|---|
| `CUSTOMER_ASSIGNED` | New owner | Info | In-app, push |
| `REMINDER_DUE` | Assignee | Info | In-app, push |
| `REMINDER_OVERDUE` | Assignee/manager | Warning | In-app, push |
| `VISIT_RESCHEDULED` | Assignee | Info | In-app |
| `CHECKIN_REVIEW_REQUIRED` | Manager | Warning | In-app |
| `CHECKIN_DECIDED` | Employee | Info | In-app |
| `HANDOFF_SUBMITTED` | Approver/tech queue | Info | In-app |
| `HANDOFF_REJECTED` | Sale | Warning | In-app, push |
| `WORKORDER_ASSIGNED` | Technician | Info | In-app, push |
| `WORKORDER_SLA_RISK` | Technician/manager | Warning | In-app, push |
| `SECURITY_SESSION_REVOKED` | User | Critical | In-app, email |
| `EXPORT_READY` | Requester | Info | In-app |

## B.6 Error code catalog

| HTTP | Code | Meaning |
|---:|---|---|
| 400 | `REQUEST_MALFORMED` | JSON/query không parse |
| 401 | `AUTHENTICATION_REQUIRED`, `TOKEN_INVALID`, `MFA_INVALID` | Chưa xác thực |
| 403 | `PERMISSION_DENIED`, `SCOPE_DENIED`, `ACCOUNT_LOCKED` | Không được phép |
| 404 | `RESOURCE_NOT_FOUND` | Không tồn tại hoặc anti-enumeration |
| 409 | `RESOURCE_CONFLICT`, `CUSTOMER_DUPLICATE`, `SESSION_ACTIVE`, `IDEMPOTENCY_CONFLICT` | Xung đột trạng thái |
| 412 | `VERSION_MISMATCH` | ETag/rowversion cũ |
| 413 | `PAYLOAD_TOO_LARGE` | File/batch vượt giới hạn |
| 422 | `VALIDATION_FAILED`, `TRANSITION_INVALID`, `HANDOFF_INCOMPLETE` | Dữ liệu hợp lệ cú pháp nhưng sai rule |
| 429 | `RATE_LIMITED` | Vượt quota |
| 503 | `DEPENDENCY_UNAVAILABLE`, `SERVICE_DEGRADED` | Tạm lỗi |

## B.7 Glossary

| Thuật ngữ | Định nghĩa |
|---|---|
| Lead | Khách hàng tiềm năng chưa hoàn tất vòng đời bán |
| Customer 360 | Góc nhìn tổng hợp hồ sơ, timeline và hoạt động |
| Owner | Người chịu trách nhiệm chính với lead/customer |
| Territory | Vùng địa lý versioned dùng phân công và scope |
| Visit | Lịch gặp/khảo sát gắn customer |
| Check-in | Bằng chứng có mặt tại thời điểm/địa điểm, có quality state |
| Route session | Phiên thu GPS có consent trong ca |
| Handoff | Gói dữ liệu bàn giao từ Sale sang Kỹ thuật |
| Work order | Đơn việc triển khai kỹ thuật |
| Outbox | Bản ghi sự kiện/command chờ xử lý bền vững |
| Idempotency | Retry cùng ý định không tạo side effect trùng |
| ETag/rowversion | Phiên bản chống ghi đè đồng thời |
| RPO | Mức mất dữ liệu tối đa chấp nhận |
| RTO | Thời gian phục hồi tối đa |
| Restricted data | Dữ liệu GPS/token/security cần kiểm soát cao nhất |

