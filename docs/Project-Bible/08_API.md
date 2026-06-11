# 08. API Specification

## 8.1 Chuẩn chung

- Base URL: `/api/v1`; JSON camelCase; UTF-8; UTC ISO 8601.
- Auth: `Authorization: Bearer <access_token>`.
- Command offline/retry gửi `Idempotency-Key`; update gửi `If-Match: "<rowVersion>"`.
- Pagination: `limit` tối đa 100 và `cursor`; không dùng offset cho bảng lớn.
- Correlation: client có thể gửi `X-Correlation-ID`, server luôn trả lại.
- OpenAPI là contract executable; breaking change tạo `/v2`.

### Success envelope

```json
{
  "data": { "id": "9ddf3cf4-4be1-4c50-a645-88cf9b634225" },
  "meta": { "traceId": "00-abcd", "timestampUtc": "2026-06-11T08:00:00Z" }
}
```

### Error - RFC 9457

```json
{
  "type": "https://fptconnect/errors/validation",
  "title": "Validation failed",
  "status": 422,
  "code": "VALIDATION_FAILED",
  "traceId": "00-abcd",
  "errors": { "phone": ["PHONE_INVALID"] }
}
```

Status: `200/201/202/204`, `400` malformed, `401`, `403`, `404`, `409`, `412` ETag, `422`, `429`, `503`.

## 8.2 Endpoint catalog

