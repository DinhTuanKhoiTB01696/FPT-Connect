# 01. Giới thiệu

## 1.1 Bối cảnh

FPT Connect phục vụ lực lượng Sale, Kỹ thuật, Quản lý và Admin của FPT Telecom trong vòng đời từ phát hiện khách hàng tiềm năng đến khảo sát, ký hợp đồng, triển khai kỹ thuật và chăm sóc sau hoàn tất. Hệ thống kết hợp CRM với bản đồ và bằng chứng hiện trường để giảm dữ liệu rời rạc, tăng tỷ lệ chuyển đổi và minh bạch năng suất.

## 1.2 Bài toán

| Mã | Hiện trạng | Hệ quả đo được | Năng lực cần có |
|---|---|---|---|
| P-01 | Lead nằm trong sổ tay, Excel, chat cá nhân | Trùng lead, mất lịch sử, không rõ chủ sở hữu | CRM tập trung, chống trùng |
| P-02 | Địa chỉ dạng văn bản thiếu tọa độ | Sale tìm sai vị trí, khó chia địa bàn | Geocoding, pin map, nearby |
| P-03 | Quản lý không biết tuyến thực tế | Khó xác minh công việc và tối ưu vùng | GPS consent, route session |
| P-04 | Nhắc việc phụ thuộc trí nhớ | Quá hạn follow-up, giảm chuyển đổi | Reminder, escalation |
| P-05 | Check-in không có bằng chứng | Dữ liệu năng suất thiếu tin cậy | Geofence, ảnh, anti-spoof |
| P-06 | Bàn giao Sale-Kỹ thuật qua chat | Thiếu dữ liệu khảo sát, trễ SLA | Handoff có checklist |
| P-07 | Báo cáo tổng hợp thủ công | Số liệu chậm và không đồng nhất | Dashboard near-real-time |
| P-08 | Không có lịch sử quyết định | Khó điều tra và tuân thủ | Audit log bất biến |

## 1.3 Tầm nhìn

Trở thành nền tảng vận hành hiện trường thống nhất, giúp mỗi nhân viên biết khách hàng nào cần phục vụ, ở đâu, khi nào và hành động tốt nhất tiếp theo; giúp quản lý điều hành bằng dữ liệu đáng tin cậy thay vì báo cáo thủ công.

## 1.4 Sứ mệnh

1. Chuẩn hóa dữ liệu khách hàng và quy trình chuyển đổi.
2. Giảm thao tác hành chính cho nhân viên hiện trường.
3. Cung cấp bằng chứng hoạt động có tôn trọng quyền riêng tư.
4. Tạo quyết định điều hành từ dữ liệu, phân tích và AI có kiểm soát.

## 1.5 Mục tiêu và KPI

| Mã | Mục tiêu 12 tháng | KPI | Baseline giả định | Target |
|---|---|---|---|---|
| BG-01 | Tăng chuyển đổi lead | Lead-to-contract conversion | 12% | >= 18% |
| BG-02 | Follow-up đúng hạn | Reminder completed within SLA | 65% | >= 92% |
| BG-03 | Giảm lead trùng | Duplicate lead rate | 8% | < 1.5% |
| BG-04 | Minh bạch hiện trường | Valid check-in ratio | Không đo | >= 95% |
| BG-05 | Bàn giao nhanh | Median sale-to-tech handoff | 24 giờ | < 4 giờ |
| BG-06 | Phản hồi nhanh | P95 API latency | Không đo | < 500 ms |
| BG-07 | Hệ thống ổn định | Monthly availability | Không đo | >= 99.9% |
| BG-08 | Tăng sử dụng | Weekly active field staff | Không đo | >= 85% |

KPI baseline phải được đo lại trong 30 ngày pilot; target chỉ thay đổi qua Product Steering Committee.

## 1.6 Phạm vi

### Trong phạm vi

- Tổ chức, người dùng, vai trò, permission và địa bàn.
- Lead/customer 360, lịch sử trạng thái, ghi chú, tệp.
- Bản đồ, tìm gần, geocode/reverse geocode.
- Ca làm, GPS route, check-in/check-out và visit.
- Reminder, notification in-app/email/push tùy cấu hình.
- Hợp đồng metadata và bàn giao kỹ thuật; không thay thế core billing.
- Dashboard, export có kiểm soát, audit và AI assistant.
- Web responsive/PWA; API cho ứng dụng mobile trong tương lai.

### Ngoài phạm vi phiên bản 1

- Tính cước, thu tiền, hóa đơn điện tử.
- Quản lý kho vật tư chi tiết và tối ưu đội xe.
- Theo dõi GPS ngoài ca làm việc.
- Nhận diện khuôn mặt và giám sát âm thanh.
- AI tự động thay đổi trạng thái, ký hợp đồng hoặc gửi nội dung cho khách mà không có con người duyệt.

## 1.7 Stakeholder

| Stakeholder | Trách nhiệm | Quyết định |
|---|---|---|
| Product Sponsor | Ngân sách, chiến lược | Scope và go-live |
| Product Owner | Backlog, nghiệm thu | Ưu tiên và rule |
| Branch Manager | Hiệu suất chi nhánh | Địa bàn, SLA cục bộ |
| Sales Manager | Lead và đội Sale | Phân công, dashboard |
| Technical Manager | Bàn giao, triển khai | Lịch và kết quả kỹ thuật |
| DPO/Security | Quyền riêng tư, an toàn | Retention, incident |
| Admin | Cấu hình và hỗ trợ | Master data có ủy quyền |
| End User | Thực thi nghiệp vụ | Phản hồi usability |

## 1.8 Nguyên tắc sản phẩm

- Mobile-first và thao tác chính không quá ba lần chạm.
- Dữ liệu tối thiểu cần thiết; GPS có mục đích, consent và retention.
- Offline-tolerant: không mất ghi chú/check-in khi mạng chập chờn.
- Explainable AI: luôn hiển thị nguồn và mức tin cậy.
- Secure by default, least privilege, deny by default.
- Không dùng chỉ số GPS đơn lẻ để kỷ luật nhân viên; phải có quy trình xác minh.

## 1.9 Giả định và ràng buộc

| Loại | Nội dung |
|---|---|
| Giả định | Nhân viên có smartphone hỗ trợ GPS và HTTPS |
| Giả định | HR cung cấp mã nhân viên và cơ cấu tổ chức chuẩn |
| Ràng buộc | Google Maps cần billing account và quota |
| Ràng buộc | SQL Server là system of record |
| Ràng buộc | Dữ liệu khách hàng phải lưu theo chính sách FPT và pháp luật Việt Nam |
| Phụ thuộc | SMS/email/push provider, S3 hoặc local storage |
| Phụ thuộc | Hệ thống hợp đồng/billing ngoài phạm vi qua integration adapter |

## 1.10 Rủi ro cấp sản phẩm

| Risk | Xác suất | Tác động | Giảm thiểu |
|---|---:|---:|---|
| Người dùng xem GPS là giám sát quá mức | Cao | Cao | Consent rõ, chỉ trong ca, minh bạch dữ liệu |
| Tọa độ đô thị sai do nhà cao tầng | Cao | Trung bình | Accuracy threshold, ảnh, manual review |
| Maps quota tăng chi phí | Trung bình | Cao | Cache, debounce, budget alert |
| Dữ liệu master không sạch | Cao | Cao | Import validation, dedupe, owner |
| Mất mạng hiện trường | Cao | Cao | Outbox offline, idempotency |
| AI tạo khuyến nghị sai | Trung bình | Cao | Human approval, grounding, evaluation |

