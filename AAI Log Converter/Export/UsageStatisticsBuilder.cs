using System.Collections.Generic;
using System.Text;

namespace AAI_Log_Converter.Export
{
    internal class UsageStatisticsBuilder
    {
        public static void AppendRowToFile(string service)
        {

            StringBuilder csv = new StringBuilder();
            double timesCalled = 0;
            double timesNull = 0;
            double timesEmpty = 0;
            int usagePercentage = 0;

            //iterate through the call log information
            foreach (KeyValuePair<string, string> column in Program.serviceColumns[service])
            {
                if (Program.columnSeenCount[service].ContainsKey(column.Key))
                {
                    timesCalled = Program.columnSeenCount[service][column.Key];
                    timesNull = Program.columnNullCount[service][column.Key];
                    timesEmpty = Program.columnEmptyCount[service][column.Key];
                    usagePercentage = (int)(((timesCalled - (timesNull + timesEmpty)) / timesCalled) * 100);
                    csv.AppendLine(column.Key + "," + timesCalled + "," + timesNull + "," + timesEmpty + "," + "%" + usagePercentage);
                }
            }
            FileUtils.WriteToFile(service + "_Data.csv", csv);
            csv.Clear();
        }

        public static void WriteColumnHeaders(string service)
        {
            StringBuilder csv = new StringBuilder("Parameter Name, Times Called, Times Null, Times Empty, Usage Percentage");
            FileUtils.WriteToFile(service + "_Usages.csv", csv);
            csv.Clear();
        }
    
}
}