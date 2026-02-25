$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$WinDrv = (Get-PSDrive -PSProvider FileSystem | Where-Object { Test-Path (Join-Path $_.Root 'Windows') } | Select-Object -First 1).Root

$MsbuildPath = $WinDrv + 'Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\'

$BuildToolsURL = 'https://aka.ms/vs/17/release/vs_BuildTools.exe'
$BuildToolsInstall = Join-Path (Get-Location) 'vs_BuildTools.exe'

# Define log file path
$LogFile = '.\BuildLog.txt'
# Overwrite BuildLog.txt at the start of each run
if (Test-Path $LogFile) {
    Clear-Content -Path $LogFile -ErrorAction SilentlyContinue
} else {
    New-Item -Path $LogFile -ItemType File -Force | Out-Null
}

# Overwrite BuildLog.txt at the start of each run
Clear-Content -Path $LogFile -ErrorAction SilentlyContinue
function Write-LogLine {
    param([string]$Message)
    $line = "[{0}] {1}" -f (Get-Date -Format 'yyyy-MM-dd HH:mm:ss.fff'), $Message, "`r`n"
    Write-Host $Message
    Add-Content -Path $LogFile -Value $line -Encoding UTF8
}

$BuildStartTime = Get-Date
Write-LogLine '=================================================='
Write-LogLine "MicroWin Build Log - $($BuildStartTime.ToString('MM/dd/yyyy - HH:mm:ss'))"
Write-LogLine "Script directory: $ScriptDir"
Write-LogLine "Script: $([System.IO.Path]::Combine((Get-Location).Path, 'build.bat'))"
Write-LogLine "Working directory: $((Get-Location).Path)"
Write-LogLine '=================================================='
Add-Content -Path $LogFile -Value "`r`n" -Encoding UTF8

#################################################################################################
# --- 1. SETUP PATHS ---
$InstallDir = Get-Location
$NuGetExe = Join-Path $InstallDir 'nuget.exe'
$DevToolsOutfile = Join-Path $InstallDir 'DevToolsInstall.exe'
$BuildToolsInstall = Join-Path $InstallDir 'vs_BuildTools.exe'

# URLs
$NugetURL = 'https://dist.nuget.org'
$DevToolsURL = 'https://go.microsoft.com'
$BuildToolsURL = 'https://aka.ms'

# --- 2. DOWNLOAD & INSTALL TOOLS ---

# Download MSBuild Tools installer
if (!(Test-Path $BuildToolsInstall)) {
    Write-LogLine "Downloading MSBuildTools..."
    Invoke-WebRequest -Uri $BuildToolsURL -OutFile $BuildToolsInstall
}
Write-LogLine "Ensuring MSBuild Workload is installed..."
Start-Process -FilePath $BuildToolsInstall -ArgumentList '--quiet --wait --norestart --add Microsoft.VisualStudio.Workload.MSBuildTools' -Wait

# Download NuGet CLI
if (!(Test-Path $NuGetExe)) {
    Write-LogLine "Downloading NuGet CLI..."
    Invoke-WebRequest -Uri $NugetURL -OutFile $NuGetExe
}

# Download & Install .NET 4.8 Dev Pack
if (!(Test-Path $DevToolsOutfile)) {
    Write-LogLine "Downloading .NET 4.8 Developer Pack..."
    Invoke-WebRequest -Uri $DevToolsURL -OutFile $DevToolsOutfile
}
Write-LogLine "Running DevPack installer..."
Start-Process -FilePath $DevToolsOutfile -ArgumentList '/quiet /norestart' -Wait

# --- 3. LOCATE MSBUILD EXE ---
$vswherePath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$VSPath = & $vswherePath -latest -products * -requires Microsoft.Component.MSBuild -property installationPath

if (-not $VSPath) {
    Write-LogLine "ERROR: Visual Studio installation path not found."
    exit 1
}
$MSBuildExe = Join-Path $VSPath "MSBuild\Current\Bin\MSBuild.exe"

