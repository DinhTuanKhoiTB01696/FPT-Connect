# SYNC_REPORT — FPT Connect

> Lệnh: `/analyze` (+ kiểm tra nhất quán kiểu `/sync`) · Ngày: 2026-06-12 · Chế độ: **chỉ báo cáo, không sửa code, không thực thi đồng bộ**.
> Mục tiêu: phát hiện lệch giữa các nguồn sự thật (`docs/Project-Bible/`, `.claude/*`, `MEMORY.md`, root `CLAUDE.md`) và code; đề xuất hành động đồng bộ để Human/`/sync` thực hiện.

## 1. Ma trận nhất quán nguồn ↔ nguồn

| Chủ đề | docs/Project-Bible | .claude/CLAUDE.md | root CLAUDE.md | MEMORY.md | Code | Nhất quán? |
|---|---|---|---|---|---|---|
| Phiên bản .NET | (SAD nêu .NET 8/clean arch tùy chương) | **.NET 10** | **.NET 8** | **.NET 10** (ghi nợ hạ net→net10) | **net8.0** | ❌ Lệch 3 chiều |
| Frontend stack | Vue3+TS+Vite+Pinia+Tailwind(+shadcn) | +shadcn-vue | +shadcn-vue | +shadcn-vue | **không có shadcn-vue** | ⚠️ Khai báo ≠ cài đặt |
| Google Maps | có (Field) | có | (mô tả) | có | chưa code | ⚪ Chưa tới sprint |
| Soft-delete columns | `IsDeleted/DeletedAtUtc/DeletedBy/DeleteReason` | đồng bộ | đồng bộ | đồng bộ | ✅ `BaseEntity` đúng | ✅ |
| Bounded context schema | iam/crm/field/ops/notify/audit/ai | đồng bộ | đồng bộ | — | ✅ iam/crm/audit (phần đã làm) | ✅ |
| Read-order trước sprint | — | CLAUDE→AI_RULES→MEMORY→SUMMARY→Bible→skills | — | (đọc trước khi code) | hooks/pre-task khớp | ✅ |
| Trạng thái sprint | S0–S18 plan | — | — | "S0 scaffolded; S1 implemented" | S0+S1 có code | ✅ Khớp |

## 2. Lệch cần đồng bộ (đề xuất, CHƯA thực thi)

### S1 — 🟠 Bất nhất phiên bản .NET (3 chiều)
- **Hiện trạng**: code `net8.0`; `.claude/CLAUDE.md` + `MEMORY.md` + `PROJECT_SUMMARY.md` nói .NET 10; root `CLAUDE.md` nói .NET 8.
- **Đề xuất** (chọn 1, kèm ADR):
  - (a) Nâng code lên `net10.0` (PR hạ tầng riêng) → giữ nguyên tài liệu .NET 10, sửa root `CLAUDE.md` thành .NET 10.
  - (b) Giữ code .NET 8 → hạ `.claude`/MEMORY/SUMMARY về .NET 8 + AI_RULES #4, ghi ADR "tạm .NET 8".
- **Khuyến nghị**: (a) — đúng định hướng AI_RULES; thực hiện đầu một sprint hạ tầng, không trộn feature.

### S2 — ⚠️ shadcn-vue: khai báo nhưng chưa cài
- **Đề xuất**: hoặc cài `shadcn-vue` vào `Frontend/` (sprint UI đầu tiên) rồi cập nhật `package.json`; hoặc nếu hoãn, ghi rõ trong `MEMORY.md` (đã có 1 dòng) là "declared, pending install". Giữ nguyên — chỉ cần không quên khi `/ui` chạy.

### S3 — 🔵 DTO login lệch tài liệu API
- Login trả thêm `mustEnrollMfa` (không có trong `08_API.md`/`APPENDIX_C`). **Đề xuất**: sau khi quyết A8 (xem ANALYSIS_REPORT), hoặc bổ sung field này vào tài liệu API, hoặc bỏ field theo flow enroll-first.

### S4 — 🔵 Tài liệu hoá quyết định MFA enroll-first
- `MEMORY.md` đã ghi deviation TC-006. **Đề xuất**: tạo ADR `docs/Project-Bible/` chốt hành vi, rồi cập nhật UC-001/TC-006 cho khớp (giữ truy vết).

## 3. Truy vết (traceability) — kiểm tra nhanh

- API mới của S1 đều có mặt trong catalog & ma trận truy vết: `API-001..009` (auth/sessions), `API-093..095` (devices, UC-053), `API-005 me`. ✅
- TC liên quan S1: `TC-001..020` (auth/token), `TC-309/310` (device). Catalog có; **integration test thực thi còn thiếu** (xem A9). ⚠️
- Không phát hiện endpoint/bảng "mồ côi" (code có nhưng tài liệu không) — `mustEnrollMfa` là field, không phải endpoint. ✅

## 4. Trạng thái MEMORY.md so với thực tế

- MEMORY mục "Trạng thái hiện tại" và "Decision log" **khớp** với code (S0 scaffold, S1 IAM, giữ net8.0, shadcn pending, commands/hooks bootstrap). ✅ Không cần sửa nội dung; chỉ append khi có quyết định mới (A1/A8).

## 5. Hành động đồng bộ đề xuất (cho `/sync` hoặc Human, theo thứ tự)

1. Chốt quyết định .NET (A1/S1) → ADR → đồng bộ root `CLAUDE.md` ↔ `.claude` ↔ code.
2. Chốt MFA enroll-first (S4) → ADR → cập nhật UC-001/TC-006 + tài liệu API (S3).
3. Lập lịch cài shadcn-vue (S2) và căn Customer↔DB-11 vào Sprint CRM (S3–S4) — ghi vào `NEXT_STEPS.md` khi đóng sprint.
4. Đưa các báo cáo (`ANALYSIS_REPORT.md`, `SYNC_REPORT.md`) vào `docs/reports/` nếu muốn lưu vết lịch sử.

> Không có thay đổi code/tài liệu nào được thực hiện trong lần chạy này. Các mục trên là đề xuất; thực thi bằng `/sync`, `/migrate`, hoặc PR hạ tầng do Human duyệt.
