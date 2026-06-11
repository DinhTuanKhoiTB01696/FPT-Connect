# FPT Connect — Kế hoạch Sprint theo giai đoạn

> Sprint length: 1 tuần · Bắt đầu: 2026-06-11 · Đội: 10 kỹ sư (4 Backend · 3 Frontend · 1 AI · 1 QA · 1 DevOps)
> Nguồn phạm vi: Project Bible (UC-001..054, API-001..098, TC-001..312, PR-001..100). Mỗi item truy vết tới `FR → UC → API → DB → TC → PR`.

## 1. Giả định và nguyên tắc lập kế hoạch

- Sprint 1 tuần (5 ngày làm việc); Mon planning, Fri demo + retro. Effective ≈ 4 ngày/người.
- Plan tới **75–80% capacity**, luôn chừa buffer cho interrupt, bug và support.
- P0 = phải xong trong sprint; P1 = nên xong; P2 = stretch, cắt đầu tiên khi trễ.
- Mỗi feature chỉ "Done" khi qua DoD ở mục 6 (gồm test, migration rollback, observability, mobile/weak-network).
- Vertical slice: mỗi sprint giao được API + UI + test cho phần mình, không để FE chờ BE cả phase.

## 2. Mô hình capacity

| Thông số | Giá trị |
|---|---|
| Số kỹ sư | 10 |
| Capacity thô/sprint | ~50 SP (10 × ~5 SP) |
| Capacity kế hoạch (80%) | ~40 SP |
| Mục tiêu load/sprint | 30–34 SP (75–80%) |
| 1 SP quy đổi | ~0.6 ideal dev-day |

Phân bổ vai trò ổn định mỗi sprint: BE×4, FE×3, AI×1, QA×1, DevOps×1. QA và DevOps là shared service xuyên suốt (test automation, CI/CD, môi trường, observability).

## 3. Roadmap tổng thể (19 sprint)

| Sprint | Tuần | Giai đoạn | Mục tiêu một câu | UC | Load |
|---|---|---|---|---|---:|
| S0 | 11–17/06 | Foundation | Khung Clean Architecture, CI/CD, DB baseline, observability chạy được | hạ tầng | 30 |
| S1 | 18–24/06 | IAM-1 | Đăng nhập, refresh rotation, MFA, session/device | UC-001..004 | 32 |
| S2 | 25/06–01/07 | IAM-2 | RBAC + permission scope + org tree + territory | UC-005..007 | 33 |
| S3 | 02–08/07 | CRM-1 | Tạo lead, list, Customer 360, search/nearby nền | UC-008,010,011 | 32 |
| S4 | 09–15/07 | CRM-2 | Dedupe/merge, import Excel, pipeline, assignment | UC-009,012..015 | 34 |
| S5 | 16–22/07 | Field-1 | Lập lịch visit + calendar + interaction/note | UC-016,017 | 30 |
| S6 | 23–29/07 | Field-2 | Check-in/out có geofence + review queue | UC-022..024 | 33 |
| S7 | 30/07–05/08 | Field-3 | Start/stop ca + GPS batch ingest + quality | UC-018..020 | 34 |
| S8 | 06–12/08 | Field-4 | Route history/polyline + map cluster + nearby + Maps nav | UC-021,025,026 | 32 |
| S9 | 13–19/08 | Engagement | Reminder recurring/escalation + notification + preferences | UC-027..030 | 32 |
| S10 | 20–26/08 | Tech-1 | Contract metadata + handoff wizard + approval | UC-031..033 | 31 |
| S11 | 27/08–02/09 | Tech-2 | Work order assign/execute/revisit + SLA | UC-034..036 | 33 |
| S12 | 03–09/09 | Files + Offline | File upload/scan + offline outbox/sync conflict | UC-041,048 | 33 |
| S13 | 10–16/09 | Analytics | Dashboard cá nhân/quản lý + export có audit | UC-037..039 | 31 |
| S14 | 17–23/09 | Governance | Audit search + settings + bulk/restore/device/version | UC-040,042,051..054 | 34 |
| S15 | 24–30/09 | AI | Summary/next-action/score/route + feedback | UC-044..047 | 32 |
| S16 | 01–07/10 | Privacy/Ops | Data request, retention/anonymize, health/incident, degraded | UC-049,050 | 30 |
| S17 | 08–14/10 | Hardening-1 | Security pentest + performance/load + resilience | TC-241..270 | 32 |
| S18 | 15–21/10 | Hardening-2 | Accessibility + UAT + production deploy/rollback | TC-271..300 | 28 |

