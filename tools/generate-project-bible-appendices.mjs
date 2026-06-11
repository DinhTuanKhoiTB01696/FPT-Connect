import fs from "node:fs";
import path from "node:path";

const root = path.resolve("docs/Project-Bible");

function read(name) {
  return fs.readFileSync(path.join(root, name), "utf8");
}

function write(name, content) {
  fs.writeFileSync(path.join(root, name), content.replace(/\n{3,}/g, "\n\n"), "utf8");
}

function clean(value) {
  return value
    .trim()
    .replace(/^`|`$/g, "")
    .replace(/<br\s*\/?>/gi, "; ");
}

function parseApiRows() {
  return read("08_API.md")
    .split(/\r?\n/)
    .map((line) => line.match(/^\|\s*(API-\d{3})\s*\|\s*`([^`]+)`\s*\|\s*([^|]+)\|\s*([^|]+)\|\s*([^|]+)\|$/))
    .filter(Boolean)
    .map((match) => ({
      id: match[1],
      operation: match[2].trim(),
      permission: clean(match[3]),
      request: clean(match[4]),
      response: clean(match[5]),
    }));
}

function apiPurpose(operation) {
  const [method, endpoint] = operation.split(/\s+/, 2);
  const resource = endpoint
    .replace(/\{[^}]+\}/g, "tài nguyên xác định")
    .replace(/^\/+/, "")
    .replaceAll("/", " / ");
  const verb = {
    GET: "Truy vấn",
    POST: "Thực thi lệnh hoặc tạo",
    PUT: "Thay thế/công bố",
    PATCH: "Cập nhật một phần",
    DELETE: "Thu hồi hoặc xóa",
  }[method] ?? "Xử lý";
  return `${verb} ${resource} theo đúng organizational scope và business rule của use case liên quan.`;
}

function apiStatusRows(api) {
  const method = api.operation.split(" ")[0];
  const rows = [];
  if (method === "POST") rows.push("| 200/201/202/204 | Lệnh thành công theo response contract ở trên. |");
  if (method === "GET") rows.push("| 200 | Trả representation hoặc page; dữ liệu đã mask và scope. |");
  if (method === "PUT" || method === "PATCH") rows.push("| 200/204 | Cập nhật thành công; trả version mới khi có aggregate. |");
  if (method === "DELETE") rows.push("| 204 | Kết quả idempotent; resource không còn active/accessible. |");
  if (api.permission === "Public" && /auth\/login/.test(api.operation)) {
    rows.push("| 401 | Credential không hợp lệ; response không tiết lộ identifier có tồn tại. |");
  } else if (api.permission === "Challenge") {
    rows.push("| 401 | Challenge/OTP hết hạn, sai hoặc đã dùng; chưa tạo access session. |");
  } else if (api.permission === "Refresh") {
    rows.push("| 401 | Refresh token hết hạn/revoke/reuse; reuse làm revoke token family. |");
  } else if (!["Public", "Infrastructure"].includes(api.permission)) {
    rows.push("| 401 | Access token thiếu, hết hạn hoặc không hợp lệ. |");
    rows.push("| 403 | Thiếu permission hoặc resource ngoài organizational scope. |");
  }
  rows.push("| 400/422 | Request malformed hoặc vi phạm validation/business rule; có field error code. |");
  if (!api.operation.startsWith("GET ")) rows.push("| 409/412 | Idempotency, trạng thái hoặc rowversion/ETag xung đột. |");
  rows.push("| 429 | Vượt rate limit/quota của endpoint. |");
  rows.push("| 503 | Dependency tạm lỗi; không làm mất hoặc nhân đôi side effect. |");
  return rows.join("\n");
}

function apiSecurity(api) {
  const notes = [];
  if (api.permission === "Public") {
    notes.push("Endpoint public phải rate-limit theo IP/device fingerprint, dùng lỗi không tiết lộ account và không ghi credential vào log.");
  } else if (api.permission === "Challenge") {
    notes.push("Xác thực bằng challenge ngắn hạn thay cho Bearer access token. Challenge phải bind với login attempt/device, hết hạn nhanh và dùng một lần.");
  } else if (api.permission === "Refresh") {
    notes.push("Xác thực bằng refresh token; server chỉ lưu hash, rotation trong transaction và revoke toàn family khi phát hiện reuse.");
  } else if (api.permission === "Infrastructure") {
    notes.push("Chỉ expose qua network/probe được kiểm soát; response không chứa connection string, version nhạy cảm hoặc chi tiết dependency.");
  } else {
    notes.push(`Authorization bắt buộc: \`${api.permission}\`; permission chỉ là điều kiện đầu, query/command vẫn phải kiểm tra tenant và resource scope.`);
  }
  if (/files|exports|audit|tracking|check-ins|ai\//.test(api.operation)) {
    notes.push("Đây là bề mặt dữ liệu nhạy cảm: bắt buộc access audit, data minimization, retention và response masking phù hợp.");
  }
  if (/auth|sessions|roles|settings/.test(api.operation)) {
    notes.push("Thao tác quản trị/xác thực phải có security event; endpoint có rủi ro cao cần reauthentication hoặc MFA theo policy.");
  }
  return notes.map((note) => `- ${note}`).join("\n");
}

