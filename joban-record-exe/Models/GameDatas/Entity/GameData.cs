using joban_record_exe.Models.GameResultDatas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace joban_record_exe.Models.GameDatas.Entity
{
    internal class GameData
    {
        public GameData() { }

        public GameData(long gameDataId)
        {
            this.gameDataId = gameDataId;
        }
        public long gameDataId { get; set; }

        public class Post
        {
            public List<string> players { get; set; }
            public string gameVersion { get; set; }
            public string mapName { get; set; } // 다른 영역의 메모리.

            public Post(List<string> players, string gameVersion, string mapName)
            {
                this.players = players;
                this.gameVersion = gameVersion;
                this.mapName = mapName;
            }

            public Post() { }
        }

        public class Patch
        {
            public long gameDataId { get; set; }
            public int playTime { get; set; }

            public Patch(long gameDataId, int playTime)
            {
                this.gameDataId = gameDataId;
                this.playTime = playTime;
            }
        }

    }
}
