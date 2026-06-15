# ANALYSIS_REPORT — FPT Connect

> Lệnh: `/analyze` · Ngày: 2026-06-12 · Chế độ: **chỉ phân tích, không sửa code**.
> Nguồn đối chiếu: `docs/Project-Bible/` (SRS/SAD, 54 UC, 98 API, 312 TC, 36 bảng) + `SPRINT_PLAN.md` (S0–S18) ↔ code thực tế trong `Backend/`, `Frontend/`.
> Mức độ: 🔴 Critical · 🟠 High · 🟡 Medium · 🔵 Low · ⚪ Expected (chưa tới sprint).

## 1. Tóm tắt điều hành

Dự án đang ở cuối **Sprint 1 (IAM-1)**. Sprint 0 (Foundation) và Sprint 1 (Auth & Session) đã hiện thực; Sprint 2–18 chưa bắt đầu (đúng kế hoạch). Clean Architecture được tôn trọng (Domain thuần, không tham chiếu EF/HTTP/JWT). Khoảng cách lớn nhất **không phải** ở phần chưa làm (đó là điều dự kiến), mà ở: (a) **bất nhất phiên bản .NET** giữa tài liệu và code; (b) một số **hạng mục P0 của Sprint 0 chưa hoàn tất** (MediatR pipeline, architecture test, observability đầy đủ); (c) **stack khai báo nhưng chưa hiện thực** (shadcn-vue); (d) **Customer entity lệch DB-11** (sẽ thành nợ khi vào Sprint CRM).

Tiến độ hiện thực so với baseline: **API 14/98** (~14%), **Bảng 9/36** (~25%), **Test 3 file/312 TC** (catalog). Các tỉ lệ thấp là *bình thường* theo lộ trình sprint.

## 2. Bảng phát hiện chính

