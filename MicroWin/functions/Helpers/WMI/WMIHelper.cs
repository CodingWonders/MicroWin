using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.Versioning;
using System.Text;

namespace MicroWin.functions.Helpers.WMI
{
	[SupportedOSPlatform("Windows")]
    public static class WMIHelper
    {
        public static ManagementObjectCollection? GetResultsFromManagementQuery(string ManagementQuery)
        {
			try
			{
				return new ManagementObjectSearcher(ManagementQuery).Get();
			}
			catch (Exception)
			{
				return null;
			}
        }

		public static object? GetObjectValue(ManagementObject? Item, string PropertyOfInterest)
		{
			if (Item is not null && PropertyOfInterest != "")
				return Item[PropertyOfInterest];
			return null;
		}

		public static string? GetEscapedValue(string ValueToEscape)
		{
			return ValueToEscape.Replace("\\", "\\\\").Replace("\"", "\\\"");
		}

    }
}
