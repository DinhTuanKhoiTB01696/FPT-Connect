# /sprint — Triển khai sprint hiện tại

Đọc: CLAUDE.md, MEMORY.md, AI_RULES.md, Project Bible, spec Sprint hiện tại (`docs/Project-Bible/SPRINT_PLAN.md`).

1. Chạy `hooks/pre-sprint.md` → tạo `PLAN.md` (kế hoạch, rủi ro, dependency, file ảnh hưởng).
2. Chỉ implement đúng phạm vi sprint hiện tại; KHÔNG nhảy/đụng sprint tương lai; P0 → P1 → P2.
3. Mỗi item: code + test + truy vết `FR→UC→API→DB→TC`; tự verify (build/lint/test).
4. Chạy `hooks/architecture-check.md`, `ui-check.md`, `database-check.md`, `security-check.md` khi liên quan.
5. Chạy `hooks/post-sprint.md`.

Output: `PLAN.md`, `TASK_REPORT.md`, `CHANGELOG.md` (đặt ở thư mục gốc repo hoặc `docs/reports/`).
