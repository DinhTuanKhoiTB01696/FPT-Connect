# 05. Business Flow

## 5.1 End-to-end flow

```mermaid
flowchart TD
  A[Sale thu thập lead] --> B{Trùng?}
  B -- Có --> C[Liên kết/đề nghị merge]
  B -- Không --> D[Geocode + địa bàn]
  C --> D
  D --> E[Phân công owner]
  E --> F[Liên hệ + reminder]
  F --> G{Qualified?}
  G -- Không --> H[Nurturing hoặc Lost có lý do]
  G -- Có --> I[Lập visit]
  I --> J[Start shift/tracking]
  J --> K[Điều hướng Google Maps]
  K --> L[Check-in geofence]
  L --> M[Ghi chú/ảnh/nhu cầu]
  M --> N{Đồng ý?}
  N -- Chưa --> F
  N -- Có --> O[Contract metadata]
  O --> P[Handoff kỹ thuật]
  P --> Q[Phân công work order]
  Q --> R[Kỹ thuật check-in]
  R --> S[Triển khai + biên bản]
  S --> T{Thành công?}
  T -- Không --> U[Exception/revisit]
  U --> Q
  T -- Có --> V[Hoàn thành + chăm sóc]
```

## 5.2 State machine khách hàng

```mermaid
stateDiagram-v2
  [*] --> New
  New --> Contacted
  Contacted --> Qualified
  Contacted --> Nurturing
  Qualified --> VisitPlanned
  VisitPlanned --> Proposal
  Proposal --> Contracted
  Contracted --> InstallationPending
  InstallationPending --> Active
  InstallationPending --> InstallationFailed
  InstallationFailed --> InstallationPending
  New --> Duplicate
  Contacted --> Lost
  Qualified --> Lost
  Proposal --> Lost
  Nurturing --> Contacted
  Lost --> Contacted: reopen + reason
```

## 5.3 Quy trình thu thập và phân công lead

| Step | Owner | Input | Xử lý | Output | SLA |
|---|---|---|---|---|---|
| 1 | Sale/System | Phone, name, address | Normalize, validate consent/source | Draft lead | 30 giây |
| 2 | System | Draft | Exact/fuzzy duplicate check | Match candidates | 2 giây |
| 3 | Sale | Candidate list | Link, request merge hoặc continue | Unique lead | 60 giây |
| 4 | System | Address/pin | Geocode, territory resolve | Coordinates/territory | 3 giây |
| 5 | Manager/rule | Lead | Assign owner by workload/area | Assignment | 15 phút |
| 6 | System | Assignment | Notify, create first-contact SLA | Task/reminder | Tức thời |

**Ngoại lệ:** địa chỉ không geocode được cho phép đặt pin thủ công; lead ngoài địa bàn vào queue `Unassigned`; lead VIP không auto-assign.

## 5.4 Follow-up và reminder

1. Sale ghi kết quả mỗi tương tác bằng outcome chuẩn.
2. Outcome `CallBack` bắt buộc thời gian tiếp theo; hệ thống tạo reminder.
3. Reminder gửi in-app trước 15 phút, push theo preference.
4. Quá hạn hiển thị đỏ; sau 24 giờ tạo escalation cho manager.
5. Hoàn thành reminder phải liên kết interaction hoặc reason `NoActionRequired`.
6. Reschedule quá ba lần trong 14 ngày được gắn cờ coaching.

## 5.5 GPS route tracking

```mermaid
sequenceDiagram
  actor U as Nhân viên
  participant W as Web/PWA
  participant A as API
  participant Q as Queue
  participant D as SQL Server
  U->>W: Bắt đầu ca + xác nhận consent
  W->>A: POST /tracking/sessions
  A->>D: Tạo session
  loop Adaptive interval
    W->>W: Thu GPS + accuracy
    W->>A: POST batch points (idempotency key)
    A->>Q: Enqueue validation
    Q->>D: Lưu raw + quality flag
  end
  U->>W: Dừng ca
  W->>A: POST /tracking/sessions/{id}/stop
  A->>D: Đóng session + aggregate
```

**Sampling:** 15-30 giây khi di chuyển, 60-180 giây khi đứng yên, ngừng khi hết ca. Client buffer tối đa 2.000 điểm; gửi batch khi có mạng. Server không tin timestamp/accuracy client tuyệt đối.

## 5.6 Check-in

