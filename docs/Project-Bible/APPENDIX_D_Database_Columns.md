# Phụ lục D. Từ điển cột dữ liệu

> Sinh tự động từ `06_Database.md`; catalog chương 6 là nguồn chuẩn về bảng và index.

## D.1 Cột chuẩn dùng lại

| Column | Data type | Default/constraint | Description |
|---|---|---|---|
| `CreatedAtUtc` | `datetime2(3)` | Server UTC, not null | Thời điểm tạo |
| `CreatedBy` | `bigint` | FK user/service actor | Actor tạo |
| `UpdatedAtUtc` | `datetime2(3)` | Nullable | Lần cập nhật gần nhất |
| `UpdatedBy` | `bigint` | Nullable FK | Actor cập nhật |
| `IsDeleted` | `bit` | `0` | Soft-delete |
| `RowVersion` | `rowversion` | Generated | Optimistic concurrency |

## DB-01 - `iam.Tenants`

**Mục đích:** Tenant logic; V1 có thể chỉ một tenant nhưng không hard-code

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `PublicId` | `uniqueidentifier` | - | NEWSEQUENTIALID | Not null theo model | UUID public dùng trong URL và integration. |
| `Code` | `varchar(32)` | - | - | Unique; Not null theo model | Mã machine-readable thuộc catalog/allowlist, ổn định cho API và báo cáo. |
| `Name` | `nvarchar(200)` | - | - | Not null theo model | Tên hiển thị, Unicode, được trim và giới hạn độ dài. |
| `Status` | `varchar(20)` | - | - | CK; Not null theo model | Trạng thái hiện tại, bị giới hạn bởi state machine/check constraint. |

**Index:** `UQ(Code)`, `IX(Status)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-02 - `iam.Departments`

**Mục đích:** Cây chi nhánh/phòng/đội; không cho vòng lặp

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `TenantId` | `bigint` | FK | - | Not null theo model | Tenant sở hữu bản ghi; luôn tham gia authorization scope. |
| `ParentId` | `bigint` | FK | - | Nullable | Thuộc tính nghiệp vụ của `iam.Departments`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Code` | `varchar(32)` | - | - | Not null theo model | Mã machine-readable thuộc catalog/allowlist, ổn định cho API và báo cáo. |
| `Name` | `nvarchar(200)` | - | - | Not null theo model | Tên hiển thị, Unicode, được trim và giới hạn độ dài. |
| `Path` | `hierarchyid` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `iam.Departments`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Status` | `varchar(100)` | - | - | Not null theo model | Trạng thái hiện tại, bị giới hạn bởi state machine/check constraint. |

**Index:** `UQ(TenantId,Code)`, `IX(Path)`, `IX(ParentId)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-03 - `iam.Users`

**Mục đích:** Danh tính người dùng

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `TenantId` | `bigint` | FK | - | Not null theo model | Tenant sở hữu bản ghi; luôn tham gia authorization scope. |
| `DepartmentId` | `bigint` | FK | - | Not null theo model | Đơn vị tổ chức chịu trách nhiệm hoặc sở hữu. |
| `EmployeeCode` | `varchar(32)` | - | - | Not null theo model | Mã machine-readable thuộc catalog/allowlist, ổn định cho API và báo cáo. |
| `EmailNormalized` | `varchar(256)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `iam.Users`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `PhoneEncrypted` | `varbinary` | - | - | Not null theo model | Dữ liệu nhạy cảm đã mã hóa; không dùng trực tiếp trong log hoặc index bản rõ. |
| `PasswordHash` | `nvarchar(500)` | - | - | Nullable | Giá trị băm một chiều để so khớp/kiểm tra; không thể dùng khôi phục dữ liệu gốc. |
| `Status` | `varchar(100)` | - | - | Not null theo model | Trạng thái hiện tại, bị giới hạn bởi state machine/check constraint. |
| `LastLoginAtUtc` | `datetime2(3)` | - | - | Nullable | Thời điểm UTC của sự kiện được nêu bởi tên cột. |

**Index:** `UQ(TenantId,EmployeeCode)`, `UQ(TenantId,EmailNormalized)`, `IX(DepartmentId,Status)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-04 - `iam.Roles`

**Mục đích:** Nhóm permission versioned

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `TenantId` | `bigint` | FK | - | Nullable | Tenant sở hữu bản ghi; luôn tham gia authorization scope. |
| `Code` | `varchar(100)` | - | - | Not null theo model | Mã machine-readable thuộc catalog/allowlist, ổn định cho API và báo cáo. |
| `Name` | `nvarchar(500)` | - | - | Not null theo model | Tên hiển thị, Unicode, được trim và giới hạn độ dài. |
| `IsSystem` | `bit` | - | 0 | Not null theo model | Thuộc tính nghiệp vụ của `iam.Roles`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Version` | `int` | - | 1 | Not null theo model | Phiên bản dùng audit, concurrency hoặc effective configuration. |

**Index:** `UQ(TenantId,Code)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-05 - `iam.Permissions`

