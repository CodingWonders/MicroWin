<# 
#============================================================
# Build Script for MicroWin - UI-Revamp Branch
#
# Description: 
#          Simple build script for MicroWin - UI-Revamp Branch.
#          Looks for the .Net 10 SDK and installs it if necessary.
#          Then just runs the dotnet build command and moves the 
#          output to the desktop.
#          Has a decent logging system and error traps.
#
# Purpose: 
#          The "Double Click" solution that allows "Normies" to build
#          and RUN MicroWin without the AV's and Defender going nuts
#          trying to scare the normies from running the software.
#          
#          There is no MOTW on the the binaires created by this script, 
#          which effectively bypasses the Windows Defender scare screen
#          and the need to pay the M$ TAX aka "THE C.A.".
#
#          No change to your code base; just simply drop the CLICKME.bat
#          and Bhelper.ps1 files into the root of the repository.
#          Instruct normies to download the github repo as a ZIP,
#          extract it right in the Downloads folder, and then double click
#          the CLICKME.bat file.
#          
#          The binaries are built and moved to the desktop with a
#          timestamp suffix (M-dd-yyyy  hh-mm-ss am/pm) matching the log format.
#          Then the BuildLog.txt is moved to the new desktop folder.
#           
#          Before the script exits, it opens the output folder in 
#          File Explorer so the normies can easily click on MicroWin.exe.
#          
# Downloads/Installs:
#          The script will download the .Net 10 SDK (~220 MB) from Microsoft
#          using the Invoke-WebRequest function if not found. The script will
#          then run the install for the .Net 10 SDK, which bloats to 
#          about ~1.2GB of disk space used. This is easily removed from the 
#          Settings->Apps->Apps & features->Microsoft .NET SDK 10.
#
# Notes:
#          I had to run the .Net SDK installer --passive inorder to keep track of
#          the installer process and reliably detect when the installation 
#          was complete so the script would not hang.
#
#          Added a Success Banner for the normies to enjoy! ;)
# ============================================================#>
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$GlobalTimeOutSeconds = 600 # 10 minutes, for long-running installer.
$TerminalWidth = 110        # If it takes longer than 10 Min. to download
$TerminalHeight = 42        # and install the SDK, MicroWin Won't help. ;)
$BufferHeight = 200
$PROJECT_PATH = Join-Path $PSScriptRoot "MicroWin\MicroWin.csproj"
$BUILD_OUTPUT = Join-Path $PSScriptRoot "MicroWin\bin\Release"
$DESKTOP = [Environment]::GetFolderPath("Desktop")
$LOG_FILE = Join-Path $PSScriptRoot "BuildLog.txt"
$LOCK_FILE = Join-Path $PSScriptRoot "build.lock" #RunOnce
$DESTINATION = ""
$BarLine = "=" * $TerminalWidth
$StarLine = "*" * $TerminalWidth
$BannerForegroundColor = "Yellow"
$BannerBackgroundColor = "DarkBlue"
$DotnetVersion = '10.0.201'


$DotnetCandidatePaths = @(
    "$env:ProgramFiles\dotnet\dotnet.exe",
    "$env:ProgramFiles(x86)\dotnet\dotnet.exe",
    "$env:USERPROFILE\.dotnet\dotnet.exe"
)

