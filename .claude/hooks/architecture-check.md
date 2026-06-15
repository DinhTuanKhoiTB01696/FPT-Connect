# hook: architecture-check

Xác minh trước khi merge code backend:
- Stack đúng: .NET 10 (target net10.0; nếu còn net8.0 chỉ nâng qua PR hạ tầng riêng), Vue 3 latest, SQL Server.
- Chiều phụ thuộc `Domain ← Application ← Infrastructure/Api/Worker`; Domain không tham chiếu EF/HTTP/JWT/provider.
- SOLID; Repository chỉ cho aggregate có hành vi (không `IRepository<T>` lộ `IQueryable`); Unit of Work + transactional outbox.
- SignalR authorize theo group; JWT access + refresh rotation; DI qua constructor.
- CQRS: command đổi 1 aggregate/transaction; query `AsNoTracking`.
Vi phạm → chặn, yêu cầu sửa hoặc tạo ADR.
