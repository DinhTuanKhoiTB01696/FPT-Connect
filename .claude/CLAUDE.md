# CLAUDE.md — FPT Connect AI Workspace

> **Claude (và mọi AI agent) PHẢI đọc file này trước khi viết bất kỳ dòng code nào.**
> Đây là điểm vào của workspace. Quy tắc chi tiết ở `.claude/AI_RULES.md` và `.claude/skills/fpt-connect/`.

## 1. Tổng quan dự án

FPT Connect là nền tảng **CRM doanh nghiệp + GPS Tracking + Google Maps + Field Service Management**: quản lý lead/khách hàng theo vị trí, nhân viên hiện trường, check-in geofence, theo dõi tuyến đường, nhắc việc, phân tích và trợ lý AI.

Nguồn sự thật duy nhất là `docs/Project-Bible/` (SRS + SAD). Mọi sai khác giữa code và tài liệu phải tạo ADR/Change Request trước khi đổi baseline.

## 2. Stack

| Lớp | Công nghệ |
|---|---|
| Backend | .NET 10, ASP.NET Core, EF Core, Clean Architecture |
| Frontend | Vue 3 (latest) + Vite + TypeScript + TailwindCSS + shadcn-vue + Pinia |
| Database | SQL Server, schema theo bounded context: iam, crm, field, ops, notify, audit, ai |
| Realtime | SignalR (Redis backplane khi scale-out) |
| Maps | Google Maps API (geocode, directions, map) |
| Auth | JWT access + refresh rotation, MFA (TOTP) |
| Hạ tầng | Docker (non-root), GitHub Actions CI/CD, Serilog, Redis, object storage |

> Phiên bản mục tiêu là **.NET 10**. Nếu solution còn net8.0, nâng TargetFramework lên net10.0 trong một PR hạ tầng riêng, không trộn với feature.

## 3. Cấu trúc thư mục

```text
FPT CONNECT/
├─ .claude/                 # AI workspace: rules + skills + commands/ (/sprint,/review,/analyze,/brain,…) + hooks/ (pre-task,post-task,…)
├─ Backend/
│  ├─ FptConnect.sln
│  └─ src/FptConnect.{Domain,Application,Infrastructure,Api,Worker}
├─ Frontend/                # Vue 3 SPA
├─ docs/Project-Bible/      # SRS/SAD + phụ lục + SPRINT_PLAN.md
├─ .github/workflows/       # ci.yml, cd.yml
└─ run.bat
```

Quy tắc phụ thuộc: Domain <- Application <- Infrastructure/Api/Worker. Domain không tham chiếu EF, HTTP, JWT, Serilog hay provider.

## 4. Triết lý phát triển

Enterprise · Minimal · Premium · Scalable · Production-ready. Ưu tiên chất lượng và khả năng bảo trì dài hạn hơn tốc độ. Code phải build được, không placeholder, không TODO bỏ ngỏ. Mỗi thay đổi truy vết được tới FR → UC → API → DB → TC.

## 5. Quy trình Sprint

Kế hoạch ở docs/Project-Bible/SPRINT_PLAN.md (S0–S18, sprint 1 tuần). Trước mỗi sprint đọc theo thứ tự: CLAUDE.md → AI_RULES.md → MEMORY.md → PROJECT_SUMMARY.md → Project Bible + Sprint → skills/fpt-connect/SKILL.md → skills/fpt-connect/RULES.md → SPRINT_RULES.md. Chỉ làm đúng phạm vi sprint hiện tại.

## 6. Yêu cầu build

- Backend: dotnet build -c Release xanh; dotnet test pass.
- Frontend: npm run typecheck, npm test, npm run build đều pass.
- Migration có forward + rollback; không commit secret; PII masked trong log.

## 7. Mục tiêu chất lượng

OWASP ASVS L2 (Admin tiệm cận L3), WCAG 2.2 AA luồng lõi, domain coverage ≥ 80%, P95 read < 500 ms / write < 800 ms ở 300 RPS. Không lỗi Critical/High khi release.

## 8. Hành vi AI

1. Đọc tài liệu trước; hỏi khi mơ hồ về phạm vi/dữ liệu; không tự đổi business rule.
2. Tôn trọng Clean Architecture và quy ước đặt tên; không refactor module không liên quan.
3. Sau khi code: tự kiểm tra build + lint + test; nêu rõ file đã đổi và mắt xích truy vết.
4. Frontend kế thừa triết lý taste-skill (skills/fpt-connect/DESIGN.md).
5. Không bịa API/endpoint/bảng; nếu thiếu, đề xuất qua Change Request.