**Mục đích:** Catalog quyền atomic

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `Code` | `varchar(100)` | - | - | Unique; Not null theo model | Mã machine-readable thuộc catalog/allowlist, ổn định cho API và báo cáo. |
| `Resource` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `iam.Permissions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Action` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `iam.Permissions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Description` | `nvarchar(max)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `iam.Permissions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** `UQ(Code)`, `IX(Resource,Action)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-06 - `iam.UserRoles`

**Mục đích:** Gán role theo scope và thời hạn

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `UserId` | `bigint` | FK | - | Not null theo model | Người dùng liên quan trực tiếp đến bản ghi. |
| `RoleId` | `bigint` | FK | - | Not null theo model | Thuộc tính nghiệp vụ của `iam.UserRoles`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ScopeType` | `varchar(20)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `iam.UserRoles`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ScopeId` | `bigint` | - | - | Nullable | Thuộc tính nghiệp vụ của `iam.UserRoles`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ValidFromUtc` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `iam.UserRoles`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ValidToUtc` | `nvarchar(500)` | - | - | Nullable | Thuộc tính nghiệp vụ của `iam.UserRoles`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** `PK(UserId,RoleId,ScopeType,ScopeId)`, `IX(RoleId)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-07 - `iam.RolePermissions`

**Mục đích:** Bảng nối role-permission

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `RoleId` | `bigint` | FK | - | Not null theo model | Thuộc tính nghiệp vụ của `iam.RolePermissions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `PermissionId` | `bigint` | FK | - | Not null theo model | Thuộc tính nghiệp vụ của `iam.RolePermissions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** `PK(RoleId,PermissionId)`, `IX(PermissionId)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-08 - `iam.Sessions`

**Mục đích:** Refresh rotation và reuse detection

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `UserId` | `bigint` | FK | - | Not null theo model | Người dùng liên quan trực tiếp đến bản ghi. |
| `TokenFamilyId` | `uniqueidentifier` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `iam.Sessions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `RefreshTokenHash` | `binary(32)` | - | - | Not null theo model | Giá trị băm một chiều để so khớp/kiểm tra; không thể dùng khôi phục dữ liệu gốc. |
| `ExpiresAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `RevokedAtUtc` | `datetime2(3)` | - | - | Nullable | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `ReplacedById` | `bigint` | FK | - | Nullable | Actor thực hiện hành động; tham chiếu user khi áp dụng. |
| `IpHash` | `binary(32)` | - | - | Not null theo model | Giá trị băm một chiều để so khớp/kiểm tra; không thể dùng khôi phục dữ liệu gốc. |
| `UserAgentHash` | `binary(32)` | - | - | Not null theo model | Giá trị băm một chiều để so khớp/kiểm tra; không thể dùng khôi phục dữ liệu gốc. |

**Index:** `UQ(RefreshTokenHash)`, `IX(UserId,RevokedAtUtc)`, `IX(TokenFamilyId)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-09 - `iam.Devices`

**Mục đích:** Thiết bị và push registration

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `UserId` | `bigint` | FK | - | Not null theo model | Người dùng liên quan trực tiếp đến bản ghi. |
| `DeviceKeyHash` | `binary(32)` | - | - | Not null theo model | Giá trị băm một chiều để so khớp/kiểm tra; không thể dùng khôi phục dữ liệu gốc. |
| `Name` | `nvarchar(500)` | - | - | Not null theo model | Tên hiển thị, Unicode, được trim và giới hạn độ dài. |
| `Platform` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `iam.Devices`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `PushTokenEncrypted` | `varbinary(max)` | - | - | Not null theo model | Dữ liệu nhạy cảm đã mã hóa; không dùng trực tiếp trong log hoặc index bản rõ. |
| `RiskStatus` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `iam.Devices`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `LastSeenAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |

**Index:** `UQ(UserId,DeviceKeyHash)`, `IX(UserId,LastSeenAtUtc)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-10 - `iam.MfaMethods`

**Mục đích:** TOTP/recovery; không lưu code bản rõ

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `UserId` | `bigint` | FK | - | Not null theo model | Người dùng liên quan trực tiếp đến bản ghi. |
| `Type` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `iam.MfaMethods`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `SecretEncrypted` | `varbinary` | - | - | Not null theo model | Dữ liệu nhạy cảm đã mã hóa; không dùng trực tiếp trong log hoặc index bản rõ. |
| `IsVerified` | `bit` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `iam.MfaMethods`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `RecoveryCodeHash` | `binary(32)` | - | - | Nullable | Giá trị băm một chiều để so khớp/kiểm tra; không thể dùng khôi phục dữ liệu gốc. |
| `UsedAtUtc` | `datetime2(3)` | - | - | Nullable | Thời điểm UTC của sự kiện được nêu bởi tên cột. |

**Index:** `IX(UserId,Type,IsVerified)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-11 - `crm.Customers`

