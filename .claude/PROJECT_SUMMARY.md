# PROJECT_SUMMARY.md — FPT Connect

## Tầm nhìn (Vision)

Trở thành nền tảng vận hành hiện trường thống nhất cho doanh nghiệp: nơi đội sale và kỹ thuật quản lý khách hàng theo vị trí, di chuyển có chủ đích, check-in xác thực, và bàn giao – triển khai khép kín, với dữ liệu sạch và trợ lý AI hỗ trợ quyết định. Mục tiêu là "một nguồn sự thật" cho toàn bộ vòng đời khách hàng từ lead đến lắp đặt và chăm sóc.

## Module chính (Bounded contexts)

| Context | Phạm vi | Aggregate chính |
|---|---|---|
| Identity & Access (iam) | Đăng nhập, MFA, RBAC + scope, thiết bị/phiên, tổ chức/địa bàn | User, Role, Session, Device, Territory |
| CRM (crm) | Lead/customer, pipeline, dedupe/merge, interaction, contract | Customer, Interaction, Contract |
| Field Operations (field) | Visit, check-in/out geofence, GPS tracking, tuyến đường | Visit, CheckIn, RouteSession |
| Technical Operations (ops) | Handoff, work order, revisit, SLA | Handoff, WorkOrder |
| Engagement (notify) | Reminder recurring/escalation, notification đa kênh | Reminder, Notification |
| Analytics & AI (ai) | Dashboard read-model, summary/score/next-action, route optimize | ReadModel, AiRun |
| Governance (audit) | Audit bất biến, settings, bulk/restore/version, retention/privacy | AuditLog, Setting |

## Mục tiêu (Goals)

- Trải nghiệm hiện trường mượt khi mạng yếu (offline-first, sync idempotent).
- Dữ liệu khách hàng sạch: chống trùng, merge có kiểm soát, lịch sử bất biến.
- Tuân thủ riêng tư GPS: consent, retention, anonymize.
- Chất lượng enterprise: bảo mật ASVS L2, hiệu năng SLO, khả năng bảo trì dài hạn.

## Roadmap (tóm tắt — chi tiết `docs/Project-Bible/SPRINT_PLAN.md`)

S0 Foundation → S1–S2 IAM → S3–S4 CRM → S5–S8 Field & GPS → S9 Engagement → S10–S11 Tech ops → S12 Files & Offline → S13 Analytics → S14 Governance → S15 AI → S16 Privacy/Ops → S17–S18 Hardening & Go-live. Sprint 1 tuần, đội 10 kỹ sư.

## Tóm tắt kiến trúc

Clean Architecture (.NET 10): `Domain ← Application ← Infrastructure/Api/Worker`. CQRS qua MediatR (command đổi 1 aggregate trong transaction; cross-aggregate dùng domain event + transactional outbox). Repository chỉ cho aggregate có hành vi; query projection `AsNoTracking`. SignalR realtime, Redis cache/backplane, background Worker cho outbox/reminder/route/retention. Frontend Vue 3 SPA/PWA tách feature-module.

## Tóm tắt database

SQL Server, schema theo bounded context. Mọi aggregate có audit + soft-delete + RowVersion. PK nội bộ `bigint IDENTITY`, public `uniqueidentifier`. Tọa độ `geography` + lat/lng `decimal(9,6)`; tiền VND `decimal(19,4)`. RoutePoints partition theo ngày; AuditLogs append-only hash-chained. 36 bảng baseline (xem `docs/Project-Bible/06_Database.md`).

## Tech stack

.NET 10 / ASP.NET Core / EF Core · Vue 3 + Vite + TypeScript + TailwindCSS + shadcn-vue + Pinia · Google Maps API · SQL Server · Redis · SignalR · JWT + refresh rotation + MFA TOTP · Docker (non-root) · GitHub Actions · Serilog · object storage.
