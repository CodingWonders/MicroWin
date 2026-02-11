using MicroWin.functions.Helpers.Loggers;
using System;
using System.IO;
using System.Text;

namespace MicroWin.functions.dism
{
    public class UnattendGenerator
    {
        public static void CreateUnattend(string destinationPath)
        {
            StringBuilder xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xml.AppendLine("<unattend xmlns=\"urn:schemas-microsoft-com:unattend\">");

            // --- Specialized Settings Pass ---
            xml.AppendLine("  <settings pass=\"specialize\">");
            xml.AppendLine("    <component name=\"Microsoft-Windows-SQMApi\" processorArchitecture=\"amd64\" publicKeyToken=\"31bf3856ad364e35\" language=\"neutral\" versionScope=\"nonSxS\" xmlns:wcm=\"http://schemas.microsoft.com/WMIConfig/2002/State\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            xml.AppendLine("      <CEIPEnabled>0</CEIPEnabled>");
            xml.AppendLine("    </component>");
            xml.AppendLine("    <component name=\"Microsoft-Windows-Shell-Setup\" processorArchitecture=\"amd64\" publicKeyToken=\"31bf3856ad364e35\" language=\"neutral\" versionScope=\"nonSxS\" xmlns:wcm=\"http://schemas.microsoft.com/WMIConfig/2002/State\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            xml.AppendLine("      <ConfigureChatAutoInstall>false</ConfigureChatAutoInstall>");
            xml.AppendLine("    </component>");
            xml.AppendLine("    <component name=\"Microsoft-Windows-Deployment\" processorArchitecture=\"amd64\" publicKeyToken=\"31bf3856ad364e35\" language=\"neutral\" versionScope=\"nonSxS\">");
            xml.AppendLine("      <RunSynchronous>");

            // Adding all 49 commands from your snippet
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>1</Order><Path>reg.exe add \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\OOBE\" /v BypassNRO /t REG_DWORD /d 1 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>2</Order><Path>reg.exe load \"HKU\\DefaultUser\" \"C:\\Users\\Default\\NTUSER.DAT\"</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>3</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\Runonce\" /v \"UninstallCopilot\" /t REG_SZ /d \"powershell.exe -NoProfile -Command \\\"Get-AppxPackage -Name 'Microsoft.Windows.Ai.Copilot.Provider' | Remove-AppxPackage;\\\"\" /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>4</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Policies\\Microsoft\\Windows\\WindowsCopilot\" /v TurnOffWindowsCopilot /t REG_DWORD /d 1 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>5</Order><Path>reg.exe unload \"HKU\\DefaultUser\"</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>6</Order><Path>reg.exe delete \"HKLM\\SOFTWARE\\Microsoft\\WindowsUpdate\\Orchestrator\\UScheduler_Oobe\\DevHomeUpdate\" /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>7</Order><Path>reg.exe load \"HKU\\DefaultUser\" \"C:\\Users\\Default\\NTUSER.DAT\"</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>8</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Notepad\" /v ShowStoreBanner /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>9</Order><Path>reg.exe unload \"HKU\\DefaultUser\"</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>10</Order><Path>cmd.exe /c \"del \"C:\\Users\\Default\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\OneDrive.lnk\"\"</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>11</Order><Path>cmd.exe /c \"del \"C:\\Windows\\System32\\OneDriveSetup.exe\"\"</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>12</Order><Path>cmd.exe /c \"del \"C:\\Windows\\SysWOW64\\OneDriveSetup.exe\"\"</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>13</Order><Path>reg.exe load \"HKU\\DefaultUser\" \"C:\\Users\\Default\\NTUSER.DAT\"</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>14</Order><Path>reg.exe delete \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\Run\" /v OneDriveSetup /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>15</Order><Path>reg.exe unload \"HKU\\DefaultUser\"</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>16</Order><Path>reg.exe delete \"HKLM\\SOFTWARE\\Microsoft\\WindowsUpdate\\Orchestrator\\UScheduler_Oobe\\OutlookUpdate\" /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order(17</Order><Path>reg.exe add \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Communications\" /v ConfigureChatAutoInstall /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>18</Order><Path>powershell.exe -NoProfile -Command \"$xml = [xml]::new(); $xml.Load('C:\\Windows\\Panther\\unattend.xml'); $sb = [scriptblock]::Create( $xml.unattend.Extensions.ExtractScript ); Invoke-Command -ScriptBlock $sb -ArgumentList $xml;\"</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>19</Order><Path>reg.exe add \"HKLM\\SOFTWARE\\Microsoft\\PolicyManager\\current\\device\\Start\" /v ConfigureStartPins /t REG_SZ /d \"{ \\\"pinnedList\\\": [] }\" /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>20</Order><Path>reg.exe add \"HKLM\\SOFTWARE\\Microsoft\\PolicyManager\\current\\device\\Start\" /v ConfigureStartPins_ProviderSet /t REG_DWORD /d 1 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>21</Order><Path>reg.exe add \"HKLM\\SOFTWARE\\Microsoft\\PolicyManager\\current\\device\\Start\" /v ConfigureStartPins_WinningProvider /t REG_SZ /d B5292708-1619-419B-9923-E5D9F3925E71 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>22</Order><Path>reg.exe add \"HKLM\\SOFTWARE\\Microsoft\\PolicyManager\\providers\\B5292708-1619-419B-9923-E5D9F3925E71\\default\\Device\\Start\" /v ConfigureStartPins /t REG_SZ /d \"{ \\\"pinnedList\\\": [] }\" /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>23</Order><Path>reg.exe add \"HKLM\\SOFTWARE\\Microsoft\\PolicyManager\\providers\\B5292708-1619-419B-9923-E5D9F3925E71\\default\\Device\\Start\" /v ConfigureStartPins_LastWrite /t REG_DWORD /d 1 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>24</Order><Path>net.exe accounts /maxpwage:UNLIMITED</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>25</Order><Path>reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\FileSystem\" /v LongPathsEnabled /t REG_DWORD /d 1 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>26</Order><Path>reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Power\" /v HiberbootEnabled /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>27</Order><Path>reg.exe add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Dsh\" /v AllowNewsAndInterests /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>28</Order><Path>reg.exe load \"HKU\\DefaultUser\" \"C:\\Users\\Default\\NTUSER.DAT\"</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>29</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"ContentDeliveryAllowed\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>30</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"FeatureManagementEnabled\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>31</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"OEMPreInstalledAppsEnabled\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>32</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"PreInstalledAppsEnabled\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>33</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"PreInstalledAppsEverEnabled\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>34</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"SilentInstalledAppsEnabled\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>35</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"SoftLandingEnabled\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>36</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"SubscribedContentEnabled\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>37</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"SubscribedContent-310093Enabled\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>38</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"SubscribedContent-338387Enabled\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>39</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"SubscribedContent-338388Enabled\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>40</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"SubscribedContent-338389Enabled\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>41</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"SubscribedContent-338393Enabled\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>42</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"SubscribedContent-353698Enabled\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>43</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"SystemPaneSuggestionsEnabled\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>44</Order><Path>reg.exe unload \"HKU\\DefaultUser\"</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>45</Order><Path>reg.exe add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\CloudContent\" /v \"DisableWindowsConsumerFeatures\" /t REG_DWORD /d 0 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>46</Order><Path>reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\BitLocker\" /v \"PreventDeviceEncryption\" /t REG_DWORD /d 1 /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>47</Order><Path>reg.exe load \"HKU\\DefaultUser\" \"C:\\Users\\Default\\NTUSER.DAT\"</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>48</Order><Path>reg.exe add \"HKU\\DefaultUser\\Software\\Microsoft\\Windows\\CurrentVersion\\Runonce\" /v \"ClassicContextMenu\" /t REG_SZ /d \"reg.exe add \\\"HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32\\\" /ve /f\" /f</Path></RunSynchronousCommand>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\"><Order>49</Order><Path>reg.exe unload \"HKU\\DefaultUser\"</Path></RunSynchronousCommand>");

            xml.AppendLine("      </RunSynchronous>");
            xml.AppendLine("    </component>");
            xml.AppendLine("  </settings>");

            // --- AuditUser Settings Pass ---
            xml.AppendLine("  <settings pass=\"auditUser\">");
            xml.AppendLine("    <component name=\"Microsoft-Windows-Deployment\" processorArchitecture=\"amd64\" publicKeyToken=\"31bf3856ad364e35\" language=\"neutral\" versionScope=\"nonSxS\" xmlns:wcm=\"http://schemas.microsoft.com/WMIConfig/2002/State\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            xml.AppendLine("      <RunSynchronous>");
            xml.AppendLine("        <RunSynchronousCommand wcm:action=\"add\">");
            xml.AppendLine("          <Order>1</Order>");
            xml.AppendLine("          <CommandLine>CMD /C echo LAU GG&gt;C:\\Windows\\LogAuditUser.txt</CommandLine>");
            xml.AppendLine("          <Description>StartMenu</Description>");
            xml.AppendLine("        </RunSynchronousCommand>");
            xml.AppendLine("      </RunSynchronous>");
            xml.AppendLine("    </component>");
            xml.AppendLine("  </settings>");

            // --- OOBE System Settings Pass ---
            xml.AppendLine("  <settings pass=\"oobeSystem\">");
            xml.AppendLine("    <component name=\"Microsoft-Windows-Shell-Setup\" processorArchitecture=\"amd64\" publicKeyToken=\"31bf3856ad364e35\" language=\"neutral\" versionScope=\"nonConditioned\">");
            xml.AppendLine("      <UserAccounts>");
            xml.AppendLine("        <LocalAccounts>");
            foreach (var user in AppState.UserAccounts)
            {
                xml.AppendLine("          <LocalAccount wcm:action=\"add\" xmlns:wcm=\"http://schemas.microsoft.com/WMIConfig/2002/State\">");
                xml.AppendLine($"            <Password><Value>{user.Password}</Value><PlainText>true</PlainText></Password>");
                xml.AppendLine($"            <Name>{user.Name}</Name>");
                xml.AppendLine($"            <Group>{(user.Role == "Administrator" ? "Administrators" : "Users")}</Group>");
                xml.AppendLine("          </LocalAccount>");
            }
            xml.AppendLine("        </LocalAccounts>");
            xml.AppendLine("      </UserAccounts>");
            xml.AppendLine("      <OOBE>");
            xml.AppendLine("        <HideOEMRegistrationScreen>true</HideOEMRegistrationScreen>");
            xml.AppendLine("        <SkipUserOOBE>true</SkipUserOOBE>");
            xml.AppendLine("        <SkipMachineOOBE>true</SkipMachineOOBE>");
            xml.AppendLine("        <HideOnlineAccountScreens>true</HideOnlineAccountScreens>");
            xml.AppendLine("        <HideWirelessSetupInOOBE>true</HideWirelessSetupInOOBE>");
            xml.AppendLine("        <HideEULAPage>true</HideEULAPage>");
            xml.AppendLine("        <NetworkLocation>Work</NetworkLocation>");
            xml.AppendLine("        <ProtectYourPC>3</ProtectYourPC>");
            xml.AppendLine("      </OOBE>");
            xml.AppendLine("      <FirstLogonCommands>");
            xml.AppendLine("        <SynchronousCommand wcm:action=\"add\">");
            xml.AppendLine("          <Order>1</Order>");
            xml.AppendLine("          <CommandLine>reg.exe add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon\" /v AutoLogonCount /t REG_DWORD /d 0 /f</CommandLine>");
            xml.AppendLine("        </SynchronousCommand>");
            xml.AppendLine("        <SynchronousCommand wcm:action=\"add\">");
            xml.AppendLine("          <Order>2</Order>");
            xml.AppendLine("          <CommandLine>cmd.exe /c echo 23&gt;c:\\windows\\csup.txt</CommandLine>");
            xml.AppendLine("        </SynchronousCommand>");
            xml.AppendLine("        <SynchronousCommand wcm:action=\"add\">");
            xml.AppendLine("          <Order>3</Order>");
            xml.AppendLine("          <CommandLine>CMD /C echo GG&gt;C:\\Windows\\LogOobeSystem.txt</CommandLine>");
            xml.AppendLine("        </SynchronousCommand>");
            xml.AppendLine("        <SynchronousCommand wcm:action=\"add\">");
            xml.AppendLine("          <Order>4</Order>");
            xml.AppendLine("          <CommandLine>powershell -ExecutionPolicy Bypass -File c:\\windows\\FirstStartup.ps1</CommandLine>");
            xml.AppendLine("        </SynchronousCommand>");
            xml.AppendLine("      </FirstLogonCommands>");
            xml.AppendLine("    </component>");
            xml.AppendLine("  </settings>");
            xml.AppendLine("</unattend>");

            // We have to create the directories that hold the answer file first
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(destinationPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                File.WriteAllText(Path.Combine(destinationPath, "autounattend.xml"), xml.ToString());
            }
            catch (Exception ex)
            {
                DynaLog.logMessage($"Unattended file could not be made: {ex.Message}");
            }
        }
    }
}