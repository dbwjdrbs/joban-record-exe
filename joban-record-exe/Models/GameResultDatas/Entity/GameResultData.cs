using joban_record_exe.Models.GameDatas.Entity;
using JobanRecordApp.Models.Members.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Models.GameResultDatas.Entity
{
    internal class GameResultData
    {
        public GameResultData() { }
        public GameResultData(Member member, GameData gameData, int isWin, int troop, int building, int troopLoss, int kill, int buildingLoss, int destroy, int tree, int grain, int teamNumber, int isHuman, int playerNumber, string lobbyNation, string inGameNation)
        {
            this.member = member;
            this.gameData = gameData;
            this.isWin = isWin;
            this.troop = troop;
            this.building = building;
            this.troopLoss = troopLoss;
            this.kill = kill;
            this.buildingLoss = buildingLoss;
            this.destroy = destroy;
            this.tree = tree;
            this.grain = grain;
            this.teamNumber = teamNumber;
            this.isHuman = isHuman;
            this.playerNumber = playerNumber;
            this.lobbyNation = lobbyNation;
            this.inGameNation = inGameNation;
        }

        public Member member { get; set; }
        public GameData gameData { get; set; } // 마지막 메모리 값 분석 후 할당.
        // ResultData
        public int isWin { get; set; }
        public int troop { get; set; }
        public int building { get; set; }
        public int troopLoss { get; set; }
        public int kill { get; set; }
        public int buildingLoss { get; set; }
        public int destroy { get; set; }
        public int tree { get; set; }
        public int grain { get; set; }

        // LobbyData
        public int teamNumber { get; set; }
        public int isHuman { get; set; }
        public int playerNumber { get; set; }
        public string lobbyNation { get; set; }

        public string inGameNation { get; set; } // inGameData 후 추출해야함.
    }
}
