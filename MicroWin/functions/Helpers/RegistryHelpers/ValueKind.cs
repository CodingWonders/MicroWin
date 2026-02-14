namespace MicroWin.functions.Helpers.RegistryHelpers
{
    public enum ValueKind : int
    {
        /// <summary>
        /// Undefined value type
        /// </summary>
        /// <remarks>Binary data stored by this key will be shown as hex in REGEDIT</remarks>
        REG_NONE = 0,
        /// <summary>
        /// Single-line, null-terminated string value type
        /// </summary>
        /// <remarks>Default value</remarks>
        REG_SZ = 1,
        /// <summary>
        /// Single-line, null-terminated string value type that contains unexpanded system environment variables
        /// </summary>
        /// <remarks>
        /// For example, "%WINDIR%\explorer.exe" will be interpreted by the target system as "%SYSTEMDRIVE%\WINDOWS\explorer.exe",
        /// therefore expanding the WINDIR environment variable to "%SYSTEMDRIVE%\WINDOWS" when getting the value data. In this example,
        /// the %SYSTEMDRIVE% environment variable is used instead of "C:" because the system volume could have been assigned a drive letter
        /// other than "C"
        /// </remarks>
        REG_EXPAND_SZ = 2,
        /// <summary>
        /// Multi-line, null-terminated string value type
        /// </summary>
        REG_MULTI_SZ = 3,
        /// <summary>
        /// Binary value type
        /// </summary>
        /// <remarks>Binary data stored by this key will be shown as hex in REGEDIT</remarks>
        REG_BINARY = 4,
        /// <summary>
        /// 32-bit integer value type
        /// </summary>
        REG_DWORD = 5,
        /// <summary>
        /// 64-bit integer value type
        /// </summary>
        REG_QWORD = 6
    }
}
