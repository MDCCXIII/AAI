using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAI_Log_Converter
{
    class ColumnInfo
    {
        public Dictionary<string, string> ColumnValues = new Dictionary<string, string>();

        public void AddColumnInfo(string columnName, string value)
        {
            AddColumnValue(columnName, value);
        }

        public void AddColumnValue(string columnName, string value)
        {
            if (!ColumnValues.ContainsKey(columnName)) {
                ColumnValues.Add(columnName, value);
            }
        }
    }
}
