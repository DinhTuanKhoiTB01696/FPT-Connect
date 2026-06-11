# 15. Prompt Bible

## 15.1 Cách dùng

Mỗi prompt giả định agent đã đọc `docs/Project-Bible/README.md` và các chương được nêu. Thay placeholder `{...}` bằng dữ liệu thật. Agent phải:

1. Đọc code trước khi sửa, giữ pattern hiện có.
2. Truy vết `FR -> UC -> API -> DB/UI -> TC`.
3. Không tự đổi business rule; bất nhất phải báo rõ.
4. Triển khai, test, cập nhật docs và nêu file đã đổi.
5. Không đưa secret/PII thật vào prompt, code, fixture hoặc log.

## 15.2 Architecture and planning - PR-001..PR-010

| ID | Agent | Prompt |
|---|---|---|
| PR-001 | Claude | Review `07_Architecture.md` against `03_Phan_Tich_Yeu_Cau.md`. List only concrete contradictions, missing failure modes, and decisions requiring ADR; cite requirement IDs and propose exact corrections. |
| PR-002 | Codex | Scaffold the .NET solution exactly as section 14.1. Add project references enforcing the Clean Architecture dependency rule and an architecture test that fails on reverse references. Run build/tests. |
| PR-003 | Claude | Threat-model FPT Connect using STRIDE. Focus on JWT refresh reuse, IDOR by organizational scope, GPS spoofing, file upload, SignalR groups, exports, and AI prompt injection. Produce threats, controls, residual risk, and verification tests. |
| PR-004 | Codex | Create ADR-001 for SQL Server `geography` and spatial indexes. Include context, alternatives, migration, performance test, rollback, and consequences for EF Core. Do not change implementation. |
| PR-005 | Claude | Analyze bounded-context boundaries in chapter 7 and the 36-table model. Flag cross-context writes and aggregates that are too large; recommend event contracts without introducing distributed services prematurely. |
| PR-006 | Codex | Implement shared application pipeline behaviors for validation, authorization scope, idempotency, logging, and transaction/outbox. Add unit and integration tests for ordering and failure behavior. |
| PR-007 | Antigravity | Design the Vue feature-module architecture from chapters 9, 10, and 14. Produce routes, layouts, stores, API clients, permissions guards, and offline boundaries before generating components. |
| PR-008 | Claude | Review an implementation plan for `{feature}`. Verify every task maps to a documented UC/API/table/screen/test and identify hidden migration, privacy, observability, or rollback work. |
| PR-009 | Codex | Inspect the repository and create a dependency diagram in Mermaid reflecting actual projects/modules. Compare it to chapter 7 and open an ADR draft for each intentional deviation. |
| PR-010 | Claude | Conduct a production-readiness architecture review using NFR-001..012. Return findings by severity, evidence, likely failure at scale, and a measurable remediation criterion. |

## 15.3 Authentication and IAM - PR-011..PR-020

| ID | Agent | Prompt |
|---|---|---|
| PR-011 | Codex | Implement API-001 login with generic credential errors, rate limiting, Serilog-safe fields, session creation, and Admin/Manager MFA challenge. Cover TC-001..010. |
| PR-012 | Codex | Implement API-003 refresh-token rotation using a transaction and hashed tokens. Detect reuse, revoke the token family, and test the concurrent refresh race in TC-013. |
| PR-013 | Claude | Security-review the auth implementation against UC-001..004 and TC-001..020. Prioritize token leakage, race conditions, clock skew, lockout abuse, and missing audit events. |
| PR-014 | Codex | Implement scoped authorization that combines permission, tenant, department hierarchy, team, and resource ownership. Demonstrate it on customer list/detail and add IDOR tests. |
| PR-015 | Codex | Implement UC-005 user management with employee-code uniqueness, activation, lock/unlock, session revoke, optimistic concurrency, and audit before/after redaction. |
| PR-016 | Antigravity | Build UI-01, UI-02, and UI-31 with accessible validation, MFA/recovery flows, session revoke, loading/error states, and responsive behavior from chapter 10. |
| PR-017 | Claude | Review role/permission design for privilege escalation. Test delegated admin scope, last-critical-admin protection, cache invalidation, and mass assignment. |
| PR-018 | Codex | Implement role versioning and permission cache invalidation. Add a test proving a removed permission becomes ineffective within 60 seconds without re-login. |
| PR-019 | Codex | Add TOTP enrollment and one-time recovery codes. Encrypt TOTP secret, hash recovery codes, require reauthentication, and never log/display the secret after confirmation. |
| PR-020 | Claude | Create an IAM abuse-case checklist for QA covering account enumeration, MFA fatigue/replay, token theft, refresh races, stale permissions, cross-branch access, and session fixation. |

