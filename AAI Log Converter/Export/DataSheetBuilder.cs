using System;
using System.Collections.Generic;
using System.Text;

namespace AAI_Log_Converter.Export
{
    internal class DataSheetBuilder
    {
        public static void Build()
        {
            bool columnsNeeded = true;
            StringBuilder csv = new StringBuilder();
            //iterate through the call log information
            foreach (string service in Program.perServiceData.Keys) {
                //build the column header row

                foreach (List<ColumnInfo> serviceDataRowGroup in Program.perServiceData[service].master) {
                    int lastKnownHeader = -1;
                    string lastHeaderName = "";
                    foreach (ColumnInfo row in serviceDataRowGroup) {
                        foreach (string header in row.ColumnValues.Keys) {
                            if (!Program.lColumnHeaders.Contains(header)) {
                                Program.lColumnHeaders.Insert(lastKnownHeader + 1, header);
                                lastKnownHeader = Program.lColumnHeaders.IndexOf(header);
                                lastHeaderName = header;
                            } else {
                                lastHeaderName = header;
                                lastKnownHeader = Program.lColumnHeaders.IndexOf(header);
                            }
                        }
                    }
                }

                foreach (List<ColumnInfo> serviceDataRowGroup in Program.perServiceData[service].master) {
                    //iterate through each row of data
                    foreach (ColumnInfo row in serviceDataRowGroup) {
                        string columnHeaders = "";
                        string csvRow = "";

                        //iterate through each column for the row 
                        foreach (string columnName in Program.lColumnHeaders) {
                            if (columnsNeeded) {
                                columnHeaders += ", " + columnName;
                            }

                            if (row.ColumnValues.ContainsKey(columnName)) {
                                csvRow += ", " + row.ColumnValues[columnName];
                            } else {
                                //The given key was not present in the dictionary.
                                csvRow += ", " + "N/A";
                            }
                        }

                        if (columnsNeeded) {
                            csv.AppendLine(columnHeaders.Remove(0, 1).Trim());
                        }
                        csv.AppendLine(csvRow.Remove(0, 1).Trim());
                        columnsNeeded = false;
                    }
                }
                FileUtils.WriteToFile(service + "_Data.csv", csv);
                csv.Clear();
                columnsNeeded = true;
                Program.lColumnHeaders.Clear();
            }
        }
    }
}