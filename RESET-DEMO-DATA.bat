@echo off
setlocal
title Reset ClearPath Payroll Demo Data

echo.
echo Reset ClearPath Payroll local demo data
echo.
echo WARNING: This removes local demo database files from this extracted app folder only.
echo It does not delete files outside this folder.
echo.
echo Tester package only. Do not use for real payroll.
echo.

cd /d "%~dp0"

choice /C YN /N /M "Reset local demo data now? Type Y or N: "
if errorlevel 2 (
    echo Reset cancelled.
    pause
    exit /b 0
)

if exist "App_Data" (
    del /f /q "App_Data\*.db" 2>nul
    del /f /q "App_Data\*.db-shm" 2>nul
    del /f /q "App_Data\*.db-wal" 2>nul
    del /f /q "App_Data\*.mdf" 2>nul
    del /f /q "App_Data\*.ldf" 2>nul
    echo Local demo database files were reset.
) else (
    echo No App_Data folder was found. Nothing was reset.
)

if exist "ClearPathPayroll.exe" (
    set ASPNETCORE_ENVIRONMENT=Development
    set DOTNET_ENVIRONMENT=Development
    set PrototypeMode__Enabled=true
    set PrototypeMode__LocalDbDatabaseName=ClearPathPayroll.TesterPackage.LocalDemo
    set LimitedLiabilityMode__Enabled=true
    set LimitedLiabilityMode__LocalDatabaseProvider=SQLite
    set LimitedLiabilityMode__LocalDatabaseName=ClearPathPayroll.TesterPackage.LocalDemo
    set LimitedLiabilityMode__AllowExternalTaxApiLookup=false
    set LimitedLiabilityMode__AllowRealAchSubmission=false
    set LimitedLiabilityMode__AllowRealTaxFiling=false
    set LimitedLiabilityMode__AllowTelemetry=false
    echo Resetting the local demo database used by this tester package...
    "ClearPathPayroll.exe" --reset-demo-data
)

echo.
echo Start the app again, then open /demo/seed-data and click Create Demo Data.
pause