**Mục đích:** Lead/customer aggregate

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `PublicId` | `uniqueidentifier` | - | - | Not null theo model | UUID public dùng trong URL và integration. |
| `TenantId` | `bigint` | FK | - | Not null theo model | Tenant sở hữu bản ghi; luôn tham gia authorization scope. |
| `OwnerUserId` | `bigint` | FK | - | Nullable | Thuộc tính nghiệp vụ của `crm.Customers`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `TerritoryId` | `bigint` | FK | - | Nullable | Thuộc tính nghiệp vụ của `crm.Customers`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Type` | `varchar(20)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `crm.Customers`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `FullName` | `nvarchar(500)` | - | - | Not null theo model | Tên hiển thị, Unicode, được trim và giới hạn độ dài. |
| `PhoneEncrypted` | `varbinary(max)` | - | - | Not null theo model | Dữ liệu nhạy cảm đã mã hóa; không dùng trực tiếp trong log hoặc index bản rõ. |
| `PhoneHash` | `binary(32)` | - | - | Not null theo model | Giá trị băm một chiều để so khớp/kiểm tra; không thể dùng khôi phục dữ liệu gốc. |
| `EmailEncrypted` | `varbinary(max)` | - | - | Not null theo model | Dữ liệu nhạy cảm đã mã hóa; không dùng trực tiếp trong log hoặc index bản rõ. |
| `StatusCode` | `varchar(100)` | - | - | Not null theo model | Mã machine-readable thuộc catalog/allowlist, ổn định cho API và báo cáo. |
| `SourceCode` | `varchar(100)` | - | - | Not null theo model | Mã machine-readable thuộc catalog/allowlist, ổn định cho API và báo cáo. |
| `Address` | `nvarchar(max)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `crm.Customers`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Latitude` | `decimal(9,6)` | - | - | Not null theo model | Dữ liệu không gian WGS84; validate bounds và xử lý theo quyền GPS/location. |
| `Longitude` | `decimal(9,6)` | - | - | Not null theo model | Dữ liệu không gian WGS84; validate bounds và xử lý theo quyền GPS/location. |
| `Geo` | `geography` | - | - | Not null theo model | Dữ liệu không gian WGS84; validate bounds và xử lý theo quyền GPS/location. |
| `Score` | `decimal(5,2)` | - | - | Nullable | Thuộc tính nghiệp vụ của `crm.Customers`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** `UQ(TenantId,PhoneHash) WHERE IsDeleted=0`; `IX(OwnerUserId,StatusCode)`; spatial `SIX(Geo)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-12 - `crm.CustomerStatusHistory`

**Mục đích:** Lịch sử pipeline bất biến

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `CustomerId` | `bigint` | FK | - | Not null theo model | Khách hàng/lead cha của dữ liệu nghiệp vụ. |
| `FromStatus` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `crm.CustomerStatusHistory`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ToStatus` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `crm.CustomerStatusHistory`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ReasonCode` | `varchar(100)` | - | - | Nullable | Mã machine-readable thuộc catalog/allowlist, ổn định cho API và báo cáo. |
| `ReasonText` | `nvarchar(max)` | - | - | Nullable | Thuộc tính nghiệp vụ của `crm.CustomerStatusHistory`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ChangedBy` | `nvarchar(500)` | FK | - | Not null theo model | Actor thực hiện hành động; tham chiếu user khi áp dụng. |
| `ChangedAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |

**Index:** `IX(CustomerId,ChangedAtUtc DESC)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-13 - `crm.CustomerAssignments`

**Mục đích:** Ownership history

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `CustomerId` | `bigint` | FK | - | Not null theo model | Khách hàng/lead cha của dữ liệu nghiệp vụ. |
| `UserId` | `bigint` | FK | - | Not null theo model | Người dùng liên quan trực tiếp đến bản ghi. |
| `AssignedBy` | `nvarchar(500)` | FK | - | Not null theo model | Actor thực hiện hành động; tham chiếu user khi áp dụng. |
| `Reason` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `crm.CustomerAssignments`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `StartedAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `EndedAtUtc` | `datetime2(3)` | - | - | Nullable | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `IsPrimary` | `bit` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `crm.CustomerAssignments`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** filtered `UQ(CustomerId) WHERE IsPrimary=1 AND EndedAtUtc IS NULL`; `IX(UserId,EndedAtUtc)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-14 - `crm.CustomerMergeMap`

**Mục đích:** Redirect/audit merge

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `DuplicateCustomerId` | `bigint` | PK | - | Not null theo model | Thuộc tính nghiệp vụ của `crm.CustomerMergeMap`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `SurvivorCustomerId` | `bigint` | FK | - | Not null theo model | Thuộc tính nghiệp vụ của `crm.CustomerMergeMap`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `MergedBy` | `nvarchar(500)` | - | - | Not null theo model | Actor thực hiện hành động; tham chiếu user khi áp dụng. |
| `MergedAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `DecisionJson` | `nvarchar(max)` | - | - | CK ISJSON; Not null theo model | JSON có schema version và `ISJSON`; application validate schema trước lưu. |

**Index:** `IX(SurvivorCustomerId)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-15 - `crm.Interactions`

