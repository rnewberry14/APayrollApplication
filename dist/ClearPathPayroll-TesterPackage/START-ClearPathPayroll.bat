@echo off
setlocal
title ClearPath Payroll Local Demo

echo.
echo ClearPath Payroll - Local Demo/Test Package
echo.
echo Tester package only. Do not use for real payroll.
echo Do not enter real SSNs, bank account numbers, EINs, API keys, or live payroll data.
echo No real ACH or tax filing is enabled for tester review.
echo.

cd /d "%~dp0"

if not exist "ClearPathPayroll.exe" (
    echo ClearPathPayroll.exe was not found in this folder.
    echo Make sure this file is in the extracted tester package folder.
    pause
    exit /b 1
)

set ASPNETCORE_ENVIRONMENT=Development
set DOTNET_ENVIRONMENT=Development
set ASPNETCORE_URLS=http://localhost:5080
set PrototypeMode__Enabled=true
set PrototypeMode__LocalDbDatabaseName=ClearPathPayroll.TesterPackage.LocalDemo
set LimitedLiabilityMode__Enabled=true
set LimitedLiabilityMode__LocalDatabaseProvider=SQLite
set LimitedLiabilityMode__LocalDatabaseName=ClearPathPayroll.TesterPackage.LocalDemo
set LimitedLiabilityMode__AllowExternalTaxApiLookup=false
set LimitedLiabilityMode__AllowRealAchSubmission=false
set LimitedLiabilityMode__AllowRealTaxFiling=false
set LimitedLiabilityMode__AllowTelemetry=false

echo Starting local app at http://localhost:5080 ...
echo Startup details will be written to ClearPathPayroll-startup.log.
echo.

if exist "ClearPathPayroll-startup.log" del /f /q "ClearPathPayroll-startup.log" 2>nul

start "ClearPath Payroll Local Demo" /D "%~dp0" cmd /c "ClearPathPayroll.exe > ClearPathPayroll-startup.log 2>&1"

echo Waiting for the local app to be ready...
powershell -NoProfile -ExecutionPolicy Bypass -Command "$deadline=(Get-Date).AddSeconds(60); do { try { Invoke-WebRequest -UseBasicParsing 'http://localhost:5080' -TimeoutSec 2 | Out-Null; exit 0 } catch { Start-Sleep -Seconds 1 } } while ((Get-Date) -lt $deadline); exit 1"

if errorlevel 1 (
    echo.
    echo The app did not answer at http://localhost:5080 within 60 seconds.
    echo Open ClearPathPayroll-startup.log in this folder and send a screenshot or the error text.
    echo.
    if exist "ClearPathPayroll-startup.log" (
        echo Last startup log lines:
        powershell -NoProfile -Command "Get-Content -Path 'ClearPathPayroll-startup.log' -Tail 20"
    )
    pause
    exit /b 1
)

start "" "http://localhost:5080"

echo If the browser did not open, open this address manually:
echo http://localhost:5080
echo.
pause
