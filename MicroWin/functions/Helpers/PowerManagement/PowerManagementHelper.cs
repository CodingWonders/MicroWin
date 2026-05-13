using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Runtime.InteropServices;
using System.Text;

namespace MicroWin.functions.Helpers.PowerManagement
{
    [SupportedOSPlatform("Windows")]
    public static class PowerManagementHelper
    {

        private enum EXECUTION_STATE : int
        {
            ES_CONTINUOUS = -2147483648,
            ES_SYSTEM_REQUIRED = 1,
            ES_DISPLAY_REQUIRED = 2
        }

        private class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
        }

        public static void DisableSystemSleepMode()
        {
            NativeMethods.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_DISPLAY_REQUIRED);
        }

        public static void EnableSystemSleepMode()
        {
            NativeMethods.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }

    }
}