**Mục đích:** Call/message/meeting/note

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `CustomerId` | `bigint` | FK | - | Not null theo model | Khách hàng/lead cha của dữ liệu nghiệp vụ. |
| `ActorUserId` | `bigint` | FK | - | Not null theo model | Thuộc tính nghiệp vụ của `crm.Interactions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Type` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `crm.Interactions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Outcome` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `crm.Interactions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `OccurredAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `ContentEncrypted` | `varbinary(max)` | - | - | Not null theo model | Dữ liệu nhạy cảm đã mã hóa; không dùng trực tiếp trong log hoặc index bản rõ. |
| `NextActionAtUtc` | `datetime2(3)` | - | - | Nullable | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `Source` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `crm.Interactions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ClientCommandId` | `uniqueidentifier` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `crm.Interactions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** `UQ(ClientCommandId)`; `IX(CustomerId,OccurredAtUtc DESC)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-16 - `crm.Territories`

**Mục đích:** Polygon địa bàn versioned

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `TenantId` | `bigint` | FK | - | Not null theo model | Tenant sở hữu bản ghi; luôn tham gia authorization scope. |
| `DepartmentId` | `bigint` | FK | - | Not null theo model | Đơn vị tổ chức chịu trách nhiệm hoặc sở hữu. |
| `Code` | `varchar(100)` | - | - | Not null theo model | Mã machine-readable thuộc catalog/allowlist, ổn định cho API và báo cáo. |
| `Name` | `nvarchar(500)` | - | - | Not null theo model | Tên hiển thị, Unicode, được trim và giới hạn độ dài. |
| `Boundary` | `geography` | - | - | Not null theo model | Dữ liệu không gian WGS84; validate bounds và xử lý theo quyền GPS/location. |
| `Version` | `int` | - | - | Not null theo model | Phiên bản dùng audit, concurrency hoặc effective configuration. |
| `EffectiveFromUtc` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `crm.Territories`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `EffectiveToUtc` | `nvarchar(500)` | - | - | Nullable | Thuộc tính nghiệp vụ của `crm.Territories`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** `UQ(TenantId,Code,Version)`; spatial `SIX(Boundary)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-17 - `crm.Contracts`

**Mục đích:** Metadata hợp đồng, không phải billing

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `CustomerId` | `bigint` | FK | - | Not null theo model | Khách hàng/lead cha của dữ liệu nghiệp vụ. |
| `ExternalReference` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `crm.Contracts`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `PackageCode` | `varchar(100)` | - | - | Not null theo model | Mã machine-readable thuộc catalog/allowlist, ổn định cho API và báo cáo. |
| `Value` | `decimal(19,4)` | - | 0 CK >=0 | CK >=0; Not null theo model | Thuộc tính nghiệp vụ của `crm.Contracts`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Status` | `varchar(100)` | - | - | Not null theo model | Trạng thái hiện tại, bị giới hạn bởi state machine/check constraint. |
| `SignedAtUtc` | `datetime2(3)` | - | - | Nullable | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `Version` | `int` | - | - | Not null theo model | Phiên bản dùng audit, concurrency hoặc effective configuration. |

**Index:** `UQ(CustomerId,ExternalReference)`; `IX(Status,SignedAtUtc)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-18 - `field.Visits`

**Mục đích:** Lịch gặp/khảo sát

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `CustomerId` | `bigint` | FK | - | Not null theo model | Khách hàng/lead cha của dữ liệu nghiệp vụ. |
| `AssigneeUserId` | `bigint` | FK | - | Not null theo model | Thuộc tính nghiệp vụ của `field.Visits`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Purpose` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.Visits`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ScheduledStartUtc` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.Visits`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ScheduledEndUtc` | `CK` | - | - | CK end>start; Not null theo model | Thuộc tính nghiệp vụ của `field.Visits`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `GeofenceRadiusM` | `int` | - | 100 CK 20..1000 | CK 20..1000; Not null theo model | Dữ liệu không gian WGS84; validate bounds và xử lý theo quyền GPS/location. |
| `Status` | `varchar(100)` | - | - | Not null theo model | Trạng thái hiện tại, bị giới hạn bởi state machine/check constraint. |
| `Outcome` | `varchar(100)` | - | - | Nullable | Thuộc tính nghiệp vụ của `field.Visits`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Version` | `int` | - | - | Not null theo model | Phiên bản dùng audit, concurrency hoặc effective configuration. |

**Index:** `IX(AssigneeUserId,ScheduledStartUtc)`; `IX(CustomerId,Status)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-19 - `field.VisitNotes`

**Mục đích:** Note riêng của visit

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `VisitId` | `bigint` | FK | - | Not null theo model | Visit cha của check-in, note hoặc kết quả. |
| `AuthorUserId` | `bigint` | FK | - | Not null theo model | Thuộc tính nghiệp vụ của `field.VisitNotes`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ContentEncrypted` | `varbinary(max)` | - | - | Not null theo model | Dữ liệu nhạy cảm đã mã hóa; không dùng trực tiếp trong log hoặc index bản rõ. |
| `OccurredAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `ClientCommandId` | `uniqueidentifier` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.VisitNotes`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** `UQ(ClientCommandId)`; `IX(VisitId,OccurredAtUtc)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-20 - `field.CheckIns`

