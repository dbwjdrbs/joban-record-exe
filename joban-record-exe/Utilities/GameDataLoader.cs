using joban_record_exe.Models.GameDatas.Entity;
using joban_record_exe.Utilities.RestApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Utilities
{
    internal static class GameDataLoader
    {
        private static GameData.Post gameDataDto;
        private static int pid;

        public static GameData.Post GetPostGameData(int cureentPid)
        {
            pid = cureentPid;

            List<string> players = GetPlayer();
            string gameVersion = GetGameVersion();
            string mapName = GetMapName();

            gameDataDto = new GameData.Post(players, gameVersion, mapName);


            return gameDataDto;
        }

        public static GameData.Patch GetPatchGameData(int cureentPid, long gameId)
        {
            pid = cureentPid;
            int time = GetPlayTime();
            return new GameData.Patch(gameId, time);
        }
        //public static async Task<GameDataDto.Patch> GetPatchGameData

        private static int GetPlayTime()
        {
            byte[] datas = MemoryManager.ReadMemory((IntPtr)EBaseAddressList.PLAY_TIME, (int)EBaseAddressList.PLAY_TIME_READSIZE, pid);
            int[] times = new GameResultLoader().HexToDecLittleEndian(datas);
            return times[0];
        }

        private static List<string> GetPlayer()
        {
            Encoding euckr = GetEuckr();

            byte[] datas = MemoryManager.ReadMemory((IntPtr)EBaseAddressList.PLAYER_NAME, (int)EBaseAddressList.PLAYER_NAME_READSIZE, pid);
            string player = euckr.GetString(datas);
            List<string> players = new List<string>();
            string currentName = "";

            for (int i = 0; i < player.Length; i++)
            {
                char c = player[i];
                // 첫 글자가 \0이 아닐때부터 ~ \0 을 만날때까지
                // 01 03 AD 00 00 00 DD FF AA 00 00 00
                if (c != '\0')
                {
                    currentName += c.ToString();
                }
                else if (c == '\0' && !currentName.Equals(""))
                {
                    players.Add(currentName);
                    currentName = "";
                }
            }

            return players;
        }

        private static string byteArrayToString(byte[] bytes)
        {
            Encoding euckr = GetEuckr();

            return euckr.GetString(bytes).Replace("\0", "");
        }

        private static string GetGameVersion()
        {
            return byteArrayToString(MemoryManager.ReadMemory((IntPtr)EBaseAddressList.GAME_VERSION, (int)EBaseAddressList.GAME_VERSION_READSIZE, pid));
        }

        private static string GetMapName()
        {
            return byteArrayToString(MemoryManager.ReadMemory((IntPtr)EBaseAddressList.MAP_NAME, (int)EBaseAddressList.MAPNAME_READSIZE, pid));
        }

        private static Encoding GetEuckr()
        {
            int euckrCodePage = 51949;  // euc-kr 코드 번호
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(euckrCodePage);
        }
    }
}