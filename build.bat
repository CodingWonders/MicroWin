@echo off
setlocal enabledelayedexpansion

set "SCRIPT_DIR=%~dp0"
set "SCRIPT_DIR_SAFE=%SCRIPT_DIR%."
set "HELPER_PS1=%SCRIPT_DIR%Bhelper.ps1"

net session >nul 2>&1
if %errorLevel% neq 0 (
    echo Relaunching with Administrator privileges...
    pause
    powershell -NoProfile -ExecutionPolicy Bypass -Command "try { Start-Process -FilePath '%~f0' -WorkingDirectory '%CD%' -Verb RunAs -ErrorAction Stop; exit 0 } catch { exit 1 }"
    if !errorLevel! neq 0 (
        echo ERROR: Failed to start elevated process. UAC may have been cancelled or blocked.
        pause
        exit /b 1
    )
    echo Elevated build started in a new window.
    pause
    exit /b 0
)

if not exist "%HELPER_PS1%" (
    echo ERROR: Missing helper script: %HELPER_PS1%
    pause
    exit /b 1
)

powershell -NoProfile -ExecutionPolicy Bypass -File "%HELPER_PS1%" -Action Build -ScriptDir "%SCRIPT_DIR_SAFE%"
set "EXIT_CODE=%errorLevel%"

pause
exit /b %EXIT_CODE%