**Mục đích:** Bằng chứng check-in/out

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `VisitId` | `bigint` | FK | - | Not null theo model | Visit cha của check-in, note hoặc kết quả. |
| `UserId` | `bigint` | FK | - | Not null theo model | Người dùng liên quan trực tiếp đến bản ghi. |
| `Type` | `CK` | - | - | CK IN/OUT; Not null theo model | Thuộc tính nghiệp vụ của `field.CheckIns`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ClientOccurredAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `ServerReceivedAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `Latitude` | `decimal(9,6)` | - | - | Not null theo model | Dữ liệu không gian WGS84; validate bounds và xử lý theo quyền GPS/location. |
| `Longitude` | `decimal(9,6)` | - | - | Not null theo model | Dữ liệu không gian WGS84; validate bounds và xử lý theo quyền GPS/location. |
| `Geo` | `geography` | - | - | Not null theo model | Dữ liệu không gian WGS84; validate bounds và xử lý theo quyền GPS/location. |
| `AccuracyM` | `decimal(8,2)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.CheckIns`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `DistanceM` | `decimal` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.CheckIns`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `MockFlag` | `bit` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.CheckIns`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Status` | `varchar(100)` | - | - | Not null theo model | Trạng thái hiện tại, bị giới hạn bởi state machine/check constraint. |
| `ReviewReason` | `nvarchar(500)` | - | - | Nullable | Thuộc tính nghiệp vụ của `field.CheckIns`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ClientCommandId` | `uniqueidentifier` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.CheckIns`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** `UQ(ClientCommandId)`; `IX(VisitId,Type)`; `IX(UserId,ServerReceivedAtUtc)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-21 - `field.RouteSessions`

**Mục đích:** Phiên tracking theo ca

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `UserId` | `bigint` | FK | - | Not null theo model | Người dùng liên quan trực tiếp đến bản ghi. |
| `DeviceId` | `bigint` | FK | - | Not null theo model | Thuộc tính nghiệp vụ của `field.RouteSessions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ShiftStartUtc` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.RouteSessions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ShiftEndUtc` | `nvarchar(500)` | - | - | Nullable | Thuộc tính nghiệp vụ của `field.RouteSessions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ConsentVersion` | `int` | - | - | Not null theo model | Phiên bản dùng audit, concurrency hoặc effective configuration. |
| `Status` | `varchar(100)` | - | - | Not null theo model | Trạng thái hiện tại, bị giới hạn bởi state machine/check constraint. |
| `DistanceM` | `decimal(12,2)` | - | 0 | Not null theo model | Thuộc tính nghiệp vụ của `field.RouteSessions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `PointCount` | `int` | - | 0 | Not null theo model | Thuộc tính nghiệp vụ của `field.RouteSessions`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** filtered `UQ(UserId) WHERE Status='Active'`; `IX(UserId,ShiftStartUtc DESC)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-22 - `field.RoutePoints`

**Mục đích:** GPS raw volume lớn

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | - | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `SessionId` | `bigint` | FK | - | Not null theo model | Route/session cha; dùng partition và truy vấn tuyến. |
| `SequenceNo` | `int` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.RoutePoints`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `OccurredAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `ReceivedAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `Latitude` | `decimal(9,6)` | - | - | Not null theo model | Dữ liệu không gian WGS84; validate bounds và xử lý theo quyền GPS/location. |
| `Longitude` | `decimal(9,6)` | - | - | Not null theo model | Dữ liệu không gian WGS84; validate bounds và xử lý theo quyền GPS/location. |
| `Geo` | `geography` | - | - | Not null theo model | Dữ liệu không gian WGS84; validate bounds và xử lý theo quyền GPS/location. |
| `AccuracyM` | `decimal` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.RoutePoints`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `SpeedMps` | `decimal` | - | - | Nullable | Thuộc tính nghiệp vụ của `field.RoutePoints`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Heading` | `decimal` | - | - | Nullable | Thuộc tính nghiệp vụ của `field.RoutePoints`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `QualityCode` | `varchar(100)` | - | - | Not null theo model | Mã machine-readable thuộc catalog/allowlist, ổn định cho API và báo cáo. |
| `Source` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.RoutePoints`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `OccurredDate` | `date` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.RoutePoints`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** clustered `PK(OccurredDate,Id)`; `UQ(SessionId,SequenceNo,OccurredDate)`; spatial index tùy partition

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-23 - `field.RouteSummaries`

**Mục đích:** Read model tuyến đã simplify

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `SessionId` | `bigint` | PK | - | Not null theo model | Route/session cha; dùng partition và truy vấn tuyến. |
| `PolylineEncoded` | `nvarchar(max)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.RouteSummaries`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `DistanceM` | `decimal` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.RouteSummaries`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `MovingSeconds` | `int` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.RouteSummaries`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `StoppedSeconds` | `int` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.RouteSummaries`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `GapCount` | `int` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `field.RouteSummaries`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `CalculatedAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `AlgorithmVersion` | `int` | - | - | Not null theo model | Phiên bản dùng audit, concurrency hoặc effective configuration. |

**Index:** `IX(CalculatedAtUtc)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-24 - `ops.Handoffs`

