using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace AAI_Log_Converter.Import
{
    class ColumnImporter
    {
        private const string Id_ServiceCall = "(";
        private const string Id_ParameterGroup = "{";
        private const string Id_DynamicDataStructure = "[";
        private const string Id_Parameter = "=";
        private const string Id_EndOfDynamicDataStructure = "]";
        private const string Id_EndOfParameterGroup = "}";
        private const string Id_EndOfServiceCall = "),";

        public LineInfo lineInfo;

        public ColumnImporter()
        {
            lineInfo = new LineInfo();
        }

        internal void ImportColumns(string serviceName, string fileLine, FileImporter fileImporter)
        {
            
            SetDynamicDataStructureNames(fileLine);
            SetParameterGroups(fileLine);
            BuildColumnHeaders(fileLine, serviceName);
            ClearDynamicDataStructureNames(fileLine);
            ClearParameterGroups(fileLine);
            lineInfo.previousLine = fileLine;
        }

        internal bool ImportColumnValues(string serviceName, string fileLine, FileImporter fileImporter)
        {
            
            SetDynamicDataStructureNames(fileLine);
            SetParameterGroups(fileLine);
            GetColumnValues(fileLine, serviceName);
            ClearDynamicDataStructureNames(fileLine);
            ClearParameterGroups(fileLine);
            lineInfo.previousLine = fileLine;
            return ProccessIfEndOfService(serviceName, fileLine, fileImporter);
        }

        internal void ClearIfNewService(string fileLine, string serviceName)
        {
            if (fileLine.Contains(Id_ServiceCall))
            {
                // Default column values to N/A for each new service record.
                for(int i = 0; i<Program.serviceColumns[serviceName].Count; i++)
                {
                    Program.serviceColumns[serviceName][i] = "N/A";
                }
            }
        }

        private void SetDynamicDataStructureNames(string fileLine)
        {
            if (fileLine.Contains(Id_DynamicDataStructure))
            {
                //set the dynamic data structure name
                lineInfo.dynamicDataStructureName.Add(fileLine.Replace(Id_DynamicDataStructure, ""));
            }
        }

        private void SetParameterGroups(string fileLine)
        {
            if (fileLine.Contains(Id_ParameterGroup))
            {
                if (!lineInfo.dynamicDataStructureName.Equals(""))
                {
                    //set the dynamic data structure sub group id if the structures name is defined
                    lineInfo.dynamicDataStructureSubGroupID.Add(lineInfo.previousLine);
                }
                else
                {
                    //if the dynamic data structure name is not set then set the parameter group name
                    lineInfo.parameterGroupName.Add(lineInfo.previousLine);
                }
            }
        }

        private void ClearParameterGroups(string fileLine)
        {
            if (fileLine.Contains(Id_EndOfParameterGroup))
            {
                if (!lineInfo.dynamicDataStructureName.Equals(""))
                {
                    //clear the dynamic data structure sub group id if the structures name is defined
                    if(lineInfo.dynamicDataStructureSubGroupID.Count > 0)
                    {
                        lineInfo.dynamicDataStructureSubGroupID.RemoveAt(lineInfo.dynamicDataStructureSubGroupID.Count - 1);
                    }
                }
                else
                {
                    //if the dynamic data structure name is not set then clear the parameter group name
                    if(lineInfo.parameterGroupName.Count > 0)
                    {
                        lineInfo.parameterGroupName.RemoveAt(lineInfo.parameterGroupName.Count - 1);
                    }
                }
            }
        }

        private void ClearDynamicDataStructureNames(string fileLine)
        {
            if (fileLine.Contains(Id_EndOfDynamicDataStructure))
            {
                lineInfo.dynamicDataStructureName.RemoveAt(lineInfo.dynamicDataStructureName.Count - 1);
            }
        }

        private void BuildColumnHeaders(string fileLine, string serviceName)
        {
            if (fileLine.Contains(Id_Parameter))
            {
                string parameterName = fileLine.Split(Id_Parameter.ToCharArray())[0];
                //string value = fileLine.Split(Id_Parameter.ToCharArray())[1];
                //build the column name. eg. parameterGroupName_DynamicDataStructureName_DynamicDataStructureSubGroupID_parameterName
                string parameterGroupName = BuildColumnPrefix(lineInfo.parameterGroupName);
                string dynamicDataStructureName = BuildColumnPrefix(lineInfo.dynamicDataStructureName);
                string dynamicDataStructureSubGroupID = BuildColumnPrefix(lineInfo.dynamicDataStructureSubGroupID);
                string columnName = parameterGroupName + dynamicDataStructureName + dynamicDataStructureSubGroupID + parameterName;

                //add the columnName to the serviceNames class
                AddColumnForService(serviceName, columnName);
                AddColumnUsageStatistics(serviceName, columnName);
            }
        }

        private void GetColumnValues(string fileLine, string serviceName)
        {
            if (fileLine.Contains(Id_Parameter))
            {
                string parameterName = fileLine.Split(Id_Parameter.ToCharArray())[0];
                string value = fileLine.Split(Id_Parameter.ToCharArray())[1];
                //build the column name. eg. parameterGroupName_DynamicDataStructureName_DynamicDataStructureSubGroupID_parameterName
                string parameterGroupName = BuildColumnPrefix(lineInfo.parameterGroupName);
                string dynamicDataStructureName = BuildColumnPrefix(lineInfo.dynamicDataStructureName);
                string dynamicDataStructureSubGroupID = BuildColumnPrefix(lineInfo.dynamicDataStructureSubGroupID);
                string columnName = parameterGroupName + dynamicDataStructureName + dynamicDataStructureSubGroupID + parameterName;

                //add the columnName to the serviceNames class
                Program.serviceColumns[serviceName][columnName] = value;
                AddUsageCounts(serviceName, columnName, value);
            }
        }

        private int lastKnownHeaderIndex = -1;
        public void AddColumnForService(string serviceName, string columnName)
        {
            if (!Program.serviceColumns[serviceName].Contains(columnName))
            {
                Program.serviceColumns[serviceName].Insert(lastKnownHeaderIndex + 1, columnName, "N/A");
            }
            lastKnownHeaderIndex = GetIndex(Program.serviceColumns[serviceName], columnName);
        }

        public static int GetIndex(OrderedDictionary dictionary, string columnName)
        {
            int index = -1;
            foreach(string key in dictionary.Keys) {
                index++;
                if (key.Equals(columnName))
                {
                    return index; // We found the item
                }
            }
            //last item in the dictionary
            return index;
        }

        private static void AddColumnUsageStatistics(string serviceName, string columnName)
        {
            if (!Program.columnNullCount[serviceName].ContainsKey(columnName))
            {
                Program.columnNullCount[serviceName].Add(columnName, 0);
            }
            if (!Program.columnEmptyCount[serviceName].ContainsKey(columnName))
            {
                Program.columnEmptyCount[serviceName].Add(columnName, 0);
            }
            if (!Program.columnSeenCount[serviceName].ContainsKey(columnName))
            {
                Program.columnSeenCount[serviceName].Add(columnName, 0);
            }
        }

        private static void AddUsageCounts(string serviceName, string columnName, string columnValue)
        {
            Program.columnSeenCount[serviceName][columnName]++;

            if (columnValue.ToLower().Equals("null"))
            {
                Program.columnNullCount[serviceName][columnName]++;
            }
            if (columnValue == null || columnValue.Equals(""))
            {
                Program.columnEmptyCount[serviceName][columnName]++;
            }
        }

        private static string BuildColumnPrefix(string str)
        {
            if (!str.Equals(""))
            {
                str += "_";
            }
            return str;
        }

        private static string BuildColumnPrefix(List<string> values)
        {
            string result = "";
            foreach(string str in values)
            {
                if (!str.Equals(""))
                {
                    result += str + "_";
                }
            }
            
            return result;
        }

        private bool ProccessIfEndOfService(string serviceName, string fileLine, FileImporter fileImporter)
        {
            bool result = false;
            if (fileLine.Contains(Id_EndOfServiceCall))
            {
                result = true;
                AddDateTimeToColumnInfo(serviceName, fileLine);
                Program.serviceColumns[serviceName][Program.Header_PartnerID] = fileImporter.partnerName;
                Program.serviceColumns[serviceName][Program.Header_ServiceName] = serviceName;
            }
            return result;
        }

        private static void AddDateTimeToColumnInfo(string serviceName, string fileLine)
        {
            //Gather time stamp convert to PSTime and add to column info with the column names of Date and Time
            string temp = fileLine.Replace('(', ' ').Replace(')', ' ').Replace(',', ' ').Trim();
            string date = temp.Split()[0];
            string time = temp.Split()[1];
            ConvertFromESTToPST(date, time, out date, out time);
            Program.serviceColumns[serviceName][Program.Header_Date] = date;
            Program.serviceColumns[serviceName][Program.Header_Time] = time;
        }

        private static void ConvertFromESTToPST(string date, string time, out string outDate, out string outTime)
        {
            string[] _date = date.Trim().Split('-');
            if (_date.Count() == 3)
            {
                DateTime dateT = new DateTime(Int32.Parse(_date[0].Trim()), Int32.Parse(_date[1].Trim()), Int32.Parse(_date[2].Trim()));
                string[] _time = time.Trim().Split(':');
                int hour;
                if ((hour = Int32.Parse(_time[0].Trim()) - 3) < 0)
                {
                    hour += 24;
                    dateT.AddDays(-1);
                }
                outTime = hour.ToString("00") + ":" + _time[1].Trim() + ":" + _time[2].Trim();
                outDate = dateT.ToString("yyyy-MM-dd");
            }
            else
            {
                outTime = time;
                outDate = date;
            }
        }
    }
}
