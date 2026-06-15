# MEMORY.md — Bộ nhớ thường trực FPT Connect

> File này là **trí nhớ dài hạn** của dự án. Mọi agent đọc nó (cùng CLAUDE.md, AI_RULES.md, PROJECT_SUMMARY.md, Project Bible, Sprint) TRƯỚC khi sinh code. Cập nhật khi có quyết định/baseline mới — append, không xoá lịch sử quan trọng.

## 1. Sự thật cố định (không đổi nếu chưa có ADR)

- Repo: `https://github.com/DinhTuanKhoiTB01696/FPT-Connect.git` · nhánh `main`/`develop`.
- Nguồn sự thật: `docs/Project-Bible/` (SRS+SAD, 100 prompt, 312 test case, 54 use case, 36 bảng).
- Backend: **.NET 10**, ASP.NET Core Web API, EF Core, SQL Server, Clean Architecture.
- Frontend: **Vue 3 (latest)**, TypeScript, Vite, Pinia, TailwindCSS, **shadcn-vue**.
- Maps: **Google Maps API** (geocode, directions, map).
- Realtime SignalR · Auth JWT + refresh rotation + MFA TOTP · Redis · Docker non-root · GitHub Actions.
- Chuẩn thị giác mặc định: **Taste Skill** (kế thừa, không thay thế).

## 2. Quyết định kiến trúc đã chốt

- Chiều phụ thuộc: `Domain ← Application ← Infrastructure/Api/Worker`. Domain thuần, không EF/HTTP/JWT.
- CQRS qua MediatR; command đổi 1 aggregate/transaction; cross-aggregate qua domain event + transactional outbox (consumer idempotent).
- Repository chỉ cho aggregate có hành vi; query projection `AsNoTracking`. Không `IRepository<T>` lộ `IQueryable`.
- Mọi aggregate: audit + soft-delete (`IsDeleted/DeletedAtUtc/DeletedBy/DeleteReason`) + `RowVersion`.
- API `/api/v1`, RFC 9457, cursor pagination, `If-Match`/ETag, `Idempotency-Key`.

## 3. Trạng thái hiện tại (cập nhật mới nhất)

- Đã có: Project Bible đầy đủ + `SPRINT_PLAN.md` (S0–S18, sprint 1 tuần, đội 10).
- Đã scaffold Sprint 0: `Backend/` (5 project + test), `Frontend/` (Vue 3 + Vite + Tailwind + Pinia), `run.bat`, CI/CD, seed, Dockerfile.
- Workspace `.claude/` đã bootstrap (CLAUDE/AI_RULES/MEMORY/PROJECT_SUMMARY/AGENTS + skill `fpt-connect` + 3 skill taste-skill tích hợp).
- **Sprint 1 (IAM-1) đã implement**: login + rate limit + audit, refresh rotation + reuse detection, MFA TOTP (enroll/confirm/verify + recovery codes), logout, sessions list/revoke, devices list/rename/revoke; FE login (MFA step) + ProfileView. Lưu ý: login của Admin/Manager CHƯA bật MFA sẽ cấp token kèm cờ `mustEnrollMfa` (tránh khoá người dùng trước khi enroll) — hơi lệch TC-006, sẽ siết về enroll-first khi có quyết định.
- **Nợ kỹ thuật đã biết**: backend scaffold đang target `net8.0` → cần PR hạ tầng nâng lên `net10.0`; chưa cài `shadcn-vue` vào `Frontend/` (mới khai báo trong stack, sẽ thêm khi dựng UI component ở sprint frontend đầu tiên).

## 4. Quy ước cần nhớ

- Truy vết bắt buộc `FR → UC → API → DB → TC`; PR thiếu mắt xích không merge.
- Một sprint / một feature một lúc; không refactor ngoài phạm vi; không placeholder/TODO.
- Không hard-code màu (dùng token); dark mode + WCAG 2.2 AA bắt buộc.
- Tài khoản seed dev: `admin@fptconnect.vn` / `Admin@12345`.
- Push lên remote và phê duyệt environment `production` là việc của Human Developer.

## 5. Nhật ký quyết định (Decision log)

| Ngày | Quyết định | Lý do |
|---|---|---|
| 2026-06-11 | Bootstrap `.claude` workspace + skill fpt-connect | Nhất quán dài hạn cho mọi agent |
| 2026-06-12 | Thêm `shadcn-vue` + `Google Maps API` vào stack cố định; tạo MEMORY.md | Theo yêu cầu khởi tạo AI mới nhất |
| 2026-06-12 | Triển khai Sprint 1 IAM (auth/session/MFA/device) trên scaffold S0, giữ net8.0 | Đúng phạm vi sprint; nâng .NET 10 là PR hạ tầng riêng |
| 2026-06-12 | Bootstrap commands/ (17) + hooks/ (10) + 5 skill (SECURITY/PERFORMANCE/DEPLOYMENT/DEVOPS/GIT_WORKFLOW) | Workspace AI-first: gọi /sprint /review /analyze… không cần lặp lại rule |
| 2026-06-12 | ADR-001: đổi layout sang src/{Domain,Application,Infrastructure,Presentation,Shared}+tests+apps/web, nâng .NET 10 | Baseline mới dài hạn; layout cũ Backend/+Frontend/ thành legacy (chưa port logic) |
| 2026-06-12 | DevDataSeeder: tự seed 5 roles + ~33 users + 200 customers VN (Development) khi DB rỗng/reset | Demo không bao giờ rỗng; phần Visits/Routes/CheckIns/Reminders/Notifications/Tasks/Tags chờ schema (S2–S9) |

> Khi thêm quyết định lớn (đổi provider, schema, auth, retention…), ghi một dòng vào bảng trên và tạo ADR trong `docs/Project-Bible/`.
