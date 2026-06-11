# 12. Testing Strategy and Test Catalog

## 12.1 Chiến lược

| Tầng | Phạm vi | Công cụ gợi ý | Gate |
|---|---|---|---|
| Unit | Domain rule, validator, mapper | xUnit, FluentAssertions | Domain >= 80% |
| Architecture | Dependency, naming | NetArchTest | 100% |
| Integration | EF Core/SQL Server/Redis/storage | Testcontainers | Critical paths |
| API contract | Status/schema/auth/idempotency | WebApplicationFactory, OpenAPI | 100% endpoint |
| Frontend | Store/composable/component | Vitest, Vue Test Utils | Critical UI |
| E2E | User journeys | Playwright | Smoke + regression |
| Security | SAST/DAST/dependency/ASVS | CodeQL/Semgrep, ZAP | No Critical/High |
| Performance | Load/stress/soak | k6 | SLO đạt |
| Resilience | Provider/DB/network fault | Fault injection | Degraded mode |

Mỗi test case dưới đây có precondition chung: dữ liệu fixture cô lập, clock kiểm soát, correlation ID lưu, và cleanup không xóa audit production-like. `Expected` là acceptance oracle bắt buộc.

## 12.2 Authentication - TC-001..TC-010

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-001 | Login đúng Sale active | `200`, token 10 phút, session/audit tạo |
| TC-002 | Sai password 1 lần | `401` chung, không tiết lộ account tồn tại |
| TC-003 | Sai 5 lần liên tiếp | Delay/lock theo policy, security metric tăng |
| TC-004 | User locked login đúng password | `403 ACCOUNT_LOCKED`, không tạo session |
| TC-005 | Tenant disabled | `403 TENANT_DISABLED` |
| TC-006 | Admin chưa MFA | Trả MFA challenge, chưa trả access token |
| TC-007 | OTP đúng trong time window | Token pair phát một lần |
| TC-008 | OTP hết hạn/replay | `401 MFA_INVALID`, challenge không tái dùng |
| TC-009 | Login payload có SQL/XSS | Không execute/reflect; validation an toàn |
| TC-010 | 100 login/phút cùng IP | `429`, `Retry-After`, user hợp lệ không bị lock vĩnh viễn |

## 12.3 Token/session - TC-011..TC-020

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-011 | Refresh token hợp lệ | Token cũ revoke, pair mới trả |
| TC-012 | Reuse refresh token cũ | Revoke family, security event |
| TC-013 | Hai refresh đồng thời | Một `200`, một reuse/conflict |
| TC-014 | Refresh hết absolute lifetime | `401`, yêu cầu login |
| TC-015 | Logout current session | Refresh fail <= 5 giây |
| TC-016 | Logout all devices | Mọi family của user revoke |
| TC-017 | Admin lock user | Session revoke và SignalR disconnect |
| TC-018 | JWT sai signature | `401`, không gọi handler |
| TC-019 | JWT đúng nhưng sai audience | `401` |
| TC-020 | Access token hết hạn 1 giây | `401` với clock skew theo policy |

## 12.4 IAM - TC-021..TC-030

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-021 | Branch Admin tạo user cùng branch | Thành công, activation event |
| TC-022 | Branch Admin tạo user branch khác | `403` |
| TC-023 | Employee code trùng tenant | `409 USER_DUPLICATE` |
| TC-024 | Cùng code khác tenant | Cho phép |
| TC-025 | Role mới với permission hợp lệ | Version 1 và audit diff |
| TC-026 | Gán permission vượt quyền admin | `403` |
| TC-027 | Xóa role đang gán | Chặn hoặc require reassignment |
| TC-028 | Role change khi user online | Quyền mới hiệu lực <= 60 giây |
| TC-029 | User tự nâng role qua mass assignment | Field bị bỏ/chặn |
| TC-030 | Manager query users | Chỉ trả team scope |

