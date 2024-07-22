using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace joban_record_exe.Utilities
{
    internal class GameResultLoader
    {
        public GameResultLoader()
        {
        }

        // GameData
        public async Task<bool> GameClientMemoryMonitor()
        {
            int pid = 0;
            Console.WriteLine("프로세스를 찾는 중...");

            /** 예외 목록
             * 1. 클라이언트 처음 접속 후 바로 종료 했을 경우. -> 처리 완료.
             * 2. 인게임 갔다가 5분도 안지나서 나갈 경우. -> 
             * 3. 게임 끝난 이후 다시 게임 기록해야하는데 작동 안됨.
             * **/

            pid = await SearchProcessId(pid);
            if (pid != 0)
            {
                // 클라이언트 상태가 인게임 모드 일 경우 메모리 값 : 03
                if (await MemoryManager.CheckMemoryAsync((IntPtr)EBaseAddressList.CLIENT_STATUS, (int)EBaseAddressList.CLIENT_STATUS_READSIZE, pid, "03"))
                {
                    Console.WriteLine("인게임 감지");
                    // 유저 로비 데이터를 가져온다.

                    if (!VerifiedExistComputer(lobbyDatas)) // 로비 데이터에서 컴퓨터가 있는지 찾는다.
                    {
                        ShowMessage("컴퓨터 감지되지 않음 (조건문 수정바람)");
                        timer.Start();

                        if (await MemoryManager.CheckMemoryAsync((IntPtr)BaseAddressList.CLIENTSTATUS, (int)BaseAddressList.CLIENTSTATUS_READSIZE, pid, "09", "0B"))
                        {
                            ShowMessage("게임 도중 종료.");
                            timer.Stop();
                            return await GameClientMemoryMonitor();
                        }
                    }
                    else
                    {
                        ShowMessage("게임 내에 컴퓨터의 존재를 확인");
                        if (!await MemoryManager.CheckMemoryAsync((IntPtr)BaseAddressList.CLIENTSTATUS, (int)BaseAddressList.CLIENTSTATUS_READSIZE, pid, "09", "0B"))
                        {
                            ShowMessage("Task.Delay(-1) 적용");
                            await Task.Delay(-1);
                        }
                    }
                }
                else
                {
                    ShowMessage("프로세스 종료.");
                    log.Hide();
                    return true;
                }
            }

            return await GameClientMemoryMonitor();
        }

        private async Task<int> SearchProcessId(int pid)
        {
            System.Timers.Timer timer = new System.Timers.Timer(1000); // 1초 간격으로 타이머 설정
            timer.Elapsed += (sender, e) =>
            {
                pid = MemoryManager.ReadPid("syw2plus.exe");
                if (pid != 0)
                {
                    Console.WriteLine($"프로세스 발견, PID: {pid}");
                }

            };
            timer.Start();

            while (timer.Enabled)
            {
                await Task.Delay(100);
            }

            return pid;
        }

        // 컴퓨터가 있을 경우 false
        private bool VerifiedExistComputer(LobbyData[] lobbyDatas)
        {
            for (int i = 0; i < lobbyDatas.Length; i++)
            {
                if (!lobbyDatas[i].nation.Equals("없음"))
                {
                    if (lobbyDatas[i].isHuman.Equals("컴퓨터"))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        private void ShowMessage(string message)
        {
            log.WriteLog($"[{DateTime.Now.ToString("HH:mm:ss")}] {message}");
        }

        private async void Timer_Tick(object sender, System.EventArgs e)
        {
            ShowMessage("타이머 이벤트 발생.");
            string document = GetDocument(pid);
            ShowMessage($"firestore document의 이름을 {document}로 지정.");
            timer.Stop();
            await SaveDB(document);
        }

        private async Task<bool> SaveDB(string document)
        {
            int integer_playerNumber = int.Parse(GetPlayerNumber(pid)) + 1;
            string playerNumber = integer_playerNumber.ToString();
            string playTime = GetPlayTime(pid);
            string gameVersion = GetGameVersion(pid);
            string inGameNation = GetInGameNation(pid);

            //GameDataPostDto gameDataPostDto = new GameDataPostDto(lobbyDatas, resultDataController.GetResultData(pid), user, playerNumber, "gameId", playTime, gameVersion, null, inGameNation);

            if (await MemoryManager.CheckMemoryAsync((IntPtr)BaseAddressList.CLIENTSTATUS, (int)BaseAddressList.CLIENTSTATUS_READSIZE, pid, "1D"))
            {
                ShowMessage("로비로 나가거나 게임을 종료하면, Result.dat 분석");
                if (await MemoryManager.CheckMemoryAsync((IntPtr)BaseAddressList.CLIENTSTATUS, (int)BaseAddressList.CLIENTSTATUS_READSIZE, pid, "09", "0B"))
                {
                    string result = GetResult();
                    ShowMessage("Result.dat 분석 완료");
                    if (result.Equals("00"))
                    {
                        ShowMessage("패배");
                        string isWin = "패배";
                        //gameDataPostDto.IsWin = isWin;
                        //if (await gameDataController.PostGameDataAsync(gameDataPostDto, document) && await userController.SetRecord(user, false))
                        //{
                        //    ShowMessage("DB저장 완료");
                        //    return true;
                        //}
                    }
                    else
                    {
                        //ResultData[] resultData = resultDataController.GetResultData(pid);
                        //ShowMessage("승리");
                        //string isWin = "승리";
                        //gameDataPostDto.IsWin = isWin;
                        //if (await gameDataController.PostGameDataAsync(gameDataPostDto, document) && await userController.SetRecord(user, true))
                        //{
                        //    ShowMessage("DB저장 완료");
                        //    return true;
                        //}
                    }
                }
            }
            else if (await MemoryManager.CheckMemoryAsync((IntPtr)BaseAddressList.CLIENTSTATUS, (int)BaseAddressList.CLIENTSTATUS_READSIZE, pid, "09", "0B"))
            {
                ShowMessage("패배");
                string isWin = "패배";
                //gameDataPostDto.IsWin = isWin;
                //    if (await gameDataController.PostGameDataAsync(gameDataPostDto, document) && await userController.SetRecord(user, false))
                //    {
                //       ShowMessage("DB저장 완료");
                //       return true;
                //    }
            }

            return false;
        }

        private string GetPlayerNumber(int pid)
        {
            byte[] playerNumber = MemoryManager.ReadMemory((IntPtr)BaseAddressList.INGAMEPLAYERNUMBER, (int)BaseAddressList.INGAMEPLAYERNUMBER_READSIZE, pid);
            return BitConverter.ToString(playerNumber);
        }

        private string GetDocument(int pid)
        {
            byte[] datas = MemoryManager.ReadMemory((IntPtr)BaseAddressList.PLAYERNAME, (int)BaseAddressList.PLAYERNAME_READSIZE, pid);
            int euckrCodePage = 51949;  // euc-kr 코드 번호
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding euckr = Encoding.GetEncoding(euckrCodePage);
            string result = euckr.GetString(datas);
            string document = $"{DateTime.Now.ToString("yy-MM-dd HH:mm")}, {result}";

            return document;
        }

        private string GetGameVersion(int pid)
        {
            // 각 바이트를 2자리 16진수로 표현된 문자열로 변환
            string result = "";
            byte[] datas = MemoryManager.ReadMemory((IntPtr)BaseAddressList.GAMEVERSION, (int)BaseAddressList.GAMEVERSION_READSIZE, pid);
            for (int i = 0; i < datas.Length; i++)
            {
                result += ((char)datas[i]).ToString();
            }

            return result;
        }

        private string GetPlayTime(int pid)
        {
            byte[] datas = MemoryManager.ReadMemory((IntPtr)BaseAddressList.PLAYTIME, (int)BaseAddressList.PLAYTIME_READSIZE, pid);

            byte[] reorderedBytes = { datas[3], datas[2], datas[1], datas[0] };
            string hexString = BitConverter.ToString(reorderedBytes).Replace("-", "");

            int totalSeconds = Convert.ToInt32(hexString, 16);

            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            return hours.ToString() + ":" + minutes.ToString() + ":" + seconds.ToString();
        }

        private string GetResult()
        {
            string filename = "D:\\[ESL]Syw2plus\\result.dat";
            byte[] bytess = System.IO.File.ReadAllBytes(filename);
            string Win = BitConverter.ToString(bytess, 128, 1);

            return Win;
        }

        private string GetInGameNation(int pid)
        {
            byte[] bytes = MemoryManager.ReadMemory((IntPtr)BaseAddressList.INGAMENATION, (int)BaseAddressList.INGAMENATION_READSIZE, pid);
            string nation = BitConverter.ToString(bytes);
            string result;
            switch(nation)
            {
                case "01":
                    result = "조선";
                    break;
                case "02":
                    result = "일본";
                    break;
                case "03":
                    result = "명";
                    break;
                case "06":
                    result = "관전";
                    break;
                default:
                    result = "없음";
                    break;
            }

            return result;
        }
    }
}
