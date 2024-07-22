using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Models.GameDatas.Entity
{
    internal class LobbyData
    {
        public LobbyData(string teamNumber, string lobbyNation, string isHuman, string playerNumber)
        {
            this.teamNumber = teamNumber;
            this.lobbyNation = lobbyNation;
            this.isHuman = isHuman;
            this.playerNumber = playerNumber;
        }

        public string teamNumber { get; set; }
        public string lobbyNation { get; set; } 
        public string isHuman { get; set; } 
        public string playerNumber { get; set; }
    }
}
