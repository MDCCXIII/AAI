using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAI_Log_Converter
{
    class BigList
    {
        public List<List<ColumnInfo>> master = new List<List<ColumnInfo>>();
        private int currentList = 0;
        private int maxIndex = 10000000;

        public BigList()
        {
            master.Add(new List<ColumnInfo>());
        }

        public void Add(ColumnInfo columnInfo)
        {
            if(master[currentList].Count == 100000) {
                
                master.Add(new List<ColumnInfo>());
                currentList++;
                
            }
            master[currentList].Add(columnInfo);
        }

        public long Count()
        {
            long result = 0;
            foreach(List<ColumnInfo> l in master) {
                result += l.Count();
            }
            return result;
        }

        public List<ColumnInfo> Get(long index)
        {
            return master[(int)(index / maxIndex)];
        }
    }
}
