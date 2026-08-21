using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroWin.functions.dism
{
    /// <summary>
    /// The <see cref="ImageModificationTask"/> class contains methods used to perform
    /// modification tasks on Windows images.
    /// </summary>
    public abstract class ImageModificationTask
    {
        /// <summary>
        /// A list of items to exclude when performing modification tasks, mostly removal tasks.
        /// </summary>
        public virtual List<string> excludedItems { get; protected set; } = [];

        /// <summary>
        /// Runs an image modification task
        /// </summary>
        /// <param name="pbReporter">A GUI reporter callback for progress bars</param>
        /// <param name="curOpReporter">A GUI reporter callback for current operations</param>
        /// <param name="logWriter">A GUI reporter callback for logs</param>
        public abstract void RunTask(Action<int> pbReporter, Action<string> curOpReporter, Action<string> logWriter);
    }
}
