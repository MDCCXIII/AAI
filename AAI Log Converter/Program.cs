using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace AAI_Log_Converter
{
    class Program
    {
        public static List<string> lColumnHeaders = new List<string>();

        public const string CallLogName = "Call Log";
        public const string Header_Date = "Date";
        public const string Header_Time = "Time";
        public const string Header_PartnerID = "Partner ID";
        public const string Header_ServiceName = "Service Name";

        public static string ServiceName = ConfigurationManager.AppSettings["ServiceName"];

        public static Dictionary<string, List<string>> serviceFilePaths = new Dictionary<string, List<string>>();
        public static Dictionary<string, OrderedDictionary> serviceColumns = new Dictionary<string, OrderedDictionary>();

        public static Dictionary<string, Dictionary<string, int>> columnNullCount = new Dictionary<string, Dictionary<string, int>>();
        public static Dictionary<string, Dictionary<string, int>> columnEmptyCount = new Dictionary<string, Dictionary<string, int>>();
        public static Dictionary<string, Dictionary<string, int>> columnSeenCount = new Dictionary<string, Dictionary<string, int>>();

        static void Main(string[] args)
        {
            if (args.Length == 0) {
                args = new string[] { ConfigurationManager.AppSettings["DefaultSourceDirectory"] };
            }
            StartProc(args);
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
            
            
            FileImporter fileImporter = new FileImporter();
            foreach(KeyValuePair<string, List<string>> kvp in serviceFilePaths)
            {
                foreach(string filePath in kvp.Value)
                {
                    //gather parameter names used in all calls of a service into [serviceColumns]
                    fileImporter.ImportColumns(kvp.Key, filePath);
                }

                foreach (string filePath in kvp.Value)
                {
                    //Gather data per service call adding counts to memory object('s)
                    //print data per service call
                    fileImporter.ImportValues(kvp.Key, filePath);
                }
            }
        }

    }
}
