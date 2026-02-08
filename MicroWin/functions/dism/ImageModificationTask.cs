using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroWin.functions.dism
{
    public abstract class ImageModificationTask
    {
        public virtual List<string> excludedItems { get; set; }

        public abstract void RunTask();
    }
}
