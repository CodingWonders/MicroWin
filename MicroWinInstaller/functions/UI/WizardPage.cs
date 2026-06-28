using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroWinInstaller.functions.UI
{
    public class WizardPage
    {
        public enum Page : int
        {
            WelcomePage = 0,
            DriveSelectionPage = 1,
            ImageTypePage = 2,
            ProgressPage = 3,
            FinishPage = 4
        }

        public Page wizardPage { get; set; }

        public const int PageCount = 4;

    }
}
