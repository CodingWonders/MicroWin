@echo off
set "HELPER_PS1=%~dp0Bhelper.ps1"

if not exist "%HELPER_PS1%" (
    echo ERROR: Missing helper script: %HELPER_PS1%
    pause
    exit /b 1
)

rem Set Lucida Console 12pt 786432:  18pt 1179648 - No effect until new power shell terminal opens
reg add "HKCU\Console" /v FaceName /d "Lucida Console" /f >nul 2>&1
reg add "HKCU\Console" /v FontSize /t REG_DWORD /d 1179648 /f >nul 2>&1

cd /d "%~dp0"

REM Launch Bhelper.ps1 in a new, elevated PowerShell window
powershell -NoProfile -ExecutionPolicy Bypass -Command "Start-Process powershell -ArgumentList '-NoProfile -ExecutionPolicy Bypass -File \"%HELPER_PS1%\"' -Verb RunAs -WindowStyle Normal"