## 12.5 Organization/territory - TC-031..TC-040

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-031 | Tạo department con hợp lệ | Path đúng, tree hiển thị |
| TC-032 | Parent là descendant | `422 ORG_CYCLE` |
| TC-033 | Xóa department có active users | Chặn và trả dependency count |
| TC-034 | Import GeoJSON polygon hợp lệ | Preview area và publish version |
| TC-035 | Polygon tự cắt | `422 GEOMETRY_INVALID` |
| TC-036 | Polygon chưa đóng | Server normalize hoặc reject rõ |
| TC-037 | Hai territory overlap cấm | Cảnh báo/chặn theo policy |
| TC-038 | Lead nằm trên boundary | Rule tie-break deterministic |
| TC-039 | Version territory tương lai | Chỉ active từ effective time |
| TC-040 | Manager xem polygon ngoài scope | Không trả geometry |

## 12.6 Customer create/dedupe - TC-041..TC-050

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-041 | Tạo lead đủ dữ liệu | `201`, status New, timeline/SLA |
| TC-042 | Phone `0901234567` | Normalize `+84901234567` |
| TC-043 | Phone sai độ dài | `422 PHONE_INVALID` |
| TC-044 | Exact phone duplicate | `409`, không tạo row |
| TC-045 | Tên+địa chỉ fuzzy match | Cảnh báo candidate, cho quyết định |
| TC-046 | Offline retry cùng idempotency key | Một customer duy nhất |
| TC-047 | Không geocode được | Lưu với needsLocation flag |
| TC-048 | Pin ngoài Việt Nam theo policy | `422 LOCATION_OUT_OF_SCOPE` |
| TC-049 | Source không tồn tại | `422 SOURCE_INVALID` |
| TC-050 | User không có create permission | `403`, DB không đổi |

## 12.7 Customer read/update - TC-051..TC-060

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-051 | Owner mở Customer 360 | Đủ tab theo permission |
| TC-052 | Kỹ thuật mở customer được handoff | Chỉ field cần triển khai |
| TC-053 | Sale khác team đoán UUID | `404/403` theo anti-enumeration policy |
| TC-054 | Update đúng ETag | `200`, version tăng, history |
| TC-055 | Update ETag cũ | `412/409`, không overwrite |
| TC-056 | Patch ownerId không allowlist | `422` hoặc field ignored có log |
| TC-057 | Soft-deleted customer trong list | Không xuất hiện mặc định |
| TC-058 | Auditor xem deleted tombstone | Thấy metadata, PII theo permission |
| TC-059 | Search không dấu tên Việt | Kết quả theo normalized search |
| TC-060 | Sort field injection | `400 SORT_INVALID` |

## 12.8 Pipeline/assignment/merge - TC-061..TC-070

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-061 | New -> Contacted | History và automation một lần |
| TC-062 | New -> Contracted trực tiếp | `422 TRANSITION_INVALID` |
| TC-063 | Qualified -> Lost thiếu reason | `422 LOSS_REASON_REQUIRED` |
| TC-064 | Manager override transition | Thành công với reason/audit |
| TC-065 | Assign active in-scope Sale | Một primary owner |
| TC-066 | Assign locked user | `422 ASSIGNEE_INACTIVE` |
| TC-067 | Hai assignment đồng thời | Một thắng, một conflict |
| TC-068 | Merge giữ survivor | Child records chuyển, redirect tồn tại |
| TC-069 | Merge contract conflict | Chặn và yêu cầu resolution |
| TC-070 | Mark not-duplicate | Pair suppression lưu, không merge |