$asciiMicrowin = @(
    "  MMMM        MMMM  IIIIII    CCCCCCC    RRRRRRRRR      OOOOOOOO      WW      WW      WW  IIIIII  NN      NN"
    "  MM MM      MM MM  IIIIII   CCCCCCCCC   RRRRRRRRRR   OOO      OOO    WW      WW      WW  IIIIII  NNN     NN"
    "  MM  MM    MM  MM    II    CC      CCC  RR      RR  OOO        OOO   WW      WW      WW    II    NNNN    NN"
    "  MM   MM  MM   MM    II    CC           RRRRRRRRRR  OO          OO    WW    WWWW    WW     II    NN NN   NN"
    "  MM    MMMM    MM    II    CC           RRRRRRRRR   OO          OO    WW    WWWW    WW     II    NN  NN  NN"
    "  MM     MM     MM    II    CC           RRRRRRRR    OO          OO    WW    WWWW    WW     II    NN   NN NN"
    "  MM            MM    II    CC           RR   RR     OO          OO     WW  WW  WW  WW      II    NN    NNNN"
    "  MM            MM    II    CC           RR    RR    OO          OO     WW  WW  WW  WW      II    NN     NNN"
    "  MM            MM    II    CC      CCC  RR     RR   OOO        OOO      WWWW    WWWW       II    NN      NN"
    "  MM            MM  IIIIII   CCCCCCCCC   RR      RR   OOO      OOO       WWWW    WWWW     IIIIII  NN      NN"
    "  MM            MM  IIIIII    CCCCCCC    RR       RR    OOOOOOOO          WW      WW      IIIIII  NN      NN"
)

# Attempt to set the terminal size for better formatting. If this fails, 
# it warns the user and continues without resizing. Font is set to Lucida
# Console 18pt in the CLICKME.bat file.
try {
    $h = $host.UI.RawUI
    $h.WindowSize = New-Object Management.Automation.Host.Size($TerminalWidth, $TerminalHeight)
    $h.BufferSize = New-Object Management.Automation.Host.Size($TerminalWidth, $BufferHeight)
}
catch {
    Write-Host "Unable to set terminal size. Output formatting may be inconsistent." -ForegroundColor Yellow
    Write-Host "Continuing without user interaction..." -ForegroundColor Yellow
}



# ==============================-Pause-On-Exit-======================================
# Helper function to pause the script and wait for user input before exiting,
# used in error traps
# Parameters:
#   -Message: The message to display when paused
# ==============================================================================
function Pause-On-Exit {
    param([string]$Message = "Press Enter to continue...")
    Write-Host $Message -ForegroundColor Yellow
    Read-Host
    }trap{  Write-Host "Unhandled error in script: $($_.Exception.Message)" -ForegroundColor Red
        Pause-On-Exit
        break
} # End Pause-On-Exit

# =============================-Logging-Functions-======================================
# Write-Log:
#   Parameters:
#     -msg: The message to log  
#     -Color: The foreground color for the console output (default: White)
#     -bColor: The background color for the console output (default: none)
#     -Raw: If set, the message is logged without a timestamp
# ==============================================================================
function Write-Log {
    param(
        [string]$msg,
        [string]$Color = "White",
        [string]$bColor = $null,
        [switch]$Raw
    )
    $timestamp = Get-Date -Format "(M/dd/yyyy)  hh:mm:ss tt"
    if ($Raw) {
        if ($bColor) {
            Write-Host $msg -ForegroundColor $Color -BackgroundColor $bColor
        } else {
            Write-Host $msg -ForegroundColor $Color
        }
        Add-Content -Path $LOG_FILE -Value "$timestamp $msg"
    } else {
        if ($msg -match "^\n") {
            Write-Host ""; Add-Content -Path $LOG_FILE -Value ""
            $msg = $msg -replace "^\n", ""
        }
        $line = "$timestamp $msg"
        if ($bColor) {
            Write-Host $line -ForegroundColor $Color -BackgroundColor $bColor
        } else {
            Write-Host $line -ForegroundColor $Color
        }
        Add-Content -Path $LOG_FILE -Value $line
    }
}# End of Write-Log function

# Convenience functions for different log levels with preset colors
function Write-Info    { param($msg) Write-Log $msg -Color Cyan }
function Write-Warn    { param($msg) Write-Log $msg -Color Yellow }
function Write-Success { param($msg) Write-Log $msg -Color Green }
function Write-Error   { param($msg) Write-Log $msg -Color Red }

