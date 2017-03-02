using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace AAI_Log_Converter.Import
{
    class ColumnImporter
    {
        /// <summary>   The new service call identifier. </summary>
        private const string Id_ServiceCall = "(";

        /// <summary>   The parameter group  identifier. </summary>
        private const string Id_ParameterGroup = "{";
        
        /// <summary>   The parameter array group identifier. </summary>
        private const string Id_ParameterArrayGroup = "[";

        /// <summary>   The parameter identifier. </summary>
        private const string Id_Parameter = "=";

        /// <summary>   The end of parameter array group identifier. </summary>
        private const string Id_ParameterArrayGroupEnd = "]";

        /// <summary>   The end of parameter group identifier. </summary>
        private const string Id_ParameterGroupEnd = "}";

        /// <summary>   The end of service call identifier. </summary>
        private const string Id_EndOfServiceCall = "),";


        /// <summary>   Information describing the line. </summary>
        public LineInfo lineInfo;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ColumnImporter()
        {
            lineInfo = new LineInfo();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Import column names. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="serviceName">  Name of the service. </param>
        /// <param name="fileLine">     The file line. </param>
        /// <param name="fileImporter"> The parent class instance. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal void ImportColumnNames(string serviceName, string fileLine, FileImporter fileImporter)
        {
            PushParameterArrayGroupName(fileLine);
            PushParameterGroupName(fileLine);
            BuildColumnHeaders(fileLine, serviceName);
            PopParameterArrayGroupName(fileLine);
            PopParameterGroupName(fileLine);
            lineInfo.previousLine = fileLine;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Import column values. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="serviceName">  Name of the service. </param>
        /// <param name="fileLine">     The file line. </param>
        /// <param name="fileImporter"> The parent class instance. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal bool ImportColumnValues(string serviceName, string fileLine, FileImporter fileImporter)
        {
            PushParameterArrayGroupName(fileLine);
            PushParameterGroupName(fileLine);
            GetColumnValues(fileLine, serviceName);
            PopParameterArrayGroupName(fileLine);
            PopParameterGroupName(fileLine);
            lineInfo.previousLine = fileLine;
            return ProccessServiceCallEnd(serviceName, fileLine, fileImporter);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets the value of all columns to N/A when looking at a new service. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="fileLine">     The file line. </param>
        /// <param name="serviceName">  Name of the service. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal void ClearIfNewService(string fileLine, string serviceName)
        {
            if (fileLine.ToUpper().Contains(serviceName.ToUpper()) && fileLine.Contains(Id_ServiceCall))
            {
                // Default column values to N/A for each new service record.
                for(int i = 0; i < Program.serviceColumns[serviceName].Count; i++)
                {
                    Program.serviceColumns[serviceName][i] = "N/A";
                }
                //lineInfo = new LineInfo();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Pushes a parameter array group name. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="fileLine"> The file line. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void PushParameterArrayGroupName(string fileLine)
        {
            if (fileLine.Contains(Id_ParameterArrayGroup))
            {
                //set the dynamic data structure name
                AppendFullColumnName(fileLine.Replace(Id_ParameterArrayGroup, ""));
                lineInfo.dynamicDataStructureName.Add(fileLine.Replace(Id_ParameterArrayGroup, ""));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Pushes a parameter group name. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="fileLine"> The file line. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void PushParameterGroupName(string fileLine)
        {
            if (fileLine.Contains(Id_ParameterGroup))
            {
                if (!lineInfo.dynamicDataStructureName.Equals(""))
                {
                    //set the dynamic data structure sub group id if the structures name is defined
                    AppendFullColumnName(lineInfo.previousLine);
                    lineInfo.dynamicDataStructureSubGroupID.Add(lineInfo.previousLine);
                }
                else
                {
                    //if the dynamic data structure name is not set then set the parameter group name
                    AppendFullColumnName(lineInfo.previousLine);
                    lineInfo.parameterGroupName.Add(lineInfo.previousLine);
                }
            }
        }

        private void AppendFullColumnName(string line)
        {
            if (lineInfo.FullColumnName.Equals(""))
            {
                lineInfo.FullColumnName += line;
            }
            else
            {
                lineInfo.FullColumnName += "_" + line;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Pops a parameter group name. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="fileLine"> The file line. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void PopParameterGroupName(string fileLine)
        {
            if (fileLine.Contains(Id_ParameterGroupEnd))
            {
                if (!lineInfo.dynamicDataStructureName.Equals(""))
                {
                    //clear the dynamic data structure sub group id if the structures name is defined
                    if(lineInfo.dynamicDataStructureSubGroupID.Count > 0)
                    {
                        int subStart = lineInfo.FullColumnName.LastIndexOf(lineInfo.dynamicDataStructureSubGroupID[lineInfo.dynamicDataStructureSubGroupID.Count - 1]);
                        int subLength = lineInfo.dynamicDataStructureSubGroupID[lineInfo.dynamicDataStructureSubGroupID.Count - 1].Length;
                        if (subStart > -1)
                        {
                            lineInfo.FullColumnName = lineInfo.FullColumnName.Remove(subStart, subLength).Replace("__", "_").Trim('_');
                        }
                        lineInfo.dynamicDataStructureSubGroupID.RemoveAt(lineInfo.dynamicDataStructureSubGroupID.Count - 1);
                    }
                }
                else
                {
                    //if the dynamic data structure name is not set then clear the parameter group name
                    if(lineInfo.parameterGroupName.Count > 0)
                    {
                        int subStart = lineInfo.FullColumnName.LastIndexOf(lineInfo.parameterGroupName[lineInfo.parameterGroupName.Count - 1]);
                        int subLength = lineInfo.parameterGroupName[lineInfo.parameterGroupName.Count - 1].Length;
                        if (subStart > -1)
                        {
                            lineInfo.FullColumnName = lineInfo.FullColumnName.Remove(subStart, subLength).Replace("__", "_").Trim('_');
                        }
                        lineInfo.parameterGroupName.RemoveAt(lineInfo.parameterGroupName.Count - 1);
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Pops a parameter array group name. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="fileLine"> The file line. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void PopParameterArrayGroupName(string fileLine)
        {
            if (fileLine.Contains(Id_ParameterArrayGroupEnd))
            {
                int subStart = lineInfo.FullColumnName.LastIndexOf(lineInfo.dynamicDataStructureName[lineInfo.dynamicDataStructureName.Count - 1]);
                int subLength = lineInfo.dynamicDataStructureName[lineInfo.dynamicDataStructureName.Count - 1].Length;
                if (subStart > -1)
                {
                    lineInfo.FullColumnName = lineInfo.FullColumnName.Remove(subStart, subLength).Replace("__", "_").Trim('_');
                }
                lineInfo.dynamicDataStructureName.RemoveAt(lineInfo.dynamicDataStructureName.Count - 1);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Builds column headers from parameter information. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="fileLine">     The file line. </param>
        /// <param name="serviceName">  Name of the service. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BuildColumnHeaders(string fileLine, string serviceName)
        {
            if (fileLine.Contains(Id_Parameter))
            {
                string columnName = BuildColumnName(fileLine);

                //add the columnName to the serviceNames class
                AddColumnForService(serviceName, columnName);
                AddColumnUsageStatistics(serviceName, columnName);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        ///     Builds column names. 
        ///     eg. 
        ///     parameterGroupName_DynamicDataStructureName_DynamicDataStructureSubGroupID_parameterName. 
        ///</summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="fileLine"> The file line. </param>
        ///
        /// <returns>   A string. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private string BuildColumnName(string fileLine)
        {
            string parameterName = fileLine.Split(Id_Parameter.ToCharArray())[0];
            //string parameterGroupName = BuildColumnHeaderPrefix(lineInfo.parameterGroupName);
            //string dynamicDataStructureName = BuildColumnHeaderPrefix(lineInfo.dynamicDataStructureName);
            //string dynamicDataStructureSubGroupID = BuildColumnHeaderPrefix(lineInfo.dynamicDataStructureSubGroupID);
            if (lineInfo.FullColumnName.Equals(""))
            {
                return parameterName;
            }
            else
            {
                return lineInfo.FullColumnName + "_" + parameterName;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets column values. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="fileLine">     The file line. </param>
        /// <param name="serviceName">  Name of the service. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void GetColumnValues(string fileLine, string serviceName)
        {
            if (fileLine.Contains(Id_Parameter))
            {
                string columnName = BuildColumnName(fileLine);
                string value = fileLine.Split(Id_Parameter.ToCharArray())[1];

                //add the columnName to the serviceNames class
                Program.serviceColumns[serviceName][columnName] = value;
                AddUsageCounts(serviceName, columnName, value);
            }
        }


        /// <summary>   Zero-based index of the last known header. </summary>
        private int lastKnownHeaderIndex = -1;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        ///     Adds a column for service at 'columnName' initializing the value to N/A. 
        ///     Sets the index of the column as the lastKnownHeaderIndex.         
        ///</summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="serviceName">  Name of the service. </param>
        /// <param name="columnName">   Name of the column. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddColumnForService(string serviceName, string columnName)
        {
            if (!Program.serviceColumns[serviceName].Contains(columnName))
            {
                Program.serviceColumns[serviceName].Insert(lastKnownHeaderIndex + 1, columnName, "N/A");
            }
            lastKnownHeaderIndex = GetIndex(Program.serviceColumns[serviceName], columnName);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the index of key in an OrderedDictionary. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="dictionary">   The dictionary. </param>
        /// <param name="keyName">   Name of the column. </param>
        ///
        /// <returns>   The index of the key if found or the last index of the OrderedDictionary. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static int GetIndex(OrderedDictionary dictionary, string keyName)
        {
            int index = -1;
            foreach(string key in dictionary.Keys) {
                index++;
                if (key.Equals(keyName))
                {
                    break; // We found the key in the dictionary
                }
            }
            
            return index;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds a column usage statistic to 'serviceName'. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="serviceName">  Name of the service. </param>
        /// <param name="columnName">   Name of the column. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds usage counts determined by the 'columnValue'. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="serviceName">  Name of the service. </param>
        /// <param name="columnName">   Name of the column. </param>
        /// <param name="columnValue">  The column value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Builds column header prefix. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="values">   The values. </param>
        ///
        /// <returns>   A string. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static string BuildColumnHeaderPrefix(List<string> values)
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Proccess service call end. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="serviceName">  Name of the service. </param>
        /// <param name="fileLine">     The file line. </param>
        /// <param name="fileImporter"> The parent class instance. </param>
        ///
        /// <returns>   True if end of service, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool ProccessServiceCallEnd(string serviceName, string fileLine, FileImporter fileImporter)
        {
            bool result = false;
            if (fileLine.StartsWith(Id_EndOfServiceCall))
            {
                result = true;
                AddDateTimeToColumnInfo(serviceName, fileLine);
                Program.serviceColumns[serviceName][Program.Header_PartnerID] = fileImporter.partnerName;
                Program.serviceColumns[serviceName][Program.Header_ServiceName] = serviceName;
            }
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds a date time to column information to 'Program.serviceColumns'. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="serviceName">  Name of the service. </param>
        /// <param name="fileLine">     The file line. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Convert from Eastern Standard Time to Pacific Standard Time. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="date">     The date. </param>
        /// <param name="time">     The time. </param>
        /// <param name="outDate">  [out] The out date. </param>
        /// <param name="outTime">  [out] The out time. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
