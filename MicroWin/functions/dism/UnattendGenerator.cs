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
            xml.AppendLine("  <settings pass=\"oobeSystem\">");
            xml.AppendLine("    <component name=\"Microsoft-Windows-Shell-Setup\" processorArchitecture=\"amd64\" publicKeyToken=\"31bf3856ad364e35\" language=\"neutral\" versionScope=\"nonConditioned\">");
            xml.AppendLine("      <UserAccounts>");

            // Add Local Accounts from the User Table
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
            xml.AppendLine("    </component>");
            xml.AppendLine("  </settings>");
            xml.AppendLine("</unattend>");

            File.WriteAllText(destinationPath, xml.ToString());
        }
    }
}