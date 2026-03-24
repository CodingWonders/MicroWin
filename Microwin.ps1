$tempPath = [System.IO.Path]::GetTempPath()
$zipFile = Join-Path $tempPath "MicroWinDownload.zip"
$extractPath = Join-Path $tempPath "MicroWinExtract"

$moduleRequest = Invoke-WebRequest -UseBasicParsing "https://api.github.com/repos/CodingWonders/MicroWin/releases/latest"

if ($moduleRequest.StatusCode -ne 200) {
    Write-Host "Could not grab data from GitHub API. (Error Code: $($moduleRequest.StatusCode): $($moduleRequest.StatusDescription))"
    return
}

$tempDir = [IO.Path]::GetTempPath().TrimEnd("\")

# Status Code 200 == OK
$assetData = $moduleRequest.Content | ConvertFrom-Json
Write-Host "Downloading release $($assetData.name) (tag $($assetData.tag_name))..."

$downloadUri = ""

if ([Environment]::Is64BitOperatingSystem) {
    $downloadUri = ($assetData.assets | Where-Object { $_.browser_download_url.Contains("x64") }).browser_download_url
} else {
    $downloadUri = ($assetData.assets | Where-Object { $_.browser_download_url.Contains("x86") }).browser_download_url
}

Invoke-WebRequest -Uri "$downloadUri" -OutFile $zipFile

Expand-Archive -Path $zipFile -DestinationPath $extractPath -Force
Remove-Item $zipFile

Start-Process -FilePath (Join-Path $extractPath "MicroWin.exe")