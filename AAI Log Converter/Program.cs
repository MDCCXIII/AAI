using AAI_Log_Converter.Export;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

namespace AAI_Log_Converter
{
    class Program
    {
        public static Dictionary<string, List<ColumnInfo>> PerServiceData = new Dictionary<string, List<ColumnInfo>>();
        public static Dictionary<string, List<ColumnInfo>> CallLogData = new Dictionary<string, List<ColumnInfo>>();
        public static Dictionary<string, int> columnNullCount = new Dictionary<string, int>();
        public static Dictionary<string, int> columnEmptyCount = new Dictionary<string, int>();
        public static Dictionary<string, int> columnSeenCount = new Dictionary<string, int>();
        public static List<string> lColumnHeaders = new List<string>();

        public const string CallLogName = "Call Log";
        public static string ServiceName = ConfigurationManager.AppSettings["ServiceName"];

        static void Main(string[] args)
        {
            if (args.Length == 0) {
                args = new string[] { ConfigurationManager.AppSettings["DefaultSourceDirectory"] };
            }
            Convert(args);
            Console.ReadLine();
        }

        private static void Convert(string[] args)
        {
            try {
                FileUtils.ValidFileExtensions.Add(".txt");
                FilePathImporter.ParseArgs(args);
                CallLogBuilder.Build();
                DataSheetBuilder.Build();
                UsageStatisticsBuilder.Build();
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                Debug.WriteLine(ex);
            }
        }

    }
}
