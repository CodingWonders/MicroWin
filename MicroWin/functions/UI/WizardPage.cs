using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroWin.functions.UI
{
    public class WizardPage
    {
        public enum Page : int
        {
            WelcomePage = 0,
            IsoChooserPage = 1,
            ImageChooserPage = 2,
            UserAccountsPage = 3,
            IsoSettingsPage = 4,
            IsoCreationPage = 5,
            FinishPage = 6
        }

        public Page wizardPage { get; set; }

        public const int PageCount = 7;

    }
}
