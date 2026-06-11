# Phụ lục A. Ma trận truy vết

Ma trận này là baseline tối thiểu. Một UC có thể được kiểm thử bởi nhiều TC hơn phạm vi ghi dưới đây; cột TC chỉ nêu dải regression chính.

| UC | Requirement | API | Database | UI | Test |
|---|---|---|---|---|---|
| UC-001 Login | FR-001, NFR-006 | API-001,002 | DB-03,08,10,32 | UI-01,02 | TC-001..010 |
| UC-002 Refresh | FR-001 | API-003 | DB-08,32 | App shell | TC-011..014 |
| UC-003 Logout | FR-001 | API-004,008,009 | DB-08,09 | UI-31 | TC-015..020 |
| UC-004 MFA | FR-002 | API-006,007 | DB-10,32 | UI-02,31 | TC-006..008 |
| UC-005 User management | FR-003 | API-010..013 | DB-02,03,06,32 | UI-32 | TC-021..024 |
| UC-006 Role/permission | FR-003 | API-014..017 | DB-04..07,32 | UI-32 | TC-025..030 |
| UC-007 Org/territory | FR-003,007 | API-018..022 | DB-02,16 | UI-32 | TC-031..040 |
| UC-008 Create lead | FR-004,007 | API-024,037,038 | DB-11,12,13,26 | UI-06 | TC-041..050 |
| UC-009 Import lead | FR-004 | API-028,029,063,064 | DB-11,30,34 | UI-07 | TC-071..080 |
| UC-010 Customer list | FR-004 | API-023 | DB-11,13,16 | UI-05 | TC-051,059,060 |
| UC-011 Customer 360 | FR-005 | API-025,034 | DB-11..19,24..31 | UI-09,10 | TC-051..053 |
| UC-012 Update lead | FR-004,005 | API-026 | DB-11,12,32 | UI-11 | TC-054..058 |
| UC-013 Merge duplicate | FR-004 | API-030,031 | DB-11,14,32 | UI-08 | TC-068..070 |
| UC-014 Pipeline transition | FR-006 | API-032 | DB-11,12,26 | UI-09 | TC-061..064 |
| UC-015 Assign lead | FR-008 | API-033 | DB-11,13,27 | UI-05,09 | TC-065..067 |
| UC-016 Interaction/note | FR-005,009 | API-035,063..065 | DB-15,30,31 | UI-10 | TC-081..090 |
| UC-017 Plan visit | FR-009 | API-039..043 | DB-18,26 | UI-13,14 | TC-091..100 |
| UC-018 Start tracking | FR-011 | API-048 | DB-09,21,32 | UI-17,18 | TC-121..124 |
| UC-019 GPS batch | FR-011,022 | API-049 | DB-21,22,34 | UI-18 | TC-131..140 |
| UC-020 Stop tracking | FR-011 | API-050 | DB-21,23,34 | UI-18 | TC-125..130 |
| UC-021 Route history | FR-011,017 | API-051,052 | DB-21..23,32 | UI-19 | TC-141..146 |
| UC-022 Check-in | FR-010,022 | API-044 | DB-18,20,30,31 | UI-15 | TC-101..108 |
| UC-023 Check-out | FR-009,010 | API-045 | DB-18..20 | UI-16 | TC-109,110 |
| UC-024 Review check-in | FR-010 | API-046,047 | DB-20,32 | UI-20 | TC-111..120 |
| UC-025 Nearby search | FR-007 | API-036 | DB-11,16 | UI-12 | TC-147,148 |
| UC-026 Navigate | FR-007 | API-037,038 | DB-11,18 | UI-12,14 | TC-149,150 |
| UC-027 Create reminder | FR-012 | API-054 | DB-26,34 | UI-22 | TC-151..153,156 |
| UC-028 Complete/snooze | FR-012 | API-055..057 | DB-26 | UI-21,22 | TC-154,155,157..159 |
| UC-029 Receive notification | FR-013,023 | API-058..060 | DB-27,28 | UI-23 | TC-161..164,167..170 |
| UC-030 Notification config | FR-013 | API-061,062 | DB-29 | UI-31 | TC-165,166 |
| UC-031 Contract metadata | FR-014 | API-066,067 | DB-17,30,31 | UI-24 | TC-171..174 |
| UC-032 Technical handoff | FR-014,015 | API-068 | DB-17,24,25 | UI-25 | TC-175..177 |
| UC-033 Approve handoff | FR-014 | API-069 | DB-24,25,32 | UI-26 | TC-178..180 |
| UC-034 Assign work order | FR-015 | API-070,071 | DB-25,27 | UI-27 | TC-181,182 |
| UC-035 Execute work order | FR-015 | API-072..074 | DB-18..20,25,30,31 | UI-28 | TC-183..190 |
| UC-036 Revisit | FR-015 | API-075 | DB-25 | UI-28 | TC-187..189 |
| UC-037 Personal dashboard | FR-017 | API-076 | Read models, DB-34 | UI-03 | TC-191,194,195 |
| UC-038 Manager dashboard | FR-017 | API-077 | Read models, DB-34 | UI-04 | TC-192..195 |
| UC-039 Export | FR-018 | API-078,079 | DB-30,32,34 | UI-29 | TC-196..200 |
| UC-040 Audit search | FR-019 | API-080 | DB-32 | UI-32 | TC-201..205 |
| UC-041 File management | FR-016 | API-063..065 | DB-30,31 | Shared uploader | TC-085..090 |
| UC-042 Settings | FR-021,024 | API-081,082 | DB-33,32 | UI-32 | TC-206..210 |
| UC-043 Devices/sessions | FR-001,003 | API-008,009 | DB-08,09 | UI-31 | TC-015..017 |
| UC-044 AI summary | FR-020 | API-083,087 | DB-35,36 | UI-30 | TC-211..220 |
| UC-045 AI next action | FR-020 | API-084,087 | DB-26,35,36 | UI-30 | TC-225..227 |
| UC-046 AI score | FR-020 | API-085 | DB-11,35 | UI-09,30 | TC-221..224 |
| UC-047 AI route | FR-020 | API-086 | DB-18,35 | UI-13,30 | TC-228..230 |
| UC-048 Offline sync | FR-022 | APIs command tương ứng | Client outbox + dedup columns | UI-06,15,18,28 | TC-231..240 |
| UC-049 Data request | FR-024, NFR-012 | Admin workflow | DB-11..36 theo discovery | UI-32 | TC-281..290 |
| UC-050 Operations | NFR-001..012 | API-088,089 | DB-34 + telemetry | Ops dashboards | TC-251..270,291..300 |
| UC-051 Bulk update | FR-025, BR-016 | API-090,098 | DB-11,32,34 | UI-05 | TC-301..305 |
| UC-052 Restore deleted | FR-026, BR-017 | API-091,092 | DB-11,32 | UI-05,32 | TC-306..308 |
| UC-053 Device management | FR-027, BR-018 | API-093..095 | DB-08,09,32 | UI-31 | TC-309,310 |
| UC-054 Version history/diff | FR-028 | API-096,097 | DB-32 + history | UI-09,10 | TC-311,312 |

## Traceability gate

- [ ] Mọi requirement Must có ít nhất một UC.
- [ ] Mọi command UC có API, persistence và negative test.
- [ ] Mọi UI action ghi có permission, error state và idempotency/concurrency strategy.
- [ ] Mọi bảng chứa Restricted data có retention, audit và scoped access.
- [ ] Mọi NFR có metric và test hoặc runbook chứng minh.