Kết thúc dự kiến: **21/10/2026** (~19 tuần). Go-live sau S18 nếu UAT và security gate đạt.

## 4. Chi tiết từng sprint

Quy ước cột: Item (kèm truy vết) · Est (SP) · Owner (vai trò) · Deps.

---

### Sprint 0 — Foundation (11–17/06)

Sprint Goal: Có khung giải pháp Clean Architecture build/test xanh, CI/CD và môi trường dev/staging, DB baseline + observability, để mọi sprint sau chỉ tập trung nghiệp vụ.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | Scaffold solution 5 project + architecture test reverse-ref (PR-002) | 5 | BE1,BE2 | — |
| P0 | Application pipeline: validation/auth/idempotency/logging/outbox (PR-006) | 5 | BE3 | scaffold |
| P0 | CI/CD pipeline, container non-root, secret scan (TC-249,297) | 5 | DevOps | — |
| P0 | DB baseline migration + audit columns chuẩn 6.1 + EF Core | 5 | BE4 | — |
| P0 | Observability: Serilog JSON + traceId + health live/ready (API-088,089) | 4 | DevOps,BE1 | — |
| P1 | FE app shell Vue3 PWA + routing + design tokens (ch.10) | 4 | FE1,FE2 | — |
| P1 | Test harness: Testcontainers + WebApplicationFactory + Playwright skeleton | 2 | QA | CI/CD |

Load: 30/40 SP (75%). Risk chính: môi trường SQL Server/Redis chậm → giảm bằng Testcontainers + IaC từ ngày 1.

---

### Sprint 1 — IAM-1: Auth & Session (18–24/06)

Sprint Goal: Người dùng đăng nhập an toàn với MFA cho Admin/Manager, refresh token rotation chống reuse, quản lý thiết bị/phiên.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | API-001 login + rate limit + generic error + audit (UC-001, TC-001..010, PR-011) | 5 | BE1 | S0 |
| P0 | API-003 refresh rotation + reuse detection family (UC-002, TC-011..020, PR-012) | 5 | BE2 | login |
| P0 | API-006,007 MFA TOTP enroll/confirm + recovery codes (UC-004, PR-019) | 4 | BE3 | login |
| P0 | API-004,008,009 logout + session list + revoke (UC-003) | 3 | BE4 | refresh |
| P0 | UI-01,02,31 login/MFA/profile-sessions accessible (PR-016) | 6 | FE1,FE2 | API login |
| P1 | API-093..095 device list/rename/revoke (UC-053, TC-309,310) | 4 | BE2 | session |
| P1 | Abuse-case test suite IAM (PR-020, TC-009,010,012,013) | 3 | QA | API |

Load: 32/40 (80%). Risk: race điều kiện refresh đồng thời (TC-013) → test concurrency sớm.

---

### Sprint 2 — IAM-2: RBAC, Org & Territory (25/06–01/07)

Sprint Goal: Phân quyền deny-by-default theo permission + scope tổ chức, cây phòng ban và polygon địa bàn versioned.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | Scoped authorization (permission+tenant+dept+team+owner) + IDOR test (PR-014, TC-241,242) | 6 | BE1,BE3 | S1 |
| P0 | API-010..013 user CRUD + lock + activation (UC-005, TC-021..024, PR-015) | 5 | BE2 | authz |
| P0 | API-014..017 role/permission versioning + cache invalidation 60s (UC-006, TC-025..030, PR-018) | 5 | BE4 | authz |
| P0 | API-018,019 department tree + cycle guard (UC-007, TC-031..033) | 3 | BE2 | user |
| P1 | API-020..022 territory GeoJSON import + spatial validate (UC-007, TC-034..040) | 5 | BE1 | dept |
| P1 | UI-32 admin console: user/role/org/territory scoped nav | 6 | FE1,FE3 | API |
| P1 | Privilege-escalation review (PR-017) | 2 | QA | role |

