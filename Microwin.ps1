[Net.ServicePointManager]::SecurityProtocol = "Tls12"

$tempPath = [System.IO.Path]::GetTempPath()
$exeFile = Join-Path $tempPath "MicroWin.exe"
$extractPath = Join-Path $tempPath "MicroWinExtract"

$moduleRequest = Invoke-WebRequest -UseBasicParsing "https://api.github.com/repos/CodingWonders/MicroWin/releases/latest"

if ($moduleRequest.StatusCode -ne 200) {
    Write-Host "Could not grab data from GitHub API. (Error Code: $($moduleRequest.StatusCode): $($moduleRequest.StatusDescription))"
    return
}

$tempDir = [IO.Path]::GetTempPath().TrimEnd("\")

# Status Code 200 == OK
$assetData = $moduleRequest.Content | ConvertFrom-Json

$downloadUri = ""
$size = 0

if ([Environment]::Is64BitOperatingSystem) {
    $downloadUri = ($assetData.assets | Where-Object { $_.browser_download_url.Contains("x64") }).browser_download_url
    $size = ($assetData.assets | Where-Object { $_.browser_download_url.Contains("x64") }).size
} else {
    $downloadUri = ($assetData.assets | Where-Object { $_.browser_download_url.Contains("x86") }).browser_download_url
    $size = ($assetData.assets | Where-Object { $_.browser_download_url.Contains("x86") }).size
}

Write-Host "Downloading release $($assetData.name) ($([Math]::Round($size / 1MB, 2)) MB)..."

# To get rid of long delay times we will not show any progress output
$ProgressPreference = 'SilentlyContinue'
Invoke-WebRequest -Uri "$downloadUri" -OutFile $exeFile
$ProgressPreference = 'Continue'

Start-Process -FilePath (Join-Path $tempPath "MicroWin.exe")