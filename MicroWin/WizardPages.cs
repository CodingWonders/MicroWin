using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroWin.functions.iso;
using MicroWin.functions.dism;
using MicroWin.functions.Helpers.RegistryHelpers;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using Microsoft.Win32;

namespace MicroWin
{
    /* TODO: Use this as the logo (in Courier New)
     * 
     *    /\/\  (_)  ___  _ __   ___  / / /\ \ \(_) _ __
     *   /    \ | | / __|| '__| / _ \ \ \/  \/ /| || '_ \
     *  / /\/\ \| || (__ | |   | (_) | \  /\  / | || | | |
     *  \/    \/|_| \___||_|    \___/   \/  \/  |_||_| |_|
     * 
     */


    // --- PAGE 1: SELECT ISO ---
    public class Page_SelectISO : UserControl
    {
        private MainForm _parent;
        private Label lblStatus;
        private ProgressBar pb;

        public Page_SelectISO(MainForm main)
        {
            _parent = main;
            var lbl = new Label { Text = "Step 1: Select Windows ISO", Location = new Point(50, 30), AutoSize = true, Font = new Font("Arial", 12, FontStyle.Bold) };
            var btn = new Button { Text = "Browse ISO", Location = new Point(50, 60), Size = new Size(120, 30) };

            lblStatus = new Label { Text = "Status: Ready", Location = new Point(50, 100), AutoSize = true };

            // The Progress Bar
            pb = new ProgressBar
            {
                Location = new Point(50, 130),
                Size = new Size(400, 20),
                Minimum = 0,
                Maximum = 100,
                Visible = false
            };

            btn.Click += async (s, e) => {
                using (OpenFileDialog ofd = new OpenFileDialog { Filter = "ISO Files|*.iso" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        btn.Enabled = false;
                        pb.Visible = true;
                        AppState.IsoPath = ofd.FileName;

                        await Task.Run(() => {
                            var iso = new IsoManager();
                            UpdateUI("Mounting ISO...", 5);

                            char drive = iso.MountAndGetDrive(AppState.IsoPath);
                            if (drive != '\0')
                            {
                                iso.ExtractIso(drive.ToString(), AppState.ExtractPath, (p) => {
                                    // Update the bar based on the 0-100 value from IsoManager
                                    UpdateUI($"Extracting: {p}%", p);
                                });

                                UpdateUI("Dismounting...", 100);
                                iso.Dismount(AppState.IsoPath);
                            }
                        });

                        _parent.ShowPage(new Page_WinVersion(_parent));
                    }
                }
            };
            this.Controls.AddRange(new Control[] { lbl, btn, lblStatus, pb });
        }

