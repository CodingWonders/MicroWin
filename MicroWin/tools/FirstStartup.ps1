# Set the global error action preference to continue
$ErrorActionPreference = "Continue"

"FirstStartup has worked" | Out-File -FilePath "$env:HOMEDRIVE\windows\LogFirstRun.txt" -Append -NoClobber

# Delete all files on the Taskbar
$taskbarPath = "$Env:AppData\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar"

if (Test-Path "$taskbarPath") {
    Remove-Item "$taskbarPath\*"
}

# Delete Edge desktop icon
Remove-Item "$Env:Public\Desktop\Microsoft Edge.lnk"

if ((Get-BitLockerVolume -MountPoint $Env:SystemDrive).ProtectionStatus -eq 'On') {
    Write-Host "Disabling BitLocker..."
    Disable-BitLocker -MountPoint $Env:SystemDrive
}

# Get BCD entries and set bootmgr timeout accordingly
# Check if the number of occurrences of "path" is 2 - this fixes the Boot Manager screen issue

    # Check if the number of occurrences of "path" is 2 - this fixes the Boot Manager screen issue (#2562)
if ((bcdedit | Select-String "path").Count -eq 2) {
    # Set bootmgr timeout to 0
    bcdedit /set  `{bootmgr`} timeout 0
}

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Notifications\Settings\Windows.SystemToast.Suggested" /f
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Notifications\Settings\Windows.SystemToast.Suggested" /v Enabled /t REG_DWORD /d 0 /f
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Notifications\Settings\Windows.SystemToast.StartupApp" /f
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Notifications\Settings\Windows.SystemToast.StartupApp" /v Enabled /t REG_DWORD /d 0 /f
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Notifications\Settings\Microsoft.SkyDrive.Desktop" /f
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Notifications\Settings\Microsoft.SkyDrive.Desktop" /v Enabled /t REG_DWORD /d 0 /f
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Notifications\Settings\Windows.SystemToast.AccountHealth" /f
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Notifications\Settings\Windows.SystemToast.AccountHealth" /v Enabled /t REG_DWORD /d 0 /f

# This will set List view in Start menu on Win11 25H2. This will not do anything in 24H2 and older
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Start" /v AllAppsViewMode /t REG_DWORD /d 2 /f

# This will disable the Recommendations in 25H2.
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\Explorer" /v HideRecommendedSection /t REG_DWORD /d 0 /f
reg add "HKLM\SOFTWARE\Microsoft\PolicyManager\current\device\Start" /v HideRecommendedSection /t REG_DWORD /d 0 /f
reg add "HKLM\SOFTWARE\Microsoft\PolicyManager\current\device\Education" /v IsEducationEnvironment /t REG_DWORD /d 0 /f

# Other Start Menu settings
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v Start_AccountNotifications /t REG_DWORD /d 0 /f
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Start" /v ShowAllPinsList /t REG_DWORD /d 0 /f
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Start" /v ShowFrequentList /t REG_DWORD /d 0 /f
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Start" /v ShowRecentList /t REG_DWORD /d 0 /f
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v Start_TrackDocs /t REG_DWORD /d 0 /f

# Enable Dark Mode
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v "AppsUseLightTheme" /t REG_DWORD /d 0 /f
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v "SystemUsesLightTheme" /t REG_DWORD /d 0 /f

# Send the WM_SETTINGCHANGE message to all windows
Add-Type -TypeDefinition @"
using System;
using System.Runtime.InteropServices;
public class Win32 {
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
    public static extern IntPtr SendMessageTimeout(
        IntPtr hWnd,
        uint Msg,
        IntPtr wParam,
        string lParam,
        uint fuFlags,
        uint uTimeout,
        out IntPtr lpdwResult);
}
"@

$HWND_BROADCAST = [IntPtr]0xffff
$WM_SETTINGCHANGE = 0x1A
$SMTO_ABORTIFHUNG = 0x2
$timeout = 100

# Send the broadcast message to all windows
[Win32]::SendMessageTimeout($HWND_BROADCAST, $WM_SETTINGCHANGE, [IntPtr]::Zero, "ImmersiveColorSet", $SMTO_ABORTIFHUNG, $timeout, [ref]([IntPtr]::Zero))