# ==============================-Write-End-======================================
# Description:
#       This function writes a message that is padded to fill the entire terminal width,
#       creating a banner effect. The message is left-aligned and padded with spaces on 
#       the right to fill the line. If the message is longer than the terminal width, it #       is truncated and padded with spaces on the 
#       right to fill the line.
#       The message is logged to the console with the specified colors and also appended 
#       to the log file. The timestamp is not included in the log entry for this
#       function to allow the banner to span the entire width of the terminal without
#       being prefixed by a timestamp.
# Parameters:
#   -msg: The message to write
#   -Color: The foreground color for the console output (default: BannerForegroundColor)
#   -bColor: The background color for the console output (default: BannerBackgroundColor
# 
# ==============================================================================
function Write-End {
    param(
        [string]$msg,
        [string]$Color = $BannerForegroundColor,
        [string]$bColor = $BannerBackgroundColor
    )
    $line = $msg
    $spaces = $TerminalWidth - $line.Length
    if ($spaces -lt 0) { $spaces = 0 }
    $line += " " * $spaces
    if ($bColor) {
        Write-Host $line -ForegroundColor $Color -BackgroundColor $bColor
    } else {
        Write-Host $line -ForegroundColor $Color
    }
    Add-Content -Path $LOG_FILE -Value $line
}# End of Write-End function

# =============================-Write-Centered-======================================
# Description:
#       This function writes a message that is centered within the terminal width.
#       The message is padded with spaces on both sides to center it in the line.
#       The message is logged to the console with the specified colors and also
#       appended to the log file. The timestamp is not included in the log entry.
#       If the message is longer than the terminal width, it is truncated and padded
#       with spaces on both sides to fill the line.
#Parameters:
#   -msg: The message to write  
#   -Color: The foreground color for the console output (default: BannerForegroundColor)
#   -bColor: The background color for the console output (default: BannerBackgroundColor)
# ==============================================================================
function Write-Centered {
    param(
        [string]$msg,
        [string]$Color = $BannerForegroundColor,
        [string]$bColor = $BannerBackgroundColor
    )
    $padLeft = [int](($TerminalWidth - $msg.Length) / 2)
    $padRight = $TerminalWidth - $msg.Length - $padLeft
    if ($padLeft -lt 0) { $padLeft = 0 }
    if ($padRight -lt 0) { $padRight = 0 }
    $line = (" " * $padLeft) + $msg + (" " * $padRight)
    if ($bColor) {
        Write-Host $line -ForegroundColor $Color -BackgroundColor $bColor
    } else {
        Write-Host $line -ForegroundColor $Color
    }
    Add-Content -Path $LOG_FILE -Value $line
}# End Write-Centered

# ==============================-DownloadSDK-===========================
# Description:
#       This function downloads content from a specified URL and saves it to a file.
#       It logs the download progress and any errors to the log file.
# Parameters:
#   -Uri: The URL to download from
#   -OutFile: The file path to save the downloaded content
#   -UseBasicParsing: Switch to use basic parsing (default: $false)
# ==============================================================================
function DownloadSDK {
    param(
        [Parameter(Mandatory=$true)][string]$Uri,
        [Parameter(Mandatory=$true)][string]$OutFile,
        [switch]$UseBasicParsing
    )

    Write-Info "Downloading $Uri to $OutFile"

    try {
        Microsoft.PowerShell.Utility\Invoke-WebRequest -Uri $Uri -OutFile $OutFile -UseBasicParsing:$UseBasicParsing -Verbose -ErrorAction Stop 2>&1 | ForEach-Object { Add-Content -Path $LOG_FILE -Value "$(Get-Date -Format '(M/dd/yyyy)  hh:mm:ss tt') [WebDL] $_" }
        Write-Success "Downloaded $Uri"
        return $true
    } catch {
        Write-Warn "Download failed: $($_.Exception.Message)"
        Add-Content -Path $LOG_FILE -Value "$(Get-Date -Format '(M/dd/yyyy)  hh:mm:ss tt') [WebDL][ERR] $($_.Exception.Message)"
        return $false
    }
} # End DownloadSDK

