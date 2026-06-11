# FPT Connect — Frontend Design Guide

Tài liệu này gắn nguyên tắc thiết kế của Project Bible (ch.09, ch.10) với cách dùng bộ **taste-skill** để frontend đẹp, nhất quán và "không generic".

## 1. Dùng taste-skill cho agent code

`taste-skill` (https://github.com/Leonxlnx/taste-skill) là bộ Agent Skills (file `SKILL.md`) nâng chất lượng UI do AI dựng. Cài cho agent code (Claude Code/Codex/Cursor):

```bash
npx skills add https://github.com/Leonxlnx/taste-skill
# hoặc một skill cụ thể
npx skills add https://github.com/Leonxlnx/taste-skill --skill "design-taste-frontend"
```

Khuyến nghị cho FPT Connect (app nghiệp vụ, mật độ dữ liệu cao):

- Dùng `minimalist-ui` (vibe Notion/Linear) làm hướng chủ đạo cho CRM dashboard.
- Dùng `high-end-visual-design` cho các màn marketing/login.
- Ba "dial" của taste-skill nên đặt: `DESIGN_VARIANCE` thấp–trung (giao diện nghiệp vụ cần rõ ràng), `MOTION_INTENSITY` thấp (tránh animate marker GPS gây sai nhận thức — Bible 10.7), `VISUAL_DENSITY` cao (dashboard nhiều thông tin).

## 2. Ràng buộc bắt buộc từ Project Bible (không được vi phạm)

- Token màu/typography/spacing theo `docs/Project-Bible/10_DesignSystem.md`; không hard-code màu trong component (đã có CSS variables trong `src/style.css`).
- Dark mode bắt buộc — mọi màu phải đọc được ở cả hai chế độ (toggle class `dark` trên `<html>`).
- Accessibility WCAG 2.2 AA: keyboard nav, focus visible, label thật, contrast AA, `prefers-reduced-motion` (Bible 9.6).
- Mobile-first: target chạm ≥ 44×44px, bottom-bar ≤ 5 mục; bản đồ luôn có list alternative.
- Trạng thái offline luôn hiển thị: "Đã lưu trên thiết bị" / "Đang đồng bộ" / "Cần xử lý".
- Phân biệt rõ "Đề xuất AI" với dữ liệu đã xác nhận.

## 3. Quy trình khi thêm màn hình mới

1. Lấy spec màn từ `09_UIUX.md` (UI-01..32) + acceptance.
2. Dựng wireframe/tham chiếu (có thể dùng `imagegen-frontend-web`/`-mobile` của taste-skill).
3. Code bằng component đã có token; chạy `npm run typecheck` và checklist component (Bible 10.8).
4. Kiểm thử accessibility + 360px + dark mode trước khi PR (DoD).