| Kiểm tra | Pass | Review | Reject |
|---|---|---|---|
| Distance | <= radius | radius đến 2x radius | > 2x radius |
| Accuracy | <= 100 m | 101-200 m | > 200 m |
| Time skew | <= 5 phút | 5-15 phút | > 15 phút |
| Mock signal | Không | Không xác định | Có |
| Attachment required | Đủ | Upload pending offline | Thiếu sau sync |

Check-in `Review` không bị mất; tạo exception cho manager. Manual override bắt buộc reason, người duyệt và audit.

## 5.7 Hợp đồng và bàn giao kỹ thuật

```mermaid
sequenceDiagram
  actor S as Sale
  participant C as CRM
  actor M as Manager
  actor T as Kỹ thuật
  S->>C: Hoàn thiện contract metadata
  C->>C: Validate handoff checklist
  alt Đủ dữ liệu, không ngoại lệ
    C->>T: Tạo work order + thông báo
  else Có ngoại lệ
    C->>M: Yêu cầu phê duyệt
    M->>C: Approve/Reject + lý do
    C->>T: Tạo work order khi approve
  end
  T->>C: Accept và đặt lịch
  T->>C: Check-in, triển khai, biên bản
  C->>S: Thông báo kết quả
```

Checklist: contact, service address + pin, product/package, bandwidth, installation window, infrastructure note, customer consent, contract reference và attachment bắt buộc.

## 5.8 Quy trình ngoại lệ

| Event | Owner đầu tiên | Escalation | Resolution |
|---|---|---|---|
| Duplicate disputed | Data steward | Branch Admin | Merge/keep separate |
| Invalid check-in | Manager | Security khi lặp lại | Accept/reject |
| GPS gap > 15 phút | Employee | Manager | Reason + evidence |
| Handoff rejected | Sale | Sales Manager | Bổ sung/resubmit |
| Installation failed | Technician | Technical Manager | Reason + revisit |
| Notification dead-letter | System | SRE | Retry/provider switch |
| AI unsafe output | User | AI Owner/Security | Flag, quarantine, evaluate |

## 5.9 RACI

| Hoạt động | Sale | Kỹ thuật | Manager | Admin | System |
|---|---|---|---|---|---|
| Tạo lead | R | I | A | I | C |
| Merge lead | C | I | A | R | C |
| Follow-up | R/A | I | C | I | C |
| Check-in | R | R | A | I | C |
| Handoff | R | C | A | I | C |
| Work order | I | R | A | C | C |
| Permission | I | I | C | R/A | C |
| Retention | I | I | C | R | A |

## 5.10 Business continuity

- Maps lỗi: cho nhập địa chỉ/pin gần nhất và mở ứng dụng bản đồ ngoài bằng deep link khi khả dụng.
- AI lỗi: ẩn suggestion, không chặn CRM.
- Notification lỗi: in-app inbox vẫn là nguồn chính; retry exponential.
- Mất mạng: thao tác ghi vào local encrypted outbox; UI hiển thị `Pending sync`.
- Conflict: server dùng version; client cho người dùng chọn giữ server, giữ bản local hoặc merge trường cho notes.

## 5.11 Data governance: bulk, restore, device, version

```mermaid
flowchart TD
  A[Manager lọc tập bản ghi] --> B[Bulk update dryRun]
  B --> C{Preview hợp lệ?}
  C -- Không --> A
  C -- Có --> D[Áp dụng: job nền theo lô]
  D --> E[Audit theo dòng + báo cáo success/skipped/failed]
  F[Soft delete] --> G{Trong restore window 30 ngày?}
  G -- Có --> H[Restore: kiểm tra unique-conflict, khôi phục quan hệ, audit]
  G -- Không --> I[Chỉ truy cập qua audit/backup]
  J[User mở quản lý thiết bị] --> K[Revoke thiết bị/đăng xuất từ xa]
  K --> L[Vô hiệu session-family + push token]
```

- Bulk update: luôn có preview (dryRun), giới hạn 500 dòng, chạy nền với job key idempotent, tôn trọng scope/RowVersion từng bản ghi (BR-016).
- Restore: chỉ trong restore window, cần permission `*.restore`, chặn khi định danh duy nhất đã bị chiếm (BR-017).
- Device: thu hồi thiết bị vô hiệu hóa toàn bộ session và push của thiết bị đó tức thì; tối đa 10 thiết bị/người (BR-018).
- Version history: diff dựng từ audit bất biến; khôi phục field tạo phiên bản mới, không ghi đè lịch sử.

