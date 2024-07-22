using joban_record_exe.Models.GameDatas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Models.GameDatas.Builder
{
    internal class GameDataBuilder
    {
        public GameData gameData;

        public string gameId;
        public string playTime;
        public string gameVersion;
        // LobbyData
        public string teamNumber;
        public string lobbyNation;
        public string inGameNation; // inGameData
        public string isHuman;
        public string playerNumber;
        public string mapName;

        public GameDataBuilder() { }

        public GameDataBuilder SetGameId(string value)
        {
            gameData.gameId = value;
            return this;
        }

        public GameDataBuilder SetPlayTime(string value)
        {
            gameData.playTime = value;
            return this;
        }

        public GameDataBuilder SetGameVersion(string value)
        {
            gameData.gameVersion = value;
            return this;
        }

        public GameDataBuilder SetTeamNumber(string value)
        {
            gameData.teamNumber = value;
            return this;
        }

        public GameDataBuilder SetLobbyNation(string value)
        {
            gameData.lobbyNation = value;
            return this;
        }

        public GameDataBuilder SetInGameNation(string value)
        {
            gameData.inGameNation = value;
            return this;
        }

        public GameDataBuilder SetIsHuman(string value)
        {
            gameData.isHuman = value;
            return this;
        }

        public GameDataBuilder SetPlayerNumber(string value)
        {
            gameData.playerNumber = value;
            return this;
        }

        public GameDataBuilder SetMapName(string value)
        {
            gameData.mapName = value;
            return this;
        }

        public GameData Build()
        {
            return new GameData(gameId, playTime, gameVersion, teamNumber, lobbyNation, inGameNation, isHuman, playerNumber, mapName);
        }
        
    }
}
