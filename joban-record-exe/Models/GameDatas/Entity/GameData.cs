using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Models.GameDatas.Entity
{
    internal class GameData
    {
        public string gameId;
        public string playTime;
        public string gameVersion;
        public string mapName; // 다른 영역의 메모리.

        // LobbyData
        public string teamNumber;
        public string lobbyNation;
        public string inGameNation; // inGameData
        public string isHuman;
        public string playerNumber;

        public GameData(string gameId, string playTime, string gameVersion, string teamNumber, string lobbyNation, string inGameNation, string isHuman, string playerNumber, string mapName)
        {
            this.gameId = gameId;
            this.playTime = playTime;
            this.gameVersion = gameVersion;
            this.teamNumber = teamNumber;
            this.lobbyNation = lobbyNation;
            this.inGameNation = inGameNation;
            this.isHuman = isHuman;
            this.playerNumber = playerNumber;
            this.mapName = mapName;
        }
    }
}
