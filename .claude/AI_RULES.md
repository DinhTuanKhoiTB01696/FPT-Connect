# AI_RULES.md — Quy tắc bất biến (Immutable Rules)

> Các quy tắc dưới đây KHÔNG được phá vỡ trong bất kỳ tình huống nào. Nếu một yêu cầu xung đột với chúng, AI phải dừng lại và nêu rõ xung đột thay vì âm thầm vi phạm.

## Kiến trúc & phạm vi

1. **Never break Clean Architecture** — giữ nguyên chiều phụ thuộc `Domain <- Application <- Infrastructure/Api/Worker`. Domain không tham chiếu EF/HTTP/JWT/provider.
2. **Never rename folders/projects** đã có (`FptConnect.Domain`, `Frontend`, `docs/Project-Bible`, …).
3. **Never refactor unrelated modules** — chỉ chạm tới code thuộc phạm vi task/sprint hiện tại.
4. **Never change framework/stack** — cố định: .NET 10 + ASP.NET Core + EF Core + SQL Server (backend); Vue 3 + TypeScript + Vite + Pinia + TailwindCSS + shadcn-vue (frontend); Google Maps API. Đổi phải qua ADR.
5. **Never ignore Sprint scope** — không làm việc của sprint khác, không "tiện tay" thêm tính năng.

## Chất lượng code

6. **Never create placeholder/fake code** — không hàm rỗng trả dữ liệu giả, không mock thay cho implement thật.
7. **Never leave TODO/FIXME bỏ ngỏ** — nếu chưa làm được, ghi rõ trong PR và tạo task, không nhét TODO im lặng vào code.
8. **Always build successfully** — backend `dotnet build` xanh, frontend `npm run build` + `typecheck` pass trước khi coi là xong.
9. **Never commit secret/PII** — key/secret nằm ở vault/biến môi trường; log phải mask PII.
10. **Always keep traceability** — mỗi thay đổi nối được `FR → UC → API → DB → TC`. PR thiếu mắt xích không merge.

## Dữ liệu & nghiệp vụ

11. **Never change business rules** (BR-xxx) hay schema baseline mà không có Change Request/ADR.
12. **Never hard delete** dữ liệu nghiệp vụ — dùng soft delete (`IsDeleted/DeletedAtUtc/DeletedBy/DeleteReason`).
13. **Never bypass authorization** — mọi endpoint kiểm tra permission + organizational scope, không chỉ role.

## Thiết kế

14. **Never produce generic AI layout** — không "Bootstrap look", không purple gradient, không animation thừa (xem DESIGN.md).
15. **Never replace taste-skill** — chỉ kế thừa và mở rộng triết lý của nó.

## Quy trình

16. **Always review before finishing** — tự đọc lại diff, chạy test/lint, liệt kê file đã đổi.
17. **One sprint / one feature at a time** — hoàn tất và verify trước khi sang việc kế tiếp.
18. **Ask, don't assume** khi yêu cầu mơ hồ về phạm vi, dữ liệu hoặc ảnh hưởng tới module khác.
