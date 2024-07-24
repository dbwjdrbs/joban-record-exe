using joban_record_exe.Models.GameResultDatas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Models.GameDatas.Entity
{
    internal class GameDataDto
    {
        public class Post
        {
            public string player { get; set; }
            public string gameVersion { get; set; }
            public string mapName { get; set; } // 다른 영역의 메모리.

            public Post(string player, string gameVersion, string mapName)
            {
                this.player = player;
                this.gameVersion = gameVersion;
                this.mapName = mapName;
            }

            public Post() { }
        }

        public class Patch
        {
            public long gameDataId { get; set; }
            public GameResultDataDto gameResultDataDto { get; set; }
            public int playTime { get; set; }
        }
    }
}
