using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Models.Datas
{
    internal class JsonData
    {
        public JsonData(Data data)
        {
            this.data = data;
        }
        public Data data { get; set; }
    }
}
