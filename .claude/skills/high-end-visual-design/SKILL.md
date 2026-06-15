---
name: high-end-visual-design
description: Hướng thị giác "cao cấp, tĩnh, đắt tiền" cho login & màn marketing của FPT Connect. Tích hợp từ Taste Skill, không thay thế.
upstream: https://github.com/Leonxlnx/taste-skill
status: integrated
---

# high-end-visual-design (cho login & marketing)

> Tích hợp từ Taste Skill (`high-end-visual-design` / soft-skill). Không ghi đè upstream.

## Cài bản gốc

```bash
npx skills add https://github.com/Leonxlnx/taste-skill --skill "high-end-visual-design"
```

## Khi nào dùng

Màn ít dữ liệu, cần ấn tượng: đăng nhập, onboarding, trang giới thiệu/marketing, empty-state lớn.

## Nguyên tắc

- Polished, calm, "expensive": tương phản mềm hơn, whitespace rộng, typography premium, spring motion nhẹ nhàng (vẫn ≤ 300ms, tôn trọng reduced-motion).
- Hình ảnh/đồ hoạ chất lượng cao; bố cục cân đối; chi tiết tinh (Stripe/Vercel-grade).
- Vẫn dùng token màu dự án + dark mode; KHÔNG purple gradient/neon, không hiệu ứng loè loẹt.

## Ràng buộc

Đồng nhất với `.claude/skills/fpt-connect/DESIGN.md`. Ưu tiên: Accessibility > token Project Bible > stylistic upstream khi xung đột.
