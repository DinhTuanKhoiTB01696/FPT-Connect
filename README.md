# FPT Connect

Hệ thống CRM theo vị trí + quản lý nhân viên hiện trường: GPS tracking, tuyến đường, check-in geofence, nhắc việc, phân tích và trợ lý AI.

- Tài liệu đầy đủ (SRS/SAD/Project Bible): [`docs/Project-Bible/`](docs/Project-Bible/)
- Kế hoạch sprint: [`docs/Project-Bible/SPRINT_PLAN.md`](docs/Project-Bible/SPRINT_PLAN.md)

## Công nghệ

| Lớp | Stack |
|---|---|
| Backend | .NET 8, ASP.NET Core, EF Core, Clean Architecture, SQL Server |
| Frontend | Vue 3, Vite, TypeScript, TailwindCSS, Pinia |
| Hạ tầng | Docker, GitHub Actions CI/CD, Serilog |

## Cấu trúc

```
FPT CONNECT/
├─ Backend/
│  ├─ FptConnect.sln
│  ├─ seed_data.sql
│  └─ src/
│     ├─ FptConnect.Domain/          # Entities, value objects
│     ├─ FptConnect.Application/     # Use cases, ports
│     ├─ FptConnect.Infrastructure/  # EF Core, security, providers
│     ├─ FptConnect.Api/            # HTTP API + Dockerfile
│     └─ FptConnect.Worker/         # Background jobs/outbox
├─ Frontend/                         # Vue 3 SPA + Dockerfile
├─ docs/Project-Bible/               # SRS/SAD + phụ lục
├─ .github/workflows/                # CI + CD
└─ run.bat                           # Chạy BE + FE một phát
```

## Yêu cầu

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org)
- SQL Server (LocalDB / Express / full) + `sqlcmd`
- EF Core tools: `dotnet tool install --global dotnet-ef`

## Chạy nhanh (Windows)

```bat
run.bat
```

Script sẽ hỏi SQL Server instance (Enter để dùng `(localdb)\MSSQLLocalDB`), tùy chọn reset DB + chạy migrations + seed, rồi mở Backend (http://localhost:5080) và Frontend (http://localhost:5173) ở hai cửa sổ.

Đăng nhập mẫu: `admin@fptconnect.vn` / `Admin@12345`.

## Chạy thủ công

```bash
# Backend
cd Backend/src/FptConnect.Api
dotnet ef database update --project ../FptConnect.Infrastructure --startup-project .
dotnet run

# Frontend
cd Frontend
npm install
npm run dev
```

## CI/CD

- `.github/workflows/ci.yml` — build + test backend (.NET), typecheck + test + build frontend (Vue), quét secret (Gitleaks). Chạy trên push/PR vào `main`/`develop`.
- `.github/workflows/cd.yml` — build & push Docker image (API + Web) lên GHCR; bước deploy đặt sau environment `production` (cần phê duyệt). Thay placeholder bằng hạ tầng thật.

## Đẩy lên GitHub lần đầu

Chạy các lệnh sau từ thư mục gốc (trên máy bạn — nơi đã đăng nhập GitHub):

```bash
git init
git add .
git commit -m "chore: scaffold Sprint 0 (backend, frontend, CI/CD, docs)"
git branch -M main
git remote add origin https://github.com/DinhTuanKhoiTB01696/FPT-Connect.git
git push -u origin main
```

> Nếu remote đã có commit: `git pull origin main --allow-unrelated-histories` trước khi push.

## License

Proprietary — FPT Connect project.
