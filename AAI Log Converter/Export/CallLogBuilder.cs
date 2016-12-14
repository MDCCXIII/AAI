using System.Collections;
using System.Text;

namespace AAI_Log_Converter.Export
{
    class CallLogBuilder
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Appends a row to the file. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="service">  The service. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void AppendRowToFile(string service)
        {
            StringBuilder csv = new StringBuilder();
            //iterate through the call log information
            foreach(DictionaryEntry column in Program.serviceColumns[service])
            {
                if (Program.Header_ServiceName.Equals(column.Key))
                {
                    csv.Append("\n" + column.Value);
                }
                else if (Program.Header_PartnerID.Equals(column.Key) || Program.Header_Date.Equals(column.Key)) {
                    csv.Append("," + column.Value);
                }
                else if (Program.Header_Time.Equals(column.Key))
                {
                    csv.Append("," + column.Value);
                    break;
                }
            }
            FileUtils.WriteToFile(Program.CallLogName + ".csv", csv);
            csv.Clear();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Writes the column headers. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void WriteColumnHeaders()
        {
            StringBuilder csv = new StringBuilder();
            csv.Append(Program.Header_ServiceName);
            csv.Append("," + Program.Header_PartnerID);
            csv.Append("," + Program.Header_Date);
            csv.Append("," + Program.Header_Time);
                    
            FileUtils.WriteToFile(Program.CallLogName + ".csv", csv);
            csv.Clear();
        }
    }
}