**Mục đích:** Bàn giao có snapshot

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `ContractId` | `bigint` | FK | - | Not null theo model | Thuộc tính nghiệp vụ của `ops.Handoffs`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `SubmittedBy` | `nvarchar(500)` | FK | - | Not null theo model | Actor thực hiện hành động; tham chiếu user khi áp dụng. |
| `Status` | `varchar(100)` | - | - | Not null theo model | Trạng thái hiện tại, bị giới hạn bởi state machine/check constraint. |
| `ChecklistJson` | `CK` | - | - | CK ISJSON; Not null theo model | JSON có schema version và `ISJSON`; application validate schema trước lưu. |
| `SnapshotJson` | `CK` | - | - | CK ISJSON; Not null theo model | JSON có schema version và `ISJSON`; application validate schema trước lưu. |
| `RequestedWindowStartUtc` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `ops.Handoffs`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `RequestedWindowEndUtc` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `ops.Handoffs`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ReviewedBy` | `nvarchar(500)` | - | - | Nullable | Actor thực hiện hành động; tham chiếu user khi áp dụng. |
| `ReviewReason` | `nvarchar(500)` | - | - | Nullable | Thuộc tính nghiệp vụ của `ops.Handoffs`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** `IX(Status,RequestedWindowStartUtc)`; `UQ(ContractId) WHERE active`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-25 - `ops.WorkOrders`

**Mục đích:** Công việc kỹ thuật/revisit

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `HandoffId` | `bigint` | FK | - | Not null theo model | Thuộc tính nghiệp vụ của `ops.WorkOrders`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ParentWorkOrderId` | `bigint` | FK | - | Nullable | Thuộc tính nghiệp vụ của `ops.WorkOrders`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `AssigneeUserId` | `bigint` | FK | - | Nullable | Thuộc tính nghiệp vụ của `ops.WorkOrders`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Status` | `varchar(100)` | - | - | Not null theo model | Trạng thái hiện tại, bị giới hạn bởi state machine/check constraint. |
| `Priority` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `ops.WorkOrders`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ScheduledStartUtc` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `ops.WorkOrders`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `SlaDueAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `FailureReasonCode` | `varchar(100)` | - | - | Nullable | Mã machine-readable thuộc catalog/allowlist, ổn định cho API và báo cáo. |
| `CompletedAtUtc` | `datetime2(3)` | - | - | Nullable | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `Version` | `int` | - | - | Not null theo model | Phiên bản dùng audit, concurrency hoặc effective configuration. |

**Index:** `IX(AssigneeUserId,Status,ScheduledStartUtc)`; `IX(SlaDueAtUtc,Status)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-26 - `notify.Reminders`

**Mục đích:** Nhắc việc và recurrence

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `CustomerId` | `bigint` | FK | - | Nullable | Khách hàng/lead cha của dữ liệu nghiệp vụ. |
| `VisitId` | `bigint` | FK | - | Nullable | Visit cha của check-in, note hoặc kết quả. |
| `AssigneeUserId` | `bigint` | FK | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.Reminders`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Title` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.Reminders`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `DueAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `OriginalDueAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `Priority` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.Reminders`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Status` | `varchar(100)` | - | - | Not null theo model | Trạng thái hiện tại, bị giới hạn bởi state machine/check constraint. |
| `RecurrenceRule` | `nvarchar(500)` | - | - | Nullable | Thuộc tính nghiệp vụ của `notify.Reminders`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `AutomationKey` | `nvarchar(500)` | - | - | Nullable | Thuộc tính nghiệp vụ của `notify.Reminders`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `CompletedAtUtc` | `datetime2(3)` | - | - | Nullable | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `Version` | `int` | - | - | Not null theo model | Phiên bản dùng audit, concurrency hoặc effective configuration. |

**Index:** `IX(AssigneeUserId,Status,DueAtUtc)`; `UQ(AutomationKey) WHERE NOT NULL`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-27 - `notify.Notifications`

**Mục đích:** Inbox hệ thống

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `UserId` | `bigint` | FK | - | Not null theo model | Người dùng liên quan trực tiếp đến bản ghi. |
| `EventType` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.Notifications`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Title` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.Notifications`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Body` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.Notifications`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `DeepLink` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.Notifications`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Severity` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.Notifications`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `CreatedAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm tạo do server ghi ở UTC. |
| `ReadAtUtc` | `datetime2(3)` | - | - | Nullable | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `DeduplicationKey` | `uniqueidentifier` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.Notifications`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ExpiresAtUtc` | `datetime2(3)` | - | - | Nullable | Thời điểm UTC của sự kiện được nêu bởi tên cột. |

