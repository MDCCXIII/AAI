using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAI_Log_Converter.Import
{
    class ConversionRules
    {
        private const string Id_ServiceCall = "(";
        private const string Id_EndOfServiceCall = "),";
        private const string Id_Parameter = "=";
        private const string Id_DynamicDataStructure = "[";
        private const string Id_EndOfDynamicDataStructure = "]";
        private const string Id_ParameterGroup = "{";
        private const string Id_EndOfParameterGroup = "}";

        private const string ColumnName_Partner = "PartnerID";
        private const string ColumnName_Date = "Date";
        private const string ColumnName_Time = "Time";
        
        
        public void Convert(ref ColumnInfo columnInfo, string serviceName, string fileLine, FileImporter fileImporter)
        {

            SetDynamicDataStructureNames(fileLine, fileImporter);

            SetParameterGroups(fileLine, fileImporter);
            
            AddParameter(columnInfo, fileLine, fileImporter, serviceName);

            ClearParameterGroups(fileLine, fileImporter);

            ClearDynamicDataStructureNames(fileLine, fileImporter);
            
            ProccessIfEndOfService(columnInfo, serviceName, fileLine, fileImporter);

            columnInfo = AddIfService(columnInfo, fileLine, serviceName);
            
        }

        private void ProccessIfEndOfService(ColumnInfo columnInfo, string serviceName, string fileLine, FileImporter fileImporter)
        {
            if (LineContains(fileLine, Id_EndOfServiceCall)) {
                AddDateTimeToColumnInfo(columnInfo, fileLine);
                columnInfo.AddColumnInfo(ColumnName_Partner, fileImporter.partnerName);
                FilePathImporter.CallLogInfo.AddColumnInfo(ColumnName_Partner, fileImporter.partnerName);
                //add the column info at the end of each service call
                Program.perServiceData[serviceName].Add(columnInfo);
                Program.callLogData[Program.CallLogName].Add(FilePathImporter.CallLogInfo);
            }
        }

        private static void AddDateTimeToColumnInfo(ColumnInfo columnInfo, string fileLine)
        {
            //Gather time stamp convert to PSTime and add to column info with the column names of Date and Time
            string temp = fileLine.Replace('(', ' ').Replace(')', ' ').Replace(',', ' ').Trim();
            string date = temp.Split()[0];
            string time = temp.Split()[1];
            ConvertFromESTToPST(date, time, out date, out time);
            columnInfo.AddColumnInfo(ColumnName_Date, date);
            columnInfo.AddColumnInfo(ColumnName_Time, time);
            FilePathImporter.CallLogInfo.AddColumnInfo(ColumnName_Date, date);
            FilePathImporter.CallLogInfo.AddColumnInfo(ColumnName_Time, time);
        }

        private static void ConvertFromESTToPST(string date, string time, out string outDate, out string outTime)
        {
            string[] _date = date.Trim().Split('-');
            if (_date.Count() == 3) {
                DateTime dateT = new DateTime(Int32.Parse(_date[0].Trim()), Int32.Parse(_date[1].Trim()), Int32.Parse(_date[2].Trim()));
                string[] _time = time.Trim().Split(':');
                int hour;
                if ((hour = Int32.Parse(_time[0].Trim()) - 3) < 0) {
                    hour += 24;
                    dateT.AddDays(-1);
                }
                outTime = hour.ToString("00") + ":" + _time[1].Trim() + ":" + _time[2].Trim();
                outDate = dateT.ToString("yyyy-MM-dd");
            } else {
                outTime = time;
                outDate = date;
            }
        }

        private static void ClearDynamicDataStructureNames(string fileLine, FileImporter fileImporter)
        {
            if (LineContains(fileLine, Id_EndOfDynamicDataStructure)) {
                fileImporter.lineInfo.dynamicDataStructureName = "";
            }
        }

        private static void ClearParameterGroups(string fileLine, FileImporter fileImporter)
        {
            if (LineContains(fileLine, Id_EndOfParameterGroup)) {
                if (!fileImporter.lineInfo.dynamicDataStructureName.Equals("")) {
                    //clear the dynamic data structure sub group id if the structures name is defined
                    fileImporter.lineInfo.dynamicDataStructureSubGroupID = "";
                } else {
                    //if the dynamic data structure name is not set then clear the parameter group name
                    fileImporter.lineInfo.parameterGroupName = "";
                }
            }
        }

        private void AddParameter(ColumnInfo columnInfo, string fileLine, FileImporter fileImporter, string serviceName)
        {
            if (LineContains(fileLine, Id_Parameter)) {
                string[] nameValue = fileLine.Split(Id_Parameter.ToCharArray());
                //build the column name. eg. parameterGroupName_DynamicDataStructureName_DynamicDataStructureSubGroupID_parameterName
                string parameterGroupName = BuildColumnPrefix(fileImporter.lineInfo.parameterGroupName);
                string dynamicDataStructureName = BuildColumnPrefix(fileImporter.lineInfo.dynamicDataStructureName);
                string dynamicDataStructureSubGroupID = BuildColumnPrefix(fileImporter.lineInfo.dynamicDataStructureSubGroupID);
                string columnName =  parameterGroupName + dynamicDataStructureName + dynamicDataStructureSubGroupID + nameValue[0];
                //get the parameter value from the line
                string columnValue = nameValue[1];
                //add the columnName and Value to the columnInfo class
                columnInfo.AddColumnInfo(columnName, columnValue);
                columnName = BuildColumnPrefix(serviceName) + columnName;
                AddColumnUsageStatistics(columnName, columnValue);
            }
        }

        private static void AddColumnUsageStatistics(string columnName, string columnValue)
        {
            if (!Program.columnNullCount.ContainsKey(columnName)) {
                Program.columnNullCount.Add(columnName, 0);
            }
            if (!Program.columnEmptyCount.ContainsKey(columnName)) {
                Program.columnEmptyCount.Add(columnName, 0);
            }
            if (!Program.columnSeenCount.ContainsKey(columnName)) {
                Program.columnSeenCount.Add(columnName, 0);
            }

            Program.columnSeenCount[columnName]++;

            if (columnValue.ToLower().Equals("null")) {
                Program.columnNullCount[columnName]++;
            }
            if(columnValue == null || columnValue.Equals("")) {
                Program.columnEmptyCount[columnName]++;
            }

        }

        private static void SetDynamicDataStructureNames(string fileLine, FileImporter fileImporter)
        {
            if (LineContains(fileLine, Id_DynamicDataStructure)) {
                //set the dynamic data structure name
                fileImporter.lineInfo.dynamicDataStructureName = fileLine.Replace(Id_DynamicDataStructure, "");
            }
        }

        private static void SetParameterGroups(string fileLine, FileImporter fileImporter)
        {
            if (LineContains(fileLine, Id_ParameterGroup)) {
                if (!fileImporter.lineInfo.dynamicDataStructureName.Equals("")) {
                    //set the dynamic data structure sub group id if the structures name is defined
                    fileImporter.lineInfo.dynamicDataStructureSubGroupID = fileImporter.lineInfo.previousLine;
                } else {
                    //if the dynamic data structure name is not set then set the parameter group name
                    fileImporter.lineInfo.parameterGroupName = fileImporter.lineInfo.previousLine;
                }
            }
        }

        private ColumnInfo AddIfService(ColumnInfo columnInfo, string fileLine, string serviceName)
        {
            if (LineContains(fileLine, Id_ServiceCall)) {
                // create new instance of Column info for each service call
                columnInfo = null;
                columnInfo = new ColumnInfo();
                FilePathImporter.CallLogInfo = new ColumnInfo();
                FilePathImporter.CallLogInfo.AddColumnInfo("Service Name", serviceName);
            }

            return columnInfo;
        }

        private static string BuildColumnPrefix(string str)
        {
            if (!str.Equals("")) {
                str += "_";
            }
            return str;
        }

        private static bool LineContains(string fileLine, string value)
        {
            return fileLine.Contains(value);
        }
    }
}