# --- 4. RESTORE & BUILD ---
# We use -restore and explicitly define SolutionDir because there is no .sln file.
# This ensures NuGet packages are restored to the correct root folder.
$currentDir = Get-Location
$buildArgs = @(
    "./MicroWin/MicroWin.csproj",
    "-restore",
    "/t:Build",
    "/p:Configuration=Release",
    "/p:Platform=AnyCPU",
    "/p:RestorePackagesConfig=true",
    "/p:SolutionDir=$currentDir\",    # <--- Crucial fix for "No solution found"
    "/verbosity:minimal"
)

Write-LogLine "Starting integrated restore and build for MicroWin..."
Write-LogLine "Command: $MSBuildExe $($buildArgs -join ' ')"

# Execute the integrated Restore and Build
& $MSBuildExe $buildArgs

# --- 5. VERIFY & MOVE OUTPUT ---
$builtExe = Join-Path $InstallDir 'MicroWin/bin/Release/MicroWin.exe'

if (Test-Path $builtExe) {
    Write-LogLine "Build completed successfully."
    $dateTime = Get-Date -Format 'dd-MM-yyyy_HH-mm-ss'
    $targetFolder = Join-Path ([Environment]::GetFolderPath('Desktop')) "MicroWin Build $dateTime"
    New-Item -Path $targetFolder -ItemType Directory -Force | Out-Null
    
    $sourceFolder = Join-Path $InstallDir 'MicroWin/bin/Release/*'
    Copy-Item -Path $sourceFolder -Destination $targetFolder -Recurse -Force
    Write-LogLine "MicroWin files moved to $targetFolder."
} else {
    Write-LogLine "ERROR: Build failed. MicroWin.exe not found at $builtExe"
    exit 1
}

########################################################################################################

