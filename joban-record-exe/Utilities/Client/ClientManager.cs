using joban_record_exe.Models.GameDatas.Entity;
using joban_record_exe.Models.GameResultDatas.Entity;
using joban_record_exe.Utilities.RestApi;
using JobanRecordApp.Models.Members.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace joban_record_exe.Utilities
{
    internal class ClientManager
    {
        private GameDataDto.Post gameDataDto;
        private GameDataDto.Patch patchGameDataDto;
        private GameResultDataDto gameResultDataDto = new GameResultDataDto();
        private Member member = new Member();
        private String str;
        private Timer timer;
        private int pid;

        public ClientManager(Member cureentMember)
        {
            member = cureentMember;
        }

        public async Task<bool> ClientStart()
        {
            InGameNameChanger inGameNameChanger = new InGameNameChanger("유정균"); // 수정 필요
            pid = await PidLoader.SearchProcessId();
            Console.WriteLine("Program PID = " + pid);
            
            if (pid != 0 && await inGameNameChanger.ChangeInGameName(pid))
            {
                if (await GameMode())
                {
                    gameDataDto = await GameDataLoader.GetPostGameData(pid); // 게임 시작 직후 실행
                    await RestClient.Post.RequestAsync("http://localhost:8080/gamedatas/", gameDataDto);

                    SetTimer();
                    timer.Start();

                    while (timer.Enabled)
                    {
                        await Task.Delay(100);
                    }
                }
                //GameDataSender.Post.PostGameData(postGameDataDto); // POST 요청하기

                //gameResultDataDto = await GameResultLoader.GetGameResultData(pid); // 게임 종료 후 실행
            }
            else
            {
                return false;
            }

            return true;
        }
        private async Task<bool> GameMode()
        {
            return await MemoryManager.CheckMemoryUntilMatchAsync((IntPtr)EBaseAddressList.CLIENT_STATUS, (int)EBaseAddressList.CLIENT_STATUS_READSIZE, pid, "03");
        }

        private void SetTimer()
        {
            timer = new Timer(1000);
            timer.Elapsed += GameTimedEvent;
        }

        private async void GameTimedEvent(object sender, EventArgs e)
        {
            byte[] clientStatus = MemoryManager.ReadMemory((IntPtr)EBaseAddressList.CLIENT_STATUS, (int)EBaseAddressList.CLIENT_STATUS_READSIZE, pid);
            Console.WriteLine($"GameTimedEvent clientStatus = {clientStatus[0]}");

            // result
            if (clientStatus[0] == 29)
            {
                Console.WriteLine("Game end. If you leave the record window, the result file analyzed and the game results will be saved in the DB.");
                if (await GetResult())
                {
                    Console.WriteLine("Analyzed after Win~");
                    timer.Stop();
                }
                else
                {
                    Console.WriteLine("Analyzed after lose");
                    timer.Stop();
                }
            }
            else if (clientStatus[0] == 09 || clientStatus[0] == 27 || clientStatus[0] == 11)
            {
                Console.WriteLine("you lose");
                timer.Stop();
            }
        }

        private async Task<bool> GetResult() // true : 승리, false : 패배
        {
            string filename = "D:\\[ESL]Syw2plus\\result.dat";
            byte[] bytess = System.IO.File.ReadAllBytes(filename);
            int win = BitConverter.ToInt32(bytess, 0);

            return win == 1;
        }
    }
}
