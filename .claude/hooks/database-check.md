# hook: database-check

Xác minh trước khi merge thay đổi DB:
- PK `bigint` + `PublicId uniqueidentifier`; FK mặc định NO ACTION (cascade chỉ cho bảng nối/chi tiết).
- Index theo access pattern; composite index cột chọn lọc trước; spatial index cho `geography`.
- Unique (filtered cho định danh nghiệp vụ, vd `UQ(TenantId,PhoneHash) WHERE IsDeleted=0`); Check constraint cho enum/tiền.
- Naming: schema theo bounded context; bảng số nhiều PascalCase; FK `<Entity>Id`.
- Audit columns + RowVersion; Soft delete (`IsDeleted/DeletedAtUtc/DeletedBy/DeleteReason`) + global query filter.
- Migration: forward + rollback, expand-migrate-contract, backfill có checkpoint; không PII trong seed/log.