# =============================-iStart-Process-===========================
# Description:
#       This helper starts a process with the specified executable and arguments,
#       captures its standard output and error, and logs them to the log file with
#       timestamps.
#       If the process does not exit within the specified timeout, it is killed and
#       an error is thrown.
#       If the process exits with a non-zero exit code, an error is logged 
#       but not thrown.
#
# Parameters:
#   -FilePath: The executable to run
#   -ArgumentList: An array of arguments to pass to the executable
#   -TimeoutSeconds: The maximum time to wait.
# ============================================================================== 
function iStart-Process {
    param(
        [Parameter(Mandatory=$true)][string]$FilePath,
        [Parameter(Mandatory=$false)][String[]]$ArgumentList,
        [int]$timeoutMs = $GlobalTimeOutSeconds * 1000
    )

    Write-Info "Starting process: $FilePath $($ArgumentList -join ' ')"
    $startInfo = New-Object System.Diagnostics.ProcessStartInfo
    $startInfo.FileName = $FilePath
    $startInfo.Arguments = $ArgumentList -join ' '
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $startInfo.UseShellExecute = $false
    $startInfo.CreateNoWindow = $true

    $proc = New-Object System.Diagnostics.Process
    $proc.StartInfo = $startInfo
    $proc.Start() | Out-Null

    $stdOut = $proc.StandardOutput.ReadToEndAsync()
    $stdErr = $proc.StandardError.ReadToEndAsync()

    
    if (!$proc.WaitForExit($timeoutMs)) {
        $proc.Kill()
        throw "Process timed out after $TimeoutSeconds seconds: $FilePath"
    }

    $out = $stdOut.Result.Trim()
    $err = $stdErr.Result.Trim()

    if ($out) { Add-Content -Path $LOG_FILE -Value "$(Get-Date -Format '(M/dd/yyyy)  hh:mm:ss tt') [PROCOUT] $out" }
    if ($err) { Add-Content -Path $LOG_FILE -Value "$(Get-Date -Format '(M/dd/yyyy)  hh:mm:ss tt') [PROCERR] $err" }

    return $proc.ExitCode
}# End Start-Process

<# Yep, spent too much time dealing with windows.......#>
function Check-Process {
    foreach ($path in $DotnetCandidatePaths) {
        if (Test-Path $path) {
            return $path
        }
    }

    return $null
}

<# !!!!!!!!!!! WARNING !!!!!!!!!!!!!!!!!!!!   WRITTEN BY AI   AHHHHHHHHHHHHHHHHH            #>
function Test-InstallerSignature {
    param([Parameter(Mandatory=$true)][string]$InstallerPath)

    if (-not (Test-Path $InstallerPath)) {
        return $false
    }

    $sig = Get-AuthenticodeSignature -FilePath $InstallerPath
    if ($sig.Status -ne 'Valid') {
        Write-Warn "Installer signature invalid: $($sig.Status) - $($sig.StatusMessage)"
        return $false
    }

    if ($sig.SignerCertificate -and ($sig.SignerCertificate.Subject -match 'Microsoft Corporation|Microsoft')) {
        Write-Info "Installer signature chain valid and signed by Microsoft: $($sig.SignerCertificate.Subject)"
        return $true
    }

    Write-Warn "Installer signature is valid but signer is not explicitly Microsoft: $($sig.SignerCertificate.Subject)"
    return $true
}


