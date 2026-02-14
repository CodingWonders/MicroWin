using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using MicroWin.functions.Helpers.Loggers;

namespace MicroWin.functions.Helpers.RegistryHelpers
{
    public static class RegistryHelper
    {
        /// <summary>
        /// Constant for REG operations that finish successfully.
        /// </summary>
        private const int ERROR_SUCCESS = 0;

        /// <summary>
        /// Starts a new REG process
        /// </summary>
        /// <param name="arguments">The arguments to pass to the REG process</param>
        /// <returns>The exit code of the process</returns>
        private static int RunRegProcess(string arguments)
        {
            DynaLog.logMessage("Running REG process...");
            DynaLog.logMessage($"- Command-line Arguments: {arguments}");
            DynaLog.logMessage("Checking presence of REG program...");
            int exitCode = ERROR_SUCCESS;

            Process regProc = new()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "system32", "reg.exe"),
                    Arguments = arguments,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            regProc.Start();
            regProc.WaitForExit();
            exitCode = regProc.ExitCode;

            return exitCode;
        }

        /// <summary>
        /// Gets whether a specified registry key path exists.
        /// </summary>
        /// <param name="keyPath">The path to the key to check</param>
        /// <returns><see langword="true"/> if the key exists, <see langword="false"/> otherwise.</returns>
        public static bool RegistryKeyExists(string keyPath)
        {
            return RunRegProcess($"query \"{keyPath.Replace("\"", "")}\"") == ERROR_SUCCESS;
        }

        /// <summary>
        /// Gets whether a specified registry value exists in the given registry key path.
        /// </summary>
        /// <param name="keyPath">The path to the key containing the value to check</param>
        /// <param name="valueName">The name of the value to check</param>
        /// <returns><see langword="true"/> if both the key and the value exist, <see langword="false"/> otherwise.</returns>
        public static bool RegistryValueExists(string keyPath, string valueName = "")
        {
            return RunRegProcess($"query \"{keyPath.Replace("\"", "")}\" {(valueName != "" ? $"/v \"{valueName.Replace("\"", "")}\"" : "/ve")}") == ERROR_SUCCESS;
        }

        /// <summary>
        /// Gets a native enum value for registry value kinds out of a value kind from Win32 namespaces.
        /// </summary>
        /// <param name="valueKind">The registry value kind from the Win32 namespace</param>
        /// <returns>The value kind for the native namespace.</returns>
        private static ValueKind GetValueKindEnumValue(RegistryValueKind valueKind)
        {
            switch (valueKind)
            {
                case RegistryValueKind.None:
                    return ValueKind.REG_NONE;
                case RegistryValueKind.String:
                    return ValueKind.REG_SZ;
                case RegistryValueKind.MultiString:
                    return ValueKind.REG_MULTI_SZ;
                case RegistryValueKind.ExpandString:
                    return ValueKind.REG_EXPAND_SZ;
                case RegistryValueKind.Binary:
                    return ValueKind.REG_BINARY;
                case RegistryValueKind.DWord:
                    return ValueKind.REG_DWORD;
                case RegistryValueKind.QWord:
                    return ValueKind.REG_QWORD;
                default:
                    return ValueKind.REG_NONE;
            }
        }

        private static ValueKind GetValueKindEnumValue(string valueKind)
        {
            switch (valueKind)
            {
                case "REG_NONE":
                    return ValueKind.REG_NONE;
                case "REG_SZ":
                    return ValueKind.REG_SZ;
                case "REG_MULTI_SZ":
                    return ValueKind.REG_MULTI_SZ;
                case "REG_EXPAND_SZ":
                    return ValueKind.REG_EXPAND_SZ;
                case "REG_BINARY":
                    return ValueKind.REG_BINARY;
                case "REG_DWORD":
                    return ValueKind.REG_DWORD;
                case "REG_QWORD":
                    return ValueKind.REG_QWORD;
                default:
                    return ValueKind.REG_NONE;
            }
        }

        /// <summary>
        /// Gets a string representation of a given value kind enum value.
        /// </summary>
        /// <param name="kind">The value kind enum value</param>
        /// <returns>The string representation of the enum value.</returns>
        private static string GetValueKindString(ValueKind kind)
        {
            switch (kind)
            {
                case ValueKind.REG_NONE:
                    return "REG_NONE";
                case ValueKind.REG_SZ:
                    return "REG_SZ";
                case ValueKind.REG_MULTI_SZ:
                    return "REG_MULTI_SZ";
                case ValueKind.REG_EXPAND_SZ:
                    return "REG_EXPAND_SZ";
                case ValueKind.REG_BINARY:
                    return "REG_BINARY";
                case ValueKind.REG_DWORD:
                    return "REG_DWORD";
                case ValueKind.REG_QWORD:
                    return "REG_QWORD";
                default:
                    return "REG_NONE";
            }
        }

        /// <summary>
        /// Gets information about a given registry value in a given registry key path.
        /// </summary>
        /// <param name="keyPath">The path to the key containing the value to query</param>
        /// <param name="valueName">The name of the value to query</param>
        /// <returns>A <see cref="RegistryItem"/> object containing the registry value name, kind, and data.</returns>
        public static RegistryItem QueryRegistryValue(string keyPath, string valueName = "")
        {
            RegistryItem item = null;
            RegistryKey registryKey = null;

            // This is an exemption from the rule of no .NET API. We can safely query stuff with it
            try
            {
                registryKey = Registry.LocalMachine.OpenSubKey(keyPath, false);
                object regValueData = registryKey.GetValue(valueName != "" ? valueName : null);
                RegistryValueKind regValueKind = registryKey.GetValueKind(valueName != "" ? valueName : null);
                item = new RegistryItem(valueName, GetValueKindEnumValue(regValueKind), regValueData);
            }
            catch (Exception)
            {
                // TODO log
            }
            finally
            {
                if (registryKey is not null)
                    registryKey.Close();
            }

            return item;
        }

        /// <summary>
        /// Adds a new key to the Registry.
        /// </summary>
        /// <param name="keyPath">The path of the key to add</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <remarks>
        /// Intermediate paths, if non-existent, will not be created. You will have to call this method as
        /// many times as necessary to create all intermediate key paths.
        /// </remarks>
        public static bool AddRegistryItem(string keyPath)
        {
            return RunRegProcess($"add \"{keyPath.Replace("\"", "")}\" /f") == ERROR_SUCCESS;
        }

        /// <summary>
        /// Adds a new value to a registry key, or modifies an existing value in a registry key.
        /// </summary>
        /// <param name="keyPath">The path to the key that will store the value</param>
        /// <param name="value">A <see cref="RegistryItem"/> object with the registry value information.</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <remarks>To target the default value, leave the name of the <paramref name="value"/> parameter empty.</remarks>
        public static bool AddRegistryItem(string keyPath, RegistryItem value)
        {
            if (value is null)
                return false;

            string regArgs = $"add \"{keyPath}\" /f ";
            if (value.Name == "")
            {
                regArgs += "/ve ";
            }
            else
            {
                regArgs += $"/v \"{value.Name.Replace("\"", "")}\" ";
            }

            regArgs += $"/t {GetValueKindString(value.Kind)} /d \"{value.Data.ToString()}\"";

            return RunRegProcess(regArgs) == ERROR_SUCCESS;
        }

        /// <summary>
        /// Removes a registry key.
        /// </summary>
        /// <param name="keyPath">The path to the key to remove</param>
        /// <param name="allValues">Determines whether all values within that key are also deleted. This is optional</param>
        /// <returns>Whether the operation succeeded.</returns>
        public static bool RemoveRegistryItem(string keyPath, bool allValues = false)
        {
            return RunRegProcess($"delete \"{keyPath.Replace("\"", "")}\" {(allValues ? "/va" : "")} /f") == ERROR_SUCCESS;
        }

        /// <summary>
        /// Removes a registry value from a registry key
        /// </summary>
        /// <param name="keyPath">The path to the key containing the value to remove</param>
        /// <param name="valueName">The name of the value to remove</param>
        /// <returns>Whether the operation succeeded. <see langword="false"/> will be returned if either the key or the value don't exist, or due to another REG error.</returns>
        public static bool RemoveRegistryItem(string keyPath, string valueName = "")
        {
            return RunRegProcess($"delete \"{keyPath.Replace("\"", "")}\" {(valueName != "" ? $"/v \"{valueName.Replace("\"", "")}\"" : "/ve")} /f") == ERROR_SUCCESS;
        }

        /// <summary>
        /// Loads a registry hive file into the system Registry.
        /// </summary>
        /// <param name="hivePath">The path to the hive file to load.</param>
        /// <param name="hiveKeyName">The name of the key to which the hive will be loaded.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public static bool LoadRegistryHive(string hivePath, string hiveKeyName)
        {
            if (String.IsNullOrEmpty(hivePath) || !File.Exists(hivePath)) return false;
            if (String.IsNullOrEmpty(hiveKeyName)) return false;
            return RunRegProcess($"load \"HKLM\\{hiveKeyName}\" \"{hivePath}\"") == ERROR_SUCCESS;
        }

        /// <summary>
        /// Unloads a registry hive from the system Registry.
        /// </summary>
        /// <param name="hiveKeyName">The name of the key to unload.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public static bool UnloadRegistryHive(string hiveKeyName)
        {
            if (String.IsNullOrEmpty(hiveKeyName)) return false;
            return RunRegProcess($"unload \"HKLM\\{hiveKeyName}\"") == ERROR_SUCCESS;
        }

        /// <summary>
        /// Exports a registry key to a file.
        /// </summary>
        /// <param name="keyPath">The path to the key to export.</param>
        /// <param name="regKeyFilePath">The destination registry file.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public static bool ExportRegistryKey(string keyPath, string regKeyFilePath)
        {
            if (String.IsNullOrEmpty(keyPath)) return false;
            if (String.IsNullOrEmpty(regKeyFilePath)) return false;
            return RunRegProcess($"export \"{keyPath.Replace("\"", "")}\" \"{regKeyFilePath.Replace("\"", "")}\"") == ERROR_SUCCESS;
        }

    }
}
