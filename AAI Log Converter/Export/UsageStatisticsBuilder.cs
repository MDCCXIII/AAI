using System;
using System.Collections.Generic;
using System.Text;

namespace AAI_Log_Converter.Export
{
    internal class UsageStatisticsBuilder
    {
        internal static void Build()
        {
            StringBuilder csv = new StringBuilder();
            string columnHeaders = "Parameter Name, Times Called, Times Null, Times Empty, Usage Percentage";
            string csvRow = "";
            csv.AppendLine(columnHeaders);

            //iterate through the call log information
            foreach(string serviceName in Program.PerServiceData.Keys) {
                foreach (KeyValuePair<string, int> columnSeenCount in Program.columnSeenCount) {
                    csvRow = "";
                    if (columnSeenCount.Key.Contains(serviceName)) {
                        string parameterName = columnSeenCount.Key.Replace(serviceName + "_", "");
                        double timesCalled = columnSeenCount.Value;
                        double timesNull = Program.columnNullCount[columnSeenCount.Key];
                        double timesEmpty = Program.columnEmptyCount[columnSeenCount.Key];
                        int usagePercentage = (int)(((timesCalled - (timesNull + timesEmpty)) / timesCalled) * 100);
                        csvRow += parameterName + ", " + timesCalled + ", " + timesNull + ", " + timesEmpty + ", " + "%" + usagePercentage;
                        csv.AppendLine(csvRow);
                    }
                }
                FileUtils.WriteToFile(serviceName + "_Usages.csv", csv);
                csv.Clear();
                csv.AppendLine(columnHeaders);
            }
        }
    }
}