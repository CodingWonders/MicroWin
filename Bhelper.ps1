param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Build')]
    [string]$Action,

    [Parameter(Mandatory = $true)]
    [string]$ScriptDir
)

$ErrorActionPreference = 'Stop'

$normalizedScriptDir = $ScriptDir
if ($null -eq $normalizedScriptDir) {
    throw 'ScriptDir parameter is missing.'
}
$normalizedScriptDir = $normalizedScriptDir.Trim()
$normalizedScriptDir = $normalizedScriptDir.Trim('"')
if ([string]::IsNullOrWhiteSpace($normalizedScriptDir)) {
    throw 'ScriptDir parameter is empty after normalization.'
}
$script:ScriptDir = [System.IO.Path]::GetFullPath($normalizedScriptDir)
$script:ToolsDir = Join-Path $script:ScriptDir 'tools'
$script:LogFile = Join-Path $script:ScriptDir 'BuildLog.txt'
$script:NugetExe = Join-Path $script:ToolsDir 'nuget.exe'
$script:NugetUrl = 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe'
$script:BuildToolsInstaller = Join-Path $script:ToolsDir 'vs_buildtools.exe'
$script:BuildToolsUrl = 'https://aka.ms/vs/17/release/vs_buildtools.exe'
$script:LockDir = Join-Path $script:ScriptDir '.build.lock'
$script:ProjectPath = Join-Path $script:ScriptDir 'MicroWin\MicroWin.csproj'

function Write-LogLine {
    param([string]$Message)

    $line = "[{0}] {1}" -f (Get-Date -Format 'yyyy-MM-dd HH:mm:ss.fff'), $Message
    Write-Host $Message
    Add-Content -Path $script:LogFile -Value $line -Encoding UTF8
}

function Write-LogOutputLine {
    param([string]$Message)

    $line = "[{0}] {1}" -f (Get-Date -Format 'yyyy-MM-dd HH:mm:ss.fff'), $Message
    Add-Content -Path $script:LogFile -Value $line -Encoding UTF8
}

function Invoke-Download {
    param(
        [string]$Url,
        [string]$OutFile
    )

    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Write-LogLine "Downloading: $Url"
    Invoke-WebRequest -Uri $Url -OutFile $OutFile
    Write-LogLine "Saved: $OutFile"
}

function Invoke-External {
    param(
        [string]$FilePath,
        [string[]]$Arguments,
        [string]$WorkingDirectory
    )

    if (-not [string]::IsNullOrWhiteSpace($WorkingDirectory)) {
        Push-Location $WorkingDirectory
    }

    try {
        $argText = if ($Arguments) { ($Arguments -join ' ') } else { '' }
        Write-LogLine "Running: $FilePath $argText".Trim()

        & $FilePath @Arguments 2>&1 | ForEach-Object {
            $text = [string]$_
            Write-Host $text
            Write-LogOutputLine -Message $text
        }

        $exitCode = $LASTEXITCODE
        if ($null -eq $exitCode) {
            $exitCode = 0
        }

        if ($exitCode -ne 0) {
            throw "Command failed with exit code $exitCode"
        }
    }
    finally {
        if (-not [string]::IsNullOrWhiteSpace($WorkingDirectory)) {
            Pop-Location
        }
    }
}

function Find-MSBuild {
    $candidates = @(
        'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files (x86)\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files (x86)\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files (x86)\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe'
    )

    foreach ($candidate in $candidates) {
        if (Test-Path -Path $candidate) {
            return $candidate
        }
    }

    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path -Path $vswhere) {
        $found = & $vswhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1
        if ($found) {
            return $found
        }
    }

    return $null
}

function Find-BuiltExe {
    $candidates = @(
        (Join-Path $script:ScriptDir 'MicroWin\bin\Release\MicroWin.exe'),
        (Join-Path $script:ScriptDir 'MicroWin\bin\AnyCPU\Release\MicroWin.exe'),
        (Join-Path $script:ScriptDir 'MicroWin\bin\x64\Release\MicroWin.exe'),
        (Join-Path $script:ScriptDir 'MicroWin\bin\x86\Release\MicroWin.exe')
    )

    foreach ($candidate in $candidates) {
        if (Test-Path -Path $candidate) {
            return $candidate
        }
    }

    return $null
}

function Confirm-Yes {
    param([string]$Prompt)

    $choice = Read-Host $Prompt
    return ($choice -match '^(?i)y(es)?$')
}

function Initialize-Log {
    $tries = 0
    while ($tries -lt 5) {
        try {
            Set-Content -Path $script:LogFile -Value @(
                '==================================================',
                "MicroWin Build Log - $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss.fff')",
                "Script: $([System.IO.Path]::Combine($script:ScriptDir, 'build.bat'))",
                '==================================================',
                ''
            ) -Encoding UTF8
            return
        }
        catch {
            $tries++
            Start-Sleep -Seconds 1
        }
    }

    throw 'Could not initialize BuildLog.txt because it is in use.'
}

function Acquire-Lock {
    if (Test-Path -Path $script:LockDir) {
        throw 'Another build process is already running. Close the other build window and try again.'
    }

    New-Item -Path $script:LockDir -ItemType Directory -Force | Out-Null
}

