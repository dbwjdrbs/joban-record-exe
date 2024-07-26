using joban_record_exe.Models.GameDatas.Entity;
using joban_record_exe.Models.GameResultDatas.Entity;
using JobanRecordApp.Models.Members.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace joban_record_exe.Utilities
{
    internal class GameResultLoader
    {

        // GameData
        public GameResultData GetGameResult(int pid, Member member, long gameId, int isWin, int myNumber)
        {
            string inGameNation = GetInGameNation(MemoryManager.ReadMemory((IntPtr)EBaseAddressList.INGAME_NATION, (int)EBaseAddressList.INGAME_NATION_READSIZE, pid));

            byte[] lobbyDatas = MemoryManager.ReadMemory((IntPtr)EBaseAddressList.LOBBY_DATA, (int)EBaseAddressList.LOBBY_READSIZE, pid);

            LobbyData lobbyData = GetLobbyData(lobbyDatas, myNumber);

            byte[] resultDatas = MemoryManager.ReadMemory((IntPtr)EBaseAddressList.RESULT_DATA, (int)EBaseAddressList.RESULT_READSIZE, pid);

            ResultData resultData = GetResultData(resultDatas, myNumber);

            GameData gameData = new GameData(gameId);


            return new GameResultData(
                member,
                gameData,
                isWin,
                resultData.troop,
                resultData.building,
                resultData.troopLoss,
                resultData.kill,
                resultData.buildingLoss,
                resultData.destroy,
                resultData.tree,
                resultData.grain,
                lobbyData.teamNumber,
                lobbyData.isHuman,
                lobbyData.playerNumber,
                lobbyData.lobbyNation,
                inGameNation
                );
        }

        private string GetInGameNation(byte[] datas)
        {
            string nation = "";

            switch (datas[0])
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

            return nation;
        }

        private static Encoding GetEuckr()
        {
            int euckrCodePage = 51949;  // euc-kr 코드 번호
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(euckrCodePage);
        }

        private LobbyData GetLobbyData(byte[] lobbyDatas, int myNumber)
        {
            int index = myNumber;

            int teamNumber = 0;
            int playerNumber = 0;
            int isHuman = 0;
            string nation = "";
            for (int j = 0; j < 6; j++)
            {
                if (j == 0)
                {
                    switch (lobbyDatas[j + (index * 6)])
                    {
                        case 01:
                            nation = "조선";
                            break;
                        case 02:
                            nation = "일본";
                            break;
                        case 03:
                            nation = "명";
                            break;
                        case 05:
                            nation = "랜덤";
                            break;
                        case 06:
                            nation = "관전";
                            break;
                        default:
                            nation = "없음";
                            break;
                    }
                }
                if (j == 1)
                {
                    playerNumber = (lobbyDatas[j + (index * 6)] + 1);
                }
                if (j == 2)
                {
                    isHuman = lobbyDatas[j + (index * 6)];
                }
                if (j == 5)
                {
                    teamNumber = (lobbyDatas[j + (index * 6)] + 1);
                }
            }

            return new LobbyData(teamNumber, isHuman, playerNumber, nation);
        }

        private ResultData GetResultData(byte[] datas, int inGameNumber)
        {
            int[] convertedData = HexToDecLittleEndian(datas);

            // convertData는 8번씩 끊어서 저장, ResultData에 넣기.

            int index = inGameNumber * 8;

            ResultData result = new ResultData(
                convertedData[index],
                convertedData[index + 1],
                convertedData[index + 2],
                convertedData[index + 3],
                convertedData[index + 4],
                convertedData[index + 5],
                convertedData[index + 6],
                convertedData[index + 7]
            );

            return result;
        }

        public int[] HexToDecLittleEndian(byte[] bytes)
        {
            int[] result = new int[bytes.Length];
            int cnt = 0;
            int resultArrayCount = 0;

            // 총 20byteR
            for (int i = 0; i < bytes.Length; i++)
            {
                string hexString;
                try
                {
                    if (cnt < 6)
                    {
                        byte[] reorderedBytes = { bytes[i + 1], bytes[i] };
                        hexString = BitConverter.ToString(reorderedBytes).Replace("-", "");
                        result[resultArrayCount] = Convert.ToInt32(hexString, 16);
                        i++;
                    }

                    else
                    {
                        byte[] reorderedBytes = { bytes[i + 3], bytes[i + 2], bytes[i + 1], bytes[i] };
                        hexString = BitConverter.ToString(reorderedBytes).Replace("-", "");
                        result[resultArrayCount] = Convert.ToInt32(hexString, 16);
                        i += 3;
                    }
                    resultArrayCount++;

                    if (cnt == 7)
                    {
                        cnt = 0;
                    }
                    else
                    {
                        cnt++;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
            }

            return result;
        }
    }
}