Load: 33/40 (82% — theo dõi cắt territory import sang S3 nếu cần). Risk: polygon self-intersect/overlap (TC-035..037) → dùng SQL Server geography validate.

---

### Sprint 3 — CRM-1: Customer Core (02–08/07)

Sprint Goal: Tạo và xem khách hàng end-to-end với chống trùng cơ bản, list có scope và Customer 360.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | API-024 create customer + normalize phone + exact dedupe + geocode (UC-008, BR-001..003, TC-041..050, PR-021) | 6 | BE1 | S2 |
| P0 | API-023 list cursor + allowlist filter/sort + masked + perf 1M rows (UC-010, TC-051,059,060, PR-022) | 5 | BE2 | create |
| P0 | API-025,034 Customer 360 + timeline lazy-load (UC-011, TC-051..053) | 5 | BE3 | create |
| P0 | UI-06 fast create + duplicate hint debounce + map pin (PR-024) | 5 | FE1 | API-024 |
| P0 | UI-05 customer list desktop table/mobile cards + saved views (PR-023) | 5 | FE2 | API-023 |
| P1 | UI-09,10 Customer 360 + timeline virtual list | 4 | FE3 | API-025 |
| P1 | Geocode cache + Maps circuit breaker (ch.7.9, TC-149) | 2 | BE4 | — |

Load: 32/40 (80%). Risk: chuẩn hóa số VN + search không dấu (TC-042,059) → normalized columns sớm.

---

### Sprint 4 — CRM-2: Dedupe, Import, Pipeline (09–15/07)

Sprint Goal: Quản lý vòng đời lead: merge trùng có kiểm soát, import Excel, chuyển trạng thái và phân công.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | API-030,031 duplicate search + merge transaction (UC-013, TC-068..070) | 5 | BE1 | S3 |
| P0 | API-032 pipeline transition state machine + rule (UC-014, BR-004,005, TC-061..064) | 4 | BE2 | S3 |
| P0 | API-033 assignment + one-primary-owner + notify (UC-015, TC-065..067) | 4 | BE3 | S3 |
| P0 | API-028,029 import CSV/XLSX dry-run + per-row error (UC-009, TC-071..080) | 6 | BE4 | files baseline |
| P0 | UI-08 duplicate review diff/merge + UI-07 import wizard | 6 | FE1,FE2 | API merge/import |
| P1 | UI-11 customer edit ETag conflict + UI-09 status control (UC-012, TC-054..058) | 5 | FE3 | API-026 |
| P1 | API-026 update optimistic concurrency (UC-012) | 3 | BE1 | — |

Load: 34/40 (85% — cao, ưu tiên cắt import update-by-key sang stretch). Risk: merge mất child records (TC-068) → transaction + idempotent, test fixtures kỹ.

---

### Sprint 5 — Field-1: Visit & Interaction (16–22/07)

Sprint Goal: Lập lịch visit với timezone đúng, ghi interaction/note kèm next action.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | API-039..043 visit CRUD/cancel + overlap rule + timezone (UC-017, TC-091..100) | 6 | BE1 | S4 |
| P0 | API-035 interaction/note + next action → reminder (UC-016, TC-081..084) | 4 | BE2 | S4 |
| P0 | UI-13 visit calendar day/week/agenda mobile | 6 | FE1 | API visit |
| P0 | UI-14 visit detail + UI-10 timeline interaction | 5 | FE2 | API |
| P1 | Recurring visit + DST handling (TC-099) | 4 | BE3 | visit |
| P1 | Offline draft cho note (UC-016, TC-083) nền tảng | 3 | FE3 | — |

Load: 30/40 (75%). Risk: recurring/DST sai occurrence → test với timezone khác Asia/Ho_Chi_Minh.

---

### Sprint 6 — Field-2: Check-in/out & Review (23–29/07)

