# /migrate — Tạo & soát migration an toàn

Đọc `skills/fpt-connect/DATABASE.md`.

1. Chạy `hooks/pre-task.md` + `hooks/database-check.md`.
2. EF Core migration: forward + **script rollback**; expand → migrate → contract khi đổi cột; backfill theo batch có checkpoint + metric.
3. Online index khi edition hỗ trợ; tránh lock bảng lớn; kiểm tra query plan với dữ liệu production-like.
4. Không đưa PII vào seed/migration log.

Output: migration + `MIGRATION_NOTES.md` (kèm rollback).
