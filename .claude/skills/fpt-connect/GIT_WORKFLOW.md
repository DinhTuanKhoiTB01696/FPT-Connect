# GIT_WORKFLOW.md — Quy trình Git

> Remote: `https://github.com/DinhTuanKhoiTB01696/FPT-Connect.git`.

## Nhánh
- `main` (production), `develop` (tích hợp). Feature branch từ `develop`: `feat/<sprint>-<slug>`, `fix/<slug>`.
- Push remote và phê duyệt environment `production` do **Human Developer** thực hiện.

## Commit (Conventional Commits)
`feat|fix|refactor|docs|test|chore(scope): mô tả`. Một commit một ý; tham chiếu mã truy vết khi liên quan (FR/UC/API/TC).

## Pull Request
- Mô tả: mục tiêu, mắt xích truy vết `FR→UC→API→DB→TC`, file đã đổi, cách test, ảnh hưởng/rủi ro.
- Cổng: CI xanh, không Critical/High, đúng phạm vi sprint, không placeholder/TODO. Áp dụng `hooks/review.md`.
- Reviewer ≠ author khi four-eyes bật; Human nghiệm thu → merge (squash).

## Không commit
secret/PII, `bin/`, `obj/`, `node_modules/`, `dist/`, `.env` (đã có `.gitignore`).