<#
# =============================-Install-SDK-======================================
# Description:
#       Installs .Net 10 SDK from Micro$oft, and installs the bloat.
#       1st check to see if SDK is already installed;
#       Next check for installer exe in our Downloads folder.
#       Then download it if we have to....
# Notes:
#       I had trouble keeping the script from hanging, trying to detect when the 
#       was finished. Running the SDK installer --passive was the only way I could 
#       get it to work reliably.
# ==============================================================================#>
function Install-SDK {
    Write-Info "Using fixed .NET SDK installer URL for version $DotnetVersion."
    $url = "https://builds.dotnet.microsoft.com/dotnet/Sdk/$DotnetVersion/dotnet-sdk-$DotnetVersion-win-x64.exe"

    # Use script folder
    $downloadDir = Join-Path $PSScriptRoot 'Downloads'
    if (-not (Test-Path $downloadDir)) { New-Item -Path $downloadDir -ItemType Directory | Out-Null }

    $installerPath = Join-Path $downloadDir "dotnet-sdk-$DotnetVersion-win-x64.exe"
    $installerNeedsDownload = $true

    if (Test-Path $installerPath) {
        Write-Info "Existing installer found at $installerPath"
        if (Test-InstallerSignature -InstallerPath $installerPath) {
            Write-Info "Installer signature valid; skipping download."
            $installerNeedsDownload = $false
        } else {
            Write-Warn "Installer signature invalid or not trusted; removing and redownloading."
            Remove-Item -Path $installerPath -Force -ErrorAction SilentlyContinue
        }
    }

    if ($installerNeedsDownload) {
        Write-Info "Downloading .NET SDK installer from $url to $installerPath..."
        try {
            if (-not (DownloadSDK -Uri $url -OutFile $installerPath -UseBasicParsing)) { return $false }
        } catch {
            Write-Warn "Failed to download .NET SDK installer: $($_.Exception.Message)"
            return $false
        }

        if (-not (Test-InstallerSignature -InstallerPath $installerPath)) {
            Write-Error "Downloaded installer signature verification failed."
            return $false
        }

        Write-Info "Downloaded installer signature verified successfully."
    }

    Write-Info "Running installer $installerPath..."

    Write-Info "Running installer $installerPath..."
    $logPath = Join-Path $env:TEMP "dotnet-sdk-$($DotnetVersion.Replace('.','_'))-install.log"
    $procExit = iStart-Process -FilePath $installerPath -ArgumentList @('/passive','/norestart','/log',$logPath)

    if (Test-Path $logPath) {
        Get-Content $logPath | ForEach-Object { Add-Content -Path $LOG_FILE -Value "$(Get-Date -Format '(M/dd/yyyy)  hh:mm:ss tt') [MSI-INSTALL] $_" }
    }

    if ($procExit -ne 0) {
        Write-Warn "MSI installer returned exit code $procExit, log: $logPath"
        return $false
    }

    # Wait for installer processes to finish, and break as soon as the requested .NET SDK version is detectable.
    $start = Get-Date
    $timeoutSeconds = $GlobalTimeOutSeconds
    $pollSeconds = 5
    $dotnetReady = $false

    while ((Get-Date) -lt $start.AddSeconds($timeoutSeconds)) {
        $dotnetExe = Check-Process

        if ($dotnetExe) {
            $sdkInstalled = (& "$dotnetExe" --list-sdks 2>$null | Select-String -Pattern "^$DotnetVersion\b" -Quiet)
            if ($sdkInstalled) {
                Write-Info "Detected .NET SDK $DotnetVersion after installer wait."
                $dotnetReady = $true
                break
            }
        }

        $installerProcs = Get-Process -Name @('msiexec','winget','setup','setuphost') -ErrorAction SilentlyContinue

        if ($installerProcs) {
            Write-Info "Installer helper process still running: $($installerProcs.Name -join ', ')"
            Start-Sleep -Seconds $pollSeconds
            continue
        }

        # SDK not yet detected and installer helper processes are gone; keep waiting.
        Start-Sleep -Seconds $pollSeconds
    }

    if (-not $dotnetReady) {
        Write-Warn "Installer process wait timed out after $timeoutSeconds seconds."
    }

    $dotnetExe = Check-Process
    if (-not $dotnetExe) {
        $installDir = "$env:USERPROFILE\.dotnet"
        if (Test-Path $installDir) {
            $currentPath = [Environment]::GetEnvironmentVariable('PATH','Process')
            if ($currentPath -notmatch [regex]::Escape($installDir)) {
                $newPath = "$installDir;$currentPath"
                [Environment]::SetEnvironmentVariable('PATH', $newPath, 'Process')
                $env:PATH = $newPath
                Write-Info "Added $installDir to process PATH."
            }

            if ($env:DOTNET_ROOT -ne $installDir) {
                [Environment]::SetEnvironmentVariable('DOTNET_ROOT', $installDir, 'Process')
                $env:DOTNET_ROOT = $installDir
                Write-Info "Set DOTNET_ROOT to $installDir for current process."
            }
        }

        $dotnetExe = Check-Process
    }

    if ($dotnetExe) {
        Write-Info "dotnet is available after MSI install: $dotnetExe"
        $dotnetReady = $true
    } else {
        Write-Warn "dotnet not found after MSI install. Will rely on system PATH refresh."
        $dotnetReady = $false
    }

    Start-Sleep -Seconds 5
    return $dotnetReady
}# End Install-SDK