| # | Hạng mục | Tài liệu (mong đợi) | Code (thực tế) | Mức | Khuyến nghị |
|---|---|---|---|---|---|
| A1 | Phiên bản .NET | `.claude` + MEMORY + PROJECT_SUMMARY: **.NET 10** | `Directory.Build.props`: **net8.0**; root `CLAUDE.md`: **.NET 8** | 🟠 | Quyết định 1 nguồn: nâng `net10.0` (PR hạ tầng) **hoặc** hạ tài liệu về .NET 8 kèm ADR; đồng bộ root CLAUDE.md ↔ `.claude/CLAUDE.md` |
| A2 | MediatR pipeline (PR-006, S0 P0) | validation/auth/idempotency/logging/transaction-outbox | Chưa có; `AddApplication` chỉ đăng ký `SessionService` | 🟠 | Hoàn tất hạng mục S0 còn nợ trước khi mở rộng feature |
| A3 | Architecture test (NetArchTest, S0 P0) | test fail khi vi phạm chiều phụ thuộc | Không có project `Architecture` test | 🟡 | Thêm test giữ Clean Architecture (đang đúng nhưng chưa được "khóa") |
| A4 | Observability (S0) | correlation-id middleware + metrics + traces | Chỉ Serilog console + health live/ready | 🟡 | Bổ sung correlation/trace middleware + golden-signal metrics |
| A5 | shadcn-vue | Stack cố định (AI_RULES #4, MEMORY) | `Frontend/package.json` **không có**; UI dựng bằng Tailwind thuần | 🟡 | Cài shadcn-vue khi vào sprint UI đầu tiên; tới lúc đó là nợ đã ghi nhận |
| A6 | Customer entity vs DB-11 | `PhoneEncrypted` + `PhoneHash`, `Geo geography`, `TenantId`, `TerritoryId`, `Type`, `Score`, filtered `UQ(TenantId,PhoneHash) WHERE IsDeleted=0` | `PhoneE164` (bản rõ) + `PhoneHash`(chưa dùng), `Latitude/Longitude decimal` (không `geography`), không `TenantId/Type/Score` | 🟡 ⚪ | CRM là S3–S4 (chưa tới). Khi vào, phải khớp DB-11 + BR-001 (mã hoá phone), thêm spatial + dedupe unique |
| A7 | Multi-tenancy | `TenantId` ở hầu hết bảng + scope theo tenant | Entities IAM/CRM chưa có `TenantId` | 🟡 ⚪ | Thêm khi siết multi-tenant (S2 IAM-2 RBAC/scope) |
| A8 | MFA login flow vs TC-006 | Admin chưa MFA → trả challenge, **chưa cấp** access token | Cấp token kèm cờ `mustEnrollMfa` (tránh khoá người dùng) | 🟡 | Đã ghi MEMORY; cần ADR chọn "enroll-first" nghiêm ngặt hay giữ hiện tại |
| A9 | Abuse-case test suite IAM (S1 P1) | Integration test TC-009,010,012,013 | Chỉ unit test domain (`SessionTests`) | 🟡 | Bổ sung integration test (WebApplicationFactory) cho login/refresh race/rate-limit |
| A10 | DTO login response | API-001 trả token pair / MFA challenge | Thêm field `mustEnrollMfa` (ngoài spec) | 🔵 | Cập nhật `08_API.md`/`APPENDIX_C` cho khớp, hoặc bỏ field theo A8 |
| A11 | CQRS qua MediatR | Command/Query handler | Controller gọi `AppDbContext` trực tiếp | 🔵 ⚪ | CQRS đánh dấu "optional/planned"; gắn MediatR khi A2 hoàn tất |
| A12 | AuditWriter double-save | — | Dùng chung `AppDbContext` scoped, tự `SaveChanges` (có thể lưu sớm thay đổi đang chờ) | 🔵 | Cân nhắc `DbContext` riêng cho audit hoặc gom transaction (việc của `/review`) |
| A13 | Google Maps API | geocode/directions/map | Chưa có code | ⚪ | Field sprints S5–S8, chưa tới |
| A14 | Worker outbox/consumers | reminder/route/retention | `OutboxWorker` heartbeat | ⚪ | Consumers ở S7/S9/S16 |

## 3. Đối chiếu theo lĩnh vực

### 3.1 Kiến trúc
- ✅ Chiều phụ thuộc đúng: Domain không tham chiếu EF/HTTP/JWT; Application định nghĩa port, Infrastructure hiện thực. Repository chưa dùng kiểu `IRepository<T>` lộ `IQueryable` (tốt).
- 🟠 Thiếu pipeline behaviors (A2) và architecture test (A3) — là P0 của S0.
- 🔵 Controller đọc/ghi qua `AppDbContext` trực tiếp; chấp nhận ở giai đoạn scaffold, nhưng nên chuyển sang Application handler khi MediatR sẵn sàng.

### 3.2 Database
- ✅ Bảng IAM (Users, Roles, UserRoles, Sessions, Devices, MfaMethods) + AuditLogs + Customers/CustomerStatusHistory khớp tên schema (`iam`, `crm`, `audit`). Soft-delete + RowVersion + audit columns áp dụng đúng `BaseEntity`.
- 🟡 Customer lệch DB-11 (A6); chưa có `TenantId` (A7); thiếu spatial `geography` và filtered unique cho dedupe.
- 9/36 bảng đã hiện thực — phần còn lại thuộc sprint sau (field/ops/notify/ai). Migration sinh khi chạy `run.bat` (chưa commit thư mục Migrations — đúng, vì reset tạo `PlaneRenovation`).

### 3.3 API
- ✅ Đã có: `auth/login, mfa/verify, mfa/enroll, mfa/confirm, refresh, logout, me`; `sessions` (list/revoke); `devices` (list/rename/revoke); `health live/ready`; `customers` (list/create cơ bản). Có RFC 9457-style error, rate limit login, JWT.
- 🟡 `customers` mới ở mức tối thiểu (chưa cursor chuẩn/dedupe/merge/import — thuộc S3–S4). 14/98 endpoint.
- 🔵 Response login thêm `mustEnrollMfa` (A10) — cần đồng bộ tài liệu.

### 3.4 Frontend
- ✅ Vue 3 + Vite + TS + Pinia + Tailwind + router; Login (MFA step), Dashboard, Profile (sessions/devices/MFA). Token màu + dark mode theo Design System. Build đã verify xanh ở vòng scaffold.
- 🟡 **shadcn-vue chưa cài** (A5) dù là stack cố định.
- 🔵 Dùng `window.prompt` để đổi tên thiết bị (ProfileView) — tạm ổn nhưng không đạt chuẩn "premium SaaS"; nên thay bằng dialog shadcn-vue (việc của `/ui`).

### 3.5 Security
- ✅ Refresh rotation + reuse detection (revoke family), MFA TOTP + recovery code (hash, dùng 1 lần), rate limit, generic login error, audit hash-chained, MFA secret bảo vệ bằng DataProtection, JWT clock skew 30s.
- 🟡 MFA enroll-first chưa nghiêm ngặt (A8). Chưa có integration test bảo chứng abuse-case (A9).
- ⚪ ASVS L2 đầy đủ (pentest, SAST/DAST gate) thuộc S17.

### 3.6 Naming & dead code
- ✅ Naming nhất quán với Bible cho entity/endpoint/schema. Không phát hiện dead code đáng kể trong `src` (bỏ qua `bin/obj` đã build — nên đảm bảo `.gitignore` loại trừ, hiện đã có).
- 🔵 `Customer.PhoneE164` lệch quy ước `PhoneEncrypted` của Bible (A6).

## 4. Nợ kỹ thuật tổng hợp (ưu tiên)

1. (🟠) Hoàn tất P0 còn nợ của Sprint 0: MediatR pipeline (A2), architecture test (A3), observability (A4).
2. (🟠) Giải quyết bất nhất .NET (A1) bằng một quyết định + ADR.
3. (🟡) Quyết định MFA enroll-first (A8) + bổ sung integration abuse-case test (A9).
4. (🟡) Lên kế hoạch cài shadcn-vue (A5) và căn Customer↔DB-11 (A6/A7) đúng thời điểm sprint CRM.

## 5. Kết luận

Không có lỗi 🔴 Critical. Kiến trúc nền lành mạnh và đúng tài liệu ở phần đã làm. Cần xử lý nhóm 🟠 (nợ S0 + bất nhất .NET) trước khi mở rộng feature để tránh tích lũy lệch. Các khoảng trống còn lại phần lớn là ⚪ "chưa tới sprint" và đã nằm trong `SPRINT_PLAN.md`.

> Báo cáo này KHÔNG chỉnh sửa code. Hành động đồng bộ tài liệu/cấu hình xem `SYNC_REPORT.md`.