function Release-Lock {
    if (Test-Path -Path $script:LockDir) {
        Remove-Item -Path $script:LockDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}

function Run-Build {
    Acquire-Lock

    try {
        if (-not (Test-Path -Path $script:ToolsDir)) {
            New-Item -Path $script:ToolsDir -ItemType Directory | Out-Null
        }

        Initialize-Log

        Write-Host '========================================'
        Write-Host 'MicroWin Build Script'
        Write-Host '========================================'
        Write-Host ''
        Write-LogLine "Build log: $script:LogFile"

        Write-Host ''
        Write-LogLine '[1/4] Checking for MSBuild...'
        $msbuildPath = Find-MSBuild
        if (-not $msbuildPath) {
            Write-LogLine 'MSBuild not found. Downloading minimal build tools...'
            Invoke-Download -Url $script:BuildToolsUrl -OutFile $script:BuildToolsInstaller

            Write-LogLine 'Installing minimal build tools (MSBuild + .NET Framework 4.8 SDK)...'
            Write-LogLine 'This will download ~500MB and install required components.'
            Invoke-External -FilePath $script:BuildToolsInstaller -Arguments @(
                '--quiet','--wait','--norestart','--nocache',
                '--installPath',"$env:ProgramFiles(x86)\Microsoft Visual Studio\2022\BuildTools",
                '--add','Microsoft.VisualStudio.Workload.MSBuildTools',
                '--add','Microsoft.Net.Component.4.8.SDK',
                '--add','Microsoft.Net.Component.4.8.TargetingPack'
            ) -WorkingDirectory $script:ScriptDir

            $msbuildPath = Find-MSBuild
            if (-not $msbuildPath) {
                throw 'MSBuild not found after installation.'
            }
        }
        Write-LogLine "Found MSBuild at: $msbuildPath"

        Write-Host ''
        Write-LogLine '[2/4] Checking for NuGet...'
        if (-not (Test-Path -Path $script:NugetExe)) {
            Write-LogLine 'NuGet not found. Downloading NuGet.exe ~8MB...'
            Invoke-Download -Url $script:NugetUrl -OutFile $script:NugetExe
        }
        Write-LogLine "Found NuGet at: $script:NugetExe"

        Write-Host ''
        Write-LogLine '[3/4] Restoring NuGet packages...'
        Invoke-External -FilePath $script:NugetExe -Arguments @('restore','MicroWin\MicroWin.csproj','-PackagesDirectory','packages') -WorkingDirectory $script:ScriptDir
        Write-LogLine 'NuGet restore completed.'

        Write-Host ''
        Write-LogLine '[4/4] Building MicroWin...'
        Invoke-External -FilePath $msbuildPath -Arguments @('MicroWin\MicroWin.csproj','/p:Configuration=Release','/p:Platform=AnyCPU','/verbosity:minimal') -WorkingDirectory $script:ScriptDir

        Write-Host ''
        Write-Host '========================================'
        Write-Host 'Build completed successfully!'
        Write-Host '========================================'
        Write-Host ''
        Write-LogLine 'Build completed successfully.'

        $builtExe = Find-BuiltExe
        $outputDir = Join-Path $script:ScriptDir 'MicroWin\bin\Release\'
        if ($builtExe) {
            Write-LogLine "Output location: $builtExe"
            $outputDir = [System.IO.Path]::GetDirectoryName($builtExe)
            if (-not $outputDir.EndsWith('\')) {
                $outputDir = "$outputDir\"
            }
        }
        else {
            Write-LogLine 'Output location: not found'
        }

        Write-Host ''
        $desktopBuildDir = Join-Path $env:USERPROFILE 'Desktop\MicroWin-Build'
        if ($builtExe) {
            if (Confirm-Yes 'Copy build output files to Desktop\MicroWin-Build? (Y/N)') {
                if (Test-Path -Path $desktopBuildDir) {
                    Remove-Item -Path $desktopBuildDir -Recurse -Force -ErrorAction SilentlyContinue
                }
                New-Item -Path $desktopBuildDir -ItemType Directory -Force | Out-Null
                Copy-Item -Path (Join-Path $outputDir '*') -Destination $desktopBuildDir -Recurse -Force
                if (Test-Path -Path $script:LogFile) {
                    Copy-Item -Path $script:LogFile -Destination (Join-Path $desktopBuildDir 'BuildLog.txt') -Force
                }
                Write-LogLine "Copied build output to: $desktopBuildDir"
            }
            else {
                Write-LogLine 'Desktop output copy skipped by user.'
            }
        }
        else {
            Write-LogLine 'WARNING: Built executable not found, skipping Desktop copy option.'
        }

        Write-Host ''
        if (Confirm-Yes 'Remove downloaded build utility files from tools folder? (Y/N)') {
            if (Test-Path -Path $script:NugetExe) { Remove-Item -Path $script:NugetExe -Force -ErrorAction SilentlyContinue }
            if (Test-Path -Path $script:BuildToolsInstaller) { Remove-Item -Path $script:BuildToolsInstaller -Force -ErrorAction SilentlyContinue }
            if (Test-Path -Path $script:ToolsDir) {
                try { Remove-Item -Path $script:ToolsDir -Force -ErrorAction Stop } catch {}
            }
            Write-LogLine 'Removed downloaded build utility files from tools folder.'
        }
        else {
            Write-LogLine 'Build utility cleanup skipped by user.'
        }

        Write-Host ''
        if (Confirm-Yes 'Open build output folder now? (Y/N)') {
            if (Test-Path -Path $outputDir) {
                Start-Process -FilePath $outputDir | Out-Null
                Write-LogLine "Opened output folder: $outputDir"
            }
            else {
                Write-LogLine 'WARNING: Output folder not found.'
            }
        }
        else {
            Write-LogLine 'Open output folder skipped by user.'
        }

        Write-Host ''
        Write-LogLine 'Build script finished.'
        exit 0
    }
    catch {
        Write-LogLine "ERROR: $($_.Exception.Message)"
        exit 1
    }
    finally {
        Release-Lock
    }
}

if ($Action -eq 'Build') {
    Run-Build
}
