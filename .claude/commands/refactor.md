# /refactor — Tái cấu trúc trong phạm vi đã chọn

1. Chạy `hooks/pre-task.md`. Xác định scope CHÍNH XÁC; KHÔNG đụng module ngoài scope.
2. Giữ hành vi không đổi (refactor = đổi cấu trúc, không đổi chức năng); có test bảo chứng trước/sau.
3. Không phá Clean Architecture, không đổi public contract nếu chưa có lý do/ADR.
4. Chạy `hooks/architecture-check.md` + `hooks/post-task.md`.

Output: diff + ghi chú lý do trong `TASK_REPORT.md`.
