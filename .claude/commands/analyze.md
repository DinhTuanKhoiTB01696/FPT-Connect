# /analyze — Phân tích khoảng cách tài liệu ↔ code (KHÔNG sửa code)

Đọc: toàn bộ codebase, Project Bible, Sprint, Architecture, Database.

So sánh tài liệu với hiện thực và tìm: tính năng thiếu, hiện thực sai, vi phạm kiến trúc, naming bất nhất, DTO mismatch, API mismatch, DB mismatch, dead code, technical debt.

1. Chạy `hooks/pre-task.md`.
2. Lập bảng: hạng mục · mong đợi (tài liệu) · thực tế (code) · mức độ · khuyến nghị · truy vết ID.
3. KHÔNG chỉnh sửa code — chỉ phân tích.

Output: `ANALYSIS_REPORT.md`.
