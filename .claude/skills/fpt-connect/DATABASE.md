# DATABASE.md — Chuẩn cơ sở dữ liệu FPT Connect

> Chi tiết: `docs/Project-Bible/06_Database.md` + `APPENDIX_D_Database_Columns.md`. File này là chuẩn tối thiểu bắt buộc.

## Cột bắt buộc trên mọi aggregate (mutable)

| Cột | Kiểu | Ghi chú |
|---|---|---|
| `Id` | `bigint IDENTITY` PK | Khóa nội bộ |
| `PublicId` | `uniqueidentifier` DF `NEWSEQUENTIALID()` | ID public (API dùng) |
| `CreatedAtUtc` | `datetime2(3)` | Audit tạo |
| `CreatedBy` | `bigint NULL` | User tạo |
| `UpdatedAtUtc` | `datetime2(3) NULL` | Audit sửa |
| `UpdatedBy` | `bigint NULL` | User sửa |
| `IsDeleted` | `bit DF 0` | Soft delete |
| `DeletedAtUtc` | `datetime2(3) NULL` | Mốc xóa (cho restore window/retention) |
| `DeletedBy` | `bigint NULL` | Người xóa |
| `DeleteReason` | `nvarchar(400) NULL` | Lý do xóa |
| `RowVersion` | `rowversion` | Optimistic concurrency |

> Bảng nối / chi tiết bất biến (vd lịch sử) có thể rút gọn nhưng vẫn ghi thời gian/người tạo.

## Khóa, quan hệ, ràng buộc

- **PK**: `Id bigint`. **Public**: `PublicId uniqueidentifier`.
- **FK**: mặc định `NO ACTION`; chỉ `CASCADE` cho bảng nối/chi tiết không có ý nghĩa độc lập.
- **Unique**: dùng filtered unique cho định danh nghiệp vụ, ví dụ `UQ(TenantId, PhoneHash) WHERE IsDeleted = 0`.
- **Check**: enum lưu `varchar(32)` + CHECK hoặc lookup table; tiền `decimal(19,4) CK >= 0`.
- **Soft delete**: global query filter `IsDeleted = 0`; chỉ endpoint `*.restore` mới `IgnoreQueryFilters`.

## Index

- Index theo access pattern thực tế (filter/sort/join hay dùng), không index bừa.
- Composite index đặt cột chọn lọc cao trước; cân nhắc covering index cho list hot.
- Spatial index trên cột `geography` (Customers.Geo, Territories.Boundary).
- Bảng GPS (`field.RoutePoints`) partition theo ngày/tháng; clustered theo `(OccurredDate, Id)`.

## Audit & bảo mật

- `audit.AuditLogs` append-only, hash-chained (`PrevHash`/`EntryHash`), không sửa/xóa qua app.
- PII: số điện thoại chuẩn hoá E.164, lưu `PhoneHash` để dò trùng, bản rõ mã hoá (app-level/TDE).
- Không đưa PII vào seed, migration log hay exception.

## Quy ước đặt tên

- Schema theo bounded context: `iam`, `crm`, `field`, `ops`, `notify`, `audit`, `ai`.
- Bảng số nhiều PascalCase (`crm.Customers`). Cột PascalCase. FK: `<Entity>Id`.
- Tọa độ: `Latitude/Longitude decimal(9,6)` + `Geo geography`. Tiền VND `decimal(19,4)`.

## Quy tắc migration

- EF Core migration; production forward-only nhưng phải có **script rollback logic**.
- Thêm cột theo expand → migrate → contract; backfill theo batch có checkpoint + metric.
- Online index/rebuild khi edition hỗ trợ; tránh lock bảng lớn.
- Kiểm tra query plan với dữ liệu production-like trước khi merge.
- Mỗi migration có cặp forward + rollback; tên migration mô tả (vd `AddCustomerMergeMap`).
