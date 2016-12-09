using AAI_Log_Converter.Export;
using AAI_Log_Converter.Import;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace AAI_Log_Converter
{
    /// <summary>
    /// This class takes in a file path and passes each line of that file to the ConversionRules class
    /// </summary>
    class FileImporter
    {
        internal static string serviceName;
        public string partnerName;
        public LineInfo lineInfo = new LineInfo();

        public void ImportColumns(string serviceName, string path)
        {
            ColumnImporter columnImporter = new ColumnImporter();
            AddEntryToServiceColumns(serviceName);
            columnImporter.AddColumnForService(serviceName, Program.Header_ServiceName);
            columnImporter.AddColumnForService(serviceName, Program.Header_PartnerID);
            columnImporter.AddColumnForService(serviceName, Program.Header_Date);
            columnImporter.AddColumnForService(serviceName, Program.Header_Time);
            ReadColumnsIntoCollection(serviceName, path, columnImporter);
        }

        public void ImportValues(string serviceName, string path)
        {
            SetPartnerName(path);
            ReadColumnValuesIntoFiles(serviceName, path);
        }

        

        private static void AddEntryToServiceColumns(string serviceName)
        {
            if (!Program.serviceColumns.ContainsKey(serviceName))
            {
                Program.serviceColumns.Add(serviceName, new OrderedDictionary());
            }
            if (!Program.columnSeenCount.ContainsKey(serviceName))
            {
                Program.columnSeenCount.Add(serviceName, new Dictionary<string, int>());
            }
            if (!Program.columnNullCount.ContainsKey(serviceName))
            {
                Program.columnNullCount.Add(serviceName, new Dictionary<string, int>());
            }
            if (!Program.columnEmptyCount.ContainsKey(serviceName))
            {
                Program.columnEmptyCount.Add(serviceName, new Dictionary<string, int>());
            }
        }

        

        private void SetPartnerName(string path)
        {
            string[] filePathParts = path.Split('\\');
            partnerName = filePathParts[filePathParts.Count() - 2].Trim();

        }

        private void ReadColumnsIntoCollection(string serviceName, string path, ColumnImporter columnImporter)
        {
            string line = "";
            Console.WriteLine("Importing columns for " + path);
            using (TextReader tr = new StreamReader(path))
            {
                while ((line = tr.ReadLine()) != null)
                {
                    columnImporter.ImportColumns(serviceName, line.Trim(), this);
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            
            Console.WriteLine("Complete. \n");
        }

        private void ReadColumnValuesIntoFiles(string serviceName, string path)
        {
            ColumnImporter columnImporter = new ColumnImporter();
            string line = "";
            Console.WriteLine("Converting " + path);
            using (TextReader tr = new StreamReader(path)) {
                while ((line = tr.ReadLine()) != null) {
                    if(columnImporter.ImportColumnValues(serviceName, line.Trim(), this))
                    {
                        //append column values to file
                        CallLogBuilder.AppendRowToFile(serviceName);
                        DataSheetBuilder.AppendRowToFile(serviceName);
                        
                        columnImporter.ClearIfNewService(line.Trim(), serviceName);
                    }
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            
            Console.WriteLine("Complete. \n");
        }
    }
}
