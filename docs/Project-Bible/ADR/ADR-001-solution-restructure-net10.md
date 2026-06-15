# ADR-001 — Tái cấu trúc solution + nâng .NET 10

- Trạng thái: Accepted
- Ngày: 2026-06-12
- Người quyết định: Product Owner / Human Developer (FPT Connect)
- Liên quan: `07_Architecture.md`, `CLAUDE.md`, `.claude/AI_RULES.md`, `MEMORY.md`

## Bối cảnh

Baseline trước đó (Sprint 0–1) dùng layout `Backend/src/FptConnect.{Domain,Application,Infrastructure,Api,Worker}` + `Frontend/`, target `net8.0`. Yêu cầu khởi tạo mới muốn một layout chuẩn hơn cho phát triển dài hạn:

- Backend: `src/{Application, Domain, Infrastructure, Presentation, Shared}` + `tests/{UnitTests, IntegrationTests}`, target **.NET 10**.
- Frontend: `apps/web` (Vue 3 + TS + Vite + Pinia + Tailwind + shadcn-vue).

Thay đổi này đụng `AI_RULES` (#2 không đổi tên thư mục, #4 không đổi stack) nên bắt buộc có ADR.

## Quyết định

1. **Áp dụng layout mới** làm baseline kiến trúc mới của FPT Connect:
   - `src/Domain` (thuần), `src/Application` (use case/CQRS, ports), `src/Infrastructure` (EF Core, providers), `src/Presentation` (ASP.NET Core Web API — thay cho `Api`), `src/Shared` (kết quả/hằng số/extension dùng chung), bỏ project `Worker` (background job sẽ là hosted service trong Presentation hoặc thêm lại khi cần).
   - `tests/UnitTests`, `tests/IntegrationTests`.
   - `apps/web` cho frontend.
2. **Nâng TargetFramework lên `net10.0`** cho toàn bộ project backend mới.
3. Chiều phụ thuộc giữ nguyên Clean Architecture: `Domain ← Application ← Infrastructure/Presentation`; `Shared` không phụ thuộc tầng nào; `Domain` không tham chiếu EF/HTTP/JWT.

## Hệ quả

- ✅ Tên project trung lập theo Clean Architecture phổ biến, dễ onboard.
- ✅ Tách `Shared` rõ ràng; tách `tests` theo loại.
- ⚠️ Layout cũ `Backend/` và `Frontend/` (chứa code S0/S1: auth, sessions, MFA, devices) **trở thành legacy/superseded**. Lần khởi tạo này CHỈ tạo cấu trúc trống (không port logic) theo yêu cầu "do not implement business logic".
- ⚠️ `run.bat`, `.github/workflows/*`, `docs/Project-Bible/07_Architecture.md`, root `CLAUDE.md`, `.claude/*` cần được cập nhật để trỏ về layout mới (xem Migration).

## Migration

1. (Đã làm trong lần init) Tạo `src/`, `tests/`, `apps/web`, `FptConnect.sln` (root), Docker/compose, config.
2. (Việc tiếp theo — cần phê duyệt) Port code S0/S1 từ `Backend/src/FptConnect.*` sang `src/*` (Domain entities, IAM/auth, sessions, devices) — đây là "implement", ngoài phạm vi init.
3. Cập nhật `07_Architecture.md` (mục 7.3) + root `CLAUDE.md` + `.claude/CLAUDE.md`/`PROJECT_SUMMARY.md` sang layout mới.
4. Cập nhật `run.bat` (đường dẫn `src/Presentation`, `apps/web`) + CI (`dotnet build FptConnect.sln`).
5. Sau khi port xong & xanh: **archive/xoá** `Backend/` và `Frontend/` cũ (hành động phá huỷ — do Human xác nhận).

## Rollback

Vì layout cũ vẫn còn nguyên (chưa xoá), rollback = bỏ `src/`, `tests/`, `apps/web`, `FptConnect.sln` (root) và tiếp tục trên `Backend/`+`Frontend/`. Không mất dữ liệu/lịch sử.

## Ghi chú .NET 10

Yêu cầu máy build có .NET 10 SDK. CI `setup-dotnet` đặt `10.0.x`. Nếu cần chạy song song .NET 8 cho legacy, hai solution tách biệt nên không xung đột.
