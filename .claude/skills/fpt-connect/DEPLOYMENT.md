# DEPLOYMENT.md — Triển khai

> Chi tiết: Bible 13. Hạ tầng: Docker (non-root) + GitHub Actions.

## Môi trường
- dev (LocalDB qua `run.bat`), staging, production. Config qua biến môi trường; secret ở vault.
- API container non-root (Bible TC-297), `ASPNETCORE_URLS` cổng nội bộ; Frontend build tĩnh phục vụ qua nginx.

## CI/CD
- `ci.yml`: build/test .NET + typecheck/test/build Vue + secret scan. `cd.yml`: build & push image (GHCR) + deploy gated qua environment `production`.
- Rolling deploy với migration tương thích (old/new app cùng chạy); readiness fail loại instance khỏi LB.

## Release checklist
- [ ] Mọi cổng test xanh; không Critical/High.
- [ ] Migration forward + rollback đã kiểm thử; backfill có checkpoint.
- [ ] Secret/feature flag cấu hình đúng môi trường.
- [ ] Smoke sau deploy: login, customer read, health.
- [ ] Backup/restore drill đạt RPO ≤ 15' / RTO ≤ 2h.

## Degraded mode
CRM core vẫn chạy khi AI/Maps/notify lỗi (feature flag + circuit breaker).