# Prompt user to uninstall build tools
$uninstallQuestion = "Uninstall Files Used To Build MicroWin? `n Not Recommended If You Are Planning To Build MicroWin Again (y/n)`n"
$uninstallPrompt = Read-Host $uninstallQuestion
Write-LogLine "Prompt: $uninstallQuestion | User input: $uninstallPrompt"
if ($uninstallPrompt -eq 'y') {
    # Remove installer files
    $filesToDelete = @($NugetInstall, $DevToolsOutfile, $BuildToolsInstall)
    foreach ($file in $filesToDelete) {
        if (Test-Path $file) {
            Remove-Item $file -Force
            Write-LogLine "Deleted $file."
        }
    }
    # Uninstall MSBuildTools if installed
    $vsInstallerPath = "$env:ProgramFiles(x86)\Microsoft Visual Studio\Installer\vs_installer.exe"
    if (Test-Path $vsInstallerPath) {
        Write-LogLine "Uninstalling MSBuildTools via Visual Studio Installer..."
        Start-Process -FilePath $vsInstallerPath -ArgumentList 'modify --installPath "$env:ProgramFiles(x86)\Microsoft Visual Studio\2022\BuildTools" --remove Microsoft.VisualStudio.Workload.MSBuildTools --quiet --wait --norestart' -Wait
        Write-LogLine "MSBuildTools uninstall command issued."
        # Verification after uninstall
        Start-Sleep -Seconds 3
        $msbuildRegCheck = Get-ChildItem 'HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall' |
            Get-ItemProperty |
            Where-Object { $_.DisplayName -like '*Build Tools*' -or $_.DisplayName -like '*MSBuild*' }
        if ($msbuildRegCheck) {
            Write-LogLine "MSBuildTools still detected in registry after uninstall attempt. Manual removal may be required."
        } else {
            Write-LogLine "MSBuildTools successfully uninstalled (not detected in registry)."
        }
    } else {
        Write-LogLine "Visual Studio Installer not found. Attempting to uninstall MSBuildTools via msiexec..."
        $msbuildReg = Get-ChildItem 'HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall' |
            Get-ItemProperty |
            Where-Object { $_.DisplayName -like '*Build Tools*' -or $_.DisplayName -like '*MSBuild*' } |
            Select-Object -First 1
        if ($msbuildReg -and $msbuildReg.PSChildName) {
            $msbuildProductCode = $msbuildReg.PSChildName
            Start-Process msiexec.exe -ArgumentList "/x $msbuildProductCode /quiet /norestart" -Wait
            Write-LogLine "MSBuildTools uninstall command issued via msiexec (ProductCode: $msbuildProductCode)."
            # Verification after uninstall
            Start-Sleep -Seconds 3
            $msbuildRegCheck = Get-ChildItem 'HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall' |
                Get-ItemProperty |
                Where-Object { $_.DisplayName -like '*Build Tools*' -or $_.DisplayName -like '*MSBuild*' }
            if ($msbuildRegCheck) {
                Write-LogLine "MSBuildTools still detected in registry after uninstall attempt. Manual removal may be required."
            } else {
                Write-LogLine "MSBuildTools successfully uninstalled (not detected in registry)."
            }
        } else {
            Write-LogLine "Could not find MSBuildTools ProductCode in registry. Please uninstall manually if needed."
        }
    }
    # Uninstall .NET DevPack if installed
    $dotnetUninstaller = "$env:SystemRoot\System32\msiexec.exe"
    $devPackProductCode = '{362af3ba-ef6b-483c-9cb9-8033838e8b7d}' # Actual ProductCode for .NET Framework 4.8 Developer Pack
    if (Test-Path $dotnetUninstaller) {
        Write-LogLine "Attempting to uninstall .NET Framework 4.8 Developer Pack (if installed)..."
        Start-Process -FilePath $dotnetUninstaller -ArgumentList "/x $devPackProductCode /quiet /norestart" -Wait
        Write-LogLine ".NET Framework 4.8 Developer Pack uninstall command issued."
        # Verification after uninstall
        Start-Sleep -Seconds 3
        $devPackRegCheck = Get-ChildItem 'HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall' |
            Get-ItemProperty |
            Where-Object { $_.DisplayName -like '*Developer Pack*' -or $_.DisplayName -like '*Framework 4.8*' }
        if ($devPackRegCheck) {
            Write-LogLine ".NET Framework 4.8 Developer Pack still detected in registry after uninstall attempt. Manual removal may be required."
        } else {
            Write-LogLine ".NET Framework 4.8 Developer Pack successfully uninstalled (not detected in registry)."
        }
    }
    Write-LogLine "Build tools and installer files deleted.`n`n"
} else {
    Write-LogLine "Deletion of build tools skipped by user.`n`n"
}

# Copy BuildLog.txt to MicroWin folder on Desktop
$scriptFolder = Get-Location
$buildLog = Join-Path $scriptFolder 'BuildLog.txt'
if ($targetFolder -and (Test-Path $buildLog) -and (Test-Path $targetFolder)) {
    $targetLogPath = Join-Path $targetFolder 'BuildLog.txt'
    Copy-Item $buildLog -Destination $targetLogPath -Force
    Write-LogLine "BuildLog.txt copied to $targetLogPath.`n`n"
}


Write-LogLine "NEXT TIME INSTRUCTIONS:`n"
Write-LogLine "      1. Open the folder you are using now."
Write-LogLine "      2. Open the new MicroWin folder you extracted."
Write-LogLine "      3. Copy the entire contents of the new MicroWin folder to this folder."
Write-LogLine "      4. Select yes if asked to overwrite any existing files."
Write-LogLine "      4. Then double click CLICKME.bat to start the new build process."
write-LogLine "      5. By keeping the files like this means you don't have to download the build tools again!`n`n"
Write-LogLine "That is all you need to do - no extra steps required!`n`n"
Write-LogLine "Now you are building software like a REAL LINUX CHAD!`n`n"
Write-LogLine "Thank you for using MicroWin!`n`n"