## 12.9 Import - TC-071..TC-080

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-071 | CSV UTF-8 template đúng | Dry-run counts chính xác |
| TC-072 | XLSX 10.000 dòng | Job async, progress |
| TC-073 | File > 20 MB | `413` |
| TC-074 | MIME giả `.xlsx` | Quarantine/reject |
| TC-075 | Header bắt buộc thiếu | Mapping error trước import |
| TC-076 | 5% dòng lỗi | Import dòng hợp lệ, report lỗi |
| TC-077 | >20% lỗi | Không commit theo policy |
| TC-078 | Exact duplicate rows | Skip/update theo lựa chọn |
| TC-079 | Dòng ngoài branch scope | Reject từng dòng |
| TC-080 | Retry confirm import | Không tạo batch/customer trùng |

## 12.10 Interaction/notes/files - TC-081..TC-090

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-081 | Ghi call outcome Callback | Interaction + reminder |
| TC-082 | Note 10.001 ký tự | `422 MAX_LENGTH` |
| TC-083 | Offline note replay | Giữ occurredAt, một record |
| TC-084 | Voice draft chưa confirm | Không lưu system of record |
| TC-085 | Upload JPEG hợp lệ 2 MB | Scan clean, attachment available |
| TC-086 | Executable đổi đuôi JPEG | Magic-byte reject/quarantine |
| TC-087 | File chứa malware test signature | Quarantine, alert, không download |
| TC-088 | Signed URL hết 5 phút | Access denied |
| TC-089 | User ngoài resource tải file | `403/404` |
| TC-090 | Delete attachment | Unlink/audit; blob lifecycle đúng |

## 12.11 Visit scheduling - TC-091..TC-100

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-091 | Tạo visit tương lai hợp lệ | Scheduled + reminder |
| TC-092 | End <= start | `422 TIME_RANGE_INVALID` |
| TC-093 | Time nhập Asia/Ho_Chi_Minh | UTC lưu đúng |
| TC-094 | Assignee có lịch overlap | Warning/chặn theo policy |
| TC-095 | Manager override overlap | Lưu reason |
| TC-096 | Customer thiếu location | Yêu cầu pin/address trước submit |
| TC-097 | Reschedule visit | Version tăng, notify liên quan |
| TC-098 | Cancel visit có reason | Reminder cancel, history giữ |
| TC-099 | Recurring visit DST timezone khác | Occurrence đúng timezone user |
| TC-100 | Sale xem visit người khác | Chỉ shared/in-scope |

## 12.12 Check-in/out - TC-101..TC-110

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-101 | 50 m, accuracy 20 m | Valid |
| TC-102 | 150 m với radius 100 | Review |
| TC-103 | 250 m với radius 100 | Reject |
| TC-104 | Accuracy 150 m trong geofence | Review |
| TC-105 | Accuracy 250 m | Reject |
| TC-106 | Device time lệch 10 phút | Review |
| TC-107 | Mock location true | Reject/security flag |
| TC-108 | Offline valid check-in | Pending rồi sync, server tính distance |
| TC-109 | Check-out thiếu required outcome | `422` |
| TC-110 | Checkout trước checkin | Exception path/override, không silently complete |

## 12.13 Check-in review - TC-111..TC-120

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-111 | Manager approve Review | Final Approved + audit |
| TC-112 | Manager reject với reason | Final Rejected + notify |
| TC-113 | Reason <10 ký tự | Validation fail |
| TC-114 | User tự duyệt check-in | `403` |
| TC-115 | Manager khác branch duyệt | `403` |
| TC-116 | Hai reviewer cùng quyết định | Một success, một conflict |
| TC-117 | Request more info | State AwaitingEvidence |
| TC-118 | Raw coordinate sau approve | Không thay đổi |
| TC-119 | Evidence quarantined | Không approve cho tới resolved |
| TC-120 | Review queue 10.000 item | Cursor/page P95 đạt SLO |