## 15.4 Customer CRM - PR-021..PR-030

| ID | Agent | Prompt |
|---|---|---|
| PR-021 | Codex | Implement API-024 create customer per UC-008 and BR-001..003. Normalize Vietnamese phone numbers, exact/fuzzy duplicate detection, geocode fallback, timeline, assignment, SLA reminder, and idempotency. |
| PR-022 | Codex | Implement scoped customer list API-023 with cursor pagination, allowlisted filter/sort, normalized Vietnamese search, masked fields, and a SQL performance test at 1 million rows. |
| PR-023 | Antigravity | Build UI-05 customer list with desktop table/mobile cards, saved filters, URL state, cursor pagination, permission-aware bulk actions, skeleton/empty/error/offline states. |
| PR-024 | Antigravity | Build UI-06 fast customer creation. Run duplicate lookup after 400 ms debounce, support manual map pin and encrypted offline draft, and prevent duplicate submit. |
| PR-025 | Codex | Implement API-028/029 customer import with upload scan, column mapping, dry-run, row-level errors, 20% abort threshold, chunked transactions, progress, and retry-safe batch IDs. |
| PR-026 | Claude | Review duplicate-detection and merge behavior against UC-013. Find data-loss paths involving contracts, interactions, files, assignments, redirects, concurrent updates, and audit. |
| PR-027 | Codex | Implement transactional customer merge with field decisions, survivor mapping, child transfer, conflict guards, redirect resolution, and tests TC-068..070. |
| PR-028 | Codex | Implement the documented customer state machine. Keep invariants in Domain, reasons typed, manager override audited, and automation idempotent. |
| PR-029 | Antigravity | Build UI-09/10 Customer 360 with lazy tabs, timeline virtualization, sensitive-field masking, deep links, ETag conflict UI, and AI citation anchors. |
| PR-030 | Claude | Review Customer 360 for privacy minimization. Produce a field-by-role matrix for Sale, Kỹ thuật, Manager, Admin, Auditor and identify any overexposure in API or UI. |

## 15.5 GPS, map, visit, check-in - PR-031..PR-040

| ID | Agent | Prompt |
|---|---|---|
| PR-031 | Codex | Implement route-session start/stop per UC-018/020. Require consent version, enforce one active session, support offline stop semantics, and enqueue summary aggregation. |
| PR-032 | Codex | Implement API-049 GPS batch ingest with sequence/idempotency, bounds/time validation, quality flags, grace period, queue backpressure, and load tests for NFR-004. |
| PR-033 | Claude | Review GPS data handling for privacy and employee fairness. Check consent, visibility, retention, accuracy interpretation, anomaly handling, manager access, and disciplinary misuse safeguards. |
| PR-034 | Antigravity | Build UI-17/18 start-shift and active-tracking screens. Show purpose, consent, GPS accuracy, buffered points, offline status, persistent tracking indicator, and immediate local stop. |
| PR-035 | Codex | Implement check-in server validation for distance, accuracy, time skew, mock signal, and evidence. Never trust client-computed distance; produce Valid/Review/Reject with reason codes. |
| PR-036 | Antigravity | Build UI-15/16 check-in/out flow that samples GPS for up to 10 seconds, explains review outcomes, captures evidence offline, and routes immediately to the visit checklist. |
| PR-037 | Codex | Implement API-046/047 review queue and decision workflow. Prevent self-approval and cross-scope review, preserve raw evidence, require reason, and handle concurrent reviewers. |
| PR-038 | Codex | Implement nearby customer search with SQL Server geography, spatial index, scope predicate before distance calculation, 20 km cap, cursor pagination, and accuracy tests. |
| PR-039 | Antigravity | Build UI-12/19 map and route history with clusters, quality legend, GPS-gap polyline breaks, list fallback, keyboard access, and restricted-GPS access messaging. |
| PR-040 | Claude | Performance-review map/tracking code for excessive marker rendering, GPS battery drain, route-point query scans, SignalR overbroadcast, and provider quota leakage. |

