# /doctor — Khám sức khoẻ dự án nhanh

Chạy nhanh các hook kiểm tra để phát hiện vấn đề rõ ràng trước khi làm việc lớn.

1. `hooks/architecture-check.md` (chiều phụ thuộc, stack đúng).
2. `hooks/database-check.md` (PK/FK/index/soft-delete/migration).
3. `hooks/ui-check.md` + `hooks/security-check.md`.
4. Build/lint/test nhanh; liệt kê lỗi chặn (red flags) + cách xử lý.

Output: tóm tắt tình trạng + danh sách việc cần làm ngay.
