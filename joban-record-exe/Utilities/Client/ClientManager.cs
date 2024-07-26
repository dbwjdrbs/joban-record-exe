using joban_record_exe.Models.Datas;
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
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace joban_record_exe.Utilities
{
    internal class ClientManager
    {
        private GameResultData gameResultDataDto = new GameResultData();
        private Timer inGameTimer;
        private int pid;
        private string authentication;
        private Member member;
        private long gameId = 0;
        private int myNumber;
        public ClientManager(Member cureentMember, Member.Authentication accessToken)
        {
            member = cureentMember;
            //authentication = accessToken.accessToken;
            authentication = "Bearer eyJhbGciOiJIUzI1NiJ9.eyJyb2xlcyI6WyJVU0VSIl0sInVzZXJuYW1lIjoid2pkcmJzMzkxNEBnbWFpbC5jb20iLCJzdWIiOiJ3amRyYnMzOTE0QGdtYWlsLmNvbSIsImlhdCI6MTcyMTk4MzkwNSwiZXhwIjoxNzIxOTg1NzA1fQ.tKi4AjKXV6WrlnNrrwMK827vK-eJamUEWcLR8ywDRV4";
        }

        public async Task<bool> ClientStart()
        {
            InGameNameChanger inGameNameChanger = new InGameNameChanger(member.name); // 수정 필요
            pid = MemoryManager.ReadPid("syw2plus.exe");
            GameData.Post postGameData = GameDataLoader.GetPostGameData(pid);
            myNumber = GetMyNumber(postGameData.players, member.name);

            await inGameNameChanger.ChangeInGameName(pid);
            if (pid != 0)
            {
                bool host = SearchHostRoles();

                if (await GameMode())
                {
                    // 게임중인지 상태 체크하기
                    Console.WriteLine("[게임중인지 상태 체크]");
                    //gameId = await SerchGameDataId();

                    if (gameId == 0 && host)
                    {
                        // DB에 게임 등록 및 게임중 상태 업데이트
                        Console.WriteLine("[게임 정보 POST 요청 및 GameId GET 요청]");
                        await RestClient.Post.RequestAsync("http://localhost:8080/gamedatas/", authentication, postGameData);
                        gameId = await GetGameDataId();
                    }
                    else
                    {
                        Console.WriteLine("[GameId GET 요청]");
                        gameId = await GetGameDataId();
                    }

                    Console.WriteLine($"호스트 권한 : {host}");
                    Console.WriteLine($"게임 아이디 : {gameId}");

                    // 게임중 종료 타이밍 잡기.
                    SetTimer();
                    
                    inGameTimer.Start();

                    while (inGameTimer.Enabled)
                    {
                        await Task.Delay(70);
                    }

                    GameData.Patch patchGameData = GameDataLoader.GetPatchGameData(pid, gameId);
                    Console.WriteLine("[게임 최종 저장]");

                    await RestClient.Patch.RequestAsync("http://localhost:8080/gamedatas/end", authentication, patchGameData);
                }
                //GameDataSender.Post.PostGameData(postGameDataDto); // POST 요청하기

                //gameResultDataDto = await GameResultLoader.GetGameResultData(pid); // 게임 종료 후 실행
            }

            return true;
        }

        private int GetMyNumber(List<string> players, string name)
        {
            int num = 0;

            for(int i = 0; i < players.Count; i++)
            {
                if (players[i] == name)
                {
                    num = i;
                }
            }

            return num;
        }

        private bool SearchHostRoles()
        {
            return Convert.ToInt32(MemoryManager.ReadMemory((IntPtr)EBaseAddressList.HOST, (int)EBaseAddressList.HOST_READSIZE, pid)[0]) == 1;
        }

        //private async Task<long> SerchGameDataId()
        //{
        //    Task.Delay(5000).Wait();
        //    return await RestClient.Get.RequestGameIdAsync("http://localhost:8080/gamedatas/gamemode", authentication);
        //}

        private async Task<long> GetGameDataId()
        {
            Console.WriteLine("[GetGameDataId에서 GameDataId GET 요청]");

            long gameDataId = 0;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string gameDataId_str = await RestClient.Get.RequestGameIdAsync("http://localhost:8080/gamedatas/gamemode", authentication);

            JsonData jsonData = JsonSerializer.Deserialize<JsonData>(gameDataId_str, options);

            gameDataId = jsonData.data.gameDataID;

            Console.WriteLine($"gameDataId : {gameDataId}");

            Task.Delay(3000).Wait();

            if (gameDataId == 0)
            {
                gameDataId = await GetGameDataId();
            }

            return gameDataId;
        }

        private async Task<bool> GameMode()
        {
            return await MemoryManager.CheckMemoryUntilMatchAsync((IntPtr)EBaseAddressList.CLIENT_STATUS, (int)EBaseAddressList.CLIENT_STATUS_READSIZE, pid, "03");
        }

        private void SetTimer()
        {
            inGameTimer = new Timer(1000);
            inGameTimer.Elapsed += GameTimedEvent;
        }

        // DB 등록 이벤트
        private async void GameTimedEvent(object sender, EventArgs e)
        {
            byte[] clientStatus = MemoryManager.ReadMemory((IntPtr)EBaseAddressList.CLIENT_STATUS, (int)EBaseAddressList.CLIENT_STATUS_READSIZE, pid);
            Console.WriteLine($"[현재 메모리 번호] {clientStatus[0]}");

            // result
            if (clientStatus[0] == 29)
            {
                Console.WriteLine("[Game end. If you leave the record window, the result file analyzed and the game results will be saved in the DB.]");
                int win = GetResult();
                if (win == 1)
                {
                    GameResultData gameResultData = new GameResultLoader().GetGameResult(pid, member, gameId, win, myNumber);
                    await SaveDB(gameResultData);
                    Console.WriteLine("[Analyzed after Win~]");
                    inGameTimer.Stop();
                }
                else
                {
                    GameResultData gameResultData = new GameResultLoader().GetGameResult(pid, member, gameId, win, myNumber);
                    await SaveDB(gameResultData);
                    Console.WriteLine("[Analyzed after lose]");
                    inGameTimer.Stop();
                }
            }
            else if (clientStatus[0] == 9 || clientStatus[0] == 27 || clientStatus[0] == 11)
            {
                int win = 0;
                GameResultData gameResultData = new GameResultLoader().GetGameResult(pid, member, gameId, win, myNumber);
                await SaveDB(gameResultData);
                Console.WriteLine("[you lose]");
                inGameTimer.Stop();
            }
            else if (clientStatus[0] == 0)
            {
                int win = 0;
                GameResultData gameResultData = new GameResultLoader().GetGameResult(pid, member, gameId, win, myNumber);
                await SaveDB(gameResultData);
                Console.WriteLine("[you lose]");
                inGameTimer.Stop();
            }
        }

        private int GetResult() // true : 승리, false : 패배
        {
            string filename = "D:\\[ESL]Syw2plus\\result.dat";
            byte[] bytess = System.IO.File.ReadAllBytes(filename);
            int win = BitConverter.ToInt32(bytess, 0);

            return win;
        }

        private async Task SaveDB(GameResultData gameResult)
        {
            Console.WriteLine("[유저 게임 결과 저장]");
            await RestClient.Post.RequestAsync("http://localhost:8080/save/gameresultdatas", authentication, gameResult);
        }
    }
}
