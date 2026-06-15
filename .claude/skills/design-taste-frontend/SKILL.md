---
name: design-taste-frontend
description: Chuẩn thị giác mặc định cho FPT Connect — tích hợp Taste Skill (upstream). Dùng cho mọi quyết định UI; kế thừa triết lý, không sao chép pixel.
upstream: https://github.com/Leonxlnx/taste-skill
status: integrated-default
---

# design-taste-frontend (Taste Skill — default)

> Đây là **bản tích hợp**, không thay thế Taste Skill upstream. Nội dung "nguồn" thuộc về `Leonxlnx/taste-skill` (MIT). File này neo Taste Skill vào FPT Connect và bổ sung ràng buộc dự án. **Không ghi đè, chỉ mở rộng.**

## Cài bản gốc (khuyến nghị cho agent code)

```bash
npx skills add https://github.com/Leonxlnx/taste-skill --skill "design-taste-frontend"
```

Sau khi cài, SKILL.md upstream sẽ điều khiển chi tiết. Nếu chưa cài được, dùng phần "Triết lý kế thừa" dưới đây.

## Vai trò trong FPT Connect

Đây là **guideline thị giác mặc định**. Mọi UI phải đạt cảm nhận của Linear / Vercel / Stripe / Notion / Raycast: minimalism, phân cấp rõ, whitespace cao, editorial typography, component chất lượng, micro-interaction có chủ đích, "premium SaaS feeling".

## Ba dial mặc định cho app nghiệp vụ FPT Connect

- `DESIGN_VARIANCE`: 3–5 (rõ ràng, ít thử nghiệm layout).
- `MOTION_INTENSITY`: 2–3 (KHÔNG animate marker GPS; chuyển động ≤ 300ms; tôn trọng `prefers-reduced-motion`).
- `VISUAL_DENSITY`: 6–7 (dashboard nhiều dữ liệu nhưng có phân cấp).

## Ràng buộc bắt buộc (FPT Connect)

- Tuân thủ token ở `docs/Project-Bible/10_DesignSystem.md` (đã map `Frontend/src/style.css`); dark mode bắt buộc; WCAG 2.2 AA.
- KHÔNG: Bootstrap look, generic AI layout, purple gradient/neon, animation thừa.
- Chi tiết do/don't: `.claude/skills/fpt-connect/DESIGN.md` và `UI_GUIDE.md`.
