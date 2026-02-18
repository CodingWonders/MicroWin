using Microsoft.Dism;
using Microsoft.Win32;
using MicroWin.Classes;
using MicroWin.functions.dism;
using MicroWin.functions.Helpers.DeleteFile;
using MicroWin.functions.Helpers.DesktopWindowManager;
using MicroWin.functions.Helpers.Loggers;
using MicroWin.functions.Helpers.RegistryHelpers;
using MicroWin.functions.iso;
using MicroWin.functions.UI;
using MicroWin.OSCDIMG;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicroWin
{
    public partial class MainForm : Form
    {
        private const string swStatus = "BETA";

        private WizardPage CurrentWizardPage = new();
        private List<WizardPage.Page> VerifyInPages = [
            WizardPage.Page.IsoChooserPage,
            WizardPage.Page.ImageChooserPage,
            WizardPage.Page.UserAccountsPage
        ];

        public MainForm()
        {
            InitializeComponent();
        }

        private void SetColorMode()
        {
            RegistryKey colorRk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize");
            int colorVal = (int)colorRk.GetValue("AppsUseLightTheme", 1);
            colorRk.Close();
            if (colorVal == 0)
            {
                BackColor = Color.FromArgb(35, 38, 41);
                ForeColor = Color.FromArgb(247, 247, 247);
            }
            else
            {
                BackColor = Color.FromArgb(247, 247, 247);
                ForeColor = Color.FromArgb(35, 38, 41);
            }

            // Change colors of other components. I want consistency
            isoPathTB.BackColor = BackColor;
            isoPathTB.ForeColor = ForeColor;
            lvVersions.BackColor = BackColor;
            lvVersions.ForeColor = ForeColor;
            usrNameTB.BackColor = BackColor;
            usrNameTB.ForeColor = ForeColor;
            usrPasswordTB.BackColor = BackColor;
            usrPasswordTB.ForeColor = ForeColor;
            DriverExportCombo.BackColor = BackColor;
            DriverExportCombo.ForeColor = ForeColor;
            logTB.BackColor = BackColor;
            logTB.ForeColor = ForeColor;

            WindowHelper.ToggleDarkTitleBar(Handle, colorVal == 0);
        }

        private void ChangePage(WizardPage.Page newPage)
        {
            DynaLog.logMessage("Changing current page of the wizard...");
            DynaLog.logMessage($"New page to load: {newPage.ToString()}");

            if (newPage > CurrentWizardPage.wizardPage && VerifyInPages.Contains(CurrentWizardPage.wizardPage))
            {
                if (!VerifyOptionsInPage(CurrentWizardPage.wizardPage))
                    return;
            }

            WelcomePage.Visible = newPage == WizardPage.Page.WelcomePage;
            IsoChooserPage.Visible = newPage == WizardPage.Page.IsoChooserPage;
            ImageChooserPage.Visible = newPage == WizardPage.Page.ImageChooserPage;
            UserAccountsPage.Visible = newPage == WizardPage.Page.UserAccountsPage;
            IsoSettingsPage.Visible = newPage == WizardPage.Page.IsoSettingsPage;
            IsoCreationPage.Visible = newPage == WizardPage.Page.IsoCreationPage;
            FinishPage.Visible = newPage == WizardPage.Page.FinishPage;

            CurrentWizardPage.wizardPage = newPage;

            // Handle tasks when switching to certain pages
            switch (newPage)
            {
                case WizardPage.Page.ImageChooserPage:
                    LoadWimData();
                    break;
            }

            Next_Button.Enabled = !(newPage != WizardPage.Page.FinishPage) || !((int)newPage + 1 >= WizardPage.PageCount);
            Cancel_Button.Enabled = !(newPage == WizardPage.Page.FinishPage);
            Back_Button.Enabled = !(newPage == WizardPage.Page.WelcomePage) && !(newPage == WizardPage.Page.FinishPage);
            ButtonPanel.Visible = !(newPage == WizardPage.Page.IsoCreationPage);

            Next_Button.Text = newPage == WizardPage.Page.FinishPage ? "Close" : "Next";

            if (CurrentWizardPage.wizardPage == WizardPage.Page.IsoCreationPage)
            {
                if (isoSaverSFD.ShowDialog(this) != DialogResult.OK)
                {
                    ChangePage(CurrentWizardPage.wizardPage - 1);
                    return;
                }
                AppState.SaveISO = isoSaverSFD.FileName;
                RunDeployment();
            }
        }

        private bool VerifyOptionsInPage(WizardPage.Page wizardPage)
        {
            switch (wizardPage)
            {
                case WizardPage.Page.IsoChooserPage:
                    if (String.IsNullOrEmpty(isoPathTB.Text) || !File.Exists(isoPathTB.Text))
                    {
                        MessageBox.Show("Specify an ISO file and try again. Make sure that it exists", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    break;
                case WizardPage.Page.ImageChooserPage:
                    if (AppState.SelectedImageIndex < 1)
                    {
                        MessageBox.Show("Please specify an image to modify and try again.");
                        return false;
                    }
                    break;
                case WizardPage.Page.UserAccountsPage:
                    // Default to "User" if no name is set
                    if (String.IsNullOrEmpty(usrNameTB.Text))
                        usrNameTB.Text = "User";

                    // Trim invalid characters from the user account
                    char[] invalidChars = ['/', '\\', '[', ']', ':', ';', '|', '=', ',', '+', '*', '?', '<', '>', '\"', '%'];
                    if (AppState.UserAccounts.Any())
                    {
                        foreach (UserAccount account in AppState.UserAccounts)
                        {
                            account.Name = new string(account.Name.Where(c => !invalidChars.Contains(c)).ToArray()).TrimEnd('.');
                        }
                    }
                    break;
            }
            return true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = $"MicroWin .NET ({swStatus} 0.2)";

            string disclaimerMessage = $"Thank you for trying this {swStatus} release of MicroWin .NET.\n\n" +
                $"Because this is a prerelease version of a rewrite of the original PowerShell version, bugs may happen. We expect improvements in quality " +
                $"as time goes on, but that can be done with your help. Report the bugs over on the GitHub repository.\n\n" +
                $"This {swStatus} release already has almost every feature implemented, besides a few that couldn't make it to this release. Those will be " +
                $"implemented in future releases. Head over to the roadmap available in the repository for more info.\n\n" +
                $"Please disable your antivirus or set an exclusion to prevent conflicts. Do not worry, this is an open-source project and we take " +
                $"your computer's security seriously.\n\n" +
                $"Thanks,\n" +
                $"CWSOFTWARE and the rest of the team behind MicroWin.";

            lblDisclaimer.Text = disclaimerMessage;

            ChangePage(WizardPage.Page.WelcomePage);

            SetColorMode();

            // Insert an item in there so we can work with it
            AppState.UserAccounts.Add(new UserAccount() { Role = "Administrator" });

            // Other default settings
            DriverExportCombo.SelectedIndexChanged -= DriverExportCombo_SelectedIndexChanged;
            DriverExportCombo.SelectedIndex = (int)AppState.DriverExportMode;
            DriverExportCombo.SelectedIndexChanged += DriverExportCombo_SelectedIndexChanged;
        }

        private void Next_Button_Click(object sender, EventArgs e)
        {
            if (CurrentWizardPage.wizardPage == WizardPage.Page.FinishPage)
            {
                Close();
            }
            else
            {
                ChangePage(CurrentWizardPage.wizardPage + 1);
            }
        }

        private void Back_Button_Click(object sender, EventArgs e)
        {
            ChangePage(CurrentWizardPage.wizardPage - 1);
        }

        private void isoPickerBtn_Click(object sender, EventArgs e)
        {
            isoPickerOFD.ShowDialog(this);
        }

        private void isoPickerOFD_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isoPathTB.Text = isoPickerOFD.FileName;
        }

        private void InvokeIsoExtractionUIUpdate(string status, int progress)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => {
                    lblExtractionStatus.Text = $"Status: {status}";
                    isoExtractionPB.Value = progress;
                }));
            }
            else
            {
                lblExtractionStatus.Text = $"Status: {status}";
                isoExtractionPB.Value = progress;
            }
        }

        private void LoadWimData()
        {
            string wimPath = Path.Combine(AppState.MountPath, "sources", "install.wim");
            if (!File.Exists(wimPath)) wimPath = Path.Combine(AppState.MountPath, "sources", "install.esd");

            if (File.Exists(wimPath))
            {
                DismImageInfoCollection imageInfo = DismManager.GetImageInformation(wimPath);
                if (imageInfo is null)
                    return;

                lvVersions.Items.Clear();
                lvVersions.Items.AddRange(imageInfo.Select(image => new ListViewItem([image.ImageIndex.ToString(), 
                    image.ImageName, image.ImageDescription, 
                    image.Architecture.ToString(), 
                    image.CustomizedInfo?.ModifiedTime.ToString("dd/MM/yyyy HH:mm:ss")])).ToArray());
                if (imageInfo.Any())
                {
                    // Get and select Pro automatically
                    lvVersions.SelectedIndexChanged -= lvVersions_SelectedIndexChanged;
                    int? proIdx = imageInfo.FirstOrDefault(image => image.EditionId.Equals("Professional", StringComparison.OrdinalIgnoreCase))?.ImageIndex;
                    lvVersions.Items[proIdx - 1 ?? 0].Selected = true;
                    lvVersions.Select();
                    lvVersions.SelectedIndexChanged += lvVersions_SelectedIndexChanged;
                    AppState.SelectedImageIndex = (proIdx ?? 1);
                }
            }
            else
            {
                MessageBox.Show("Error: Image file not found in extraction folder.");
            }
        }

        private async void isoPathTB_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(isoPathTB.Text))
            {
                isoPickerBtn.Enabled = false;
                AppState.IsoPath = isoPathTB.Text;

                ButtonPanel.Enabled = false;
                WindowHelper.DisableCloseCapability(Handle);

                await Task.Run(() => {
                    var iso = new IsoManager();
                    InvokeIsoExtractionUIUpdate("Mounting ISO...", 5);

                    char drive = iso.MountAndGetDrive(AppState.IsoPath);
                    if (drive != '\0')
                    {
                        iso.ExtractIso(drive.ToString(), AppState.MountPath, (p) => {
                            // Update the bar based on the 0-100 value from IsoManager
                            InvokeIsoExtractionUIUpdate($"Extracting: {p}%", p);
                        });

                        InvokeIsoExtractionUIUpdate("Dismounting...", 100);
                        iso.Dismount(AppState.IsoPath);
                    }

                    InvokeIsoExtractionUIUpdate("Extraction complete. Click Next to continue.", 100);
                });
                isoPickerBtn.Enabled = true;

                ButtonPanel.Enabled = true;
                WindowHelper.EnableCloseCapability(Handle);
            }
        }

        private void lvVersions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvVersions.SelectedItems.Count == 1)
                AppState.SelectedImageIndex = lvVersions.FocusedItem.Index + 1;
        }

        private void lnkImmersiveAccounts_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("ms-settings:otherusers");
        }

        private void lnkLusrMgr_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "system32", "lusrmgr.msc"));
        }

        private void usrNameTB_TextChanged(object sender, EventArgs e)
        {
            AppState.UserAccounts[0].Name = usrNameTB.Text;
        }

        private void b64CB_CheckedChanged(object sender, EventArgs e)
        {
            AppState.EncodeWithB64 = b64CB.Checked;
        }

        private void usrPasswordTB_TextChanged(object sender, EventArgs e)
        {
            AppState.UserAccounts[0].Password = usrPasswordTB.Text;
        }

        private void usrNameCurrentSysNameBtn_Click(object sender, EventArgs e)
        {
            usrNameTB.Text = Environment.UserName;
        }

        private void usrPasswordRevealCB_CheckedChanged(object sender, EventArgs e)
        {
            usrPasswordTB.PasswordChar = usrPasswordRevealCB.Checked ? '\0' : '*';
        }

        private void DriverExportCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppState.DriverExportMode = (DriverExportMode)DriverExportCombo.SelectedIndex;
        }

        private void ReportToolCB_CheckedChanged(object sender, EventArgs e)
        {
            AppState.AddReportingToolShortcut = ReportToolCB.Checked;
        }

        private void UnattendCopyCB_CheckedChanged(object sender, EventArgs e)
        {
            AppState.CopyUnattendToFileSystem = UnattendCopyCB.Checked;
        }

        private void UpdateCurrentStatus(string text, bool resetBar = true)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    lblCurrentStatus.Text = text;
                    if (resetBar) pbCurrent.Value = 0;
                }));
            }
            else 
            { 
                lblCurrentStatus.Text = text;
                if (resetBar) pbCurrent.Value = 0; 
            }
        }

        private void UpdateCurrentProgressBar(int value)
        {
            int safeValue = Math.Max(0, Math.Min(value, 100));
            if (this.InvokeRequired) this.Invoke(new Action(() => pbCurrent.Value = safeValue));
            else pbCurrent.Value = safeValue;
        }

        private void UpdateOverallStatus(string text)
        {
            if (this.InvokeRequired) this.Invoke(new Action(() => { lblOverallStatus.Text = text; }));
            else { lblOverallStatus.Text = text; }
        }

        private void UpdateOverallProgressBar(int value)
        {
            int safeValue = Math.Max(0, Math.Min(value, 100));
            if (this.InvokeRequired) this.Invoke(new Action(() => pbOverall.Value = safeValue));
            else pbOverall.Value = safeValue;
        }

        private void WriteLogMessage(string message)
        {
            string fullMsg = $"[{DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss")} UTC] {message}{Environment.NewLine}";
            if (InvokeRequired)
            {
                Invoke(new Action(() => logTB.AppendText(fullMsg)));
            }
            else
            {
                logTB.AppendText(fullMsg);
            }
        }

        private async void RunDeployment()
        {
            // Clear old results and write the cool banner
            logTB.Clear();
            logTB.Text = @"
    /\/\  (_)  ___  _ __   ___  / / /\ \ \(_) _ __
   /    \ | | / __|| '__| / _ \ \ \/  \/ /| || '_ \
  / /\/\ \| || (__ | |   | (_) | \  /\  / | || | | |
  \/    \/|_| \___||_|    \___/   \/  \/  |_||_| |_|

                MicroWin .NET (BETA 0.2)

";

            WindowHelper.DisableCloseCapability(Handle);

            await Task.Run(async () => {
                string installwimPath = Path.Combine(AppState.MountPath, "sources", "install.wim");
                if (!File.Exists(installwimPath)) installwimPath = Path.Combine(AppState.MountPath, "sources", "install.esd");

                UpdateOverallStatus("Customizing install image...");
                UpdateOverallProgressBar(0);
                UpdateCurrentStatus("Mounting install image...");
                DismManager.MountImage(installwimPath, AppState.SelectedImageIndex, AppState.ScratchPath, (p) => UpdateCurrentProgressBar(p), (msg) => WriteLogMessage(msg));

                WriteLogMessage("Creating unattended answer file...");
                UnattendGenerator.CreateUnattend($"{Path.Combine(AppState.ScratchPath, "Windows", "Panther")}");

                UpdateOverallProgressBar(10);
                new OsFeatureDisabler().RunTask((p) => UpdateCurrentProgressBar(p), (msg) => UpdateCurrentStatus(msg, false), (msg) => WriteLogMessage(msg));
                UpdateOverallProgressBar(20);
                new OsPackageRemover().RunTask((p) => UpdateCurrentProgressBar(p), (msg) => UpdateCurrentStatus(msg, false), (msg) => WriteLogMessage(msg));
                UpdateOverallProgressBar(30);
                new StoreAppRemover().RunTask((p) => UpdateCurrentProgressBar(p), (msg) => UpdateCurrentStatus(msg, false), (msg) => WriteLogMessage(msg));

                UpdateOverallProgressBar(40);
                WriteLogMessage("Loading image registry hives...");
                RegistryHelper.LoadRegistryHive(Path.Combine(AppState.ScratchPath, "Windows", "System32", "config", "SOFTWARE"), "zSOFTWARE");
                RegistryHelper.LoadRegistryHive(Path.Combine(AppState.ScratchPath, "Windows", "System32", "config", "SYSTEM"), "zSYSTEM");
                RegistryHelper.LoadRegistryHive(Path.Combine(AppState.ScratchPath, "Windows", "System32", "config", "default"), "zDEFAULT");
                RegistryHelper.LoadRegistryHive(Path.Combine(AppState.ScratchPath, "Users", "Default", "ntuser.dat"), "zNTUSER");

                UpdateCurrentStatus("Modifying install image...");
                if (AppState.AddReportingToolShortcut)
                {
                    WriteLogMessage("Downloading and integrating reporting tool...");
                    using (var client = new HttpClient())
                    {
                        var data = await client.GetByteArrayAsync("https://raw.githubusercontent.com/CodingWonders/MyScripts/refs/heads/main/MicroWinHelperTools/ReportingTool/ReportingTool.ps1");
                        File.WriteAllBytes(Path.Combine(AppState.ScratchPath, "ReportingTool.ps1"), data);
                    }

                    RegistryHelper.AddRegistryItem("HKLM\\zSOFTWARE\\MicroWin");
                    RegistryHelper.AddRegistryItem("HKLM\\zSOFTWARE\\MicroWin", new RegistryItem("MicroWinVersion", ValueKind.REG_SZ, $"{AppState.Version}"));
                    RegistryHelper.AddRegistryItem("HKLM\\zSOFTWARE\\MicroWin", new RegistryItem("MicroWinBuildDate", ValueKind.REG_SZ, $"{DateTime.Now}"));

                }
                UpdateCurrentProgressBar(10);

                WriteLogMessage("Disabling WPBT...");
                RegistryHelper.AddRegistryItem("HKLM\\zSYSTEM\\ControlSet001\\Control\\Session Manager", new RegistryItem("DisableWpbtExecution", ValueKind.REG_DWORD, 1));

                // Skip first logon animation
                WriteLogMessage("Disabling FLA...");
                RegistryHelper.AddRegistryItem("HKLM\\zSOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", new RegistryItem("EnableFirstLogonAnimation", ValueKind.REG_DWORD, 0));

                WriteLogMessage("Setting execution policies...");
                RegistryHelper.AddRegistryItem("HKLM\\zSOFTWARE\\Microsoft\\PowerShell\\1\\ShellIds\\Microsoft.PowerShell", new RegistryItem("ExecutionPolicy", ValueKind.REG_SZ, "RemoteSigned"));

                // int majorver = Convert.ToInt32(RegistryHelper.QueryRegistryValue("HKLM\\zSOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "CurrentMajorVersionNumber"));
                // int minorver = Convert.ToInt32(RegistryHelper.QueryRegistryValue("HKLM\\zSOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "CurrentMinorVersionNumber"));
                // string build = Convert.ToString(RegistryHelper.QueryRegistryValue("HKLM\\zSOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "CurrentBuild"));
                // string ubr = Convert.ToString(RegistryHelper.QueryRegistryValue("HKLM\\zSOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "UBR"));

                //if (majorver == 10 && minorver == 0 && build == "26100" && ubr == "1")
                //{
                //try
                //{
                //DismApi.Initialize(DismLogLevel.LogErrors);
                //using DismSession session = DismApi.OpenOfflineSession(AppState.ScratchPath);

                //DismApi.EnableFeature(session, "Recall", false, true);
                //DismApi.Shutdown();
                //}
                //catch
                //{
                // Add logging
                //}
                //}

                UpdateCurrentProgressBar(50);
                using (var client = new HttpClient())
                {
                    try
                    {
                        var data = client.GetByteArrayAsync("https://github.com/CodingWonders/MicroWin/raw/main/MicroWin/tools/FirstStartup.ps1").GetAwaiter().GetResult();
                        File.WriteAllBytes(Path.Combine(AppState.ScratchPath, "Windows", "FirstStartup.ps1"), data);
                    }
                    catch { }
                }

                UpdateCurrentProgressBar(90);
                WriteLogMessage("Unloading image registry hives...");
                RegistryHelper.UnloadRegistryHive("zSYSTEM");
                RegistryHelper.UnloadRegistryHive("zSOFTWARE");
                RegistryHelper.UnloadRegistryHive("zDEFAULT");
                RegistryHelper.UnloadRegistryHive("zNTUSER");
                UpdateCurrentProgressBar(100);

                UpdateCurrentStatus("Unmounting install image...");
                DismManager.UnmountAndSave(AppState.ScratchPath.TrimEnd('\\'), (p) => UpdateCurrentProgressBar(p), (msg) => WriteLogMessage(msg));

                UpdateOverallProgressBar(50);

                string bootwimPath = Path.Combine(AppState.MountPath, "sources", "boot.wim");
                if (!File.Exists(bootwimPath)) bootwimPath = Path.Combine(AppState.MountPath, "sources", "boot.esd");

                UpdateOverallStatus("Customizing boot image...");
                UpdateCurrentStatus("Mounting boot image...");
                DismManager.MountImage(bootwimPath, 2, AppState.ScratchPath, (p) => UpdateCurrentProgressBar(p), (msg) => WriteLogMessage(msg));

                UpdateCurrentStatus("Modifying WinPE registry...");
                WriteLogMessage("Loading image registry hives...");
                RegistryHelper.LoadRegistryHive(Path.Combine(AppState.ScratchPath, "Windows", "System32", "config", "SOFTWARE"), "zSOFTWARE");
                RegistryHelper.LoadRegistryHive(Path.Combine(AppState.ScratchPath, "Windows", "System32", "config", "SYSTEM"), "zSYSTEM");
                RegistryHelper.LoadRegistryHive(Path.Combine(AppState.ScratchPath, "Windows", "System32", "config", "default"), "zDEFAULT");
                RegistryHelper.LoadRegistryHive(Path.Combine(AppState.ScratchPath, "Users", "Default", "ntuser.dat"), "zNTUSER");

                UpdateCurrentProgressBar(50);
                WriteLogMessage("Bypassing requirements...");
                RegistryHelper.AddRegistryItem("HKLM\\zDEFAULT\\Control Panel\\UnsupportedHardwareNotificationCache", new RegistryItem("SV1", ValueKind.REG_DWORD, 0));
                RegistryHelper.AddRegistryItem("HKLM\\zDEFAULT\\Control Panel\\UnsupportedHardwareNotificationCache", new RegistryItem("SV2", ValueKind.REG_DWORD, 0));
                RegistryHelper.AddRegistryItem("HKLM\\zNTUSER\\Control Panel\\UnsupportedHardwareNotificationCache", new RegistryItem("SV1", ValueKind.REG_DWORD, 0));
                RegistryHelper.AddRegistryItem("HKLM\\zNTUSER\\Control Panel\\UnsupportedHardwareNotificationCache", new RegistryItem("SV2", ValueKind.REG_DWORD, 0));
                RegistryHelper.AddRegistryItem("HKLM\\zSYSTEM\\Setup\\LabConfig", new RegistryItem("BypassCPUCheck", ValueKind.REG_DWORD, 1));
                RegistryHelper.AddRegistryItem("HKLM\\zSYSTEM\\Setup\\LabConfig", new RegistryItem("BypassRAMCheck", ValueKind.REG_DWORD, 1));
                RegistryHelper.AddRegistryItem("HKLM\\zSYSTEM\\Setup\\LabConfig", new RegistryItem("BypassSecureBootCheck", ValueKind.REG_DWORD, 1));
                RegistryHelper.AddRegistryItem("HKLM\\zSYSTEM\\Setup\\LabConfig", new RegistryItem("BypassStorageCheck", ValueKind.REG_DWORD, 1));
                RegistryHelper.AddRegistryItem("HKLM\\zSYSTEM\\Setup\\LabConfig", new RegistryItem("BypassTPMCheck", ValueKind.REG_DWORD, 1));
                RegistryHelper.AddRegistryItem("HKLM\\zSYSTEM\\Setup\\MoSetup", new RegistryItem("AllowUpgradesWithUnsupportedTPMOrCPU", ValueKind.REG_DWORD, 1));
                RegistryHelper.AddRegistryItem("HKLM\\zSYSTEM\\Setup\\Status\\ChildCompletion", new RegistryItem("setup.exe", ValueKind.REG_DWORD, 3));

                UpdateCurrentProgressBar(75);
                WriteLogMessage("Imposing old Setup...");
                RegistryHelper.AddRegistryItem("HKLM\\zSYSTEM\\Setup", new RegistryItem("CmdLine", ValueKind.REG_SZ, "\\sources\\setup.exe"));

                UpdateCurrentProgressBar(95);
                WriteLogMessage("Unloading image registry hives...");
                RegistryHelper.UnloadRegistryHive("zSYSTEM");
                RegistryHelper.UnloadRegistryHive("zSOFTWARE");
                RegistryHelper.UnloadRegistryHive("zDEFAULT");
                RegistryHelper.UnloadRegistryHive("zNTUSER");

                UpdateCurrentStatus("Unmounting boot image...");
                DismManager.UnmountAndSave(AppState.ScratchPath.TrimEnd('\\'), (p) => UpdateCurrentProgressBar(p), (msg) => WriteLogMessage(msg));

                UpdateOverallStatus("Generating ISO file...");
                UpdateOverallProgressBar(90);
                UpdateCurrentStatus("Generating ISO file...");
                OscdimgUtilities.checkoscdImg();

                UpdateOverallStatus("Finishing up...");
                UpdateOverallProgressBar(95);
                UpdateCurrentStatus("Finishing up...");
                WriteLogMessage("Deleting temporary files...");
                DeleteFiles.SafeDeleteDirectory(AppState.TempRoot);
            });

            WindowHelper.EnableCloseCapability(Handle);
            WriteLogMessage("Finished.");
            UpdateCurrentStatus("Generation complete");
            UpdateOverallProgressBar(100);
            UpdateCurrentProgressBar(100);
            ChangePage(WizardPage.Page.FinishPage);
        }

        private void lnkUseDT_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/CodingWonders/DISMTools");
        }

        private void lnkUseNtLite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://ntlite.com");
        }

        private void lnkOpenIsoLoc_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe"),
                $"/select,\"{AppState.SaveISO}\"");
        }
    }
}