using joban_record_exe.Models.GameResultDatas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Models.GameResultDatas.Builder
{
    internal class GameResultDataBuilder
    {
        private GameResultData gameResultData;

        public string gameId;
        public long memberId;

        public string isWin;
        public int troop;
        public int building;
        public int troopLoss;
        public int kill;
        public int buildingLoss;
        public int destroy;
        public int tree;
        public int grain;

        public GameResultDataBuilder() { }

        public GameResultDataBuilder SetGameId(string value)
        {
            gameResultData.gameId = value;
            return this;
        }

        public GameResultDataBuilder SetMemberId(long value)
        {
            gameResultData.memberId = value;
            return this;
        }

        public GameResultDataBuilder SetIsWin(string value)
        {
            gameResultData.isWin = value;
            return this;
        }

        public GameResultDataBuilder SetTroop(int value)
        {
            gameResultData.troop = value;
            return this;
        }

        public GameResultDataBuilder SetBuilding(int value)
        {
            gameResultData.building = value;
            return this;
        }

        public GameResultDataBuilder SetTroopLoss(int value)
        {
            gameResultData.troopLoss = value;
            return this;
        }

        public GameResultDataBuilder SetKill(int value)
        {
            gameResultData.kill = value;
            return this;
        }

        public GameResultDataBuilder SetBuildingLoss(int value)
        {
            gameResultData.buildingLoss = value;
            return this;
        }

        public GameResultDataBuilder SetDestroy(int value)
        {
            gameResultData.destroy = value;
            return this;
        }

        public GameResultDataBuilder SetTree(int value)
        {
            gameResultData.tree = value;
            return this;
        }

        public GameResultDataBuilder SetGrain(int value)
        {
            gameResultData.grain = value;
            return this;
        }

        public GameResultData Build()
        {
            return new GameResultData(gameId, memberId, isWin, troop, building, troopLoss, kill, buildingLoss, destroy, tree, grain);
        }
    }
}
