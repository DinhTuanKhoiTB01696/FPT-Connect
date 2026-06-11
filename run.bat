@echo off
chcp 65001 >nul
setlocal EnableDelayedExpansion
title FPT Connect - Run Backend + Frontend

echo =======================================
echo   KHOI DONG HE THONG FPT CONNECT
echo =======================================
echo.

REM --- Cau hinh SQL Server ---
set "DEFAULT_SERVER=(localdb)\MSSQLLocalDB"
set "DB_NAME=FptConnectDB"
set /p SQL_SERVER="Nhap SQL Server instance [Enter = %DEFAULT_SERVER%]: "
if "%SQL_SERVER%"=="" set "SQL_SERVER=%DEFAULT_SERVER%"

REM Connection string dung chung cho EF va API (env override appsettings)
set "ConnectionStrings__Default=Server=%SQL_SERVER%;Database=%DB_NAME%;Trusted_Connection=True;TrustServerCertificate=True"
echo Su dung: %ConnectionStrings__Default%
echo.

set /p resetDB="Ban co muon RESET Database + chay Migrations + Seed khong? (Y/N): "

cd /d "%~dp0Backend\src\FptConnect.Api"

if /I "%resetDB%"=="Y" (
    echo.
    echo --- DANG RESET DATABASE ---
    echo 1. Drop database cu...
    dotnet ef database drop -f --project ..\FptConnect.Infrastructure --startup-project .
    echo 2. Xoa migration cu...
    if exist "..\FptConnect.Infrastructure\Migrations" rd /s /q "..\FptConnect.Infrastructure\Migrations"
    echo 3. Tao migration moi 'PlaneRenovation'...
    dotnet ef migrations add PlaneRenovation --project ..\FptConnect.Infrastructure --startup-project .
    echo 4. Cap nhat database...
    dotnet ef database update --project ..\FptConnect.Infrastructure --startup-project .
    echo 5. Chay seed_data.sql...
    sqlcmd -S "%SQL_SERVER%" -d "%DB_NAME%" -i "..\..\seed_data.sql" -E -C
    echo --- RESET DATABASE THANH CONG ---
    echo.
) else (
    echo.
    echo --- KIEM TRA / TAO DATABASE NEU CHUA CO ---
    if not exist "..\FptConnect.Infrastructure\Migrations" (
        echo Chua co Migrations, dang tao 'InitialCreate'...
        dotnet ef migrations add InitialCreate --project ..\FptConnect.Infrastructure --startup-project .
    )
    echo Cap nhat / tao database...
    dotnet ef database update --project ..\FptConnect.Infrastructure --startup-project .
    echo --- HOAN TAT KIEM TRA ---
    echo.
)

cd /d "%~dp0"

echo 1. Khoi dong Backend (.NET Web API) tai http://localhost:5080 ...
start "FPT Connect API" cmd /k "set ConnectionStrings__Default=%ConnectionStrings__Default% && cd /d "%~dp0Backend\src\FptConnect.Api" && title FPT Connect API && dotnet run"

echo 2. Khoi dong Frontend (Vue 3) tai http://localhost:5173 ...
start "FPT Connect Frontend" cmd /k "cd /d "%~dp0Frontend" && title FPT Connect Frontend && if not exist node_modules (echo Cai dependencies... && npm install) && npm run dev"

echo.
echo Da gui lenh khoi dong Backend va Frontend o hai cua so rieng biet!
echo API Swagger: http://localhost:5080/swagger
echo =======================================
endlocal
