using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace MicroWin.functions.Helpers.RegistryHelpers
{
    public static class RegistryHelper
    {

        private const int ERROR_SUCCESS = 0;

        private static int RunRegProcess(string arguments)
        {
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

        public static bool RegistryKeyExists(string keyPath)
        {
            return RunRegProcess($"query \"{keyPath.Replace("\"", "")}\"") == ERROR_SUCCESS;
        }

        public static bool RegistryValueExists(string keyPath, string valueName = "")
        {
            return RunRegProcess($"query \"{keyPath.Replace("\"", "")}\" {(valueName != "" ? $"/v \"{valueName.Replace("\"", "")}\"" : "/ve")}") == ERROR_SUCCESS;
        }

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

        public static bool AddRegistryItem(string keyPath)
        {
            return RunRegProcess($"add \"{keyPath.Replace("\"", "")}\" /f") == ERROR_SUCCESS;
        }

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

        public static bool RemoveRegistryItem(string keyPath, bool allValues = false)
        {
            return RunRegProcess($"delete \"{keyPath.Replace("\"", "")}\" {(allValues ? "/va" : "")} /f") == ERROR_SUCCESS;
        }

        public static bool RemoveRegistryItem(string keyPath, string valueName = "")
        {
            return RunRegProcess($"delete \"{keyPath.Replace("\"", "")}\" {(valueName != "" ? $"/v \"{valueName.Replace("\"", "")}\"" : "/ve")} /f") == ERROR_SUCCESS;
        }

        public static bool LoadRegistryHive(string hivePath, string hiveKeyName)
        {
            if (String.IsNullOrEmpty(hivePath) || !File.Exists(hivePath)) return false;
            if (String.IsNullOrEmpty(hiveKeyName)) return false;
            return RunRegProcess($"load \"HKLM\\{hiveKeyName}\" \"{hivePath}\"") == ERROR_SUCCESS;
        }

        public static bool UnloadRegistryHive(string hiveKeyName)
        {
            if (String.IsNullOrEmpty(hiveKeyName)) return false;
            return RunRegProcess($"unload \"HKLM\\{hiveKeyName}\"") == ERROR_SUCCESS;
        }

        public static bool ExportRegistryKey(string keyPath, string regKeyFilePath)
        {
            if (String.IsNullOrEmpty(keyPath)) return false;
            if (String.IsNullOrEmpty(regKeyFilePath)) return false;
            return RunRegProcess($"export \"{keyPath.Replace("\"", "")}\" \"{regKeyFilePath.Replace("\"", "")}\"") == ERROR_SUCCESS;
        }

    }
}