function apiIdempotency(api) {
  const [method] = api.operation.split(" ");
  if (method === "GET") return "Read-only; cache chỉ được dùng khi không làm lộ dữ liệu giữa scope. Cursor và filter phải tạo kết quả ổn định trong cutoff.";
  if (/login|refresh|mfa\/verify/.test(api.operation)) return "Không dùng response cache. Replay bị kiểm soát bằng challenge/token state, transaction và rate limit; refresh rotation phải chống race.";
  return "Client gửi `Idempotency-Key` cho command có thể retry. Server lưu fingerprint request và kết quả; cùng key khác payload trả `409 IDEMPOTENCY_CONFLICT`.";
}

function generateApiAppendix() {
  const apis = parseApiRows();
  const sections = apis.map((api) => {
    const [method, endpoint] = api.operation.split(/\s+/, 2);
    const concurrency = method === "GET"
      ? "Read-only; dùng cursor/cutoff khi phân trang."
      : /auth|health/.test(endpoint)
        ? "Theo state/challenge/token; không áp dụng ETag aggregate."
        : "Command cập nhật aggregate nhận `If-Match` khi có rowversion; response trả version mới.";
    const authHeader = api.permission === "Public"
      ? "Không yêu cầu `Authorization`; vẫn gửi `X-Correlation-ID` và device context allowlisted khi contract cần."
      : api.permission === "Challenge"
        ? "Gửi challenge ID/token ngắn hạn theo request contract; không yêu cầu Bearer access token."
        : api.permission === "Refresh"
          ? "Gửi refresh token trong body/cookie bảo vệ theo auth design; không dùng access token hết hạn để xác thực refresh."
          : api.permission === "Infrastructure"
            ? "Probe qua network/identity hạ tầng được kiểm soát; không dùng user Bearer token."
            : "Gửi `Authorization: Bearer <access_token>`, `X-Correlation-ID` và `Accept-Language`.";
    return `## ${api.id} - \`${method} ${endpoint}\`

**Mục đích.** ${apiPurpose(api.operation)}

| Thuộc tính | Contract |
|---|---|
| Permission/scope | ${api.permission} |
| Request và validation đặc thù | ${api.request} |
| Success response | ${api.response} |
| Content type | \`application/json; charset=utf-8\`, trừ upload/download đã ký |
| Version/concurrency | ${concurrency} |

### Request contract

- Header/auth: ${authHeader} Command retryable thêm \`Idempotency-Key\` khi contract cho phép.
- Path/query parameter phải parse theo kiểu mạnh, có allowlist cho filter/sort/include và giới hạn kích thước.
- Contract đặc thù: **${api.request}**.
- Không bind trực tiếp entity. Unknown field bị reject hoặc bỏ theo policy thống nhất và có contract test.

### Response contract

- Thành công: **${api.response}**. JSON dùng camelCase, timestamp UTC ISO 8601, public ID UUID.
- Response chỉ chứa field actor được phép xem. Phone/email/GPS/file URL được mask hoặc cấp ngắn hạn theo permission.
- List trả \`data[]\` và \`meta.nextCursor\`; command async trả job/resource status có thể poll.

### Status và error

| Status | Điều kiện |
|---:|---|
${apiStatusRows(api)}

### Security notes

${apiSecurity(api)}

### Idempotency, consistency và cache

${apiIdempotency(api)}

### Observability và test bắt buộc

- Log event có \`traceId\`, endpoint template, status, duration, tenant pseudonym; không log request body nhạy cảm.
- Metric tối thiểu: request count, P95/P99 latency, error code, rate-limit count; command async thêm queue age và completion.
- Contract test phải phủ success, validation boundary, unauthenticated, permission denied, resource ngoài scope, conflict/replay và dependency failure.
`;
  });

  return `# Phụ lục C. API Contract chi tiết

> Sinh tự động từ catalog chuẩn trong \`08_API.md\`. Khi thay endpoint, cập nhật catalog và chạy \`node tools/generate-project-bible-appendices.mjs\`.

## C.1 Quy tắc áp dụng

Mỗi endpoint bên dưới kế thừa error envelope RFC 9457, UTC, correlation ID, cursor pagination, idempotency và security baseline của chương 8. Phụ lục làm rõ Definition of Done cho implementer và contract tester; OpenAPI sinh từ code vẫn là schema executable.

${sections.join("\n")}
`;
}

