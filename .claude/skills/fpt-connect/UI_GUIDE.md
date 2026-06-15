# UI_GUIDE.md — Hệ thống giao diện FPT Connect

> Token nguồn: `docs/Project-Bible/10_DesignSystem.md`, đã map vào `Frontend/src/style.css`. Triết lý: `DESIGN.md`.
>
> **Component primitives: shadcn-vue** (ưu tiên dùng/điều chỉnh primitives của shadcn-vue thay vì tự dựng từ đầu; style bằng token, không hard-code màu). Bản đồ dùng **Google Maps API** (cluster + list alternative).

## Layout

- Sidebar trái (desktop) + bottom-bar ≤ 5 mục (mobile). Content max-width ~1440px; form đọc tốt ~720px.
- Mobile-first: thiết kế cho 360px trước, mở rộng lên. Bản đồ luôn có list alternative.

## Spacing

- Base 4px: 1=4, 2=8, 3=12, 4=16, 6=24, 8=32, 12=48. Ưu tiên whitespace rộng, ít đường kẻ.

## Radius

- control 8px, card 12px, dialog 16px, pill 999px. Không bo góc trên border một cạnh.

## Shadow

- `sm` hover card, `md` popover, `lg` dialog. Dark mode ưu tiên border thay shadow. Tránh shadow nặng ("Bootstrap look").

## Animation

- Fast 120ms (hover/focus), standard 180ms (sheet/dropdown), slow 240ms (page). Easing `cubic-bezier(0.2,0,0,1)`.
- KHÔNG animate marker GPS liên tục; cập nhật vị trí transition ≤ 300ms. Tôn trọng `prefers-reduced-motion`.

## Color

- Dùng CSS variables (`--primary`, `--surface`, …); KHÔNG hard-code hex trong component.
- Primary cam (#F97316 light / #FB923C dark), secondary xanh. Status luôn kèm icon/text, không chỉ màu. KHÔNG purple gradient/neon.

## Typography

- Inter. Display 32/40·700, H1 28/36·700, H2 22/30·600, H3 18/26·600, Body 16/24·400, Small 14/20, Caption 12/16·500.
- Editorial: phân cấp rõ, dòng không quá rộng, sentence case.

## Dark mode

- Bắt buộc, toggle class `dark` trên `<html>`. Mọi màu đọc được ở cả hai chế độ.

## Accessibility (WCAG 2.2 AA)

- Keyboard nav + focus visible; focus trap đúng trong dialog. Label thật + `aria-describedby` cho lỗi; live region cho toast/sync.
- Tương phản AA; chart kèm bảng dữ liệu/pattern. Target chạm ≥ 44×44px.

## Responsive

- Breakpoints: sm 640, md 768, lg 1024, xl 1280, 2xl 1536. DataTable desktop → cards mobile.

## Trạng thái bắt buộc mỗi màn

loading (skeleton khớp layout) · empty · error · offline ("Đã lưu trên thiết bị"/"Đang đồng bộ"/"Cần xử lý") · permission denied. Phân biệt rõ "Đề xuất AI" với dữ liệu đã xác nhận.
