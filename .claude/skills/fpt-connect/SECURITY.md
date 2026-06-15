# SECURITY.md — Chuẩn bảo mật FPT Connect

> Mục tiêu: OWASP ASVS L2 (Admin tiệm cận L3). Chi tiết: Bible 3.5 + 7.11.

## Identity & Access
- JWT access 10 phút; refresh token tối đa 30 ngày, **rotation mỗi lần dùng**, chỉ lưu hash, **reuse detection revoke cả token family**.
- MFA TOTP bắt buộc cho Admin/Manager; recovery code chỉ lưu hash, dùng một lần.
- Password ≥ 12 ký tự (local identity), kiểm tra breached-password list.
- RBAC + permission + **organizational scope**; deny-by-default; kiểm tra object-id chống IDOR (không chỉ role).

## Input & API
- DTO allowlist (chống mass assignment); validate boundary; filter/sort theo allowlist.
- Chống XSS (encode khi render), SQLi (EF parameterized), CSRF (nếu cookie), SSRF (egress allowlist cho mọi URL import/webhook), path traversal (storage key random).
- Rate limit riêng: login, geocode, export, AI, GPS ingest.

## Data & Secrets
- TLS 1.2+; HSTS; CSP không inline script; CORS allowlist nghiêm ngặt.
- Secret ở vault/biến môi trường (không commit). PII: phone E.164 + hash dò trùng, bản rõ mã hoá; MFA secret bảo vệ bằng DataProtection.
- Log mask PII; không log token/password/phone đầy đủ. Audit bất biến hash-chained cho hành vi nhạy cảm.

## Files & AI
- Upload: kiểm tra MIME + magic-byte + antivirus, tên ngẫu nhiên, signed URL ngắn hạn, không execute.
- AI: redact input gửi provider (không raw GPS/full phone), chống prompt injection (coi nội dung là dữ liệu), không tự ghi system of record.

## Gate
STRIDE threat model + pentest trước production; SAST/DAST/dependency/secret scan trong CI; không Critical/High khi release.
