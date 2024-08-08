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

            List<string> players = GetPlayers();
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

        public static int GetPlayTime()
        {
            byte[] datas = MemoryManager.ReadMemory((IntPtr)EBaseAddressList.PLAY_TIME, (int)EBaseAddressList.PLAY_TIME_READSIZE, pid);
            int times = HexToDecLittleEndian(datas);
            return times;
        }

        public static int HexToDecLittleEndian(byte[] datas)
        {
            string hexString;
            byte[] reorderedBytes = { datas[3], datas[2], datas[1], datas[0] };
            hexString = BitConverter.ToString(reorderedBytes).Replace("-", "");
            return Convert.ToInt32(hexString, 16);
        }

        private static List<string> GetPlayers()
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

        public static int GetCureentUserNumber(string name)
        {
            Encoding euckr = GetEuckr();

            byte[] datas = MemoryManager.ReadMemory((IntPtr)EBaseAddressList.PLAYER_LIST, (int)EBaseAddressList.PLAYER_NAME_READSIZE, pid);

            List<string> players = new List<string>();
            int chunkSize = 15;

            for (int i = 0; i < datas.Length; i += chunkSize)
            {
                int remainingBytes = datas.Length - i;
                int currentChunkSize = Math.Min(chunkSize, remainingBytes);
                byte[] chunk = new byte[currentChunkSize];
                Array.Copy(datas, i, chunk, 0, currentChunkSize);

                players.Add(euckr.GetString(chunk).Replace("\0", ""));
            }

            foreach (string str in players)
            {
                Console.WriteLine(str);
            }

            return players.IndexOf(name);
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