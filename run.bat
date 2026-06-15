@echo off
chcp 65001 >nul
setlocal
title FPT Connect - Run Backend + Frontend

echo =======================================
echo   KHOI DONG HE THONG FPT CONNECT
echo =======================================
echo.
echo Database lay theo ConnectionStrings:Default trong
echo   Backend\src\FptConnect.Api\appsettings.json
echo (Khong can go duong dan SQL o day nua.)
echo.

set "FptConnect__RecreateDatabase=false"
set /p resetDB="Xoa va tao lai database (reset sach du lieu)? (Y/N): "
if /I "%resetDB%"=="Y" set "FptConnect__RecreateDatabase=true"

echo.
echo 1. Khoi dong Backend (.NET Web API) tai http://localhost:5080 ...
echo    (API tu tao schema + seed admin; reset=%FptConnect__RecreateDatabase%)
start "FPT Connect API" cmd /k "set FptConnect__RecreateDatabase=%FptConnect__RecreateDatabase% && cd /d "%~dp0Backend\src\FptConnect.Api" && title FPT Connect API && dotnet run"

echo 2. Khoi dong Frontend (Vue 3) tai http://localhost:5173 ...
start "FPT Connect Frontend" cmd /k "cd /d "%~dp0Frontend" && title FPT Connect Frontend && if not exist node_modules (echo Cai dependencies... && npm install) && npm run dev"

echo.
echo Da gui lenh khoi dong Backend va Frontend o hai cua so rieng biet!
echo API Swagger: http://localhost:5080/swagger
echo Tai khoan: admin@fptconnect.vn / Admin@12345
echo =======================================
endlocal
