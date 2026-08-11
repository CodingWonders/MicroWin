using MicroWinInstaller.functions.UI;
using MicroWinInstaller.functions.Dism;
using System.Managment;
using MicroWinInstaller.AppState;

namespace MicroWinInstaller
{
    [SupportedOSPlatform("Windows")]
    public partial class MainForm : Form
    {
        private WizardPage CurrentWizardPage = new();
        private List<WizardPage.Page> VerifyInPages = [
            WizardPage.Page.DriveSelectionPage
        ];

        private bool BusyCannotClose = false;

        public MainForm()
        {
            InitializeComponent();
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
            FinishPage.Visible = newPage == WizardPage.Page.FinishPage;

            CurrentWizardPage.wizardPage = newPage;

            // Handle tasks when switching to certain pages
            switch (newPage)
            {

            }

            Next_Button.Enabled = !(newPage != WizardPage.Page.FinishPage) || !((int)newPage + 1 >= WizardPage.PageCount);
            Cancel_Button.Enabled = !(newPage == WizardPage.Page.FinishPage);
            Back_Button.Enabled = !(newPage == WizardPage.Page.WelcomePage) && !(newPage == WizardPage.Page.FinishPage);
            ButtonPanel.Visible = !(newPage == WizardPage.Page.IsoCreationPage);

            Next_Button.Text = newPage == WizardPage.Page.FinishPage ? "Close" : "Next";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ChangePage(WizardPage.Page.WelcomePage);
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

        private void Get_Drives(object sender, EventArgs e)
        {
            Process diskpart = new Process();
            diskpart.StartInfo.UseShellExecute = false;

        }

        private void lvDrives_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lvDrives.SelectedItems.Count == 1)
            {
                AppState.diskNumber = lvDrives.FocusedItem?.Text;
            }
        }
    }
}