function parseDatabaseRows() {
  return read("06_Database.md")
    .split(/\r?\n/)
    .map((line) => line.match(/^\|\s*(DB-\d{2})\s+`([^`]+)`\s*\|\s*(.+?)\s*\|\s*(.+?)\s*\|\s*(.+?)\s*\|$/))
    .filter(Boolean)
    .map((match) => ({
      id: match[1],
      table: match[2],
      columnsRaw: match[3],
      indexes: match[4],
      description: match[5],
    }));
}

const columnDescriptions = {
  Id: "Khóa định danh nội bộ, không expose trực tiếp ra API.",
  PublicId: "UUID public dùng trong URL và integration.",
  TenantId: "Tenant sở hữu bản ghi; luôn tham gia authorization scope.",
  DepartmentId: "Đơn vị tổ chức chịu trách nhiệm hoặc sở hữu.",
  UserId: "Người dùng liên quan trực tiếp đến bản ghi.",
  CustomerId: "Khách hàng/lead cha của dữ liệu nghiệp vụ.",
  VisitId: "Visit cha của check-in, note hoặc kết quả.",
  SessionId: "Route/session cha; dùng partition và truy vấn tuyến.",
  Status: "Trạng thái hiện tại, bị giới hạn bởi state machine/check constraint.",
  CreatedAtUtc: "Thời điểm tạo do server ghi ở UTC.",
  UpdatedAtUtc: "Thời điểm cập nhật gần nhất do server ghi ở UTC.",
  RowVersion: "Concurrency token SQL Server dùng cho optimistic concurrency.",
  IsDeleted: "Cờ soft-delete; query nghiệp vụ mặc định loại bỏ.",
};

function inferColumnDescription(name, table) {
  if (columnDescriptions[name]) return columnDescriptions[name];
  if (/AtUtc$/.test(name)) return "Thời điểm UTC của sự kiện được nêu bởi tên cột.";
  if (/By$|ById$/.test(name)) return "Actor thực hiện hành động; tham chiếu user khi áp dụng.";
  if (/Hash$/.test(name)) return "Giá trị băm một chiều để so khớp/kiểm tra; không thể dùng khôi phục dữ liệu gốc.";
  if (/Encrypted$/.test(name)) return "Dữ liệu nhạy cảm đã mã hóa; không dùng trực tiếp trong log hoặc index bản rõ.";
  if (/Json$/.test(name)) return "JSON có schema version và `ISJSON`; application validate schema trước lưu.";
  if (/Code$/.test(name)) return "Mã machine-readable thuộc catalog/allowlist, ổn định cho API và báo cáo.";
  if (/Name$/.test(name)) return "Tên hiển thị, Unicode, được trim và giới hạn độ dài.";
  if (/Latitude|Longitude|Geo|Boundary/.test(name)) return "Dữ liệu không gian WGS84; validate bounds và xử lý theo quyền GPS/location.";
  if (/Version/.test(name)) return "Phiên bản dùng audit, concurrency hoặc effective configuration.";
  return `Thuộc tính nghiệp vụ của \`${table}\`; validation và khả năng sửa phụ thuộc aggregate/use case sở hữu.`;
}

