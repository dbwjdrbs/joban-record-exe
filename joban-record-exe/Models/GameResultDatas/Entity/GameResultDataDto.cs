using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Models.GameResultDatas.Entity
{
    internal class GameResultDataDto
    {
        public long memberId;
        public int isWin; // 마지막 메모리 값 분석 후 할당.
        // ResultData
        public int troop;
        public int building;
        public int troopLoss;
        public int kill;
        public int buildingLoss;
        public int destroy;
        public int tree;
        public int grain;

        // LobbyData
        public int teamNumber;
        public int isHuman;
        public int playerNumber;
        public string lobbyNation;

        public string inGameNation; // inGameData 후 추출해야함.
    }
}
