---
name: minimalist-ui
description: Hướng thị giác chủ đạo cho dashboard & CRM của FPT Connect (vibe Notion/Linear). Tích hợp từ Taste Skill, không thay thế.
upstream: https://github.com/Leonxlnx/taste-skill
status: integrated
---

# minimalist-ui (cho dashboard & CRM)

> Tích hợp từ Taste Skill (`minimalist-ui`). Không ghi đè upstream — chỉ ánh xạ vào FPT Connect.

## Cài bản gốc

```bash
npx skills add https://github.com/Leonxlnx/taste-skill --skill "minimalist-ui"
```

## Khi nào dùng

Mặc định cho: danh sách khách hàng, Customer 360, dashboard cá nhân/quản lý, work order, audit, settings — nơi mật độ dữ liệu cao và cần đọc nhanh.

## Nguyên tắc

- Editorial product UI (vibe Notion/Linear): bảng/cột gọn, đường kẻ tối thiểu, whitespace có chủ đích.
- Palette hạn chế (dùng token; status kèm icon/label, không chỉ màu). Một primary action mỗi vùng.
- Cấu trúc rõ ràng hơn trang trí; bàn phím-first cho thao tác lặp (vibe Linear/Raycast).
- DataTable desktop → cards mobile; skeleton khớp layout thật.

## Ràng buộc

Theo `.claude/skills/fpt-connect/UI_GUIDE.md` (spacing/radius/typography/dark mode/accessibility). Không Bootstrap look, không gradient tím.
