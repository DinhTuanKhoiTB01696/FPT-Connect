# /review — Soát toàn diện + tự sửa lỗi an toàn

Soát: Architecture · Security · Performance · Naming · Code style · TypeScript · .NET · Database · API · UI.

1. Chạy `hooks/pre-task.md`. Lấy diff/scope cần soát.
2. Đối chiếu `AI_RULES.md` + skills/fpt-connect/*; liệt kê phát hiện theo mức (Critical/High/Medium/Low) kèm bằng chứng (file:line).
3. **Tự sửa** các lỗi AN TOÀN, nhỏ, không đổi hành vi (format, import thừa, naming, thiếu validate rõ ràng). Lỗi rủi ro → chỉ đề xuất, không tự sửa.
4. Chạy `hooks/post-task.md` (build/lint/test).

Output: `REVIEW_REPORT.md` (findings + đã-sửa-gì + còn-lại).