## 15.6 Reminder and notification - PR-041..PR-050

| ID | Agent | Prompt |
|---|---|---|
| PR-041 | Codex | Implement Reminder aggregate with one-time/recurring rules, original due time, snooze, complete, cancel, deterministic automation keys, and optimistic concurrency. |
| PR-042 | Codex | Implement reminder scheduler and 24-hour escalation. Ensure exactly-once business effect under retries and process 100,000 reminders due in one minute. |
| PR-043 | Antigravity | Build UI-21/22 reminder list/editor with today/upcoming/overdue tabs, recurrence preview, timezone display, swipe actions, validation, and offline pending states. |
| PR-044 | Codex | Implement transactional notification outbox, per-channel delivery records, exponential retry with jitter, dead-letter handling, and provider adapters. |
| PR-045 | Codex | Implement SignalR notification delivery with authenticated connections, authorized groups, reconnect-safe unread count, and no assumption that SignalR is the source of truth. |
| PR-046 | Antigravity | Build UI-23 notification inbox with cursor loading, type filters, mark-read/read-all, safe deep links, unread badge, and graceful handling of deleted resources. |
| PR-047 | Codex | Implement quiet hours and mandatory security notifications using user timezone. Add boundary tests at quiet-start, quiet-end, and timezone changes. |
| PR-048 | Claude | Review reminder/notification implementation for duplicate sends, missed jobs, recurrence drift, timezone errors, preference bypass, dead-letter invisibility, and noisy escalation. |
| PR-049 | Codex | Add delivery metrics and alerts: success rate by channel/provider/event, P95 delay, retries, dead letters, and queue age. Exclude message body and PII labels. |
| PR-050 | Antigravity | Add accessible toast and notification components from chapter 10. Critical errors persist until dismissed; routine success auto-dismisses without stealing focus. |

## 15.7 Contract and technical operations - PR-051..PR-060

| ID | Agent | Prompt |
|---|---|---|
| PR-051 | Codex | Implement contract metadata API-066/067 with external-reference uniqueness, package/value validation, signed-state prerequisites, file scan dependency, and audit. |
| PR-052 | Codex | Implement Handoff aggregate with versioned checklist/snapshot, auto-approval rules, four-eyes option, rejection/resubmission, and atomic work-order creation. |
| PR-053 | Antigravity | Build UI-25 handoff wizard with saved draft, required checklist, map/contact/package review, installation window, validation summary, and final immutable snapshot preview. |
| PR-054 | Antigravity | Build UI-26 approval screen showing changed fields, risk/exception badges, reviewer independence, approve/reject/return actions, and reason requirements. |
| PR-055 | Codex | Implement work-order assignment using territory, skill, availability, workload, and manager override reason. Preserve complete assignment history and SLA start. |
| PR-056 | Codex | Implement work-order execution state machine including accept, travel, check-in, start, pause, complete, fail, and revisit. Validate checklist/evidence for completion. |
| PR-057 | Antigravity | Build UI-27/28 work-order list/execution optimized for mobile, large touch targets, offline checklist/evidence, SLA indicator, safe failure/revisit flow. |
| PR-058 | Claude | Review Sale-to-Technical handoff for race conditions and missing information. Test contract cancellation, duplicate handoff, changed customer data after snapshot, reassignment, and repeated revisit. |
| PR-059 | Codex | Add SLA monitoring and notifications for handoff acceptance, scheduling, completion, and overdue work orders. Make every escalation deduplicated and observable. |
| PR-060 | Claude | Produce UAT scripts for one successful installation, customer absent, infrastructure failure, expedited approval, reassignment, and three-attempt escalation using exact expected states. |