Sprint Goal: Check-in/out có geofence và anti-spoof, hàng đợi duyệt ngoại lệ cho manager.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | API-044,045 check-in/out server-side distance + accuracy/time/mock (UC-022,023, BR-006, TC-101..110) | 6 | BE1 | S5 |
| P0 | API-046,047 review queue + decision four-eyes (UC-024, TC-111..120) | 5 | BE2 | check-in |
| P0 | UI-15,16 check-in/out accuracy/distance + checklist | 6 | FE1 | API |
| P0 | UI-20 check-in review map/evidence/approve (PR no self-approval) | 5 | FE2 | API review |
| P1 | Mock-location/anti-spoof signal + security flag (TC-107) | 4 | BE3 | check-in |
| P1 | Offline check-in pending + sync nền (TC-108) | 3 | FE3 | — |

Load: 33/40 (82%). Risk: tính distance phía client bị gian lận → bắt buộc tính server-side, test TC-101..105.

---

### Sprint 7 — Field-3: GPS Tracking & Ingest (30/07–05/08)

Sprint Goal: Theo dõi tuyến theo ca có consent, ingest GPS batch idempotent với quality flag, chịu tải 10M điểm/ngày.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | API-048,050 start/stop session + consent + indicator (UC-018,020, BR-007, TC-121..130) | 5 | BE1 | S6 |
| P0 | API-049 GPS batch ingest idempotent + bounds/sequence + queue (UC-019, TC-131..140) | 6 | BE2 | session |
| P0 | RoutePoints partition + async persist + worker (DB-22, ch.7.8) | 5 | BE3 | ingest |
| P0 | UI-17,18 start shift consent + active tracking privacy indicator + pending points | 6 | FE1 | API |
| P1 | Adaptive sampling + offline buffer 2000 điểm client (ch.5.5) | 4 | FE2 | tracking |
| P1 | Load profile 10M points/day (TC-254) | 3 | QA,DevOps | ingest |

Load: 34/40 (85% — theo dõi). Risk: backlog ingest > 5 phút → đo sớm bằng k6, tách worker scale.

---

### Sprint 8 — Field-4: Route, Map & Navigation (06–12/08)

Sprint Goal: Hiển thị tuyến đã simplify với gap, bản đồ cluster, tìm gần đây và điều hướng Google Maps.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | API-052 route polyline/stops/gaps + RouteSummaries aggregate (UC-021, TC-141..146) | 5 | BE1 | S7 |
| P0 | API-036 nearby spatial query <=20km (UC-025, TC-147,148) | 4 | BE2 | S3 |
| P0 | API-037,038 geocode/reverse + nav URL encode (UC-026, TC-149,150) | 3 | BE3 | — |
| P0 | UI-19 route history polyline + quality legend | 5 | FE1 | API-052 |
| P0 | UI-12 customer map cluster + list fallback (TC-147,149) | 6 | FE2 | API nearby |
| P1 | Compare planned vs actual route (UC-021) | 3 | FE3 | route |
| P1 | Map performance 2000 markers <2s (NFR-003, TC-147) | 3 | QA | UI-12 |

Load: 32/40 (80%). Risk: render hàng nghìn DOM marker → bắt buộc cluster, test viewport.

---

### Sprint 9 — Engagement: Reminder & Notification (13–19/08)

Sprint Goal: Nhắc việc định kỳ/escalation không trùng và inbox notification đa kênh có preference.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | API-053..057 reminder CRUD/complete/snooze + recurrence + escalation (UC-027,028, BR-010, TC-151..160) | 6 | BE1 | S5 |
| P0 | API-058..060 notification inbox + dedup + read (UC-029, TC-161..170) | 5 | BE2 | outbox |
| P0 | NotificationDeliveries outbox + retry/dead-letter + provider (DB-28, TC-167,168) | 4 | BE3 | inbox |
| P0 | UI-21,22,23 reminder list/editor + notification inbox | 6 | FE1,FE2 | API |
| P1 | API-061,062 preferences + quiet hours timezone (UC-030, TC-165,166) | 4 | BE4 | — |
| P1 | SignalR reconnect + auth groups (FR-023, TC-170) | 3 | BE2 | — |

Load: 32/40 (80%). Risk: gửi trùng reminder/notification → deterministic dedup key, test TC-156,162.

