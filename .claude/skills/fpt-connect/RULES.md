# RULES.md — Quy tắc dự án (project-wide)

> Bổ sung cho `.claude/AI_RULES.md` (bất biến). Đây là quy tắc vận hành hằng ngày.

## Phạm vi & nhịp làm việc

1. **One sprint at a time** — chỉ làm sprint hiện tại (xem `docs/Project-Bible/SPRINT_PLAN.md`).
2. **One feature at a time** — hoàn tất + verify một vertical slice trước khi sang việc kế.
3. **Never modify unrelated code** — không "dọn dẹp" hay đổi style file ngoài phạm vi.
4. **Đề xuất qua Change Request** nếu phát hiện thiếu API/bảng/quy tắc — không tự bịa.

## Trước khi kết thúc (Definition of Done)

5. **Always review before finishing** — đọc lại toàn bộ diff của mình.
6. **Always check build** — backend `dotnet build -c Release` xanh; frontend `npm run build` pass.
7. **Always check TypeScript** — `npm run typecheck` không lỗi.
8. **Always check lint/format** — không cảnh báo lint mới; format theo `.editorconfig`.
9. **Always run tests** — unit/integration liên quan pass; thêm test cho code mới.
10. **Traceability** — ghi mắt xích `FR → UC → API → DB → TC` trong mô tả PR.

## Dữ liệu & bảo mật

11. Soft delete, không hard delete dữ liệu nghiệp vụ.
12. Optimistic concurrency (RowVersion/ETag) cho mọi update; xử lý 409/412.
13. Kiểm tra permission + organizational scope ở Application layer và query filter.
14. Không log token/password/PII đầy đủ; không commit secret.

## Frontend

15. Dùng token thiết kế, dark mode, accessibility; theo `DESIGN.md` + `UI_GUIDE.md`.
16. Không thêm dependency UI nặng nếu chưa cần; ưu tiên component nội bộ.

## Khi mơ hồ

17. Hỏi 1 câu rõ ràng về phạm vi/dữ liệu thay vì đoán; nêu giả định nếu buộc phải tiếp tục.
