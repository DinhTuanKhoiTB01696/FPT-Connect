-- FPT Connect - Seed data ban dau (Sprint 0)
-- Chay sau khi 'dotnet ef database update'. Idempotent: chi insert khi chua co.
SET NOCOUNT ON;

-- Roles
IF NOT EXISTS (SELECT 1 FROM iam.Roles WHERE Code = 'ADMIN')
    INSERT INTO iam.Roles (PublicId, Code, Name, IsSystem, [Version], CreatedAtUtc, IsDeleted)
    VALUES (NEWID(), 'ADMIN', 'Administrator', 1, 1, SYSUTCDATETIME(), 0);

IF NOT EXISTS (SELECT 1 FROM iam.Roles WHERE Code = 'SALE')
    INSERT INTO iam.Roles (PublicId, Code, Name, IsSystem, [Version], CreatedAtUtc, IsDeleted)
    VALUES (NEWID(), 'SALE', 'Sale', 1, 1, SYSUTCDATETIME(), 0);

-- Admin user (mat khau: Admin@12345 - BCrypt hash workFactor 12)
IF NOT EXISTS (SELECT 1 FROM iam.Users WHERE EmailNormalized = 'admin@fptconnect.vn')
    INSERT INTO iam.Users (PublicId, EmployeeCode, FullName, EmailNormalized, PasswordHash, Status, CreatedAtUtc, IsDeleted)
    VALUES (NEWID(), 'EMP0001', N'Quan tri vien', 'admin@fptconnect.vn',
            '$2b$12$AhA1/P33ue.91Ua5NC.4Iu7qKR9zVVKVov45SjYvXSev39b1gReKy', 'Active', SYSUTCDATETIME(), 0);

-- Gan role ADMIN cho admin user
IF NOT EXISTS (
    SELECT 1 FROM iam.UserRoles ur
    JOIN iam.Users u ON u.Id = ur.UserId
    JOIN iam.Roles r ON r.Id = ur.RoleId
    WHERE u.EmailNormalized = 'admin@fptconnect.vn' AND r.Code = 'ADMIN')
INSERT INTO iam.UserRoles (UserId, RoleId)
SELECT u.Id, r.Id FROM iam.Users u CROSS JOIN iam.Roles r
WHERE u.EmailNormalized = 'admin@fptconnect.vn' AND r.Code = 'ADMIN';

-- Customers mau
IF NOT EXISTS (SELECT 1 FROM crm.Customers WHERE FullName = N'Nguyen Van An')
    INSERT INTO crm.Customers (PublicId, FullName, PhoneE164, StatusCode, SourceCode, Address, CreatedAtUtc, IsDeleted)
    VALUES
    (NEWID(), N'Nguyen Van An', '+84901234567', 'Qualified', 'FIELD_SURVEY', N'12 Nguyen Hue, Quan 1, TP.HCM', SYSUTCDATETIME(), 0),
    (NEWID(), N'Tran Thi Binh', '+84908887766', 'New', 'HOTLINE', N'45 Le Loi, Quan 1, TP.HCM', SYSUTCDATETIME(), 0),
    (NEWID(), N'Le Hoang Cuong', '+84912345678', 'Contacted', 'REFERRAL', N'88 Vo Van Tan, Quan 3, TP.HCM', SYSUTCDATETIME(), 0);

PRINT 'Seed data hoan tat.';