## 12.14 Tracking session - TC-121..TC-130

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-121 | Start sau consent | Active session + indicator |
| TC-122 | Start không consent | Không thu/lưu điểm |
| TC-123 | Start khi session active | `409 SESSION_ACTIVE` |
| TC-124 | GPS permission denied | Reason path, không tracking ngầm |
| TC-125 | Stop online | Flush, close, aggregate job |
| TC-126 | Stop offline | Local stop ngay, command queued |
| TC-127 | Auto-stop end-of-day | Close reason System, notify |
| TC-128 | App restart giữa ca | Khôi phục state/session |
| TC-129 | Token refresh giữa tracking | Ingest tiếp, không mất sequence |
| TC-130 | User locked giữa ca | Stop/reject ingest sau grace |

## 12.15 GPS ingest/quality - TC-131..TC-140

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-131 | Batch 100 điểm đúng sequence | `202`, accepted through 100 |
| TC-132 | Batch 101 điểm | `413/422` |
| TC-133 | Retry cùng key | Point count không tăng |
| TC-134 | Sequence duplicate khác payload | `409 IDEMPOTENCY_CONFLICT` |
| TC-135 | Latitude 91 | Reject điểm cụ thể |
| TC-136 | Timestamp 1 giờ tương lai | Reject/flag |
| TC-137 | Accuracy 300 m | Lưu quality Bad, loại KPI |
| TC-138 | Impossible speed 300 km/h | Flag anomaly, không tự kết tội |
| TC-139 | Late points trong 15 phút grace | Accept |
| TC-140 | Late points sau grace | Reject với code |

## 12.16 Route/history/map - TC-141..TC-150

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-141 | Route có GPS gap 20 phút | Polyline ngắt tại gap |
| TC-142 | Simplification zoom thấp | Ít điểm, shape trong tolerance |
| TC-143 | Distance aggregate known fixture | Sai số <= 2% |
| TC-144 | Manager xem team route | Thành công + GPS access audit |
| TC-145 | Manager xem ngoài team | `403` |
| TC-146 | Raw points quá retention | Chỉ summary/anonymized |
| TC-147 | 2.000 customer markers | Cluster render <2 giây target |
| TC-148 | Nearby radius 5 km | Đúng thứ tự distance |
| TC-149 | Map provider timeout | List/address fallback |
| TC-150 | Navigation URL special chars | URL encode, đúng destination |

## 12.17 Reminder - TC-151..TC-160

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-151 | Reminder một lần | Job đúng due UTC |
| TC-152 | Due quá khứ | Reject hoặc explicit immediate |
| TC-153 | Weekly recurrence | Tạo đúng occurrence kế |
| TC-154 | Complete recurring | Không sửa history cũ |
| TC-155 | Snooze 30 phút | OriginalDue giữ nguyên |
| TC-156 | Automation retry | Một reminder theo automation key |
| TC-157 | Overdue 24 giờ | Escalation manager một lần |
| TC-158 | Private reminder overdue | Không escalate |
| TC-159 | Assignee locked | Reassignment/exception queue |
| TC-160 | 100k reminder cùng phút | Job throughput, không duplicate |

## 12.18 Notification - TC-161..TC-170

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-161 | In-app event mới | Unread count +1 <=5 giây |
| TC-162 | Duplicate event key | Một notification |
| TC-163 | Mark read | `readAt` set, count giảm |
| TC-164 | Read-all before timestamp | Chỉ item phù hợp |
| TC-165 | Quiet hours push | Delay đúng timezone |
| TC-166 | Critical security event | Bỏ quiet hours có nhãn |
| TC-167 | Provider 500 | Retry exponential |
| TC-168 | Retry max | Dead-letter + alert |
| TC-169 | Deep link resource deleted | Safe not-found screen |
| TC-170 | SignalR reconnect | Không mất inbox; không gửi duplicate |

## 12.19 Contract/handoff - TC-171..TC-180

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-171 | Contract draft valid | Tạo metadata |
| TC-172 | External reference trùng | `409` |
| TC-173 | Value âm | `422` |
| TC-174 | Mark Signed thiếu document/reference | Chặn |
| TC-175 | Handoff đủ checklist | Submit và auto-create/approval |
| TC-176 | Handoff thiếu pin | `422 HANDOFF_INCOMPLETE` |
| TC-177 | Duplicate active handoff | `409` |
| TC-178 | Four-eyes submitter tự approve | `403` |
| TC-179 | Reject handoff | Sale notified, reason stored |
| TC-180 | Contract canceled khi pending | Handoff invalidated |