**Index:** `UQ(UserId,DeduplicationKey)`; `IX(UserId,ReadAtUtc,CreatedAtUtc DESC)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-28 - `notify.NotificationDeliveries`

**Mục đích:** Outbox/delivery tracking

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `NotificationId` | `bigint` | FK | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.NotificationDeliveries`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Channel` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.NotificationDeliveries`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ProviderMessageId` | `bigint` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.NotificationDeliveries`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Status` | `varchar(100)` | - | - | Not null theo model | Trạng thái hiện tại, bị giới hạn bởi state machine/check constraint. |
| `AttemptCount` | `DF` | - | 0 | Not null theo model | Thuộc tính nghiệp vụ của `notify.NotificationDeliveries`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `NextAttemptAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `DeliveredAtUtc` | `datetime2(3)` | - | - | Nullable | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `ErrorCode` | `varchar(100)` | - | - | Nullable | Mã machine-readable thuộc catalog/allowlist, ổn định cho API và báo cáo. |

**Index:** `IX(Status,NextAttemptAtUtc)`; `UQ(NotificationId,Channel)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-29 - `notify.Preferences`

**Mục đích:** Notification preference

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `UserId` | `bigint` | FK | - | Not null theo model | Người dùng liên quan trực tiếp đến bản ghi. |
| `EventType` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.Preferences`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Channel` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.Preferences`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Enabled` | `bit` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.Preferences`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `QuietStart` | `nvarchar(500)` | - | - | Nullable | Thuộc tính nghiệp vụ của `notify.Preferences`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `QuietEnd` | `nvarchar(500)` | - | - | Nullable | Thuộc tính nghiệp vụ của `notify.Preferences`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Timezone` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `notify.Preferences`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** `PK(UserId,EventType,Channel)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-30 - `core.Files`

**Mục đích:** Blob metadata

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `StorageProvider` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `core.Files`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `StorageKey` | `UQ` | - | - | Unique; Not null theo model | Thuộc tính nghiệp vụ của `core.Files`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `OriginalName` | `nvarchar(500)` | - | - | Not null theo model | Tên hiển thị, Unicode, được trim và giới hạn độ dài. |
| `ContentType` | `nvarchar(max)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `core.Files`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `SizeBytes` | `CK` | - | - | CK >0; Not null theo model | Thuộc tính nghiệp vụ của `core.Files`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Sha256` | `binary(32)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `core.Files`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ScanStatus` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `core.Files`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `UploadedBy` | `nvarchar(500)` | - | - | Not null theo model | Actor thực hiện hành động; tham chiếu user khi áp dụng. |
| `ExpiresAtUtc` | `datetime2(3)` | - | - | Nullable | Thời điểm UTC của sự kiện được nêu bởi tên cột. |

**Index:** `UQ(StorageKey)`; `IX(Sha256)`; `IX(ScanStatus)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-31 - `core.Attachments`

**Mục đích:** Polymorphic link được Application validate

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `FileId` | `bigint` | FK | - | Not null theo model | Thuộc tính nghiệp vụ của `core.Attachments`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `EntityType` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `core.Attachments`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `EntityId` | `bigint` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `core.Attachments`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Category` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `core.Attachments`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Caption` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `core.Attachments`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `IsPrimary` | `DF` | - | 0 | Not null theo model | Thuộc tính nghiệp vụ của `core.Attachments`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** `IX(EntityType,EntityId)`; `IX(FileId)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-32 - `audit.AuditLogs`

**Mục đích:** Append-only, hash chained

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `TenantId` | `bigint` | - | - | Not null theo model | Tenant sở hữu bản ghi; luôn tham gia authorization scope. |
| `ActorUserId` | `bigint` | - | - | Nullable | Thuộc tính nghiệp vụ của `audit.AuditLogs`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `Action` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `audit.AuditLogs`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ResourceType` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `audit.AuditLogs`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ResourceId` | `bigint` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `audit.AuditLogs`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `OccurredAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `TraceId` | `bigint` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `audit.AuditLogs`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `IpHash` | `binary(32)` | - | - | Not null theo model | Giá trị băm một chiều để so khớp/kiểm tra; không thể dùng khôi phục dữ liệu gốc. |
| `BeforeJson` | `nvarchar(max)` | - | - | Nullable | JSON có schema version và `ISJSON`; application validate schema trước lưu. |
| `AfterJson` | `nvarchar(max)` | - | - | Nullable | JSON có schema version và `ISJSON`; application validate schema trước lưu. |
| `PrevHash` | `binary(32)` | - | - | Not null theo model | Giá trị băm một chiều để so khớp/kiểm tra; không thể dùng khôi phục dữ liệu gốc. |
| `EntryHash` | `binary(32)` | - | - | Not null theo model | Giá trị băm một chiều để so khớp/kiểm tra; không thể dùng khôi phục dữ liệu gốc. |

**Index:** `IX(ResourceType,ResourceId,OccurredAtUtc)`; `IX(ActorUserId,OccurredAtUtc)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-33 - `core.Settings`

