# /database — Soát schema & migration

Đọc `skills/fpt-connect/DATABASE.md` + `docs/Project-Bible/06_Database.md`.

Kiểm tra: PK, FK, Cascade, Index, Composite index, Unique (filtered), Constraint/Check, chuẩn hoá (3NF), Soft delete (`IsDeleted/DeletedAtUtc/DeletedBy/DeleteReason`), audit columns + RowVersion, partition (GPS), migration an toàn (forward + rollback, expand-migrate-contract).

1. Chạy `hooks/pre-task.md` + `hooks/database-check.md`.
2. Liệt kê thiếu/sai + khuyến nghị; nếu sửa schema → migration có rollback, không phá dữ liệu.

Output: `DATABASE_REVIEW.md`.
