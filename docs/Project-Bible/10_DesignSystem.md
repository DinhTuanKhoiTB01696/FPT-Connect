# 10. Design System

## 10.1 Nguyên tắc

Tên hệ thống token: `FPT Connect UI`. Dùng TailwindCSS semantic tokens và Shadcn Vue primitives; không hard-code màu trong feature component.

## 10.2 Color tokens

| Token | Light | Dark | Dùng cho |
|---|---|---|---|
| `--background` | `#F8FAFC` | `#0B1220` | Nền app |
| `--surface` | `#FFFFFF` | `#111827` | Card/sheet |
| `--foreground` | `#0F172A` | `#F8FAFC` | Text chính |
| `--muted` | `#64748B` | `#94A3B8` | Text phụ |
| `--primary` | `#F97316` | `#FB923C` | Primary action |
| `--secondary` | `#2563EB` | `#60A5FA` | Link/info |
| `--success` | `#15803D` | `#4ADE80` | Thành công |
| `--warning` | `#B45309` | `#FBBF24` | Cảnh báo |
| `--danger` | `#B91C1C` | `#F87171` | Lỗi/destructive |
| `--border` | `#E2E8F0` | `#334155` | Viền |

Màu status luôn đi cùng icon/text. Logo/brand chính thức cần brand approval; token trên là baseline sản phẩm, không tuyên bố thay thế guideline thương hiệu FPT.

## 10.3 Typography

Font: `Inter`, fallback `system-ui, "Segoe UI", sans-serif`.

| Style | Size/Line | Weight | Use |
|---|---|---:|---|
| Display | 32/40 | 700 | Dashboard headline |
| H1 | 28/36 | 700 | Page title |
| H2 | 22/30 | 600 | Section |
| H3 | 18/26 | 600 | Card title |
| Body | 16/24 | 400 | Default/mobile |
| Small | 14/20 | 400 | Metadata |
| Caption | 12/16 | 500 | Label/badge |

## 10.4 Spacing, radius, shadow

- Spacing base 4 px: `1=4`, `2=8`, `3=12`, `4=16`, `6=24`, `8=32`, `12=48`.
- Radius: control 8 px, card 12 px, dialog 16 px, pill 999 px.
- Shadow: `sm` card hover, `md` popover, `lg` dialog; dark mode ưu tiên border.
- Content width: 1440 px max; form readable 720 px; gutters 16/24/32.

## 10.5 Components

| Component | Variants/State | Quy tắc |
|---|---|---|
| Button | primary/secondary/outline/ghost/destructive; loading/disabled | Một primary action mỗi vùng |
| Input | default/error/success/read-only | Label cố định, helper/error không làm layout nhảy mạnh |
| Select/Combobox | single/multi/async | Search khi > 10 options |
| Card | default/actionable/metric | Actionable có hover + keyboard |
| Badge | neutral/info/success/warning/danger | Không chỉ dùng màu |
| Toast | success/info/warning/error | 5 giây; error quan trọng không tự biến mất |
| Dialog | confirm/form | Không chứa wizard dài |
| Sheet | mobile filters/detail | Swipe/close có guard khi dirty |
| DataTable | sort/filter/select/pin | Server-side; responsive card fallback |
| Map | marker/cluster/geofence/polyline | Legend và list alternative |
| Skeleton | text/card/table | Khớp layout thật |

## 10.6 Responsive

| Breakpoint | Width | Behavior |
|---|---:|---|
| `sm` | 640 | Form 2 cột tùy nội dung |
| `md` | 768 | Sidebar compact |
| `lg` | 1024 | Table/map split view |
| `xl` | 1280 | Sidebar full, 12-column grid |
| `2xl` | 1536 | Max content, không kéo text quá rộng |

Mobile navigation dùng bottom bar tối đa 5 item; phần còn lại trong More. Admin console yêu cầu tối thiểu tablet nhưng vẫn đọc được trên mobile.

## 10.7 Motion

- Fast 120 ms hover/focus; standard 180 ms sheet/dropdown; slow 240 ms page transition.
- Easing `cubic-bezier(0.2,0,0,1)`.
- Không animate marker GPS liên tục gây sai nhận thức; cập nhật vị trí có transition tối đa 300 ms.

## 10.8 Component acceptance checklist

- [ ] Light/dark/high contrast.
- [ ] Keyboard, screen reader, focus và zoom 200%.
- [ ] Loading, empty, error, offline, disabled, permission denied.
- [ ] Vietnamese text dài và số lớn.
- [ ] Mobile 360 px và desktop 1440 px.
- [ ] Không phụ thuộc hover cho action quan trọng.

