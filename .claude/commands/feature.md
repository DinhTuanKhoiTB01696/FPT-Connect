# /feature — Thêm tính năng theo spec (trong phạm vi sprint)

1. Chạy `hooks/pre-task.md`. Xác nhận tính năng thuộc sprint hiện tại (nếu không → dừng, đề xuất xếp lịch).
2. Lấy FR/UC/API/DB/TC liên quan từ Project Bible; nếu thiếu → Change Request, không tự bịa.
3. Vertical slice: Domain → Application → Infrastructure → Api → Frontend; mỗi tầng có test.
4. UI theo `/ui`; API theo `/api`; DB theo `/database`. Chạy `hooks/post-task.md`.

Output: cập nhật `TASK_REPORT.md` + `CHANGELOG.md`.
