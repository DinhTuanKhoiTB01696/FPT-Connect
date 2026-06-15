# hook: post-task — Verify sau khi code

Bắt buộc sau mỗi task:
1. Compile/Build: backend `dotnet build -c Release`; frontend `npm run build`.
2. Lint + TypeScript: `npm run typecheck`; format theo `.editorconfig`.
3. Test: `dotnet test` + `npm test`; thêm test cho code mới.
4. Self-review diff (kiến trúc, security, scope, naming).
5. Sinh `TASK_REPORT.md`: file đã đổi, mắt xích truy vết, cách test, rủi ro còn lại.

Không tuyên bố "xong" nếu build/test chưa xanh hoặc còn lỗi Critical/High.
