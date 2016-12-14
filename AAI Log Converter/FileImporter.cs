using AAI_Log_Converter.Export;
using AAI_Log_Converter.Import;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace AAI_Log_Converter
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     This class takes in a file path and passes each line of that file to the ConversionRules
    ///     class.
    /// </summary>
    ///
    /// <remarks>   Ahaynes, 12/13/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    class FileImporter
    {
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Import values. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="serviceName">  Name of the service. </param>
        /// <param name="path">         Full pathname of the file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ImportValues(string serviceName, string path)
        {
            SetPartnerName(path);
            ReadColumnValuesIntoFiles(serviceName, path);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds an entry to service columns. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="serviceName">  Name of the service. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets partner name. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetPartnerName(string path)
        {
            string[] filePathParts = path.Split('\\');
            partnerName = filePathParts[filePathParts.Count() - 2].Trim();

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Reads columns into collection. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="serviceName">      Name of the service. </param>
        /// <param name="path">             Full pathname of the file. </param>
        /// <param name="columnImporter">   The column importer instance. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ReadColumnsIntoCollection(string serviceName, string path, ColumnImporter columnImporter)
        {
            ServiceLogger.WriteLine("Importing columns for " + path);
            string line = "";
            using (TextReader tr = new StreamReader(path))
            {
                while ((line = tr.ReadLine()) != null)
                {
                    columnImporter.ImportColumnNames(serviceName, line.Trim(), this);
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Reads column values into files. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="serviceName">  Name of the service. </param>
        /// <param name="path">         Full pathname of the file. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ReadColumnValuesIntoFiles(string serviceName, string path)
        {
            ServiceLogger.WriteLine("Converting " + path);
            ColumnImporter columnImporter = new ColumnImporter();
            string line = "";
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
        }
    }
}
