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
set ASPNETCORE_URLS=http://localhost:5080

echo Starting local app at http://localhost:5080 ...
echo A separate app window may open. Keep it open while testing.
echo.

start "ClearPath Payroll Local Demo" "%~dp0ClearPathPayroll.exe"

timeout /t 4 /nobreak >nul
start "" "http://localhost:5080"

echo If the browser did not open, open this address manually:
echo http://localhost:5080
echo.
pause
