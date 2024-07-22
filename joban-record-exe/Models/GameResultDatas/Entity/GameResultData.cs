using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Models.GameResultDatas.Entity
{
    internal class GameResultData
    {

        // ResultData
        public string gameId;
        public long memberId;

        public string isWin; // 마지막 메모리 값 분석 후 할당.
        public int troop;
        public int building;
        public int troopLoss;
        public int kill;
        public int buildingLoss;
        public int destroy;
        public int tree;
        public int grain;

        public GameResultData(string gameId, long memberId, string isWin, int troop, int building, int troopLoss, int kill, int buildingLoss, int destroy, int tree, int grain)
        {
            this.gameId = gameId;
            this.memberId = memberId;
            this.isWin = isWin;
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