## 15.8 Analytics, export, audit - PR-061..PR-070

| ID | Agent | Prompt |
|---|---|---|
| PR-061 | Codex | Implement personal dashboard read model for tasks, visits, overdue reminders, pipeline, and KPI. Define cutoff/freshness and prove widget totals against source fixtures. |
| PR-062 | Codex | Implement manager dashboard with team scope, funnel, SLA, workload, activity map, previous-period comparison, and drill-down using one semantic metric definition. |
| PR-063 | Antigravity | Build UI-03/04 dashboards with parallel widgets, skeletons, partial errors, freshness labels, metric tooltips, responsive charts, and accessible data-table alternatives. |
| PR-064 | Codex | Implement export API-078/079: synchronous under 5,000 rows, background above it, scoped field projection, watermark, purpose, expiring signed URL, and download audit. |
| PR-065 | Claude | Review export code for CSV formula injection, PII overexport, stale permission at download, unbounded memory, insecure file links, and insufficient audit. |
| PR-066 | Codex | Implement append-only audit writer with redacted before/after JSON, trace ID, actor/resource context, partitioning, hash chaining, and a verification command. |
| PR-067 | Antigravity | Build audit viewer in UI-32 with scoped filters, cursor pagination, redacted diff, correlation chain, restricted-export warning, and no mutation actions. |
| PR-068 | Claude | Validate dashboard metric definitions for conversion, follow-up SLA, valid check-in, handoff duration, work-order completion, and weekly active staff. Identify denominator/cutoff ambiguity. |
| PR-069 | Codex | Add query telemetry that records query name, duration, row count, cache hit, and trace ID without SQL text or PII. Alert on SLO regression. |
| PR-070 | Claude | Review SQL execution plans for customer list, nearby, audit search, reminder due scan, route history, dashboard, and export. Recommend only evidence-based indexes. |

## 15.9 AI - PR-071..PR-080

| ID | Agent | Prompt |
|---|---|---|
| PR-071 | Codex | Implement an `IAiGateway` and customer-summary pipeline with scoped retrieval, PII minimization, prompt version, JSON schema validation, citation verification, timeout, cache, and AiRun lineage. |
| PR-072 | Claude | Red-team AI Summary with prompt injection in notes, conflicting events, hidden instructions in attachments, cross-customer exfiltration requests, system-prompt requests, and unsupported claims. |
| PR-073 | Antigravity | Build UI-30 AI panel that labels generated content, shows citations/confidence/missing information, supports accept-as-draft and feedback, and never implies automatic truth. |
| PR-074 | Codex | Implement next-best-action as rule-first with optional model explanation. Accepting creates only a draft/reminder; rejecting stores feedback; provider failure uses deterministic fallback. |
| PR-075 | Claude | Review customer-score design for leakage, proxy discrimination, poor calibration, stale scores, unsupported probability language, and automation that could unfairly deprioritize customers. |
| PR-076 | Codex | Implement model registry/config with version, status, approved features, thresholds, rollout percentage, kill switch, and audit. Do not store provider secrets in settings. |
| PR-077 | Codex | Build the AI evaluation harness using 200 anonymized fixtures. Score factual citation precision, schema adherence, privacy leakage, latency, cost, and human rubric export. |
| PR-078 | Codex | Implement route optimization with hard time-window/work-hour constraints and deterministic objective weights. Use LLM only to explain; add infeasible-plan tests. |
| PR-079 | Claude | Audit AI provider integration contract for retention/training terms, data residency, logging, deletion, incident notice, subprocessor, rate limit, and fallback risk. |
| PR-080 | Codex | Add AI observability for use case/model/prompt version, latency, tokens, cost, cache, safety rejection, feedback, and drift. Never label metrics with customer/user IDs. |

## 15.10 Frontend and design system - PR-081..PR-090

