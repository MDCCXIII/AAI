using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAI_Log_Converter
{
    public static class ServiceLogger
    {
        private static string LogFileName = "AAILC_" + string.Format("{0:yyyyMMddHHmmss}.txt", DateTime.Now);
        private static string LogFileDirectory = Program.WorkingDirectory + "\\AAI Conversion Logs";
        private static string LogFile = LogFileDirectory + "\\" + LogFileName;

        public static void CreateLogFileDirectory()
        {
            FileUtils.CreateDirectory(LogFileDirectory);
        }
        public static void WriteLine(string message = "\n", 
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!message.Equals("\n"))
            {
                message = "\n" + message + ",\n";
            }
            message += memberName + ", " + sourceFilePath + ", Line: " + sourceLineNumber;

            File.AppendAllText(LogFile, message);
        }
    }
}
