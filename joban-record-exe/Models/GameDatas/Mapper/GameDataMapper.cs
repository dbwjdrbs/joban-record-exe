using joban_record_exe.Models.GameDatas.Builder;
using joban_record_exe.Models.GameDatas.Entity;
using joban_record_exe.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Models.GameDatas.Mapper
{
    internal class GameDataMapper
    {
        public GameData getGameData(GameData gameData)
        {
            GameDataBuilder gameDataBuilder = new GameDataBuilder();
            return gameData = gameDataBuilder.Build();
        }

        private LobbyData[] GetLobbyData(int pid)
        {

            byte[] datas = MemoryManager.ReadMemory((IntPtr)EBaseAddressList.LOBBY_DATA, (int)EBaseAddressList.LOBBY_READSIZE, pid);

            return ByteArrayToLobbyDataArray(datas);
        }

        // -------------------- Hex코드를 LobbyData 변환 --------------------
        private LobbyData[] ByteArrayToLobbyDataArray(byte[] datas)
        {
            LobbyData[] result = new LobbyData[8];

            for (int i = 0; i < 8; i++)
            {
                string nation = "";
                string playerNumber = "";
                string isHuman = "";
                string teamNumber = "";
                for (int j = 0; j < 6; j++)
                {
                    if (j == 0)
                    {
                        switch (datas[j + (i * 6)])
                        {
                            case 1:
                                nation = "조선";
                                break;
                            case 2:
                                nation = "일본";
                                break;
                            case 3:
                                nation = "명";
                                break;
                            case 5:
                                nation = "랜덤";
                                break;
                            case 6:
                                nation = "관전";
                                break;
                            default:
                                nation = "없음";
                                break;
                        }
                    }
                    if (j == 1)
                    {
                        playerNumber = (datas[j + (i * 6)] + 1).ToString();
                    }
                    if (j == 2)
                    {
                        isHuman = datas[j + (i * 6)] == 0 ? "사람" : "컴퓨터";
                    }
                    if (j == 5)
                    {
                        teamNumber = (datas[j + (i * 6)] + 1).ToString();
                    }
                }
                result[i] = new LobbyData(nation, playerNumber, isHuman, teamNumber);
            }
            return result;
        }
    }
}