## 12.20 Work order - TC-181..TC-190

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-181 | Assign technician đúng skill | Assigned + SLA |
| TC-182 | Assign locked technician | Reject |
| TC-183 | Technician accept own order | InProgress/Accepted |
| TC-184 | Technician access order khác | `403` |
| TC-185 | Complete đủ checklist/evidence | Completed |
| TC-186 | Complete thiếu checkout | `422` |
| TC-187 | Fail CustomerAbsent | Failure reason + revisit option |
| TC-188 | Revisit tạo child | Parent immutable, child linked |
| TC-189 | Vượt max attempts | Approval required |
| TC-190 | SLA overdue | Alert/metric một lần |

## 12.21 Dashboard/export - TC-191..TC-200

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-191 | Personal dashboard today | Chỉ KPI user |
| TC-192 | Team funnel | Tổng bằng drill-down cùng cutoff |
| TC-193 | Previous-period compare | Date boundaries đúng |
| TC-194 | Read model trễ | Hiển thị freshness timestamp |
| TC-195 | Một widget lỗi | Các widget khác vẫn hiển thị |
| TC-196 | Export 4.000 rows | Sync/stream theo policy |
| TC-197 | Export 100.000 rows | `202` async |
| TC-198 | Export restricted field thiếu quyền | Field loại/mask |
| TC-199 | Download sau 24 giờ | Link expired |
| TC-200 | Export/download | Hai audit events có purpose |

## 12.22 Audit/settings - TC-201..TC-210

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-201 | Customer update | Audit before/after redacted |
| TC-202 | Audit mutation qua API | Không endpoint/`405` |
| TC-203 | Hash chain tamper fixture | Verification phát hiện |
| TC-204 | Audit query ngoài scope | Không trả record |
| TC-205 | GPS access | Restricted access audit |
| TC-206 | Update setting đúng schema | Version mới/effective |
| TC-207 | Geofence radius 5 m | Validation reject |
| TC-208 | Generic setting chứa secret | Reject; hướng dẫn vault |
| TC-209 | Rollback setting | Tạo version mới, không xóa cũ |
| TC-210 | Concurrent setting publish | Optimistic conflict |

## 12.23 AI summary/note - TC-211..TC-220

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-211 | Summary grounded fixture | Facts có citation đúng |
| TC-212 | Timeline mâu thuẫn | Nêu mâu thuẫn, không tự chọn |
| TC-213 | Thiếu dữ liệu | `missingInformation`, không bịa |
| TC-214 | Note chứa prompt injection | Instruction bị coi là data |
| TC-215 | Note yêu cầu system prompt | Không tiết lộ |
| TC-216 | Restricted event trong context | Không retrieve/output |
| TC-217 | Provider timeout | Fallback/cache, CRM không lỗi |
| TC-218 | Output JSON sai schema | Reject/retry bounded |
| TC-219 | User feedback negative | AiFeedback lưu |
| TC-220 | Cùng input hash/model | Cache theo policy |

## 12.24 AI score/suggestion/route - TC-221..TC-230

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-221 | Eligible customer | Score/band/factors/version |
| TC-222 | Insufficient features | Không tạo điểm giả |
| TC-223 | Protected attribute injected | Feature pipeline loại |
| TC-224 | Model drift vượt threshold | Kill switch/fallback |
| TC-225 | Next action accepted | Chỉ tạo draft/reminder sau confirm |
| TC-226 | Suggestion rejected | Không thay dữ liệu, feedback |
| TC-227 | Suggestion gửi khách tự động | Bị policy chặn |
| TC-228 | Route 10 visits khả thi | Tôn trọng time windows |
| TC-229 | Constraint bất khả thi | Nêu violations, không giả route |
| TC-230 | Maps route API down | Deterministic heuristic fallback |