| ID | Method/Path | Permission | Request/Validation | Response |
|---|---|---|---|---|
| API-001 | `POST /auth/login` | Public | identifier, password, device; rate limit | token pair/MFA challenge |
| API-002 | `POST /auth/mfa/verify` | Challenge | challengeId, 6-digit OTP | token pair |
| API-003 | `POST /auth/refresh` | Refresh | refreshToken; rotation | token pair |
| API-004 | `POST /auth/logout` | User | session/current/all flag | `204` |
| API-005 | `GET /auth/me` | User | - | profile + effective scopes |
| API-006 | `POST /auth/mfa/enroll` | User | reauth | secret QR one-time |
| API-007 | `POST /auth/mfa/confirm` | User | OTP | recovery codes one-time |
| API-008 | `GET /sessions` | User | cursor | sessions masked |
| API-009 | `DELETE /sessions/{id}` | Owner/Admin | id in scope | `204` |
| API-010 | `GET /users` | `users.read` | department/status/search | page |
| API-011 | `POST /users` | `users.manage` | unique employee/email | `201` user |
| API-012 | `PATCH /users/{id}` | `users.manage` | ETag, scoped fields | user |
| API-013 | `POST /users/{id}/lock` | `users.manage` | reason | `204` |
| API-014 | `GET /roles` | `roles.read` | - | roles |
| API-015 | `POST /roles` | `roles.manage` | code/name/permissions | role |
| API-016 | `PUT /roles/{id}/permissions` | `roles.manage` | permissionIds/version | role version |
| API-017 | `PUT /users/{id}/roles` | `roles.assign` | roles + scopes | assignments |
| API-018 | `GET /departments/tree` | `org.read` | rootId | tree |
| API-019 | `POST /departments` | `org.manage` | parent/code/name | department |
| API-020 | `GET /territories` | `territories.read` | effectiveAt | GeoJSON list |
| API-021 | `POST /territories` | `territories.manage` | valid GeoJSON polygon | territory |
| API-022 | `POST /territories/import` | `territories.manage` | GeoJSON file | job |
| API-023 | `GET /customers` | `customers.read` | filter/sort/cursor | scoped page |
| API-024 | `POST /customers` | `customers.create` | name, phone, source | `201` + duplicates |
| API-025 | `GET /customers/{id}` | `customers.read` | include allowlist | 360 summary |
| API-026 | `PATCH /customers/{id}` | `customers.update` | ETag, patch allowlist | customer |
| API-027 | `DELETE /customers/{id}` | `customers.delete` | reason/reauth | `204` soft delete |
| API-028 | `POST /customers/imports` | `customers.import` | fileId/mapping/dryRun | import job |
| API-029 | `GET /customers/imports/{id}` | `customers.import` | - | progress/report |
| API-030 | `POST /customers/duplicates/search` | `customers.read` | normalized fields | candidates |
| API-031 | `POST /customers/{id}/merge` | `customers.merge` | duplicateId/field decisions | survivor |
| API-032 | `POST /customers/{id}/status-transitions` | `customers.update` | target/reason/version | customer |
| API-033 | `POST /customers/{id}/assignments` | `customers.assign` | userId/reason | assignment |
| API-034 | `GET /customers/{id}/timeline` | `customers.read` | types/cursor | events |
| API-035 | `POST /customers/{id}/interactions` | `interactions.create` | type/outcome/content | interaction |
| API-036 | `GET /customers/nearby` | `customers.read` | lat/lng/radius<=20km | distance page |
| API-037 | `POST /geocoding/resolve` | `maps.use` | address/country=VN | candidates |
| API-038 | `POST /geocoding/reverse` | `maps.use` | valid lat/lng | address |
| API-039 | `GET /visits` | `visits.read` | assignee/date/status | page/calendar |
| API-040 | `POST /visits` | `visits.create` | customer/time/purpose | visit |
| API-041 | `GET /visits/{id}` | `visits.read` | - | detail |
| API-042 | `PATCH /visits/{id}` | `visits.update` | ETag | visit |
| API-043 | `POST /visits/{id}/cancel` | `visits.update` | reason | visit |
| API-044 | `POST /visits/{id}/check-ins` | `checkins.create` | coordinates/accuracy/time | validation result |
| API-045 | `POST /visits/{id}/check-outs` | `checkins.create` | location/outcome/checklist | result |
| API-046 | `GET /check-ins/review-queue` | `checkins.review` | status/team | page |
| API-047 | `POST /check-ins/{id}/decisions` | `checkins.review` | approve/reject/reason | decision |
| API-048 | `POST /tracking/sessions` | `tracking.create` | device/consentVersion | session |
| API-049 | `POST /tracking/sessions/{id}/points:batch` | Owner | 1..100 ordered points | accepted sequence |
| API-050 | `POST /tracking/sessions/{id}/stop` | Owner/System | stoppedAt/reason | summary pending |
| API-051 | `GET /tracking/sessions` | `tracking.read.*` | user/date | page |
| API-052 | `GET /tracking/sessions/{id}/route` | `tracking.read.*` | simplification level | polyline/stops/gaps |
| API-053 | `GET /reminders` | Owner/Manager | status/due/customer | page |
| API-054 | `POST /reminders` | `reminders.create` | title/due/recurrence | reminder |
| API-055 | `PATCH /reminders/{id}` | Owner | ETag | reminder |
| API-056 | `POST /reminders/{id}/complete` | Owner | outcome/interactionId | reminder |
| API-057 | `POST /reminders/{id}/snooze` | Owner | until/reason | reminder |
| API-058 | `GET /notifications` | Owner | unread/cursor | inbox |
| API-059 | `POST /notifications/{id}/read` | Owner | - | `204` |
| API-060 | `POST /notifications/read-all` | Owner | beforeUtc | count |
| API-061 | `GET /notification-preferences` | Owner | - | matrix |
| API-062 | `PUT /notification-preferences` | Owner | channel/event settings | matrix |
| API-063 | `POST /files/upload-requests` | Resource writer | name/type/size/hash | signed upload |
| API-064 | `POST /files/{id}/complete` | Uploader | etag/storage proof | scan pending |
| API-065 | `GET /files/{id}/download` | Resource reader | - | short signed URL |
| API-066 | `POST /customers/{id}/contracts` | `contracts.create` | package/value/reference | contract |
| API-067 | `PATCH /contracts/{id}` | `contracts.update` | ETag | contract |
| API-068 | `POST /contracts/{id}/handoffs` | `handoffs.create` | checklist/window | handoff |
| API-069 | `POST /handoffs/{id}/decision` | `handoffs.approve` | decision/reason | handoff/work order |
| API-070 | `GET /work-orders` | `workorders.read` | assignee/status/date | page |
| API-071 | `POST /work-orders/{id}/assign` | `workorders.assign` | assignee/schedule | work order |
| API-072 | `POST /work-orders/{id}/accept` | Assignee | - | work order |
| API-073 | `POST /work-orders/{id}/progress` | Assignee | state/checklist/note | work order |
| API-074 | `POST /work-orders/{id}/complete` | Assignee | result/evidence | work order |
| API-075 | `POST /work-orders/{id}/revisit` | Tech/Manager | reason/schedule | child work order |
| API-076 | `GET /dashboard/me` | User | period | personal widgets |
| API-077 | `GET /dashboard/team` | `analytics.read.team` | team/period | team widgets |
| API-078 | `POST /exports` | `exports.create` | template/filter/purpose | job |
| API-079 | `GET /exports/{id}` | Owner/Auditor | - | status/download |
| API-080 | `GET /audit-logs` | `audit.read` | actor/action/resource/time | page |
| API-081 | `GET /settings` | `settings.read` | namespace | typed values |
| API-082 | `PUT /settings/{key}` | `settings.manage` | schema-valid value | version |
| API-083 | `POST /ai/customers/{id}/summary` | `ai.summary` | promptVersion optional | AI run/result |
| API-084 | `POST /ai/customers/{id}/next-actions` | `ai.suggest` | constraints | suggestions |
| API-085 | `GET /ai/customers/{id}/score` | `ai.score` | - | score/factors/version |
| API-086 | `POST /ai/routes/optimize` | `ai.route` | visits/start/constraints | route plan |
| API-087 | `POST /ai/runs/{id}/feedback` | User | rating/reason/comment | feedback |
| API-088 | `GET /health/live` | Infrastructure | - | liveness |
| API-089 | `GET /health/ready` | Infrastructure | - | dependency readiness |
| API-090 | `POST /customers/bulk-update` | `customers.bulk` | filterOrIds, patch allowlist, dryRun, max 500 | `202` job + preview/impact |
| API-091 | `GET /customers/deleted` | `customers.restore` | cursor/restore window | soft-deleted page |
| API-092 | `POST /customers/{id}/restore` | `customers.restore` | reason; unique-conflict check | customer hoặc `409` |
| API-093 | `GET /devices` | User | - | thiết bị + lastSeen masked |
| API-094 | `PATCH /devices/{id}` | Owner | rename | device |
| API-095 | `DELETE /devices/{id}` | Owner/Admin | id in scope; reauth cho all | `204` revoke session+push |
| API-096 | `GET /customers/{id}/versions` | `customers.read` | two versions/cursor | version list/diff |
| API-097 | `POST /customers/{id}/versions/{version}/restore-field` | `customers.update` | field/version; ETag | customer + audit |
| API-098 | `GET /bulk-jobs/{id}` | Owner/Manager | - | progress/success/skipped/failed |