<# 
#=============================-Build-Project-===========================================
# Description:
#        Invokes dotnet build targeting the Release configuration against the MicroWin
#        project. 
#        Output is piped through Write-Log -Raw so dotnet's native formatting is 
#        preserved on screen while still being timestamped in the log.
#        Exit code is captured after the pipeline to reliably detect build failures.
# Parameters:
#        -Configuration: The build configuration to use (default: "Release")   
#        -OutputPath: The directory to place the build output 
#          (default: "$PSScriptRoot\MicroWin\bin\Release")
# ==============================================================================
#>
function Build-Project {
    param (
        [string]$Configuration = "Release",
        [string]$OutputPath = (Join-Path $PSScriptRoot "MicroWin\bin\Release")
    )
    Write-Info "Build-Project(Configuration=$Configuration, OutputPath=$OutputPath)"
    Write-Info "Starting build process for MicroWin..."

    # Construct the project path
    $PROJECT_PATH = Join-Path $PSScriptRoot "MicroWin\MicroWin.csproj"
    Write-Info "Constructed project path: $PROJECT_PATH"

    if (-not (Test-Path $PROJECT_PATH)) {
        Write-Error "No project file named 'MicroWin' found at: $PROJECT_PATH"
        throw "Build process aborted due to missing project file."
    }
    Write-Info "Found project file: $PROJECT_PATH"

    # Ensure .NET SDK is installed
    Write-Info "Checking if .NET SDK is installed..."
    try {
        $dotnetExe = Check-Process

        if (-not $dotnetExe) {
            Write-Warn "dotnet executable not found. Attempting to install SDK..."
            Install-SDK
            $dotnetExe = Check-Process
        }

        Write-Info "Using dotnet executable: $dotnetExe"
    } catch {
        Write-Error "Failed to check for .NET SDK. Error: $($_.Exception.Message)"
        throw "Build process aborted due to .NET SDK check failure."
    }

    # Build the project
    Write-Info "Building project: $PROJECT_PATH"
    Write-Info "Build command: $dotnetExe build $PROJECT_PATH --configuration $Configuration --output $OutputPath"

    try {
        & $dotnetExe 'build' $PROJECT_PATH '--configuration' $Configuration '--output' $OutputPath
    } catch {
        Write-Error "Build command execution failed. Error: $($_.Exception.Message)"
        throw "Build process failed."
    }

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed. Check the build log for details."
        Pause
        throw "Build process failed."
    } else {
        Write-Success "Build succeeded. Output located at: $OutputPath"
    }
}# End Build-Project

