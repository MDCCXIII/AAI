using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAI_Log_Converter.Import
{
    class LineInfo
    {
       
        public string previousLine = ""; // always populate after first line of file
        public string dynamicDataStructureName = ""; // populate when '[' is found empty when ']' is found
        public string dynamicDataStructureSubGroupID = ""; //if the dynamicDataStructureName has value then set this for the parameter group if identified.
        public string parameterGroupName = ""; //populate when '{' is found empty when '}' is found

        private Dictionary<string, int> columnNullCount = new Dictionary<string, int>();
        private Dictionary<string, int> columnEmptyCount = new Dictionary<string, int>();
        private Dictionary<string, int> columnSeenCount = new Dictionary<string, int>();
    }
}
