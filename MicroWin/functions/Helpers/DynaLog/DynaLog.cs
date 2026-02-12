using System;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace MicroWin.functions.Helpers.Loggers
{
    public class DynaLog
    {
        /// <summary>
        /// Version identifier for DynaLog in this implementation
        /// </summary>
        /// <remarks></remarks>
        private const string DYNALOG_VERSION = "1.0.3";

        /// <summary>
        /// The minimum amount of days a current log should be kept for archiving
        /// </summary>
        /// <remarks>Negative value to reflect how old a file is</remarks>
        private const int DYNALOG_LOG_ARCHIVE_MINIMUM_THRESHOLD_DAYS = -14;
        /// <summary>
        /// The maximum amount of days log archives should be kept for deletion
        /// </summary>
        /// <remarks>Negative value to reflect how old a file is</remarks>
        private const int DYNALOG_LOG_ARCHIVE_RETENTION_MAXIMUM_THRESHOLD_DAYS = -28;

        // Absolute values for constants
        private static readonly int DYNALOG_LOG_ARCHIVE_RETENTION_MAXIMUM_THRESHOLD_DAYS_ABS = -DYNALOG_LOG_ARCHIVE_RETENTION_MAXIMUM_THRESHOLD_DAYS;

        /// <summary>
        /// Determines whether the logger is temporarily enabled or disabled
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>This can be called by any function/method</remarks>
        public static bool LoggerEnabled = true;

        private static void RemoveLogArchives()
        {
            logMessage($"Removing archived logs older than {DYNALOG_LOG_ARCHIVE_RETENTION_MAXIMUM_THRESHOLD_DAYS_ABS} days...", false);
            try
            {
                string[] LogArchives = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"), "MW_DynaLog_*.old", SearchOption.TopDirectoryOnly);
                logMessage($"Archives found: {LogArchives.Length}");
                if (LogArchives.Any())
                {
                    logMessage("Removing log archives...");
                    foreach (string LogArchive in LogArchives)
                    {
                        try
                        {
                            // Determine if log file is older than retention maximum threshold constant
                            DateTime CreationDate = File.GetCreationTimeUtc(LogArchive);
                            if (CreationDate < DateTime.UtcNow.AddDays(DYNALOG_LOG_ARCHIVE_RETENTION_MAXIMUM_THRESHOLD_DAYS))
                            {
                                logMessage($"- Log archive \"{LogArchive}\" is more than {DYNALOG_LOG_ARCHIVE_RETENTION_MAXIMUM_THRESHOLD_DAYS_ABS} days old. Removing...");
                                File.Delete(LogArchive);
                            }
                        }
                        catch (Exception ex)
                        {
                            logMessage($"Could not delete log archive. Error message: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logMessage($"Could not detect log archives. Error message: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks the age of the active DynaLog log file
        /// </summary>
        /// <remarks>If a DynaLog log file is older than 2 weeks, it will be archived</remarks>
        public static void CheckLogAge()
        {
            logMessage("Checking existing logs...", false);
            if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\logs\\MW_DynaLog.log"))
            {
                logMessage("Log File Found. Checking log file creation DateTime...", false);
                try
                {
                    DateTime CreationDate = File.GetCreationTimeUtc($"{AppDomain.CurrentDomain.BaseDirectory}\\logs\\MW_DynaLog.log");
                    if (CreationDate < DateTime.UtcNow.AddDays(DYNALOG_LOG_ARCHIVE_MINIMUM_THRESHOLD_DAYS))
                    {
                        logMessage("Current log file is more than 2 weeks old. Archiving...", false);
                        string ArchivedFileName = $"MW_DynaLog_{DateTime.UtcNow.ToString("yyMMdd-HHmm")}.old";
                        try
                        {
                            File.Move("{AppDomain.CurrentDomain.BaseDirectory}\\logs\\MW_DynaLog.log", $"{AppDomain.CurrentDomain.BaseDirectory}\\logs\\{ArchivedFileName}");
                            logMessage("The old log file has been archived. New messages will be shown in a new log file", false);      // A blank sheet of... logs?
                            File.SetCreationTimeUtc($"{AppDomain.CurrentDomain.BaseDirectory}\\logs\\MW_DynaLog.log", DateTime.UtcNow);
                        }
                        catch (Exception ex)
                        {
                            logMessage($"Could not archive log. Error info:\n\n{ex}", false);
                        }
                    }
                    else
                    {
                        // Don't archive
                    }
                }
                catch (Exception ex)
                {
                    logMessage($"Could not check log file age. Error info:\n\n{ex}", false);
                }
                RemoveLogArchives();
            }
            else
            {
                // Don't do anything
            }
        }

        public static void BeginLogging()
        {
            logMessage($"----- Dynamic Logging (DynaLog) version {DYNALOG_VERSION} -----", false);
            logMessage($"DynaLog Logger has begun logging program operations...", false);
            logMessage($"--- Time Stamps are shown in UTC Time!!! ---", false);
        }


        /// <summary>
        /// Logs a message with DynaLog to the log file
        /// </summary>
        /// <param name="message">The message to log. It cannot be empty</param>
        /// <param name="GetParentCaller">Determines whether or not to get the name of the caller that called a specific method that called DynaLog logging</param>
        /// <remarks></remarks>
        public static void logMessage(string message, bool getParentCaller = false)
        {
            if ((!LoggerEnabled) || (message == ""))
            {
                Debug.WriteLine($"Logger Enabled? {(LoggerEnabled ? "Yes" : "No")}");
                Debug.WriteLine($"Message: {message}");
                Debug.WriteLine($"Either the logger is not enabled or the message is empty. Message cannot be logged.");
                return;
            }

            Debug.WriteLine(message);
            try
            {
                if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs")))
                {
                    Debug.WriteLine("Creating log directory...");
                    Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"));
                }
                long fileLength = 0L;
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "MW_DynaLog.log")))
                {
                    fileLength = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "MW_DynaLog.log")).Length;
                }
                string messagePrefix = $"[{DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}] [PID {Process.GetCurrentProcess().Id}] [{new StackFrame(1).GetMethod().Name}{(getParentCaller ? $" ({new StackFrame(2).GetMethod().Name})" : "")}] ";
                string messageLine = $"{messagePrefix}{message.Replace("\n", $"\n{messagePrefix}").Trim()}";
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "MW_DynaLog.log"), $"{(fileLength > 0 ? "\n" : "")}{messageLine}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DynaLog logging could not log this operation. Error: \n\n{ex}");
            }
        }
    }
}