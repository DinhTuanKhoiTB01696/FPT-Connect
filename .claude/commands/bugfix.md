# /bugfix — Sửa tận gốc, không vá triệu chứng

1. Chạy `hooks/pre-task.md`. Tái hiện lỗi; thu thập log/trace/test thất bại.
2. Truy nguyên **root cause** (5 Whys); không sửa khi chưa hiểu nguyên nhân.
3. Viết test tái hiện lỗi (đỏ) trước; sửa tối thiểu trong đúng module; test xanh.
4. Không refactor ngoài phạm vi; chạy `hooks/post-task.md`.

Output: `ROOT_CAUSE_ANALYSIS.md` → rồi mới fix.
