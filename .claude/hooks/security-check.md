# hook: security-check

Xác minh trước khi merge:
- Authorization theo permission + organizational scope (không chỉ role); kiểm tra object-id (chống IDOR).
- JWT access ngắn + refresh rotation/reuse-detection; MFA cho Admin/Manager; rate limit login/geocode/export/AI/GPS.
- Validation + DTO allowlist (chống mass assignment); chống XSS/SQLi/CSRF/SSRF/path traversal.
- Upload: MIME + magic-byte + scan; secret ở vault/env (không commit); PII masked trong log.
- Audit bất biến cho hành vi nhạy cảm; AI: redact input + chống prompt injection.
- Không Critical/High tồn đọng khi release.
