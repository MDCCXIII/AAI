﻿using System.Collections.Generic;
using System.Text;

namespace AAI_Log_Converter.Export
{
    internal class DataSheetBuilder
    {
        public static void AppendRowToFile(string service)
        {

            StringBuilder csv = new StringBuilder();
            //iterate through the call log information
            foreach (KeyValuePair<string, string> column in Program.serviceColumns[service])
            {
                if (Program.Header_PartnerID.Equals(column.Key))
                {
                    csv.Append(column.Value);
                }
                else if (!Program.Header_ServiceName.Equals(column.Key))
                {
                    csv.Append("," + column.Value);
                }
                
            }
            FileUtils.WriteToFile(service + "_Data.csv", csv);
            csv.Clear();
        }

        public static void WriteColumnHeaders(string service)
        {
            StringBuilder csv = new StringBuilder();
            foreach (KeyValuePair<string, string> column in Program.serviceColumns[service])
            {
                if (Program.Header_PartnerID.Equals(column.Key))
                {
                    csv.Append(column.Key);
                }
                else if (!Program.Header_ServiceName.Equals(column.Key))
                {
                    csv.Append("," + column.Key);
                }

            }
            FileUtils.WriteToFile(service + "_Data.csv", csv);
            csv.Clear();
        }
    }
}