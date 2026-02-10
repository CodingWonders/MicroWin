using System;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace MicroWin.functions.Helpers.DynaLog
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
        private static readonly int DYNALOG_LOG_ARCHIVE_MINIMUM_THRESHOLD_DAYS_ABS = -DYNALOG_LOG_ARCHIVE_MINIMUM_THRESHOLD_DAYS;
        private static readonly int DYNALOG_LOG_ARCHIVE_RETENTION_MAXIMUM_THRESHOLD_DAYS_ABS = -DYNALOG_LOG_ARCHIVE_RETENTION_MAXIMUM_THRESHOLD_DAYS;

        /// <summary>
        /// Determines whether the logger is temporarily enabled or disabled
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>This can be called by any function/method</remarks>
        public static bool LoggerEnabled = True;

        private static void RemoveLogArchives()
        {
            LogMessage("Removing archived logs older than " & DYNALOG_LOG_ARCHIVE_RETENTION_MAXIMUM_THRESHOLD_DAYS_ABS & " days...", False);
            try
            {
                string[] LogArchives = Directory.GetFiles(Path.Combine(Application.StartupPath, "logs"), "DT_DynaLog_*.old", SearchOption.TopDirectoryOnly);
                LogMessage("Archives found: " & LogArchives.Count);
                if (LogArchives.Count > 0)
                {
                    LogMessage("Removing log archives...");
                    foreach (var LogArchive in LogArchives)
                    {
                        try
                        {
                            // Determine if log file is older than retention maximum threshold constant
                            DateTime CreationDate = File.GetCreationTimeUtc(LogArchive);
                            if (CreationDate < DateTime.UtcNow.AddDays(DYNALOG_LOG_ARCHIVE_RETENTION_MAXIMUM_THRESHOLD_DAYS))
                            {
                                LogMessage("- Log archive " & Quote & LogArchive & Quote & " is more than " & DYNALOG_LOG_ARCHIVE_RETENTION_MAXIMUM_THRESHOLD_DAYS_ABS & " days old. Removing...");
                                File.Delete(LogArchive);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogMessage("Could not delete log archive. Error message: " & ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage("Could not detect log archives. Error message: " & ex.Message);
            }
        }

        /// <summary>
        /// Checks the age of the active DynaLog log file
        /// </summary>
        /// <remarks>If a DynaLog log file is older than 2 weeks, it will be archived</remarks>
        public static void CheckLogAge()
        {
            LogMessage("Checking existing logs...", False);
            if (File.Exists(Application.StartupPath & "\\logs\\DT_DynaLog.log"))
            {
                LogMessage("Log File Found. Checking log file creation date...", False);
                try
                {
                    DateTime CreationDate = File.GetCreationTimeUtc(Application.StartupPath & "\\logs\\DT_DynaLog.log");
                    if (CreationDate < DateTime.UtcNow.AddDays(DYNALOG_LOG_ARCHIVE_MINIMUM_THRESHOLD_DAYS))
                    {
                        LogMessage("Current log file is more than 2 weeks old. Archiving...", False);
                        string ArchivedFileName = "DT_DynaLog_" & DateTime.UtcNow.ToString("yyMMdd-HHmm") & ".old";
                        try
                        {
                            File.Move(Application.StartupPath & "\\logs\\DT_DynaLog.log", Application.StartupPath & "\\logs\\" & ArchivedFileName);
                            LogMessage("The old log file has been archived. New messages will be shown in a new log file", False);      // A blank sheet of... logs?
                            File.SetCreationTimeUtc(Application.StartupPath & "\\logs\\DT_DynaLog.log", Date.UtcNow);
                        }
                        catch (Exception ex)
                        {
                            LogMessage("Could not archive log. Error info:" & CrLf & CrLf & ex.ToString(), False);
                        }
                    }
                    else
                    {
                        // Don't archive
                    }
                }
                catch (Exception ex)
                {
                    LogMessage("Could not check log file age. Error info:" & CrLf & CrLf & ex.ToString(), False);
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
            LogMessage("----- Dynamic Logging (DynaLog) version " & DYNALOG_VERSION & " -----", False);
            LogMessage("DynaLog Logger has begun logging program operations...", False);
            LogMessage("--- Time Stamps are shown in UTC Time!!! ---", False);
        }

        Public Shared Sub EndLogging()
            LogMessage("DynaLog Logger has stopped logging program operations...", False)
            LogMessage("========================================================", False)
        End Sub

        Public Shared Sub DisableLogging()
            LogMessage("Logger has been temporarily disabled by caller " & New StackFrame(1).GetMethod().Name, False)
            LoggerEnabled = False
        End Sub

        Public Shared Sub EnableLogging()
            If MainForm.EnableDynaLog Then
                LoggerEnabled = True
                LogMessage("Logger has been enabled again by caller " & New StackFrame(1).GetMethod().Name, False)
            Else
                Debug.WriteLine("The logger has been ultimately disabled and cannot be enabled with this method. Use the Ultimate switch to bypass.")
            End If
        End Sub

        /// <summary>
        /// Logs a message with DynaLog to the log file
        /// </summary>
        /// <param name="message">The message to log. It cannot be empty</param>
        /// <param name="GetParentCaller">Determines whether or not to get the name of the caller that called a specific method that called DynaLog logging</param>
        /// <remarks></remarks>
        Public Shared Sub LogMessage(message As String, Optional GetParentCaller As Boolean = True)
            If Not LoggerEnabled OrElse message = "" Then
                Debug.WriteLine("Logger Enabled? " & If(LoggerEnabled, "Yes", "No"))
                Debug.WriteLine("Message: " & Quote & message & Quote)
                Debug.WriteLine("Either the logger is not enabled or the message is empty. Message cannot be logged.")
                Exit Sub
            End If
            Debug.WriteLine(message)
            Try
                ' DynaLog will NOT display logs for log file/folder creation - ONLY in debugger.
                If Not Directory.Exists(Application.StartupPath & "\logs") Then
                    Debug.WriteLine("Creating log directory...")
                    Directory.CreateDirectory(Application.StartupPath & "\logs")
                End If
                Dim FileLength As Long = 0
                If File.Exists(Application.StartupPath & "\logs\DT_DynaLog.log") Then
                    FileLength = New FileInfo(Application.StartupPath & "\logs\DT_DynaLog.log").Length
                End If
                Dim MessagePrefix As String = "[" & Date.UtcNow.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) & "] [PID " & Process.GetCurrentProcess().Id & "] [" & New StackFrame(1).GetMethod().Name & If(GetParentCaller, " (" & New StackFrame(2).GetMethod().Name & ")", "") & "] "
                Dim MessageLine As String = MessagePrefix & message.Replace(CrLf, CrLf & MessagePrefix).Trim()
                File.AppendAllText(Application.StartupPath & "\logs\DT_DynaLog.log", If(FileLength > 0, CrLf, "") & MessageLine)
            Catch ex As Exception
                Debug.WriteLine("DynaLog logging could not log this operation. Error:" & CrLf & CrLf & ex.ToString())
            End Try
        End Sub

    End Class
}