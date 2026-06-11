# CLAUDE.md — Ngữ cảnh dự án FPT Connect

> File này để các AI agent (Claude Code / Codex / Cursor) đọc trước khi làm việc.

## Repository

- Git remote: `https://github.com/DinhTuanKhoiTB01696/FPT-Connect.git`
- Nhánh chính: `main`; nhánh phát triển: `develop`.
- Nguồn sự thật duy nhất: `docs/Project-Bible/` (SRS + SAD). Code khác tài liệu phải tạo ADR/Change Request trước.

## Kiến trúc (tóm tắt — chi tiết ở docs/Project-Bible/07_Architecture.md)

- Backend: .NET 8, Clean Architecture — `Backend/src/FptConnect.{Domain,Application,Infrastructure,Api,Worker}`.
- Frontend: Vue 3 + Vite + TypeScript + TailwindCSS + Pinia — `Frontend/`.
- DB: SQL Server, EF Core, schema theo bounded context (`iam`, `crm`, `field`, `ops`, `notify`, `audit`, `ai`).
- Quy tắc phụ thuộc: `Domain <- Application <- Infrastructure/Api/Worker`. Domain không tham chiếu EF/HTTP/JWT.

## Chạy local

- Chạy cả BE + FE: double-click `run.bat` ở thư mục gốc (hỏi SQL Server instance, mặc định LocalDB).
- API: http://localhost:5080 (Swagger `/swagger`). Frontend: http://localhost:5173.
- Tài khoản seed: `admin@fptconnect.vn` / `Admin@12345`.

## Quy ước bắt buộc

- Mọi aggregate có audit + soft-delete (`IsDeleted/DeletedAtUtc/DeletedBy/DeleteReason`) + `RowVersion`.
- ID public dùng `Guid`; khóa nội bộ `bigint`. Tiền VND `decimal(19,4)`; tọa độ `decimal(9,6)`.
- API base `/api/v1`; lỗi theo RFC 9457; pagination cursor; update dùng `If-Match`/ETag.
- Truy vết bắt buộc `FR -> UC -> API -> DB -> TC` (xem APPENDIX_A). PR thiếu mắt xích không merge.
- Không commit secret; PII masked trong log.

## Frontend design

- Tuân thủ token ở `docs/Project-Bible/10_DesignSystem.md` (đã map vào `Frontend/src/style.css`). Dark mode bắt buộc, WCAG 2.2 AA.
- Dùng bộ skill `taste-skill` (https://github.com/Leonxlnx/taste-skill) cho chất lượng UI — xem `Frontend/DESIGN.md`.

## Prompt Bible

- 100 prompt theo từng lĩnh vực ở `docs/Project-Bible/15_PromptBible.md` (Claude/Codex/Antigravity). Dùng khi triển khai từng sprint (xem `docs/Project-Bible/SPRINT_PLAN.md`).