## 12.25 Offline/sync - TC-231..TC-240

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-231 | Tạo lead offline, restart app | Draft/outbox còn nguyên |
| TC-232 | Sync lead khi online | Mapping local-server ID |
| TC-233 | Note phụ thuộc lead local | Gửi đúng thứ tự |
| TC-234 | Partial batch failure | Item độc lập vẫn sync |
| TC-235 | Auth hết hạn | Refresh rồi tiếp tục |
| TC-236 | Account revoked offline | Dừng sync, bảo vệ local data |
| TC-237 | Server record đổi cùng field | NeedsReview conflict |
| TC-238 | Server/local đổi field khác | Merge an toàn theo rule |
| TC-239 | Retry sau timeout không rõ kết quả | Idempotency tránh trùng |
| TC-240 | Local DB schema cũ | Migration hoặc safe block |

## 12.26 Security - TC-241..TC-250

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-241 | IDOR customer UUID | Scope chặn |
| TC-242 | Horizontal privilege check-in | Scope chặn |
| TC-243 | Stored XSS trong note | Encode khi render |
| TC-244 | SQL injection search | Parameterized, không delay/leak |
| TC-245 | CSRF khi cookie mode | Token/origin chặn |
| TC-246 | CORS unknown origin | Không ACAO |
| TC-247 | Path traversal filename | Storage key random |
| TC-248 | SSRF import URL nội bộ | Block private/metadata IP |
| TC-249 | Secret scan repository/image | Build fail |
| TC-250 | Dependency Critical CVE | Release gate fail/approved exception |

## 12.27 Performance - TC-251..TC-260

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-251 | Customer list 300 RPS | P95 <500 ms, error <1% |
| TC-252 | Customer write 100 RPS | P95 <800 ms |
| TC-253 | Nearby 2.000 candidates | P95 <500 ms |
| TC-254 | GPS 10M points/day profile | Không backlog vượt 5 phút |
| TC-255 | 5.000 SignalR concurrent | Stable reconnect/broadcast |
| TC-256 | Dashboard 200 concurrent | P95 <800 ms từ read model |
| TC-257 | Export load song song | API interactive không suy giảm >20% |
| TC-258 | 4-hour soak | Không memory/connection leak |
| TC-259 | Stress tới saturation | Graceful 429, phục hồi |
| TC-260 | DB index regression dataset | Query plan không scan bảng lớn bất ngờ |

## 12.28 Resilience/recovery - TC-261..TC-270

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-261 | Google Maps timeout | Circuit breaker/list fallback |
| TC-262 | Notification provider down 30m | Queue giữ/retry |
| TC-263 | AI provider down | Core CRM bình thường |
| TC-264 | Redis unavailable | DB fallback có rate protection |
| TC-265 | One API instance killed | LB chuyển, no session loss |
| TC-266 | Worker crash giữa job | Lock expiry/retry idempotent |
| TC-267 | SQL transient disconnect | Retry safe, không double commit |
| TC-268 | Restore latest backup | RPO <=15m, integrity pass |
| TC-269 | Full region/site fail drill | RTO <=2h |
| TC-270 | Corrupt backup sample | Phát hiện và dùng restore point khác |

## 12.29 Frontend/accessibility - TC-271..TC-280

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-271 | Keyboard login/customer flow | Không focus trap/lost focus |
| TC-272 | Screen reader form error | Label/error được đọc |
| TC-273 | Contrast light/dark | WCAG AA |
| TC-274 | Zoom 200% | Không mất action/content |
| TC-275 | Viewport 360 px | Không horizontal scroll core flow |
| TC-276 | Reduced motion | Animation không thiết yếu tắt |
| TC-277 | Map unavailable keyboard | List alternative đủ chức năng |
| TC-278 | Vietnamese text dài | Không truncate action quan trọng |
| TC-279 | Slow 3G loading | Skeleton, không duplicate submit |
| TC-280 | Browser back dirty form | Confirm/restore draft |

