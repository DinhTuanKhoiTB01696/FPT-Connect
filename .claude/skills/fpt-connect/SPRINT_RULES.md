# SPRINT_RULES.md — Kỷ luật Sprint

> Sprint 1 tuần. Kế hoạch chi tiết: `docs/Project-Bible/SPRINT_PLAN.md` (S0–S18).

## Trước mỗi Sprint — đọc theo thứ tự (BẮT BUỘC)

1. `.claude/CLAUDE.md`
2. `.claude/AI_RULES.md`
3. `.claude/MEMORY.md`
4. `.claude/PROJECT_SUMMARY.md`
5. Project Bible (`docs/Project-Bible/`) phần liên quan + spec Sprint hiện tại trong `SPRINT_PLAN.md` (UC/API/TC).
6. `.claude/skills/fpt-connect/SKILL.md`
7. `.claude/skills/fpt-connect/RULES.md`

Chỉ sau khi đọc xong mới bắt đầu code.

## Trong Sprint

- **Never jump sprint** — không làm việc của sprint khác.
- **Never implement future sprint** — không "chuẩn bị trước" tính năng chưa tới.
- Bám backlog P0 trước, P1 sau, P2 chỉ khi còn capacity; P2 là phần cắt đầu tiên khi trễ.
- Giữ load ≤ 80% capacity; còn lại là buffer cho bug/support.
- Mỗi item phải có mắt xích truy vết và đủ test trước khi coi là Done.

## Kết Sprint

- Demo + review + retro. Carryover phải nêu lý do, không tự động cam kết lại.
- Cập nhật tài liệu/ADR nếu có thay đổi baseline.

## Định nghĩa "Done" của một item

Build xanh · test pass · không lỗi Critical/High · migration có rollback · observability cập nhật · acceptance criteria được nghiệm thu · truy vết đầy đủ.