---

### Sprint 10 — Tech-1: Contract & Handoff (20–26/08)

Sprint Goal: Ghi metadata hợp đồng và bàn giao kỹ thuật bằng checklist có phê duyệt four-eyes.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | API-066,067 contract metadata + uniqueness + signed rule (UC-031, TC-171..174) | 5 | BE1 | S4 |
| P0 | API-068 handoff submit + checklist snapshot (UC-032, BR-009, TC-175..177) | 5 | BE2 | contract |
| P0 | API-069 handoff decision + auto work order (UC-033, TC-178..180) | 4 | BE3 | handoff |
| P0 | UI-24,25 contract detail + handoff wizard save draft | 6 | FE1 | API |
| P0 | UI-26 handoff approval diff/four-eyes | 4 | FE2 | API-069 |
| P1 | Event HandoffApproved → WorkOrder outbox (ch.7.8) | 3 | BE4 | decision |

Load: 31/40 (78%). Risk: submitter tự duyệt (TC-178) → enforce four-eyes ở authz.

---

### Sprint 11 — Tech-2: Work Orders & SLA (27/08–02/09)

Sprint Goal: Vòng đời work order từ phân công đến hoàn thành/revisit với SLA và bằng chứng bất biến.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | API-070,071 work order list + assign skill/availability (UC-034, TC-181,182) | 5 | BE1 | S10 |
| P0 | API-072..074 accept/progress/complete + checklist/evidence (UC-035, TC-183..186) | 6 | BE2 | assign |
| P0 | API-075 revisit child order + parent immutable (UC-036, TC-187..189) | 4 | BE3 | complete |
| P0 | UI-27,28 work order list + execution large touch targets offline | 6 | FE1,FE2 | API |
| P1 | SLA due tracking + overdue alert (DB-25, TC-190) | 4 | BE4 | work order |
| P1 | Reuse check-in cho kỹ thuật (UC-035) | 3 | FE3 | S6 |

Load: 33/40 (82%). Risk: evidence/biên bản bị sửa sau hoàn thành → bất biến + audit.

---

### Sprint 12 — Files & Offline/Sync (03–09/09)

Sprint Goal: Upload file an toàn (scan/signed URL) và đồng bộ offline outbox idempotent có xử lý conflict.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | API-063..065 upload request/complete/download + scan + signed URL (UC-041, TC-085..090) | 6 | BE1 | S0 |
| P0 | Offline outbox engine: replay idempotent + dependency order (UC-048, FR-022, TC-231..240, PR offline) | 6 | BE2,FE1 | nhiều API |
| P0 | Conflict resolution server-version + client merge note (TC-237,238) | 5 | BE3 | outbox |
| P0 | Local encrypted store + sync status UI (UI-06,15,18,28) | 5 | FE2 | outbox |
| P1 | Malware/MIME magic-byte + quarantine (TC-086,087) | 4 | BE4 | upload |
| P1 | App restart không mất outbox (TC-231) | 3 | QA | sync |

Load: 33/40 (82%). Risk: mất dữ liệu offline khi restart/auth revoke → test TC-231,236,239 kỹ.

---

### Sprint 13 — Analytics: Dashboard & Export (10–16/09)

Sprint Goal: Dashboard cá nhân/quản lý từ read model và export có audit/watermark.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | Read model/aggregate + rebuild + freshness (ch.6.7, TC-194) | 5 | BE1 | dữ liệu các phase |
| P0 | API-076,077 dashboard me/team scoped (UC-037,038, TC-191..195) | 5 | BE2 | read model |
| P0 | API-078,079 export job + watermark + masked + 24h link (UC-039, BR-013, TC-196..200) | 5 | BE3 | — |
| P0 | UI-03,04 dashboard cá nhân/quản lý + drill-down | 6 | FE1,FE2 | API |
| P0 | UI-29 reports/export + job history | 4 | FE3 | API export |
| P1 | Dashboard load 200 concurrent <800ms (TC-256) | 3 | QA | API |

Load: 31/40 (78%). Risk: tổng drill-down lệch widget (TC-192) → cùng cutoff, test reconciliation.

