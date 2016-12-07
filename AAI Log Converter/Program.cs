using AAI_Log_Converter.Export;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

namespace AAI_Log_Converter
{
    class Program
    {
        //public static Dictionary<string, List<ColumnInfo>> perServiceData = new Dictionary<string, List<ColumnInfo>>();
        public static Dictionary<string, BigList> perServiceData = new Dictionary<string, BigList>();
        public static Dictionary<string, BigList> callLogData = new Dictionary<string, BigList>();
        
        
        public static List<string> lColumnHeaders = new List<string>();

        public const string CallLogName = "Call Log";
        public const string Header_Date = "Date";
        public const string Header_Time = "Time";
        public const string Header_PartnerID = "Partner ID";

        public static string ServiceName = ConfigurationManager.AppSettings["ServiceName"];

        public static Dictionary<string, List<string>> serviceFilePaths = new Dictionary<string, List<string>>();
        public static Dictionary<string, List<string>> serviceColumns = new Dictionary<string, List<string>>();

        public static Dictionary<string, int> columnNullCount = new Dictionary<string, int>();
        public static Dictionary<string, int> columnEmptyCount = new Dictionary<string, int>();
        public static Dictionary<string, int> columnSeenCount = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            if (args.Length == 0) {
                args = new string[] { ConfigurationManager.AppSettings["DefaultSourceDirectory"] };
            }
            StartProc(args);
            Convert(args);
            Console.ReadLine();
        }

        /// <summary>
        /// new algorythm implimentation
        /// </summary>
        /// <param name="args"></param>
        private static void StartProc(string[] args)
        {
            //Gather the filePaths
            FilePathImporter.ParseArgs(args);

            /*1. Gather all column names per service into object*/
            //[object] serviceColumns = Dictionary<string[service name], List<string[unique column names]>
            //foreach file
            //gather parameter names used in all calls of a service into [serviceColumns]



            /*2. Store formatted data to temp file*/
            //Foreach service
            //print column names to temp file
            //Foreach file
            //Clear pre-existing data for service call from memory
            //Gather data per service call adding counts to memory object('s)
            //print data to new row in temp file value of N/A to columns that are not used by the service call

            /*3. Create and finalize the Files*/
            //foreach temp file 
            //foreach line
            //print data to [service]_Data.csv CallLog.csv
            //foreach service
            //print counts from memory object to [service]_Usage.csv

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
