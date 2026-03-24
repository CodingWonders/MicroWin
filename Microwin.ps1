$tempPath = [System.IO.Path]::GetTempPath()
$zipFile = Join-Path $tempPath "MicroWinDownload.zip"
$extractPath = Join-Path $tempPath "MicroWinExtract"

if ([Environment]::Is64BitOperatingSystem) {
    Invoke-WebRequest -Uri "https://github.com/CodingWonders/MicroWin/releases/download/latest/MicroWin_x64.zip" -OutFile $zipFile
} else {
    Invoke-WebRequest -Uri "https://github.com/CodingWonders/MicroWin/releases/download/latest/MicroWin_x86.zip" -OutFile $zipFile
}

Expand-Archive -Path $zipFile -DestinationPath $extractPath -Force
Remove-Item $zipFile

Start-Process -FilePath (Join-Path $extractPath "MicroWin.exe") -Wait