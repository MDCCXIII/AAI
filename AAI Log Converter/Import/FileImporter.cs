﻿using AAI_Log_Converter.Import;
using System;
using System.Collections.Generic;
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

        public FileImporter(string path)
        {
            AddServiceToDataObject(path);
            SetPartnerName(path);
            ReadAllLines(path);
        }

        private void SetPartnerName(string path)
        {
            string[] filePathParts = path.Split('\\');
            partnerName = filePathParts[filePathParts.Count() - 2].Trim();

        }

        private void AddServiceToDataObject(string path)
        {
            serviceName = FileUtils.GetFileName(path);
            if (!Program.PerServiceData.ContainsKey(serviceName)) {
                Program.PerServiceData.Add(serviceName, new List<ColumnInfo>(10000000));
            }
        }

        private void ReadAllLines(string path)
        {
            ColumnInfo columnInfo = new ColumnInfo();
            string line = "";
            Console.WriteLine("Converting " + path);
            using (TextReader tr = new StreamReader(path)) {
                while ((line = tr.ReadLine()) != null) {
                    new ConversionRules(ref columnInfo, serviceName, line.Trim(), this);
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            Console.WriteLine("Complete. \n");
        }
    }
}