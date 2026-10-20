using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MicroWin.functions.Helpers.ProgramResources
{
    public class ResourceHelper
    {
        
        public static Stream? GetProgramResourceStream(string resourceName) {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            try {
                return currentAssembly.GetManifestResourceStream(resourceName);
            } catch {
                return null;
            }
        }

    }
}
