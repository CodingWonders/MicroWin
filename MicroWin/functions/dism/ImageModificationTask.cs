using System.Collections.Generic;

namespace MicroWin.functions.dism
{
    public abstract class ImageModificationTask
    {
        public virtual List<string> excludedItems { get; protected set; } = [];

        public abstract void RunTask();
    }
}
