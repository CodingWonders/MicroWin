using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using WinPEInstallerApp.Core;

namespace WinPEInstallerApp
{
    public partial class Form1 : Form
    {
        private TreeView? tvDisks;
        private Button? btnInstall;
        private ProgressBar? progressBar;
        private Label? lblStatus;
        private TextBox? txtProductKey;

        public Form1()
        {
            this.Text = "MicroWin Installer";
            this.Size = new Size(600, 500);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            InitializeCustomUI();
        }

        private void InitializeCustomUI()
        {
            tvDisks = new TreeView { Location = new Point(20, 20), Size = new Size(540, 200) };
            Label lblKey = new Label { Text = "Product Key (Optional):", Location = new Point(20, 230), Size = new Size(200, 20) };
            txtProductKey = new TextBox { Location = new Point(20, 250), Size = new Size(250, 25) };
            lblStatus = new Label { Text = "Status: Ready", Location = new Point(20, 290), Size = new Size(300, 20) };
            progressBar = new ProgressBar { Location = new Point(20, 310), Size = new Size(540, 30) };

            btnInstall = new Button
            {
                Text = "Wipe and Install",
                Location = new Point(410, 380),
                Size = new Size(150, 45),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat
            };

            btnInstall.Click += BtnInstall_Click;

            this.Controls.Add(tvDisks);
            this.Controls.Add(lblKey);
            this.Controls.Add(txtProductKey);
            this.Controls.Add(lblStatus);
            this.Controls.Add(progressBar);
            this.Controls.Add(btnInstall);

            DiskInfoService.PopulateDiskTree(tvDisks);
        }

        private async void BtnInstall_Click(object? sender, EventArgs e)
        {
            if (tvDisks?.SelectedNode == null)
            {
                MessageBox.Show("Please select a target disk.");
                return;
            }

            // DYNAMIC SEARCH: Find install.wim on any drive
            string wimPath = "";
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady);
            foreach (var drive in drives)
            {
                string potential = Path.Combine(drive.RootDirectory.FullName, "sources", "install.wim");
                if (File.Exists(potential)) { wimPath = potential; break; }
            }

            if (string.IsNullOrEmpty(wimPath))
            {
                MessageBox.Show("Could not find sources\\install.wim on any drive.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string diskId = tvDisks.SelectedNode.Tag?.ToString() ?? "";
            string diskNum = diskId.Replace("\\\\.\\PHYSICALDRIVE", "");

            if (MessageBox.Show($"WIPE disk {diskNum} and Install?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                btnInstall!.Enabled = false;

                await Task.Run(() => {
                    Invoke(new Action(() => lblStatus!.Text = "Status: Partitioning..."));
                    InstallerEngine.PrepareDisk(diskNum);

                    var prog = new Progress<int>(p => Invoke(new Action(() => {
                        progressBar!.Value = p;
                        lblStatus!.Text = $"Status: Applying Image... {p}%";
                    })));

                    InstallerEngine.ApplyImage(wimPath, prog);
                    InstallerEngine.FinalizeInstall(txtProductKey?.Text ?? "");
                });

                MessageBox.Show("Install Success! System will now reboot.", "Done");

                // REBOOT LOGIC
                try { Process.Start("wpeutil", "reboot"); }
                catch { Process.Start("shutdown", "/r /t 0"); }
                Application.Exit();
            }
        }
    }
}