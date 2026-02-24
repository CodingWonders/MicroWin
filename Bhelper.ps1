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
}
else {
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

#NUGET
Write-LogLine "Downloading Nuget Package From https://www.nuget.org/packages/MicroWin/"
# NuGet command line tool download link
$NugetURL = 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe'
$NugetInstall = Join-Path (Get-Location) 'nugetinstall.exe'

#DEVTOOLS
Write-LogLine "Downloading .NET 4.8 Developer Pack from https://go.microsoft.com/fwlink/?linkid=2088517"
# .NET 4.8 Developer Pack download link
$DevTools = 'https://go.microsoft.com/fwlink/?linkid=2088517'
$DevToolsOutfile = Join-Path (Get-Location) 'DevToolsInstall.exe'

#MSBUILD
Write-LogLine "Downloading MSBuildTools from https://aka.ms/vs/17/release/vs_BuildTools.exe"
if (!(Test-Path $BuildToolsInstall)) {
    Invoke-WebRequest -Uri $BuildToolsURL -OutFile $BuildToolsInstall
    Write-LogLine "MSBuildTools downloaded successfully."
}
else {
    Write-LogLine "MSBuildTools already exists, skipping download."
}
Start-Process -FilePath $BuildToolsInstall -ArgumentList '--quiet --wait --norestart --add Microsoft.VisualStudio.Workload.MSBuildTools' -Wait


#Download NuGet command line tool
if (!(Test-Path $NugetInstall)) {
    Invoke-WebRequest -Uri $NugetURL -OutFile $NugetInstall
    Write-LogLine "NuGet command line tool downloaded successfully."
}
else {
    Write-LogLine "NuGet command line tool already exists, skipping download."
}
# NuGet should now be downloaded to the project dir

# Download .NET 4.8 Developer Pack installer
Write-LogLine "Downloading .NET 4.8 Developer Pack installer from https://aka.ms/vs/17/release/vs_BuildTools.exe..."
if (!(Test-Path $DevToolsOutfile)) {
    try {
        Invoke-WebRequest -Uri $DevTools -OutFile $DevToolsOutfile
        Write-LogLine ".NET 4.8 Developer Pack installer downloaded successfully."
    }
    catch {
        Write-LogLine "ERROR: Failed to download .NET 4.8 Developer Pack installer. $_"
    }
}
else {
    Write-LogLine ".NET 4.8 Developer Pack installer already exists, skipping download."
}
# The .NET 4.8 Developer Pack installer is now downloaded to the project dir.

#Install all tools
if (Test-Path $BuildToolsInstall) {
    Start-Process -FilePath $BuildToolsInstall -ArgumentList '--quiet --wait --norestart --add Microsoft.VisualStudio.Workload.MSBuildTools' -Wait
}
else {
    Write-LogLine " MSBuildTools installer already installed $BuildToolsInstall"
}
if (Test-Path $DevToolsOutfile) {
    # Check if DevToolsInstall.exe exists in source dir
    $srcDevTools = Join-Path (Get-Location) 'src/DevToolsInstall.exe'
    if (Test-Path $srcDevTools) {
        Write-LogLine "DevPack installer exists in source dir, skipping install."
    }
    else {
        Write-LogLine "Running DevPack installer..."
        Start-Process -FilePath $DevToolsOutfile -ArgumentList '/quiet' -Wait
        Write-LogLine "DevPack installer finished."
    }
}
else {
    Write-LogLine "ERROR: DevPack installer not found, skipping install."
}
# NuGet CLI does not require installation, just use the downloaded exe for package restore


# Log the build command
$MSBuildExe = $MsbuildPath + 'MSBuild.exe'
$buildCmd = "$MSBuildExe './MicroWin/MicroWin.csproj' /p:Configuration=Release /p:Platform=AnyCPU /verbosity:minimal"
Write-LogLine "Build command: $buildCmd"

# Run MSBuild and capture result
#buildResult = & $MSBuildExe './MicroWin/MicroWin.csproj' /p:Configuration=Release /p:Platform=AnyCPU /verbosity:minimal

# Check if built MicroWin.exe exists
$builtExe = Join-Path (Get-Location) 'MicroWin/bin/Release/MicroWin.exe'
if (Test-Path $builtExe) {
    Write-LogLine "Build completed successfully."
}
else {
    Write-LogLine "ERROR: Cannot find built file MicroWin.exe. Build failed."
    exit 1
}


$buildDuration = (Get-Date) - $BuildStartTime
$minutes = [int]$buildDuration.TotalMinutes
$seconds = [math]::Round($buildDuration.Seconds + $buildDuration.Milliseconds / 1000, 1)
Write-LogLine "Build process completed. Total Build Time: $minutes min $seconds sec`n`n"

# Check if built MicroWin.exe exists
$builtExe = Join-Path (Get-Location) 'MicroWin/bin/Release/MicroWin.exe'
if (!(Test-Path $builtExe)) {
    Write-LogLine "ERROR: Cannot find built file MicroWin.exe. Exiting."
    exit 1
}
$dateTime = Get-Date -Format 'dd-MM-yyyy_HH-mm-ss'
$desktop = [Environment]::GetFolderPath('Desktop')
$targetFolder = Join-Path $desktop "MicroWin Build $dateTime"
New-Item -Path $targetFolder -ItemType Directory -Force | Out-Null
$sourceFolder = Join-Path (Get-Location) 'MicroWin/bin/Release/*'
Copy-Item -Path $sourceFolder -Destination $targetFolder -Recurse -Force
Write-LogLine "MicroWin files moved to $targetFolder.`n`n"

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
        }
        else {
            Write-LogLine "MSBuildTools successfully uninstalled (not detected in registry)."
        }
    }
    else {
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
            }
            else {
                Write-LogLine "MSBuildTools successfully uninstalled (not detected in registry)."
            }
        }
        else {
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
        }
        else {
            Write-LogLine ".NET Framework 4.8 Developer Pack successfully uninstalled (not detected in registry)."
        }
    }
    Write-LogLine "Build tools and installer files deleted.`n`n"
}
else {
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