using System.Collections;
using System.Text;

namespace AAI_Log_Converter.Export
{
    internal class DataSheetBuilder
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
            foreach (DictionaryEntry column in Program.serviceColumns[service])
            {
                if (Program.Header_PartnerID.Equals(column.Key))
                {
                    csv.Append("\n" + column.Value);
                }
                else if (!Program.Header_ServiceName.Equals(column.Key))
                {
                    csv.Append("," + column.Value);
                }
                
            }
            FileUtils.WriteToFile(service + "_Data.csv", csv);
            csv.Clear();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Writes the column headers. </summary>
        ///
        /// <remarks>   Ahaynes, 12/13/2016. </remarks>
        ///
        /// <param name="service">  The service. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void WriteColumnHeaders(string service)
        {
            StringBuilder csv = new StringBuilder();
            foreach (DictionaryEntry column in Program.serviceColumns[service])
            {
                if (Program.Header_PartnerID.Equals(column.Key))
                {
                    csv.Append(column.Key);
                }
                else if (!Program.Header_ServiceName.Equals(column.Key))
                {
                    csv.Append("," + column.Key);
                }

            }
            FileUtils.WriteToFile(service + "_Data.csv", csv);
            csv.Clear();
        }
    }
}