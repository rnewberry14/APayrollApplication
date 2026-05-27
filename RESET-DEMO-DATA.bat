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

echo.
echo Start the app again, then open /demo/seed-data and click Create Demo Data.
pause
