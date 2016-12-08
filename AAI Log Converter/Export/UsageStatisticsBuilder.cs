using System.Collections;
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
            foreach (DictionaryEntry column in Program.serviceColumns[service])
            {
                if (Program.columnSeenCount[service].ContainsKey(column.Key.ToString()))
                {
                    timesCalled = Program.columnSeenCount[service][column.Key.ToString()];
                    timesNull = Program.columnNullCount[service][column.Key.ToString()];
                    timesEmpty = Program.columnEmptyCount[service][column.Key.ToString()];
                    if(timesCalled != 0)
                    {
                        usagePercentage = (int)(((timesCalled - (timesNull + timesEmpty)) / timesCalled) * 100);
                    }
                    
                    csv.Append("\n" + column.Key + "," + timesCalled + "," + timesNull + "," + timesEmpty + "," + "%" + usagePercentage);
                    FileUtils.WriteToFile(service + "_Usages.csv", csv);
                    csv.Clear();
                }
            }
        }

        public static void WriteColumnHeaders(string service)
        {
            StringBuilder csv = new StringBuilder("Parameter Name, Times Called, Times Null, Times Empty, Usage Percentage");
            FileUtils.WriteToFile(service + "_Usages.csv", csv);
            csv.Clear();
        }
    
}
}