using System.IO;
using System.Text;

namespace MicroWin.functions.dism
{
    public class UnattendGenerator
    {
        public void CreateUnattend(string destinationPath)
        {
            StringBuilder xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xml.AppendLine("<unattend xmlns=\"urn:schemas-microsoft-com:unattend\">");
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
            xml.AppendLine("        <HideEULAPage>true</HideEULAPage>");
            xml.AppendLine("        <NetworkLocation>Work</NetworkLocation>");
            xml.AppendLine("        <ProtectYourPC>3</ProtectYourPC>");
            xml.AppendLine("      </OOBE>");
            xml.AppendLine("    </component>");
            xml.AppendLine("  </settings>");
            xml.AppendLine("</unattend>");

            File.WriteAllText(Path.Combine(destinationPath, "autounattend.xml"), xml.ToString());
        }
    }
}