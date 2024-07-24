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
        private static GameDataDto.Post gameDataDto;
        private static int pid;

        public static async Task<GameDataDto.Post> GetPostGameData(int cureentPid)
        {
            pid = cureentPid;
            if (await MemoryManager.CheckMemoryUntilMatchAsync((IntPtr)EBaseAddressList.CLIENT_STATUS, (int)EBaseAddressList.CLIENT_STATUS_READSIZE, pid, "03"))
            {
                string player = GetPlayer();
                string gameVersion = GetGameVersion();
                string mapName = GetMapName();

                gameDataDto = new GameDataDto.Post(player, gameVersion, mapName);
            }

            return gameDataDto;
        }

        //public static async Task<GameDataDto.Patch> GetPatchGameData

        private static string GetPlayer()
        {
            return byteArrayToString(MemoryManager.ReadMemory((IntPtr)EBaseAddressList.PLAYER_NAME, (int)EBaseAddressList.PLAYER_NAME_READSIZE, pid));
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