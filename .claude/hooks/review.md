# hook: review — Cổng review PR

Trước khi coi PR là sẵn sàng merge:
- Truy vết `FR→UC→API→DB→TC` đầy đủ (thiếu mắt xích → không merge).
- Build/lint/test xanh; không lỗi Critical/High.
- Đúng phạm vi sprint; không refactor ngoài phạm vi; không placeholder/TODO.
- Migration có rollback; observability cập nhật; PII masked; không commit secret.
- UI: Taste Skill compliance + WCAG AA + dark mode + 360px.
Người duyệt khác người viết khi policy four-eyes bật. Human nghiệm thu acceptance → merge.
