# FPT Connect - Project Bible

> Phiên bản: 1.0  
> Ngày baseline: 2026-06-11  
> Trạng thái: Implementation-ready baseline  
> Chủ sở hữu: Product Owner FPT Connect

## 1. Mục đích

Bộ tài liệu này là nguồn sự thật duy nhất (Single Source of Truth) cho sản phẩm FPT Connect: hệ thống CRM theo vị trí, quản lý nhân viên hiện trường, GPS tracking, tuyến đường, nhắc việc, phân tích và trợ lý AI. Khi code khác tài liệu, đội dự án phải tạo Architecture Decision Record hoặc Change Request trước khi thay đổi baseline.

## 2. Đối tượng đọc

| Vai trò | Chương ưu tiên |
|---|---|
| Product Owner/BA | 01, 02, 03, 04, 05 |
| Backend Engineer | 03, 06, 07, 08, 14 |
| Frontend Engineer | 03, 04, 08, 09, 10, 14 |
| QA/Security | 03, 04, 08, 12, 13 |
| DevOps/SRE | 03, 07, 12, 13 |
| AI Engineer | 03, 06, 08, 11, 12 |

## 3. Mục lục

1. [Giới thiệu](01_Gioi_Thieu.md)
2. [Khảo sát](02_Khao_Sat.md)
3. [Phân tích yêu cầu](03_Phan_Tich_Yeu_Cau.md)
4. [Use Case](04_UseCase.md)
5. [Business Flow](05_BusinessFlow.md)
6. [Database](06_Database.md)
7. [Architecture](07_Architecture.md)
8. [API](08_API.md)
9. [UI/UX](09_UIUX.md)
10. [Design System](10_DesignSystem.md)
11. [AI](11_AI.md)
12. [Testing](12_Testing.md)
13. [Deployment](13_Deployment.md)
14. [Coding Convention](14_CodingConvention.md)
15. [Prompt Bible](15_PromptBible.md)

### Phụ lục triển khai

- [Ma trận truy vết](APPENDIX_A_Traceability.md)
- [Master data và glossary](APPENDIX_B_MasterData_Glossary.md)
- [API contract chi tiết](APPENDIX_C_API_Detailed.md)
- [Từ điển cột dữ liệu](APPENDIX_D_Database_Columns.md)
- [Phiếu thực thi 300 test case](APPENDIX_E_TestCase_Execution.md)

## 4. Quy ước bắt buộc

- Múi giờ nghiệp vụ: `Asia/Ho_Chi_Minh`; dữ liệu thời gian lưu UTC.
- Ngôn ngữ giao diện mặc định: tiếng Việt; mã và API dùng tiếng Anh.
- ID public dùng UUID; khóa nội bộ SQL Server có thể dùng `bigint`.
- Xóa nghiệp vụ là soft delete, trừ token, dữ liệu tạm và dữ liệu hết hạn theo retention.
- Tiền tệ: VND, `decimal(19,4)`; hiển thị không có phần thập phân.
- Tọa độ: WGS84, latitude/longitude `decimal(9,6)`.
- API base: `/api/v1`; lỗi theo RFC 9457 Problem Details.
- Role không thay thế permission; mọi endpoint kiểm tra permission và organizational scope.
- Mọi thay đổi dữ liệu nhạy cảm phải có audit log bất biến.

## 5. Definition of Ready

- [ ] User story liên kết ít nhất một Use Case.
- [ ] Có business rule, dữ liệu vào/ra và acceptance criteria.
- [ ] Có wireframe hoặc màn hình đích.
- [ ] Có API và permission tương ứng.
- [ ] Có tiêu chí security, observability và test.
- [ ] Không còn câu hỏi ảnh hưởng phạm vi hoặc dữ liệu.

## 6. Definition of Done

- [ ] Code review và static analysis đạt.
- [ ] Unit/integration/API/UI test đạt; không có lỗi Critical/High.
- [ ] Migration có forward/rollback plan.
- [ ] OpenAPI, audit, log, metric và runbook được cập nhật.
- [ ] Kiểm thử trên mobile viewport và điều kiện mạng yếu.
- [ ] Product Owner nghiệm thu acceptance criteria.

## 7. Ma trận truy vết

Chuỗi truy vết chuẩn: `Business Goal -> Requirement -> Use Case -> API -> Table -> Screen -> Test Case`. ID dùng tiền tố `BG`, `FR/NFR`, `UC`, `API`, `DB`, `UI`, `TC`. Pull request thiếu mắt xích truy vết không được merge.