**Mục đích:** Typed config; secret nằm vault

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `TenantId` | `bigint` | FK | - | Not null theo model | Tenant sở hữu bản ghi; luôn tham gia authorization scope. |
| `Key` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `core.Settings`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ValueJson` | `CK` | - | - | CK ISJSON; Not null theo model | JSON có schema version và `ISJSON`; application validate schema trước lưu. |
| `SchemaVersion` | `int` | - | - | Not null theo model | Phiên bản dùng audit, concurrency hoặc effective configuration. |
| `EffectiveFromUtc` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `core.Settings`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `EffectiveToUtc` | `nvarchar(500)` | - | - | Nullable | Thuộc tính nghiệp vụ của `core.Settings`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `IsSecret` | `DF` | - | 0 CK IsSecret=0 | CK IsSecret=0; Not null theo model | Thuộc tính nghiệp vụ của `core.Settings`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** `UQ(TenantId,Key,EffectiveFromUtc)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-34 - `core.BackgroundJobs`

**Mục đích:** Job bền vững/outbox nội bộ

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | - | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `Type` | `varchar(100)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `core.BackgroundJobs`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `PayloadJson` | `nvarchar(max)` | - | - | Not null theo model | JSON có schema version và `ISJSON`; application validate schema trước lưu. |
| `Status` | `varchar(100)` | - | - | Not null theo model | Trạng thái hiện tại, bị giới hạn bởi state machine/check constraint. |
| `Attempts` | `int` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `core.BackgroundJobs`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ScheduledAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm UTC của sự kiện được nêu bởi tên cột. |
| `LockedUntilUtc` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `core.BackgroundJobs`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `LastError` | `nvarchar(max)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `core.BackgroundJobs`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `DeduplicationKey` | `uniqueidentifier` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `core.BackgroundJobs`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |

**Index:** `IX(Status,ScheduledAtUtc)`; `UQ(DeduplicationKey)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-35 - `ai.AiRuns`

**Mục đích:** AI lineage, cost, evaluation

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `CustomerId` | `bigint` | FK | - | Nullable | Khách hàng/lead cha của dữ liệu nghiệp vụ. |
| `RequestedBy` | `nvarchar(500)` | FK | - | Not null theo model | Actor thực hiện hành động; tham chiếu user khi áp dụng. |
| `UseCase` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `ai.AiRuns`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ModelProvider` | `nvarchar(500)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `ai.AiRuns`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ModelName` | `nvarchar(500)` | - | - | Not null theo model | Tên hiển thị, Unicode, được trim và giới hạn độ dài. |
| `PromptVersion` | `int` | - | - | Not null theo model | Phiên bản dùng audit, concurrency hoặc effective configuration. |
| `InputHash` | `binary(32)` | - | - | Not null theo model | Giá trị băm một chiều để so khớp/kiểm tra; không thể dùng khôi phục dữ liệu gốc. |
| `OutputJson` | `nvarchar(max)` | - | - | Not null theo model | JSON có schema version và `ISJSON`; application validate schema trước lưu. |
| `Status` | `varchar(100)` | - | - | Not null theo model | Trạng thái hiện tại, bị giới hạn bởi state machine/check constraint. |
| `Confidence` | `decimal` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `ai.AiRuns`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `TokenIn` | `int` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `ai.AiRuns`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `TokenOut` | `int` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `ai.AiRuns`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `LatencyMs` | `int` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `ai.AiRuns`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `CreatedAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm tạo do server ghi ở UTC. |

**Index:** `IX(CustomerId,CreatedAtUtc DESC)`; `IX(UseCase,Status)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

## DB-36 - `ai.AiFeedback`

**Mục đích:** Human feedback

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
| `Id` | `bigint` | PK | - | Not null theo model | Khóa định danh nội bộ, không expose trực tiếp ra API. |
| `AiRunId` | `bigint` | FK | - | Not null theo model | Thuộc tính nghiệp vụ của `ai.AiFeedback`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `UserId` | `bigint` | FK | - | Not null theo model | Người dùng liên quan trực tiếp đến bản ghi. |
| `Rating` | `smallint` | - | - | CK -1..1; Not null theo model | Thuộc tính nghiệp vụ của `ai.AiFeedback`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `ReasonCode` | `varchar(100)` | - | - | Not null theo model | Mã machine-readable thuộc catalog/allowlist, ổn định cho API và báo cáo. |
| `Comment` | `nvarchar(max)` | - | - | Not null theo model | Thuộc tính nghiệp vụ của `ai.AiFeedback`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu. |
| `CreatedAtUtc` | `datetime2(3)` | - | - | Not null theo model | Thời điểm tạo do server ghi ở UTC. |

**Index:** `UQ(AiRunId,UserId)`; `IX(ReasonCode)`

**Quy tắc bổ sung**

- Cột FK dùng `NO ACTION` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và `RowVersion` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.