## 8.3 Request mẫu: tạo customer

```json
{
  "fullName": "Nguyen Van An",
  "phone": "+84901234567",
  "sourceCode": "FIELD_SURVEY",
  "address": "12 Nguyen Hue, Ben Nghe, Quan 1, TP.HCM",
  "location": { "latitude": 10.774267, "longitude": 106.703703 },
  "needs": ["INTERNET_HOME"],
  "consent": { "contactAllowed": true, "capturedAtUtc": "2026-06-11T08:00:00Z" }
}
```

Validation: `fullName` 2-200; phone E.164 VN; source thuộc catalog; lat `[-90,90]`; lng `[-180,180]`; address tối đa 500. Exact duplicate trả `409 CUSTOMER_DUPLICATE` kèm ID được phép xem.

## 8.4 Request mẫu: GPS batch

```json
{
  "firstSequence": 101,
  "points": [
    {
      "sequence": 101,
      "occurredAtUtc": "2026-06-11T08:00:00Z",
      "latitude": 10.774267,
      "longitude": 106.703703,
      "accuracyM": 18.5,
      "speedMps": 4.2,
      "source": "gps"
    }
  ]
}
```

Giới hạn 100 điểm/256 KB; sequence tăng; timestamp không quá 24 giờ trước và không quá 5 phút tương lai. Response `202` gồm `acceptedThroughSequence`, `rejected[]` với code.

### Request mẫu: bulk update (API-090)

```json
{
  "filter": { "statusCode": "New", "ownerUserId": null, "territoryId": 12 },
  "patch": { "ownerUserId": 845, "addTags": ["Q3_CAMPAIGN"] },
  "dryRun": true,
  "expectedCount": 137
}
```

`dryRun=true` trả preview gồm `affectedCount`, `skipped[]` (kèm `reasonCode` như `OUT_OF_SCOPE`, `RULE_VIOLATION`) và không thay đổi dữ liệu. Khi áp dụng thật (`dryRun=false`) server trả `202` với `jobId`; theo dõi qua API-098. `expectedCount` lệch thực tế quá ngưỡng sẽ chặn để tránh thao tác trên tập dữ liệu thay đổi ngoài ý muốn.

## 8.5 API security checklist

- [ ] Object ID luôn kiểm tra scope, không chỉ permission.
- [ ] Mass assignment bị chặn bằng request DTO allowlist.
- [ ] Query sort/filter theo allowlist.
- [ ] Endpoint nhạy cảm reauth/MFA khi policy yêu cầu.
- [ ] Rate limit riêng login, geocode, export, AI và GPS ingest.
- [ ] OpenAPI không chứa secret/example PII thật.
- [ ] Contract test cho mọi status/error code.
- [ ] Bulk operation luôn dryRun-able, giới hạn số dòng, tôn trọng scope từng bản ghi và sinh audit theo dòng.
- [ ] Restore kiểm tra restore window và xung đột unique trước khi khôi phục.
- [ ] Device revoke vô hiệu cả session-family lẫn push token; revoke-all yêu cầu reauth.