---

### Sprint 14 — Governance: Audit, Settings & Data Governance (17–23/09)

Sprint Goal: Tra cứu audit bất biến, settings typed versioned, và 4 năng lực governance mới (bulk/restore/device/version).

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | API-080 audit search + hash chain verify (UC-040, TC-201..205) | 4 | BE1 | audit từ S0 |
| P0 | API-081,082 settings typed schema + version + rollback (UC-042, TC-206..210) | 4 | BE2 | — |
| P0 | API-090,098 bulk update dryRun + job + per-row audit (UC-051, BR-016, TC-301..305) | 5 | BE3 | read model |
| P0 | API-091,092 restore deleted + window + unique-conflict (UC-052, BR-017, TC-306..308) | 4 | BE4 | soft-delete S0 |
| P0 | API-096,097 version history/diff + restore-field (UC-054, TC-311,312) | 4 | BE1 | audit |
| P1 | UI-05/32 bulk action + deleted/restore view + history tab | 6 | FE1,FE2 | API |
| P1 | Device management hoàn thiện UI-31 (UC-053, TC-309,310) | 3 | FE3 | S1 |

Load: 34/40 (85% — cắt version-diff UI sang S15 nếu trễ). Risk: restore tạo trùng unique (TC-308) → kiểm tra trước khi khôi phục.

---

### Sprint 15 — AI Assistant (24–30/09)

Sprint Goal: Trợ lý AI có grounding/citation, explainable, human-in-the-loop, không tự thay system of record.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | API-083 summary có citation + redact + AiRuns lineage (UC-044, BR-012, TC-211..220, PR-071..) | 6 | AI,BE1 | CRM data |
| P0 | API-084 next-best-action rule+model human-confirm (UC-045, TC-225..227) | 5 | AI,BE2 | summary |
| P0 | API-085 customer score + factors + bias/drift monitor (UC-046, TC-221..224) | 5 | AI | data |
| P0 | UI-30 AI panel summary/actions/citations/feedback + provider fallback | 6 | FE1 | API |
| P1 | API-086 route optimize + heuristic fallback (UC-047, TC-228..230) | 4 | AI,BE3 | visits |
| P1 | API-087 feedback + prompt-injection defense (TC-214,215) | 3 | BE4 | AI |

Load: 32/40 (80%). Risk: prompt injection / unsafe output → input redaction + output validation, test TC-214..216,218.

---

### Sprint 16 — Privacy & Operations (01–07/10)

Sprint Goal: Quy trình dữ liệu cá nhân, retention/anonymize tự động, và vận hành health/incident/degraded.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | Data subject workflow: discover/export/anonymize dual-approval (UC-049, FR-024, TC-285..287) | 6 | BE1 | data |
| P0 | Retention jobs: GPS 90d, audit/notif/AI + legal hold (ch.6.8, TC-281..284) | 5 | BE2 | worker |
| P0 | Degraded mode + feature flags (Maps/AI/notify) (UC-050, TC-261..264) | 4 | BE3 | — |
| P0 | Incident runbook + maintenance mode + synthetic probe (UC-050) | 4 | DevOps | health |
| P1 | PII log scanner gate + AI payload redaction (TC-288,289) | 4 | QA,BE4 | — |
| P1 | Privacy notice version/consent (TC-290) | 3 | FE1 | — |

Load: 30/40 (75%). Risk: anonymize không hồi phục được → dual approval + evidence, test TC-286.

---

### Sprint 17 — Hardening-1: Security, Performance, Resilience (08–14/10)

Sprint Goal: Đạt security gate (no Critical/High), SLO hiệu năng và degraded mode dưới lỗi hạ tầng.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | Pentest + fix: IDOR, XSS, SQLi, CSRF, SSRF, path traversal (TC-241..248, PR-003) | 7 | BE1,BE2,QA | toàn hệ |
| P0 | Load/stress/soak k6 đạt NFR-002/004 (TC-251..260) | 6 | DevOps,QA | toàn hệ |
| P0 | Resilience: Maps/AI/notify/Redis/DB fault injection (TC-261..267) | 5 | BE3 | — |
| P0 | Backup/restore drill RPO<=15m, RTO<=2h (TC-268..270) | 4 | DevOps | — |
| P1 | Dependency/SBOM/container scan gate (TC-249,250) | 3 | DevOps | — |
| P1 | DB index regression + query plan (TC-260) | 3 | BE4 | — |

