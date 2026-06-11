@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

where dotnet >nul 2>nul

if %ERRORLEVEL% gtr 0 (
	echo .NET SDK not installed. Exiting...
	exit /b
)

if exist "bin\release\net10.0-windows" (
	rd "bin\release\net10.0-windows" /s /q
)

cls

echo MicroWin Release Publish
echo (c) 2026. CodingWonders Software

REM Publish Script Options
REM ----------------------------------------------------------
REM - This option specifies whether to disable dotnet publish telemetry.
REM   - Set to 1 to disable telemetry
REM   - Set to 0 to enable telemetry
set DOTNET_CLI_TELEMETRY_OPTOUT=1

echo.
echo OPTIONS:
echo - Opt out of dotnet telemetry: %DOTNET_CLI_TELEMETRY_OPTOUT%
echo.
echo.

echo Publishing self-contained binaries...

:: Publish self-contained versions
for %%a in (win-x86 win-x64 win-arm64) do (
	echo ------------------------ Building self-contained binary for target %%a ------------------------
	dotnet publish -r:%%a -p:PublishSingleFile=true --sc -o "bin\release\net10.0-windows\sc\%%a" --framework net10.0-windows
	if !ERRORLEVEL! equ 0 (
		REM give the final EXE its final name
		if /I "%%a" == "win-x86" copy "bin\release\net10.0-windows\sc\%%a\MicroWin.exe" "bin\release\net10.0-windows\sc\MicroWin_x86.exe"
		if /I "%%a" == "win-x64" copy "bin\release\net10.0-windows\sc\%%a\MicroWin.exe" "bin\release\net10.0-windows\sc\MicroWin_x64.exe"
		if /I "%%a" == "win-arm64" copy "bin\release\net10.0-windows\sc\%%a\MicroWin.exe" "bin\release\net10.0-windows\sc\MicroWin_arm64.exe"
	)
)

echo Completed.
ENDLOCAL
powershell -noprofile -nologo -command Get-FileHash "bin\release\net10.0-windows\sc\*.exe" ^| Select-Object Hash, @{Name='FileName'; Expression={[IO.Path]^:^:GetFileName($_.Path)}}
exit /b