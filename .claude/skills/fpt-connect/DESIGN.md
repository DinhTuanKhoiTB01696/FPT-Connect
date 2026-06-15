# DESIGN.md — Ngôn ngữ thiết kế FPT Connect (kế thừa Taste Skill)

> Taste Skill (https://github.com/Leonxlnx/taste-skill) là **chuẩn thị giác mặc định**. Tài liệu này KHÔNG thay thế Taste Skill — chỉ neo nó vào ngữ cảnh FPT Connect và ràng buộc của Project Bible (ch.09, ch.10). Khi xung đột, ưu tiên: an toàn dữ liệu/Accessibility > Project Bible tokens > Taste Skill stylistic.

## Tham chiếu chất lượng (benchmark)

Mọi quyết định UI nên đạt chuẩn cảm nhận của: **Linear** (tốc độ, bàn phím, mật độ vừa), **Vercel** (sạch, tương phản, editorial), **Stripe** (tin cậy, dữ liệu rõ ràng), **Notion** (cấu trúc, khoảng trắng), **Raycast** (command-driven, micro-interaction tinh). Không sao chép pixel — kế thừa triết lý.

## Triết lý kế thừa từ Taste Skill

Minimalism · Hierarchy · Whitespace · Editorial typography · Component quality · Micro-interaction có chủ đích · cảm giác "premium SaaS". Ba "dial" của Taste Skill cho app nghiệp vụ FPT Connect:

- `DESIGN_VARIANCE`: thấp–trung (giao diện vận hành cần rõ ràng, không thử nghiệm layout quá tay).
- `MOTION_INTENSITY`: thấp (KHÔNG animate marker GPS liên tục — gây sai nhận thức; chuyển vị trí ≤ 300ms).
- `VISUAL_DENSITY`: trung–cao (dashboard nhiều thông tin nhưng có phân cấp).

Khuyến nghị mapping skill: `minimalist-ui` cho dashboard/CRM, `high-end-visual-design` cho login/marketing.

## Luật bắt buộc (do / don't)

DO:
- High whitespace, phân cấp thị giác rõ; editorial typography (Inter, cỡ/độ đậm theo ch.10.3).
- Ưu tiên UX: trạng thái loading/empty/error/offline luôn có; thao tác chính trong tầm ngón cái trên mobile.
- Dùng token màu (`src/style.css`); dark mode bắt buộc; tương phản WCAG 2.2 AA.
- Micro-interaction tinh tế, có mục đích (hover/focus/feedback), tôn trọng `prefers-reduced-motion`.

DON'T:
- ❌ "Bootstrap look" / generic AI layout (card bo tròn lớn, shadow nặng, mọi nút xanh dương mặc định).
- ❌ Purple gradient, neon, glow.
- ❌ Animation thừa, parallax, marker nhấp nháy.
- ❌ Hard-code màu trong component; dùng nhiều hơn một primary action mỗi vùng.
- ❌ Chỉ dùng màu để truyền trạng thái (luôn kèm icon/label).

## Quy trình thiết kế màn mới

1. Lấy spec từ `docs/Project-Bible/09_UIUX.md` (UI-01..32) + acceptance.
2. (Tùy chọn) tạo reference bằng `imagegen-frontend-*` của Taste Skill.
3. Code bằng component + token sẵn có; chạy checklist component (ch.10.8).
4. Verify accessibility + 360px + dark mode trước PR.
