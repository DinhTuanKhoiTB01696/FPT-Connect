---
name: fpt-connect
description: Quy chuẩn kỹ thuật và triết lý sản phẩm FPT Connect (CRM + GPS + Field Service). Đọc trước khi code bất kỳ phần nào của dự án này.
---

# Skill: FPT Connect

Skill này gói toàn bộ "cách làm đúng" của FPT Connect. Khi làm việc trong repo này, agent phải tuân theo các file kèm theo: `ARCHITECTURE.md`, `DATABASE.md`, `API_GUIDE.md`, `UI_GUIDE.md`, `DESIGN.md`, `CODING_STYLE.md`, `TESTING.md`, `RULES.md`, `SPRINT_RULES.md`, `SECURITY.md`, `PERFORMANCE.md`, `DEPLOYMENT.md`, `DEVOPS.md`, `GIT_WORKFLOW.md`.

## Triết lý sản phẩm

- **Enterprise** — thiết kế cho 20.000 user, multi-tenant-ready, bảo mật ASVS L2, audit bất biến. Không "demo-grade".
- **Minimal** — bề mặt nhỏ, rõ ràng; ít phụ thuộc; ít khái niệm; mỗi thứ có một lý do tồn tại.
- **Premium** — chất lượng cảm nhận cao: UI tinh, micro-interaction có chủ đích, copy tiếng Việt chuẩn, không "generic AI look".
- **Scalable** — partition dữ liệu lớn (GPS), read-model cho dashboard, cache Redis, outbox cho cross-aggregate, stateless API sau load balancer.
- **Production-ready** — build xanh, test đạt, migration có rollback, observability đầy đủ, không placeholder/TODO.

## Khi nào dùng

Mọi task trong repo FPT Connect: thêm endpoint, bảng, màn hình, sửa lỗi, viết test, review. Luôn neo về `docs/Project-Bible/` làm nguồn sự thật và giữ truy vết `FR → UC → API → DB → TC`.

## Nguyên tắc vàng

1. Đọc tài liệu trước, code sau. Không bịa API/bảng/business rule.
2. Đúng phạm vi sprint; một feature một lúc; không refactor ngoài phạm vi.
3. Clean Architecture là bất khả xâm phạm.
4. Bảo mật và quyền theo scope mặc định bật; deny-by-default.
5. Tự verify (build/lint/test) trước khi tuyên bố hoàn thành.