| ID | Agent | Prompt |
|---|---|---|
| PR-081 | Antigravity | Implement Tailwind semantic tokens and Shadcn Vue primitives from chapter 10 for light/dark themes. Add Storybook-equivalent examples for all states and accessibility checks. |
| PR-082 | Antigravity | Create the responsive application shell: desktop sidebar, mobile bottom navigation, permission-filtered routes, global search, notification badge, offline banner, and user menu. |
| PR-083 | Claude | Accessibility-review `{screen}` against WCAG 2.2 AA. Report keyboard order, names/roles/states, focus management, contrast, reflow, errors, motion, and map alternatives with severity. |
| PR-084 | Antigravity | Implement a reusable server data table with cursor pagination, URL filters, allowlisted sort, selection, loading/empty/error states, desktop table and mobile-card rendering. |
| PR-085 | Antigravity | Implement encrypted offline outbox for lead, note, check-in, and GPS commands. Show item state and conflict resolution; test app restart and partial sync failure. |
| PR-086 | Claude | Review frontend state management for duplicated server state, stale permissions, unbounded stores, PII persistence, race conditions, and unsafe optimistic updates. |
| PR-087 | Antigravity | Implement the Customer 360 conflict dialog for ETag mismatch. Show server/local values by changed field and allow safe merge only for permitted fields. |
| PR-088 | Antigravity | Add route-level code splitting and performance budgets. Measure mobile 4G startup, map chunk size, long tasks, and render count for list/map screens. |
| PR-089 | Claude | UX-review the five critical mobile tasks: create lead, find nearby, start/stop shift, check-in/out, complete work order. Count taps, identify ambiguity, and propose measurable improvements. |
| PR-090 | Antigravity | Add Playwright tests for UI-01, UI-06, UI-15, UI-18, UI-25, and UI-28 across 360x800 and 1440x900, including keyboard and offline simulation. |

## 15.11 Testing, security, deployment - PR-091..PR-100

| ID | Agent | Prompt |
|---|---|---|
| PR-091 | Codex | Convert TC-001..300 into test-management import CSV with columns ID, title, preconditions, steps, expected, layer, priority, automation status, requirement, owner. Preserve every ID. |
| PR-092 | Codex | Implement Testcontainers infrastructure for SQL Server, Redis, and S3-compatible storage. Isolate parallel tests, deterministic clock, seeded synthetic data, and automatic cleanup. |
| PR-093 | Claude | Review the full test suite for false positives, missing authorization assertions, weak expected outcomes, non-production database substitutions, flaky timing, and untested rollback. |
| PR-094 | Codex | Create k6 scenarios for TC-251..260 with staged load, realistic data distribution, thresholds, correlation IDs, and a summary report comparing SLOs. |
| PR-095 | Codex | Add CI stages from chapter 13: format, unit, architecture, SAST, secret/dependency scan, build/SBOM, integration, API, image scan/sign, E2E. Fail on Critical/High. |
| PR-096 | Codex | Create production Dockerfiles and Compose for local dependencies. Use multi-stage builds, pinned images, non-root users, healthchecks, read-only mounts where possible, and no embedded secrets. |
| PR-097 | Claude | Security-review Docker, IIS, Nginx, TLS, forwarded headers, CORS, CSP, WebSocket proxying, upload limits, and management endpoint exposure. Return deploy-blocking findings first. |
| PR-098 | Codex | Write SQL backup/restore scripts and a quarterly restore-drill runbook meeting RPO 15 minutes/RTO 2 hours. Include integrity checks, evidence capture, and failed-backup handling. |
| PR-099 | Codex | Implement OpenTelemetry logs, metrics, traces and dashboards for API RED, resource USE, SQL, Redis, queues, GPS lag, notification delivery, AI cost, and business KPIs. |
| PR-100 | Claude | Perform final release review using README Definition of Done and chapters 3, 12, 13. Produce Go/No-Go, blocking evidence, accepted risks with owner/expiry, rollback readiness, and post-deploy checks. |

## 15.12 Prompt quality checklist

- [ ] Nêu rõ file/chương/ID nguồn.
- [ ] Nêu output, giới hạn, test và tiêu chí hoàn tất.
- [ ] Yêu cầu agent kiểm tra code hiện có trước khi sửa.
- [ ] Không yêu cầu agent tự phát minh business rule.
- [ ] Bao gồm security/privacy/observability khi có dữ liệu hoặc side effect.
- [ ] Có rollback/migration cho thay đổi production.