function inferColumnType(name) {
  if (/PublicId|FamilyId|ClientCommandId|DeduplicationKey/.test(name)) return "uniqueidentifier";
  if (name === "Id" || /Id$/.test(name)) return "bigint";
  if (/AtUtc$/.test(name)) return "datetime2(3)";
  if (/Date$/.test(name)) return "date";
  if (/^Is|Enabled$|Flag$/.test(name)) return "bit";
  if (/Latitude|Longitude/.test(name)) return "decimal(9,6)";
  if (/Accuracy|Distance|Speed|Heading|Confidence|Score|Value/.test(name)) return "decimal";
  if (/Geo$|Boundary$/.test(name)) return "geography";
  if (/Hash$/.test(name)) return "binary(32)";
  if (/Encrypted$|SecretEncrypted/.test(name)) return "varbinary(max)";
  if (/Json$|PolylineEncoded|Content|Address|Description|Comment|ReasonText|LastError/.test(name)) return "nvarchar(max)";
  if (/Count$|Attempts$|AttemptCount|Sequence|Version$|Seconds$|LatencyMs|TokenIn|TokenOut/.test(name)) return "int";
  if (/Status|Type|Code|Action|Resource|Source|Channel|Severity|Priority|Outcome|Purpose|Platform|RiskStatus/.test(name)) return "varchar(100)";
  if (/Name|Title|Email|Phone|UserAgent|Reference|Timezone|DeepLink/.test(name)) return "nvarchar(500)";
  return "nvarchar(500)";
}

function parseColumns(raw, table) {
  const codeParts = [...raw.matchAll(/`([^`]+)`/g)].map((match) => match[1]);
  const rows = [];
  for (const part of codeParts) {
    const first = part.trim().split(/\s+/)[0];
    if (!/^[A-Za-z][A-Za-z0-9]*$/.test(first)) continue;
    const typeMatch = part.match(/^[A-Za-z][A-Za-z0-9]*\s+([A-Za-z]+(?:\([^)]*\))?)/);
    const parsedType = typeMatch?.[1];
    const type = !parsedType || ["PK", "FK", "NULL"].includes(parsedType)
      ? inferColumnType(first)
      : parsedType;
    const key = /\bPK\b/.test(part) ? "PK" : /\bFK\b/.test(part) ? "FK" : "-";
    const defaultMatch = part.match(/\bDF\s+([^;]+)/);
    const constraints = [
      /\bUQ\b/.test(part) ? "Unique" : "",
      /\bCK\b/.test(part) ? part.slice(part.indexOf("CK")) : "",
      /\bNULL\b/.test(part) ? "Nullable" : "Not null theo model",
    ].filter(Boolean).join("; ");
    rows.push({
      name: first,
      type,
      key,
      defaultValue: defaultMatch?.[1]?.trim() ?? "-",
      constraints,
      description: inferColumnDescription(first, table),
    });
  }
  return rows;
}

function generateDatabaseAppendix() {
  const tables = parseDatabaseRows();
  const sections = tables.map((item) => {
    const columns = parseColumns(item.columnsRaw, item.table);
    const columnRows = columns.map((column) =>
      `| \`${column.name}\` | \`${column.type}\` | ${column.key} | ${column.defaultValue} | ${column.constraints} | ${column.description} |`
    ).join("\n");
    return `## ${item.id} - \`${item.table}\`

**Mục đích:** ${item.description}

| Column | Data type | PK/FK | Default | Constraint/nullability | Description |
|---|---|---|---|---|---|
${columnRows}

**Index:** ${item.indexes}

**Quy tắc bổ sung**

- Cột FK dùng \`NO ACTION\` trừ child detail được tài liệu cho phép cascade; mọi delete nghiệp vụ đi qua aggregate.
- Các cột string được trim, giới hạn theo EF configuration và không nhận control character không cần thiết.
- Nếu bảng là aggregate mutable, áp dụng audit columns và \`RowVersion\` chuẩn ở mục D.1 dù catalog rút gọn không lặp lại.
- Query phải dùng index theo access pattern đã nêu; index mới chỉ thêm sau khi kiểm tra execution plan và write amplification.
`;
  });

  return `# Phụ lục D. Từ điển cột dữ liệu

> Sinh tự động từ \`06_Database.md\`; catalog chương 6 là nguồn chuẩn về bảng và index.

## D.1 Cột chuẩn dùng lại

| Column | Data type | Default/constraint | Description |
|---|---|---|---|
| \`CreatedAtUtc\` | \`datetime2(3)\` | Server UTC, not null | Thời điểm tạo |
| \`CreatedBy\` | \`bigint\` | FK user/service actor | Actor tạo |
| \`UpdatedAtUtc\` | \`datetime2(3)\` | Nullable | Lần cập nhật gần nhất |
| \`UpdatedBy\` | \`bigint\` | Nullable FK | Actor cập nhật |
| \`IsDeleted\` | \`bit\` | \`0\` | Soft-delete |
| \`RowVersion\` | \`rowversion\` | Generated | Optimistic concurrency |

${sections.join("\n")}
`;
}