        private void UpdateUI(string status, int progress)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => {
                    lblStatus.Text = $"Status: {status}";
                    pb.Value = progress;
                }));
            }
            else
            {
                lblStatus.Text = $"Status: {status}";
                pb.Value = progress;
            }
        }
    }

    // --- PAGE 2: SELECT WINDOWS VERSION ---
    public class Page_WinVersion : UserControl
    {
        private MainForm _parent;
        private ListBox lstVersions;

        public Page_WinVersion(MainForm main)
        {
            _parent = main;
            var lbl = new Label { Text = "Step 2: Select Windows Version", Location = new Point(50, 20), AutoSize = true };
            lstVersions = new ListBox { Location = new Point(50, 50), Size = new Size(350, 200) };
            var btnNext = new Button { Text = "Next", Location = new Point(50, 270), Size = new Size(100, 30) };

            btnNext.Click += (s, e) => {
                if (lstVersions.SelectedIndex != -1)
                {
                    AppState.SelectedImageIndex = lstVersions.SelectedIndex + 1;
                    _parent.ShowPage(new Page_SetupType(_parent));
                }
            };

            this.Controls.AddRange(new Control[] { lbl, lstVersions, btnNext });

            // Important: Handle the Load event to ensure files exist before reading
            this.Load += (s, e) => LoadWimData();
        }

        private void LoadWimData()
        {
            string wimPath = Path.Combine(AppState.ExtractPath, "sources", "install.wim");
            if (!File.Exists(wimPath)) wimPath = Path.Combine(AppState.ExtractPath, "sources", "install.esd");

            if (File.Exists(wimPath))
            {
                Dictionary<int, string> versions = DismManager.GetWimVersions(wimPath);
                lstVersions.Items.Clear();
                lstVersions.Items.AddRange(versions.Select(kvp => String.Format("{0}: {1}", kvp.Key, kvp.Value)).ToArray());
            }
            else
            {
                MessageBox.Show("Error: Image file not found in extraction folder.");
            }
        }
    }

    // --- PAGE 3: SETUP TYPE ---
    public class Page_SetupType : UserControl
    {
        private MainForm _parent;
        public Page_SetupType(MainForm main)
        {
            _parent = main;
            var btnAuto = new Button { Text = "Auto Setup (Automated OOBE)", Location = new Point(50, 80), Size = new Size(200, 40) };
            btnAuto.Click += (s, e) => { AppState.IsAuto = true; _parent.ShowPage(new Page_Users(_parent)); };
            this.Controls.Add(btnAuto);
        }
    }

    // --- PAGE 4: USERS (TABLE VIEW) ---
    public class Page_Users : UserControl
    {
        private MainForm _parent;
        private DataGridView grid;

        public Page_Users(MainForm main)
        {
            _parent = main;
            var txtUser = new TextBox { Location = new Point(20, 40), Width = 100 };
            var txtPass = new TextBox { Location = new Point(130, 40), Width = 100, PasswordChar = '*' };
            var cmbRole = new ComboBox { Location = new Point(240, 40), Width = 100 };
            cmbRole.Items.AddRange(new string[] { "Administrator", "User" });
            cmbRole.SelectedIndex = 0;

            var btnAdd = new Button { Text = "Add", Location = new Point(350, 38) };
            grid = new DataGridView { Location = new Point(20, 80), Size = new Size(500, 150), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AllowUserToAddRows = false };
            grid.Columns.Add("U", "User"); grid.Columns.Add("P", "Password"); grid.Columns.Add("R", "Role");

            btnAdd.Click += (s, e) => {
                if (!string.IsNullOrEmpty(txtUser.Text))
                {
                    AppState.UserAccounts.Add(new UserAccount { Name = txtUser.Text, Password = txtPass.Text, Role = cmbRole.Text });
                    grid.Rows.Add(txtUser.Text, "********", cmbRole.Text);
                    txtUser.Clear(); txtPass.Clear();
                }
            };

            var btnNext = new Button { Text = "Next", Location = new Point(20, 250) };
            btnNext.Click += (s, e) => _parent.ShowPage(new Page_Tools(_parent));
            this.Controls.AddRange(new Control[] { txtUser, txtPass, cmbRole, btnAdd, grid, btnNext });
        }
    }

    // --- PAGE 5: TOOLS & SHORTCUTS ---
    public class Page_Tools : UserControl
    {
        private MainForm _parent;
        public Page_Tools(MainForm main)
        {
            _parent = main;
            var chkReport = new CheckBox { Text = "Add Reporting Tool Shortcut", Location = new Point(50, 80), AutoSize = true };
            var btnNext = new Button { Text = "Deploy Image", Location = new Point(50, 150), Size = new Size(150, 40), BackColor = Color.LightGreen };

            btnNext.Click += (s, e) => {
                AppState.AddReportingToolShortcut = chkReport.Checked;
                _parent.ShowPage(new Page_Progress(_parent));
            };
            this.Controls.AddRange(new Control[] { chkReport, btnNext });
        }
    }

    // --- PAGE 6: Save ---
    public class Page_Save : UserControl
    {
        private MainForm _parent;
        public Page_Save(MainForm main)
        {
            _parent = main;

            var lbl = new Label { Text = "Save ISO", Location = new Point(50, 30), AutoSize = true, Font = new Font("Arial", 12, FontStyle.Bold) };
            var btn = new Button { Text = "Browse", Location = new Point(50, 60), Size = new Size(120, 30) };

            btn.Click += (s, e) => {
                using (SaveFileDialog ofd = new SaveFileDialog {})
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        AppState.saveISO = ofd.FileName;
                        _parent.ShowPage(new Page_WinVersion(_parent));
                    }
                }
            };
            this.Controls.AddRange(new Control[] { lbl, btn });
        }

    }

    // --- PAGE 7: PROGRESS & DEPLOYMENT ---
    public class Page_Progress : UserControl
    {
        private Label lblStatus;
        private ProgressBar pb;
        private MainForm _main;

        public Page_Progress(MainForm main)
        {
            _main = main;
            lblStatus = new Label { Text = "Initializing...", Location = new Point(50, 50), Width = 400, AutoSize = true };
            pb = new ProgressBar { Location = new Point(50, 80), Size = new Size(400, 25), Style = ProgressBarStyle.Continuous };
            this.Controls.AddRange(new Control[] { lblStatus, pb });
            RunDeployment();
        }

        private void UpdateStatus(string text)
        {
            if (this.InvokeRequired) this.Invoke(new Action(() => { lblStatus.Text = text; pb.Value = 0; }));
            else { lblStatus.Text = text; pb.Value = 0; }
        }

        private void UpdateProgressBar(int value)
        {
            int safeValue = Math.Max(0, Math.Min(value, 100));
            if (this.InvokeRequired) this.Invoke(new Action(() => pb.Value = safeValue));
            else pb.Value = safeValue;
        }

        private async void RunDeployment()
        {
            await Task.Run(async () => {
                string installwimPath = Path.Combine(AppState.MountPath, "sources", "install.wim");
                if (!File.Exists(installwimPath)) installwimPath = Path.Combine(AppState.ExtractPath, "sources", "install.esd");

                UpdateStatus("Mounting Install WIM...");
                DismManager.MountImage(installwimPath, AppState.SelectedImageIndex, AppState.ScratchPath, (p) => UpdateProgressBar(p));

                RegistryHelper.LoadRegistryHive(Path.Combine(AppState.ScratchPath, "Windows", "System32", "config", "SOFTWARE"), "HKLM\\zSOFTWARE");
                RegistryHelper.LoadRegistryHive(Path.Combine(AppState.ScratchPath, "Windows", "System32", "config", "SYSTEM"), "HKLM\\zSYSTEM");
                RegistryHelper.LoadRegistryHive(Path.Combine(AppState.ScratchPath, "Windows", "System32", "config", "default"), "HKLM\\zDEFAULT");
                RegistryHelper.LoadRegistryHive(Path.Combine(AppState.ScratchPath, "Users", "Default", "ntuser.dat"), "HKLM\\zNTUSER");

                new OsPackageRemover().RunTask();

                if (AppState.AddReportingToolShortcut)
                {
                    using (var client = new HttpClient())
                    {
                        var data = await client.GetByteArrayAsync("https://raw.githubusercontent.com/CodingWonders/MyScripts/refs/heads/main/MicroWinHelperTools/ReportingTool/ReportingTool.ps1");
                        File.WriteAllBytes(Path.Combine(AppState.ScratchPath, "ReportingTool.ps1"), data);
                    }

                    RegistryHelper.AddRegistryItem("HKLM\\zSOFTWARE\\MicroWin");
                    RegistryHelper.AddRegistryItem("HKLM\\zSOFTWARE\\MicroWin", new RegistryItem("MicroWinVersion", ValueKind.REG_SZ, $"{AppState.Version}"));
                    RegistryHelper.AddRegistryItem("HKLM\\zSOFTWARE\\MicroWin", new RegistryItem("MicroWinBuildDate", ValueKind.REG_SZ, $"{DateTime.Now}"));

                }

                RegistryHelper.AddRegistryItem("HKLM\\zSYSTEM\\ControlSet001\\Control\\Session Manager", new RegistryItem("DisableWpbtExecution", ValueKind.REG_DWORD, 1));

                // Skip first logon animation
                RegistryHelper.AddRegistryItem("HKLM\\zSOFTWARE\\Microsoft\\Active Setup\\Installed Components\\CMP_NoFla");
                RegistryHelper.AddRegistryItem("HKLM\\zSOFTWARE\\Microsoft\\Active Setup\\Installed Components\\CMP_NoFla", new RegistryItem("", ValueKind.REG_SZ, "Stop First Logon Animation Process"));
                // CW Please look at the one below as unsure if this is correct. 
                RegistryHelper.AddRegistryItem("HKLM\\zSOFTWARE\\Microsoft\\Active Setup\\Installed Components\\CMP_NoFla", new RegistryItem("StubPath", ValueKind.REG_SZ, "\'%WINDIR%\\System32\\cmd.exe\\' /C \\'taskkill /f /im firstlogonanim.exe\\'"));

                RegistryHelper.UnloadRegistryHive("HKLM\\zSYSTEM");
                RegistryHelper.UnloadRegistryHive("HKLM\\zSOFTWARE");
                RegistryHelper.UnloadRegistryHive("HKLM\\zDEFAULT");
                RegistryHelper.UnloadRegistryHive("HKLM\\zNTUSER");

                UpdateStatus("Finalizing...");
                DismManager.UnmountAndSave(AppState.ScratchPath.TrimEnd('\\'), (p) => UpdateProgressBar(p));

                string bootwimPath = Path.Combine(AppState.ExtractPath, "sources", "boot.wim");
                if (!File.Exists(bootwimPath)) bootwimPath = Path.Combine(AppState.ExtractPath, "sources", "boot.esd");

                UpdateStatus("Mounting Boot WIM...");
                DismManager.MountImage(bootwimPath, 2, AppState.ScratchPath, (p) => UpdateProgressBar(p));

                // Add editing boot.wim (Edit registry)

                UpdateStatus("Finalizing...");
                DismManager.UnmountAndSave(AppState.ScratchPath.TrimEnd('\\'), (p) => UpdateProgressBar(p));

                // Download oscdimg

                using (var client = new HttpClient())
                {
                    var data = await client.GetByteArrayAsync("https://github.com/CodingWonders/MicroWin/raw/main/MicroWin/tools/oscdimg.exe");
                    File.WriteAllBytes(Path.Combine(AppState.TempRoot, "oscdimg.exe"), data);
                }

                Process.Start(Path.Combine(AppState.TempRoot, "oscdimg.exe"), 
                    $"-m - o - u2 - udfver102 - bootdata:2#p0,e,b\"{Path.Combine(AppState.MountPath, "boot", "etfsboot.com")}\"#pEF,e,b\"{Path.Combine(AppState.MountPath, "efi", "microsoft", "boot", "efisys.bin")}\" \"{AppState.MountPath}\" \"{AppState.saveISO}\"");

                if (Directory.Exists(AppState.TempRoot))
                {
                    Directory.Delete(AppState.TempRoot, true);
                }
            });

            _main.ShowPage(new Page_Finish(_main));
        }
    }

    // --- PAGE 8: FINISH ---
    public class Page_Finish : UserControl
    {
        public Page_Finish(MainForm main)
        {
            var lbl = new Label { Text = "Finished!", Location = new Point(50, 50), AutoSize = true };
            var btnClose = new Button { Text = "Close", Location = new Point(50, 100) };
            btnClose.Click += (s, e) => Application.Exit();
            this.Controls.AddRange(new Control[] { lbl, btnClose });
        }
    }
}