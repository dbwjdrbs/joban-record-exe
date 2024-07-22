using joban_record_exe.Models.GameDatas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Utilities
{
    internal static class GameDataLoader
    {
        public static async Task<GameData> GetGameData(int pid)
        {
            if (await MemoryManager.CheckMemoryAsync((IntPtr)EBaseAddressList.CLIENT_STATUS, (int)EBaseAddressList.CLIENT_STATUS_READSIZE, pid, "03"))
            {
                Console.WriteLine("인게임 감지");
                // 유저 로비 데이터를 가져온다.
                
            }
        }

    }
}
