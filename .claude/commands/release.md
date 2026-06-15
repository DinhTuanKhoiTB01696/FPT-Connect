# /release — Đóng gói phát hành

1. Chạy `hooks/pre-task.md` + `/testing` (mọi cổng xanh) + `/review`.
2. Tổng hợp thay đổi từ các sprint/PR liên quan.

Output:
- `CHANGELOG.md` (Added/Changed/Fixed/Security).
- `RELEASE_NOTES.md` (tóm tắt cho stakeholder).
- `MIGRATION_NOTES.md` (forward + rollback, backfill).
- `DEPLOYMENT_CHECKLIST.md` (env, secret, feature flag, smoke).
- `RISK_REPORT.md` + `ROLLBACK_PLAN.md`.

Không tự deploy production (việc của Human Developer + environment gated).
