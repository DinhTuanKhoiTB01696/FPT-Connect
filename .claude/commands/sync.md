# /sync — Đồng bộ tài liệu ↔ code ↔ MEMORY

1. Chạy `/analyze` để tìm lệch tài liệu/code (không sửa code).
2. Cập nhật `MEMORY.md` (trạng thái hiện tại + decision log) và tài liệu Project Bible nếu baseline đổi (kèm ADR).
3. Đảm bảo `.claude/*` nhất quán (stack, read-order, không trùng lặp rule).

Output: cập nhật `MEMORY.md` + danh sách lệch còn tồn tại.
