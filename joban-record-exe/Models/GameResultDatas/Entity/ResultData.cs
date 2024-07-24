using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Models.GameResultDatas.Entity
{
    internal class ResultData
    {
        public int troop { get; set; }
        public int building { get; set; }
        public int troopLoss { get; set; }
        public int kill { get; set; }
        public int buildingLoss { get; set; }
        public int destroy { get; set; }
        public int tree { get; set; }
        public int grain { get; set; }

        public ResultData(int troop, int building, int troopLoss, int kill, int buildingLoss, int destroy, int tree, int grain)
        {
            this.troop = troop;
            this.building = building;
            this.troopLoss = troopLoss;
            this.kill = kill;
            this.buildingLoss = buildingLoss;
            this.destroy = destroy;
            this.tree = tree;
            this.grain = grain;
        }
    }
}
