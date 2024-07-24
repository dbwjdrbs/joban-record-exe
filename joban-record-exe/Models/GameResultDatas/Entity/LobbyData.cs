using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Models.GameResultDatas.Entity
{
    internal class LobbyData
    {
        public int teamNumber { get; set; }
        public int isHuman { get; set; }
        public int playerNumber { get; set; }
        public string lobbyNation { get; set; }

        public LobbyData(int teamNumber, int isHuman, int playerNumber, string lobbyNation)
        {
            this.teamNumber = teamNumber;
            this.isHuman = isHuman;
            this.playerNumber = playerNumber;
            this.lobbyNation = lobbyNation;
        }
    }
}