function parseTestCases() {
  const lines = read("12_Testing.md").split(/\r?\n/);
  let group = "";
  const cases = [];
  for (const line of lines) {
    const heading = line.match(/^##\s+12\.\d+\s+(.+?)\s+-\s+TC-\d+\.\.TC-\d+/);
    if (heading) group = heading[1].trim();
    const row = line.match(/^\|\s*(TC-\d{3})\s*\|\s*([^|]+)\|\s*([^|]+)\|$/);
    if (row) cases.push({ id: row[1], scenario: clean(row[2]), expected: clean(row[3]), group });
  }
  return cases;
}

function testLayer(group) {
  if (/Security|Privacy/.test(group)) return "Security/API/Integration";
  if (/Performance/.test(group)) return "k6 performance";
  if (/Resilience|recovery|Deployment|observability/.test(group)) return "Resilience/operations";
  if (/Frontend|accessibility/.test(group)) return "Playwright + accessibility";
  if (/AI/.test(group)) return "AI evaluation + integration";
  if (/Offline/.test(group)) return "E2E offline + integration";
  return "Unit + API/integration; E2E khi có UI";
}

function testPreconditions(test) {
  const common = "Môi trường test cô lập; clock, UUID và provider có thể điều khiển; dữ liệu chỉ là synthetic.";
  const rules = [
    [/Authentication|Token|IAM/, "Tạo tenant active, user/role/session đúng trạng thái của scenario; lưu baseline session và audit count."],
    [/Organization|territory/, "Seed cây phòng ban và polygon WGS84 đã biết; actor có scope cụ thể để kiểm tra boundary."],
    [/Customer|Pipeline|Import|Interaction/, "Seed customer/owner/status/version và dữ liệu duplicate/search theo fixture; không dùng PII thật."],
    [/Visit|Check-in/, "Seed visit, geofence, assignee và tọa độ có khoảng cách/accuracy tính trước."],
    [/Tracking|GPS|Route/, "Seed device, consent và route session; sequence/timestamp/điểm GPS lấy từ deterministic fixture."],
    [/Reminder|Notification/, "Dùng fake clock và provider spy; queue/outbox bắt đầu rỗng."],
    [/Contract|Work order/, "Seed customer đủ điều kiện, contract/handoff/work order ở state trước hành động."],
    [/Dashboard|Audit|settings/, "Seed source events với cutoff rõ; ghi baseline aggregate/audit/settings version."],
    [/AI/, "Dùng model stub hoặc recorded safe fixture, prompt version cố định và retrieval scope đã biết."],
    [/Offline/, "Client local store mã hóa được reset; network có thể chuyển online/offline và server có fixture conflict."],
    [/Performance/, "Dataset production-like, warm-up tách khỏi measurement, threshold/SLO cấu hình trong test."],
    [/Resilience|recovery|Deployment|observability/, "Fault injection và môi trường staging/ephemeral có telemetry, backup hoặc deployment artifact cần thiết."],
  ];
  const specific = rules.find(([pattern]) => pattern.test(test.group))?.[1] ?? "Seed aggregate và actor đúng phạm vi của scenario; ghi baseline database, outbox, audit và metric.";
  return `${common} ${specific}`;
}

function testSteps(test) {
  const action = test.scenario;
  return `1. Arrange actor, permission/scope, aggregate state và dependency theo Preconditions; ghi lại row count, version, outbox/audit baseline.
2. Chuẩn bị input cụ thể: **${action}**; giữ nguyên correlation ID và idempotency key để có thể kiểm tra replay khi liên quan.
3. Thực thi qua tầng gần production nhất: HTTP/UI/job/provider adapter theo cột Automation; không gọi thẳng private method để bỏ qua pipeline.
4. Thu response/UI state, database transaction, domain event/outbox, audit, log/metric/trace và external-provider calls.
5. Lặp boundary/retry hoặc concurrent action nếu scenario yêu cầu; xác nhận không có side effect ngoài dự kiến.
6. Cleanup fixture hoặc rollback transaction; giữ artifact lỗi gồm trace ID, request đã redacted và screenshot/plan khi phù hợp.`;
}

function testAssertions(test) {
  return `- Oracle chính: **${test.expected}**.
- Status/error code và response schema đúng API contract; UI không diễn giải khác domain state.
- Database chỉ thay đổi các row/field được use case cho phép; rowversion, history và transaction nhất quán.
- Event/outbox/notification/audit xuất hiện đúng số lần; retry không nhân đôi business effect.
- Log không chứa token, password, phone/email đầy đủ, raw restricted payload hoặc stack trace trả về client.
- Với negative case, chứng minh resource ngoài scope không bị đọc/ghi và không để lại partial state.`;
}

function testPriority(test) {
  if (/auth|permission|scope|GPS|Check-in|Security|Privacy|backup|restore|rò|revoke/i.test(`${test.group} ${test.scenario}`)) return "P0/Critical";
  if (/Performance|Resilience|AI|Export|Work order|Handoff/.test(test.group)) return "P1/High";
  return "P1/High cho regression core; hạ P2 chỉ khi Product/QA phê duyệt.";
}

function generateTestAppendix() {
  const cases = parseTestCases();
  const sections = cases.map((test) => `## ${test.id} - ${test.scenario}

| Thuộc tính | Nội dung |
|---|---|
| Nhóm | ${test.group} |
| Priority | ${testPriority(test)} |
| Automation | ${testLayer(test.group)} |
| Requirement oracle | Chương 3, use case/API liên quan và expected của ${test.id} |

### Preconditions

${testPreconditions(test)}

### Steps

${testSteps(test)}

### Expected assertions

${testAssertions(test)}
`);

  return `# Phụ lục E. Phiếu thực thi 300 Test Case

> Sinh tự động từ catalog \`12_Testing.md\`. Mỗi mục là test specification có thể chuyển sang TestRail/Xray/Azure Test Plans; scenario và expected trong chương 12 là nguồn chuẩn.

## E.1 Quy ước bằng chứng

- API: request/response redacted, trace ID, database/outbox/audit assertions.
- UI: screenshot/video chỉ khi lỗi, DOM/accessibility assertion ưu tiên hơn pixel-only.
- Performance: script, dataset version, topology, raw result và percentile.
- Security: payload, affected control và evidence đã loại secret/PII.
- Recovery: backup ID, restore timeline, integrity check và actual RPO/RTO.

${sections.join("\n")}
`;
}

write("APPENDIX_C_API_Detailed.md", generateApiAppendix());
write("APPENDIX_D_Database_Columns.md", generateDatabaseAppendix());
write("APPENDIX_E_TestCase_Execution.md", generateTestAppendix());

console.log("Generated appendices C, D, E.");
