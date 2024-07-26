using joban_record_exe.Models.GameDatas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace joban_record_exe.Models.Datas
{
    internal class Data
    {
        public Data(long gameDataID)
        {
            this.gameDataID = gameDataID;
        }
        public long gameDataID { get; set; }
    }
}
