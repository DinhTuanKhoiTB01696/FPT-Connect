# hook: commit — Quy ước commit/PR

- Conventional Commits: `feat|fix|refactor|docs|test|chore(scope): mô tả`.
- Một commit một ý; PR một feature/slice; mô tả PR kèm mắt xích truy vết + cách test + ảnh hưởng.
- Trước commit: chạy `post-task` (build/lint/test xanh). Không commit secret/PII/`bin`/`obj`/`node_modules` (đã có `.gitignore`).
- Branch: feature từ `develop`; release theo `main`. Push remote do Human thực hiện.