## 12.30 Privacy/retention - TC-281..TC-290

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-281 | Tracking trước consent | Zero point stored |
| TC-282 | User stop consent/session | Thu thập dừng ngay |
| TC-283 | Route raw quá 90 ngày | Deleted/anonymized theo policy |
| TC-284 | Aggregate sau raw delete | Không tái nhận diện cá nhân ngoài scope |
| TC-285 | Data access request | Export đủ resource được phép |
| TC-286 | Anonymize có dual approval | PII thay thế, evidence giữ |
| TC-287 | Legal hold active | Delete bị chặn có reason |
| TC-288 | PII trong application log | Scanner không tìm phone/token |
| TC-289 | AI provider payload | Không chứa raw GPS/full phone |
| TC-290 | User xem privacy notice | Version/acceptedAt lưu |

## 12.31 Deployment/observability - TC-291..TC-300

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-291 | `/health/live` không dependency ngoài | `200` khi process healthy |
| TC-292 | DB down `/health/ready` | `503`, instance khỏi LB |
| TC-293 | Request API | Log/trace/metric cùng traceId |
| TC-294 | HTTP 5xx >2% 5m | Alert đúng route/on-call |
| TC-295 | GPS lag >5m | Alert và dashboard |
| TC-296 | Deploy migration compatible | Old/new app cùng chạy rolling |
| TC-297 | Container chạy non-root | Pass policy |
| TC-298 | TLS/headers scan | TLS1.2+, HSTS/CSP đúng |
| TC-299 | Rollback app | Schema vẫn tương thích |
| TC-300 | Production smoke sau deploy | Login, customer read, check-in sandbox, health pass |

## 12.32 Data governance: bulk/restore/device/version - TC-301..TC-312

| ID | Scenario/Input | Expected |
|---|---|---|
| TC-301 | Bulk update dryRun 137 bản ghi | Trả `affectedCount`, không thay đổi dữ liệu |
| TC-302 | Bulk update vượt 500 dòng | `422 BULK_LIMIT_EXCEEDED`, gợi ý chia nhỏ |
| TC-303 | Bulk update có bản ghi ngoài scope | Bản ghi đó `skipped` với `OUT_OF_SCOPE`, dòng khác vẫn chạy |
| TC-304 | Bulk update gửi lại cùng job key | Idempotent, không áp dụng hai lần |
| TC-305 | Bulk update partial failure | Dòng thành công giữ nguyên, báo cáo success/skipped/failed |
| TC-306 | Restore customer trong window | Active trở lại, audit before/after, owner thông báo |
| TC-307 | Restore quá restore window | `409`/`410`, chỉ còn truy cập qua audit |
| TC-308 | Restore khi PhoneHash bị bản ghi khác chiếm | `409 RESTORE_CONFLICT`, yêu cầu xử lý trùng |
| TC-309 | User revoke một thiết bị | Session-family + push token thiết bị đó vô hiệu <= 5 giây |
| TC-310 | Revoke-all không reauth | Bị chặn, yêu cầu reauth/MFA |
| TC-311 | Xem diff hai phiên bản customer | Diff field-level đúng thứ tự thời gian, field nhạy cảm masked |
| TC-312 | Restore-field về phiên bản cũ | Sinh phiên bản/audit mới, không ghi đè lịch sử cũ |

## 12.33 Exit criteria

- [ ] 312 test case được map tới requirement/use case và có owner.
- [ ] 100% Critical/High, 95% tổng regression pass.
- [ ] Không flaky test vượt 1%; test quarantine có ticket và hạn.
- [ ] Security/performance/recovery gate đạt.
- [ ] UAT ký duyệt bởi Sale, Kỹ thuật, Manager và Admin đại diện.

