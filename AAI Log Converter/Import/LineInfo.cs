using System.Collections.Generic;

namespace AAI_Log_Converter.Import
{
    class LineInfo
    {

        /// <summary>   always populate for each line after the first line of file. </summary>
        public string previousLine = "";

        public string FullColumnName = "";

        /// <summary>   push when '[' is found pop when ']' is found. </summary>
        public List<string> dynamicDataStructureName = new List<string>();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     if the dynamicDataStructureName has value then push when '{' is found pop when '}' is found
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<string> dynamicDataStructureSubGroupID = new List<string>();

        /// <summary>   push when '{' is found pop when '}' is found. </summary>
        public List<string> parameterGroupName = new List<string>();
    }
}
