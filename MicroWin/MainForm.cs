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
            this.Text = "MicroWin - Windows Image Customizer";
            this.Size = new Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;

            pnlContent = new Panel { Dock = DockStyle.Fill };
            this.Controls.Add(pnlContent);

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