Load: 32/40 (80%). Risk: lỗ hổng High phát hiện muộn → chạy DAST/SAST liên tục từ S0, sprint này chỉ còn fix dư.

---

### Sprint 18 — Hardening-2: Accessibility, UAT & Go-live (15–21/10)

Sprint Goal: Đạt WCAG 2.2 AA luồng lõi, UAT ký duyệt và triển khai production rolling có rollback.

| P | Item | Est | Owner | Deps |
|---|---|---:|---|---|
| P0 | Accessibility audit + fix: keyboard/SR/contrast/zoom/reduced-motion (TC-271..280) | 6 | FE1,FE2,QA | UI |
| P0 | Production smoke + rolling deploy migration compatible + rollback (TC-291..300) | 5 | DevOps,BE1 | S17 |
| P0 | UAT với Sale/Kỹ thuật/Manager/Admin + ký nghiệm thu (ch.12.33) | 6 | QA,toàn đội | tất cả |
| P1 | Mobile 360px + weak-network pass (TC-275,279) | 4 | FE3 | — |
| P1 | Runbook + on-call + alert tuning hoàn thiện | 3 | DevOps | — |

Load: 28/40 (70% — chừa buffer cho UAT fix). Risk: UAT phát sinh thay đổi phạm vi → chỉ nhận P0 critical, còn lại đưa backlog hậu go-live.

## 5. Rủi ro toàn chương trình

| Rủi ro | Ảnh hưởng | Giảm thiểu |
|---|---|---|
| GPS ingest và route là phần nặng nhất (S7-S8) | Trễ kéo theo Field & Analytics | Spike sớm ở S0, đo tải k6 ngay S7, tách worker scale-out |
| Phụ thuộc Google Maps/AI provider | Gián đoạn tính năng | Circuit breaker + fallback + degraded mode (S16), test S17 |
| Sprint 1 tuần dễ over-commit | Carryover dồn | Giữ load ≤80%, P2 luôn là van xả; retro mỗi tuần điều chỉnh velocity |
| Offline/sync phức tạp (S12) | Mất/đúp dữ liệu hiện trường | Idempotency-Key xuyên suốt từ S1, contract test sớm |
| Security/perf để cuối (S17) | Phát hiện muộn, tốn rework | Chạy SAST/DAST/k6 liên tục từ S0, S17 chỉ còn fix |
| Đội 10 người, nhiều phụ thuộc chéo | Nghẽn review/BE-FE | Vertical slice mỗi sprint; QA/DevOps là shared service |

## 6. Definition of Done (áp dụng mọi sprint)

- [ ] Code review + static analysis đạt; architecture test xanh.
- [ ] Unit/integration/API/UI test đạt; không lỗi Critical/High.
- [ ] Migration có forward + rollback; backfill có checkpoint.
- [ ] OpenAPI, audit, log, metric, runbook cập nhật.
- [ ] Kiểm thử mobile viewport (360px) và mạng yếu.
- [ ] Truy vết `FR→UC→API→DB→TC` đầy đủ (PR thiếu mắt xích không merge).
- [ ] Product Owner nghiệm thu acceptance criteria.

## 7. Nghi thức và mốc mỗi sprint

| Thứ | Hoạt động |
|---|---|
| Thứ 2 | Sprint planning (1h) + refinement backlog tuần sau (0.5h) |
| Hằng ngày | Standup 15 phút |
| Thứ 4 | Mid-sprint check-in, đánh giá nguy cơ carryover, quyết định cắt P2 |
| Thứ 6 | Demo + Sprint review + Retro (1.5h) |

Mốc lớn: M1 IAM xong (cuối S2) · M2 CRM xong (S4) · M3 Field/GPS xong (S8) · M4 Tech-ops xong (S11) · M5 feature-complete (S16) · M6 Go-live (S18, 21/10/2026).
