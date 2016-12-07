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
            foreach (string CallLog in Program.callLogData.Keys) {
                foreach(List<ColumnInfo> callLogRowCollection in Program.callLogData[CallLog].master) {
                    //iterate through each row of data
                    foreach (ColumnInfo row in callLogRowCollection) {
                        string columnHeaders = "";
                        string csvRow = "";

                        //iterate through each column for the row 
                        foreach (KeyValuePair<string, string> column in row.ColumnValues) {
                            if (columnsNeeded) {
                                columnHeaders += ", " + column.Key;
                            }
                            csvRow += ", " + column.Value;
                        }
                        if (columnsNeeded) {
                            csv.AppendLine(columnHeaders.Remove(0, 1).Trim());
                        }
                        csv.AppendLine(csvRow.Remove(0, 1).Trim());
                        columnsNeeded = false;
                    }
                }
                    FileUtils.WriteToFile(CallLog + ".csv", csv);
                    csv.Clear();
                    columnsNeeded = true;
            }
        }
    }
}