# =============================-Move-BuildOutput-======================================
# Verifies the build output folder exists then moves it to the desktop with a
# timestamp suffix (M-dd-yyyy  hh-mm-ss am/pm) matching the log format.
# The build log is moved into the output folder.
# ==============================================================================
function Move-BuildOutput {
    Write-Info "`nVerifying build output..."
    if (-not (Test-Path $BUILD_OUTPUT)) { throw "Build output not found at: $BUILD_OUTPUT" }
    $script:DESTINATION = "$DESKTOP\MicroWin $(Get-Date -Format 'M-dd-yyyy  hh-mm-ss tt')"
    Write-Info "Moving output to desktop: $script:DESTINATION"
    Move-Item -Path $BUILD_OUTPUT -Destination $script:DESTINATION
    
    Write-Success "Output moved to: $script:DESTINATION"
    Move-Item -Path $LOG_FILE -Destination "$script:DESTINATION\BuildLog.txt"
    
    # redirect further logging to the moved file
    $script:LOG_FILE = "$script:DESTINATION\BuildLog.txt" 
    

    Write-Log "Build process completed."
    Write-Log "The MicroWin folder contains the new Software and the BuildLog.txt file."
    Write-Log "===== Build Complete ====="

    Write-End $barLine
    Write-End ""
    foreach ($line in $asciiMicrowin) { Write-End $line }
    Write-End ""
    Write-End $StarLine 
    Write-End ""
    Write-Centered "CONGRATULATIONS! YOU HAVE SUCCESSFULLY BUILT MICROWIN!"
    Write-End ""
    Write-Centered "You are building Software from Source Code like a REAL LINUX CHAD now!"
    Write-End ""
    Write-Centered "These are the people who have created or helped improve MicroWin over the years"
    Write-Centered "in [WinUtil](https://github.com/ChrisTitusTech/winutil), *in no particular order*:"
    Write-End ""
    Write-End "             [KonTy](https://github.com/KonTy)"
    Write-End "             [CodingWonders](https://github.com/CodingWonders)"
    Write-End "             [Real-MullaC](https://github.com/Real-MullaC)"
    Write-End "             [MyDrift-user](https://github.com/MyDrift-user)"
    Write-End "             [og-mrk](https://github.com/og-mrk)"
    Write-End "             [Chris Titus](https://github.com/ChrisTitusTech)"
    Write-End "             [psyirius](https://github.com/psyirius)"
    Write-End ""
    Write-Centered "These are the tools that have helped during testing:"
    Write-End ""
    Write-Centered "[DISMTools](https://github.com/CodingWonders/DISMTools)"
    Write-End ""
    Write-End $StarLine
    Write-End ""
    Write-Centered "Thank you for using MicroWin!"
    Write-End ""
    Write-End $barLine
}# End Move-BuildOutput

# =============================-Main-===========================================
# Orchestrates the full build pipeline. On first run each dependency is checked
# and installed only if missing. 
# ==============================================================================
function Main {
    "" | Set-Content $LOG_FILE
    Write-Log "Using PowerShell $($PSVersionTable.PSVersion)" -ForegroundColor Yellow
    Write-Log "===== Build Started ====="

    if (Test-Path $LOCK_FILE) { Write-Error "Another instance is already running. Exiting."; Pause-On-Exit; exit 1 }
    New-Item $LOCK_FILE -ItemType File | Out-Null
    
    while ($true) {
        try {
                Build-Project
                Move-BuildOutput
                break
            } catch {
            # ---- Failure banner ----
            Write-Host ""
            Write-Host "  !! BUILD FAILED -- $($_.Exception.Message)" -ForegroundColor Red
            Write-Host ""

            # ---- Prompt for retry and log both the question and the user response ----
            Write-Log "=====-Build Failed!-====="
            break
        }
    } # End of while loop
} # End of Main function

# <---- Entry Point ----
try { Main } 
finally {
    # Always release the lock file
    Remove-Item $LOCK_FILE -Force -ErrorAction SilentlyContinue

    # If the log still exists here it means an unexpected error.
    # Save it with a failure timestamp so it is not lost
    $scriptLog = Join-Path $PSScriptRoot 'BuildLog.txt'
    if (Test-Path $scriptLog) {
        [System.GC]::Collect()
        [System.GC]::WaitForPendingFinalizers()

        $ts = Get-Date -Format "M-dd-yyyy  hh-mm-ss tt"
        $failedLog = Join-Path $PSScriptRoot "BuildLog-FAILED-$ts.txt"
        Move-Item -Path $LOG_FILE -Destination $failedLog
        Write-End "Log saved to: $failedLog"
        Pause
    }
}

Read-Host "`nPress Enter to continue..."

# Open the output folder in File Explorer if the destination exists
if ($script:DESTINATION -and (Test-Path -Path $script:DESTINATION)) {
    Start-Process explorer.exe $script:DESTINATION
} else {
    Write-Warn "Output destination not found: $script:DESTINATION. Skipping explorer launch."
}
