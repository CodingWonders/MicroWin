using System;
using System.Drawing;
using System.Windows.Forms;

namespace MicroWin
{
    public partial class MainForm : Form
    {
        private Panel pnlContent;

        public MainForm()
        {
            this.Text = "MicroWin .NET (ALPHA 0.1)";
            this.Size = new Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;

            pnlContent = new Panel { Dock = DockStyle.Fill };
            this.Controls.Add(pnlContent);

            string disclaimerMessage = $"Thank you for trying this ALPHA release of MicroWin .NET.\n\n" +
                $"Because this is an alpha version of a rewrite of the original PowerShell version, bugs may happen. We expect improvements in quality " +
                $"as time goes on, but that can be done with your help. Report the bugs over on the GitHub repository.\n\n" +
                $"This ALPHA release already has almost every feature implemented, besides a few that couldn't make it to this release. Those will be " +
                $"implemented in future releases. Head over to the roadmap available in the repository for more info.\n\n" +
                $"Please disable your antivirus or set an exclusion to prevent conflicts. Do not worry, this is an open-source project and we take " +
                $"your computer's security seriously.\n\n" +
                $"Thanks,\n" +
                $"CWSOFTWARE and the rest of the team behind MicroWin.";

            MessageBox.Show(disclaimerMessage, "ALPHA RELEASE", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Start at the first page
            ShowPage(new Page_SelectISO(this));
        }

        public void ShowPage(UserControl page)
        {
            pnlContent.Controls.Clear();
            page.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(page);
        }
    }
}