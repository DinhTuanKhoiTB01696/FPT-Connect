# /security — Soát bảo mật (OWASP ASVS L2)

Đọc `skills/fpt-connect/SECURITY.md` + Bible 3.5/7.11.

Kiểm tra: authn/authz theo scope (chống IDOR), JWT + refresh rotation/reuse, MFA, rate limit, mass assignment, input validation, XSS/SQLi/CSRF/SSRF/path traversal, upload (MIME/magic-byte/scan), secret management, PII masking trong log, audit bất biến, prompt injection (AI).

1. Chạy `hooks/pre-task.md` + `hooks/security-check.md`.
2. SAST/DAST/dependency scan; xếp hạng theo mức; không để Critical/High khi release.

Output: `SECURITY_REVIEW.md`.
