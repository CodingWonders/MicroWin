@echo off

REM MicroWin Development Environment Preparation script
REM ------------------------------------------------------
REM The files in this repository (https://github.com/CodingWonders/MicroWin)
REM represent the CANARY version of MicroWin. The Release version can be found here:
REM https://github.com/ChrisTitusTech/winutil.
REM ------------------------------------------------------
REM By using the CANARY version of MicroWin, you can help us work on the next version by
REM testing the latest changes. Be aware though that you may well find more bugs than
REM usual.
REM ------------------------------------------------------
REM DO NOT REPORT ISSUES YOU HAVE IN THE RELEASE VERSION OF MICROWIN HERE. Use the WinUtil repo to report these.

TITLE MicroWin Development Environment Setup

REM This will prevent it from putting everything in system32
CD %~dp0

NET SESSION >NUL 2>&1
IF %ERRORLEVEL% NEQ 0 (
    ECHO This script needs to be run as an administrator because some features require administrator access. Press any key to exit...
    PAUSE > NUL
    EXIT /B 1
)

ECHO This script will set up user-wide environment variables to set up the MicroWin
ECHO environment and to allow reverse-integration into WinUtil.
ECHO.
SET "MicroWinWinUtilSubPath=functions\microwin"
SET "MicroWinCanaryDir=%CD%"
SET /P "MicroWinWinUtilDir=Please enter the path you cloned WinUtil to: " || EXIT /B 1

ECHO Setting environment variables...
CALL :SET_ENVIRONMENT_VARIABLE MicroWin_RI_SourceDir "%MicroWinCanaryDir%" 0
CALL :SET_ENVIRONMENT_VARIABLE MicroWin_RI_DestDir "%MicroWinWinUtilDir%" 0
CALL :SET_ENVIRONMENT_VARIABLE MicroWin_RI_DestDirSubPath "%MicroWinWinUtilSubPath%" 0
ECHO.
ECHO User-wide environment variables were set successfully. To update them, either
ECHO run "sysdm.cpl" or this script.
ECHO.
ECHO You will now be asked a series of questions. These are optional, but can help
ECHO you with development and testing. Press ENTER to continue...
PAUSE > NUL
IF NOT "%1" == "SKIP_MSSTORE_UPDATE" (
    CALL :INVOKE_MICROSOFT_STORE_APP_UPDATE
)
CALL :DT_PROJECT_CREATE
REM CALL :COPY_MICROWIN_FROM_DESTINATION            --- no longer needed; we have already committed the files
CALL :SET_PWSH_EXECUTION_POLICY
ECHO Your development environment is now ready.

EXIT /B

:INVOKE_MICROSOFT_STORE_APP_UPDATE
ECHO Invoking an update of all Microsoft Store apps...
powershell -command Get-CimInstance -Namespace "Root\cimv2\mdm\dmmap" -ClassName "MDM_EnterpriseModernAppManagement_AppManagement01" ^| Invoke-CimMethod -MethodName UpdateScanMethod > nul 2>&1
ECHO Press any key to continue preparation...
PAUSE > NUL
EXIT /B

:SET_ENVIRONMENT_VARIABLE
REM Sets an environment variable
REM ----- CALL :SET_ENVIRONMENT_VARIABLE env_var_name "env_var_value (single value only)" 0|1
REM -- Third flag indicates whether to set a variable machine-wide. It is mandatory. Values: 0 -- user-wide; 1 -- machine-wide
IF "%3" == "" (
    ECHO Machine-wide flag needs to be passed.
    EXIT /B
)

IF %3 EQU 1 (
    SETX /M %1 %2
) ELSE (
    SETX %1 %2
)
EXIT /B

:DT_PROJECT_CREATE
:: This is a constant value obtained with PowerShell and (New-Guid).Guid.
:: Update this value by running that command.
SET "ProjGuid=75ab4fb5-bfbe-49a3-84aa-d4b5923f59c8"

ECHO.
ECHO To facilitate testing of MicroWin in your environment, we can create a DISMTools
ECHO project for you. You can still use the DISM command-line if you prefer.
ECHO.
ECHO Neither the project nor any of your changes from within this project will be
ECHO tracked by source control, so they are completely local.
ECHO.
SET /P "DT_Option=Do you want to create a DT project (Y/N)? " || SET "DT_Option=N"
IF "%DT_Option%" == "Y" (
    IF NOT EXIST "%ProgramFiles%\DISMTools" (
        ECHO Installing DISMTools Latest Preview...
        winget install DISMTools-pre --accept-source-agreements --accept-package-agreements
        REM we do this because I don't want to deal with false positives in the environment. Defender is f***ing with us at this point.
        ECHO Setting Defender exclusion for DT Preview directory. This prevents Defender from flagging this software at an inopportune time.
        powershell -command Start-Process powershell -argumentList \"Add-MpPreference -ExclusionPath '%ProgramFiles%\DISMTools\Preview'"\" -verb runas
    )
    ECHO Creating project folder...
    MD MicroWin_DT
    ECHO Creating project files and subdirectories...
    ECHO # DISMTools project file. File version: 0.1 > MicroWin_DT\MicroWin_DT.dtproj 2>nul
    ECHO [Settings] >> MicroWin_DT\MicroWin_DT.dtproj 2>nul
    ECHO SettingsInclude=\settings\project.ini >> MicroWin_DT\MicroWin_DT.dtproj 2>nul
    ECHO. >> MicroWin_DT\MicroWin_DT.dtproj 2>nul
    ECHO [Project] >> MicroWin_DT\MicroWin_DT.dtproj 2>nul
    ECHO ProjName=MicroWin_DT >> MicroWin_DT\MicroWin_DT.dtproj 2>nul
    ECHO ProjGuid=%ProjGuid% >> MicroWin_DT\MicroWin_DT.dtproj 2>nul
    MD MicroWin_DT\DandI
    MD MicroWin_DT\mount
    MD MicroWin_DT\reports
    MD MicroWin_DT\scr_temp
    MD MicroWin_DT\settings
    MD MicroWin_DT\unattend_xml
    ECHO [ProjOptions] > MicroWin_DT\settings\project.ini 2>nul
    ECHO Name="MicroWin_DT" >> MicroWin_DT\settings\project.ini 2>nul
    ECHO Location=%CD% >> MicroWin_DT\settings\project.ini 2>nul
    ECHO EpochCreationTime=1698278400 >> MicroWin_DT\settings\project.ini 2>nul
    ECHO. >> MicroWin_DT\settings\project.ini 2>nul
    ECHO [ImageOptions] >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageFile=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageIndex=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageMountPoint=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageVersion=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageName=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageDescription=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageWIMBoot=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageArch=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageHal=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageSPBuild=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageSPLevel=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageEdition=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImagePType=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImagePSuite=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageSysRoot=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageDirCount=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageFileCount=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageEpochCreate=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageEpochModify=N/A >> MicroWin_DT\settings\project.ini 2>nul
    ECHO ImageLang=N/A >> MicroWin_DT\settings\project.ini 2>nul
)
EXIT /B

:COPY_MICROWIN_FROM_DESTINATION
ECHO Preparing to copy files from destination...
IF EXIST "%MicroWinCanaryDir%\%MicroWinWinUtilSubPath%" (
    ECHO Copying MicroWin files from the target WinUtil repo is not necessary. Skipping...
) ELSE (
    REM We will prepare the compiler and the pre-processor because we still want
    REM to have consistent indentation
    ECHO Copying Build Tools...
    COPY /Y "%MicroWinWinUtilDir%\compile.ps1" compile.ps1
    COPY /Y "%MicroWinWinUtilDir%\.gitignore" .gitignore
    MD tools
    XCOPY "%MicroWinWinUtilDir%\tools\*.*" .\tools /cehyi
    ECHO Copying MicroWin Source Files...
    MD %MicroWinWinUtilSubPath%
    XCOPY "%MicroWinWinUtilDir%\%MicroWinWinUtilSubPath%\*.*" %MicroWinWinUtilSubPath% /cehyi
)
EXIT /B

:SET_PWSH_EXECUTION_POLICY
FOR /F "tokens=3" %%A IN ('REG QUERY "HKLM\SOFTWARE\Microsoft\PowerShell\1\ShellIds\Microsoft.PowerShell" /V ExecutionPolicy 2^>NUL') DO SET "ExecPol=%%A"

IF "%ExecPol%" == "Unrestricted" (
    ECHO PowerShell Execution Policy is already set to unrestricted. Skipping...
    EXIT /B
)

ECHO.
ECHO To work on PowerShell scripts, your execution policy needs to be set to unrestricted. You can
ECHO get your current execution policy by running "Get-ExecutionPolicy" in a PowerShell window. You
ECHO can skip this question if your execution policy already allows you to run PowerShell scripts.
ECHO.
ECHO Do note that execution policy changes are applied SYSTEM-WIDE. You will still be fine, unless
ECHO you perform stupid things with your computer. In that case, setting execution policies can make it more
ECHO likely that your computer will catch a virus.
ECHO.
SET /P "PWSH_Option=Do you want to set execution policies (Y/N)? " || SET "PWSH_Option=N"
IF "%PWSH_OPTION%" == "Y" (
    powershell -command Set-ExecutionPolicy Unrestricted -Force
    IF %ERRORLEVEL% NEQ 0 (
        ECHO The PowerShell method did not work; attempting to set execution policy from Registry...
        REG ADD "HKLM\SOFTWARE\Microsoft\PowerShell\1\ShellIds\Microsoft.PowerShell" /V ExecutionPolicy /T REG_SZ /D Unrestricted /F
    )
)
EXIT /B
