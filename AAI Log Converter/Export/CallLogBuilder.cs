using System.Collections.Generic;
using System.Text;

namespace AAI_Log_Converter.Export
{
    class CallLogBuilder
    {
        public static void Build()
        {
            bool columnsNeeded = true;
            StringBuilder csv = new StringBuilder();
            //iterate through the call log information
            foreach (KeyValuePair<string, List<ColumnInfo>> CallLog in Program.CallLogData) {
                //iterate through each row of data
                foreach(ColumnInfo row in CallLog.Value) {
                    string columnHeaders = "";
                    string csvRow = "";
                    
                    //iterate through each column for the row 
                    foreach(KeyValuePair<string, string> column in row.ColumnValues) {
                        if (columnsNeeded) {
                            columnHeaders += ", " + column.Key;
                        }
                        csvRow += ", " + column.Value;
                    }
                    if (columnsNeeded) {
                        csv.AppendLine(columnHeaders.Remove(0,1).Trim());
                    }
                    csv.AppendLine(csvRow.Remove(0, 1).Trim());
                    columnsNeeded = false;
                }
                FileUtils.WriteToFile(CallLog.Key + ".csv", csv);
                csv.Clear();
                columnsNeeded = true;
            }
        }
    }
}
