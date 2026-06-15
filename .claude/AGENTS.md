# AGENTS.md — Vai trò & luồng phối hợp

> Định nghĩa ai sở hữu việc gì. Mục tiêu: nhiều agent + người cùng làm mà không phá tính nhất quán.

## Vai trò

### Claude (Architecture · Review · Security · Docs)
- Phân tích yêu cầu, lập kế hoạch, thiết kế, viết/raà soát tài liệu và ADR.
- Security review, threat modeling, kiểm tra truy vết FR→UC→API→DB→TC.
- Review PR: kiến trúc, bảo mật, nghiệp vụ, đúng phạm vi sprint.
- Viết code cho phần cần lập luận cao (auth, authorization scope, offline/sync, domain rule).

### Codex (Implementation backend & test)
- Hiện thực endpoint/handler/repository theo spec và Prompt Bible (PR-xxx).
- Viết unit/integration/API contract test, đạt coverage domain ≥ 80%.
- Scaffold migration (forward + rollback). Không tự đổi business rule.

### Antigravity (Frontend & design system)
- Dựng feature-module Vue, component theo Design System + taste-skill.
- Bảo đảm accessibility (WCAG 2.2 AA), dark mode, responsive/mobile-first.
- Không hard-code màu; dùng token; phân biệt "Đề xuất AI" với dữ liệu xác nhận.

### Human Developer (Owner · Decision · Merge)
- Quyết định phạm vi sprint, ưu tiên, phê duyệt ADR/Change Request.
- Nghiệm thu acceptance criteria, bấm merge, quản lý secret/hạ tầng/credential.
- Người duy nhất push lên remote production và phê duyệt environment `production`.

## Ma trận sở hữu (RACI rút gọn)

| Loại task | Claude | Codex | Antigravity | Human |
|---|---|---|---|---|
| Kiến trúc / ADR | A/R | C | C | A |
| API & domain backend | C/R | R | I | A |
| Frontend / UI | C | I | R | A |
| Test & QA | R | R | R | A |
| Security review | R | C | C | A |
| Merge / release | I | I | I | R/A |

(R=Responsible, A=Accountable, C=Consulted, I=Informed)

## Review Flow

1. Đọc bộ tài liệu bắt buộc (xem SPRINT_RULES.md) và xác định phạm vi sprint.
2. Implement đúng một feature/slice; tự kiểm tra build + lint + test.
3. Mở PR kèm: mắt xích truy vết, file đã đổi, ảnh hưởng, cách test.
4. Claude review (kiến trúc + security + scope). Sửa theo feedback.
5. Human nghiệm thu acceptance → merge. CI phải xanh, không lỗi Critical/High.

## Nguyên tắc phối hợp

- Một nguồn sự thật: `docs/Project-Bible/`. Bất nhất phải nêu, không tự quyết.
- Không agent nào refactor module ngoài phạm vi của mình.
- Mọi quyết định đổi stack/schema/auth/provider → ADR do Human duyệt.
