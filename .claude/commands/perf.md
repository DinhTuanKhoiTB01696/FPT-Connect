# /perf — Soát hiệu năng & khả năng mở rộng

Đọc `skills/fpt-connect/PERFORMANCE.md` + Bible 3.6/7.12.

Kiểm tra SLO: P95 read < 500ms / write < 800ms @300 RPS; map < 2s/2000 marker; GPS 10M điểm/ngày không backlog > 5 phút. Soát: N+1 query, index, cursor pagination, read-model cho dashboard, cache Redis, batch GPS, streaming export, circuit breaker.

1. Chạy `hooks/pre-task.md`. Đo bằng k6/EF logging khi có thể.
2. Đề xuất tối ưu kèm bằng chứng (query plan, benchmark).

Output: `PERFORMANCE_REVIEW.md`.
