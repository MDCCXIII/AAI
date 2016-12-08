using System.Collections.Generic;

namespace AAI_Log_Converter.Import
{
    class LineInfo
    {
       
        public string previousLine = ""; // always populate after first line of file
        public string dynamicDataStructureName = ""; // populate when '[' is found empty when ']' is found
        public List<string> dynamicDataStructureSubGroupID = new List<string>(); //if the dynamicDataStructureName has value then set this for the parameter group if identified.
        public List<string> parameterGroupName = new List<string>(); //populate when '{' is found empty when '}' is found
    }
}
