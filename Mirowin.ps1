$tempPath = [System.IO.Path]::GetTempPath()
$zipFile = Join-Path $tempPath "MicroWinDownload.zip"
$extractPath = Join-Path $tempPath "MicroWinExtract"

Invoke-WebRequest -Uri "https://github.com/CodingWonders/MicroWin/releases/download/latest/MicroWin.zip" -OutFile $zipFile

Expand-Archive -Path $zipFile -DestinationPath $extractPath -Force
Remove-Item $zipFile

Start-Process -FilePath (Join-Path $extractPath "MicroWin.exe") -Wait