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
        private LobbyData lobbyData;
        private Timer inGameTimer;
        private Timer gameSaveTimer;
        private int pid;
        private string authentication;
        private Member member;
        private long gameId = 0;
        private bool host;

        public ClientManager(Member cureentMember, Member.Authentication accessToken)
        {
            member = cureentMember;
            //authentication = accessToken.accessToken;
            authentication = "Bearer eyJhbGciOiJIUzI1NiJ9.eyJyb2xlcyI6WyJVU0VSIl0sInVzZXJuYW1lIjoid2pkcmJzMzkxNEBnbWFpbC5jb20iLCJzdWIiOiJ3amRyYnMzOTE0QGdtYWlsLmNvbSIsImlhdCI6MTcyMjMxODY5NSwiZXhwIjoxNzIyMzIwNDk1fQ.4AyO-lCgE2wc8aEyvCRRziuJjAOMALdgJsRnF7v7YMw";
        }

        public async Task<bool> ClientStart()
        {
            InGameNameChanger inGameNameChanger = new InGameNameChanger(member.name); // 수정 필요
            pid = MemoryManager.ReadPid("syw2plus.exe");

            await inGameNameChanger.ChangeInGameName(pid);

            if (pid != 0)
            {
                if (await GameMode())
                {
                    lobbyData = GameResultLoader.GetLobbyData(pid, member);
                    // 게임중인지 상태 체크하기
                    Console.WriteLine("[게임중인지 상태 체크]");
                    gameId = await SerchGameDataId();
                    
                    // 제약 : 컴퓨터 있는지? 인게임 5분 탐지 (이벤트 리스너로 5분 지정해서 그 안에 방장이 GET요청 안보내면 DB 삭제 이벤트 적용)
                    // 게임 종료 후, 로비 데이터가 약간 훼손되기 때문에 미리 빼와야함.
                    if (gameId == 0)
                    {
                        SetGameSaveTimer();
                        gameSaveTimer.Start();
                    }
                    else
                    {
                        SetTimer();
                        inGameTimer.Start();
                    }

                    if (host)
                    {
                        while (gameSaveTimer.Enabled)
                        {
                            await Task.Delay(3000);
                        }
                    }

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

        private bool SearchHostRoles()
        {
            return Convert.ToInt32(MemoryManager.ReadMemory((IntPtr)EBaseAddressList.HOST, (int)EBaseAddressList.HOST_READSIZE, pid)[0]) == 1;
        }

        //private async Task<long> SerchGameDataId()
        //{
        //    Task.Delay(5000).Wait();
        //    return await RestClient.Get.RequestGameIdAsync("http://localhost:8080/gamedatas/gamemode", authentication);
        //}

        private async Task<long> SerchGameDataId()
        {
            Console.WriteLine("[SerchGameDataId에서 GET 요청]");

            long gameDataId = 0;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string gameDataId_str = await RestClient.Get.RequestGameIdAsync("http://localhost:8080/gamedatas/gamemode", authentication);

            JsonData jsonData = JsonSerializer.Deserialize<JsonData>(gameDataId_str, options);

            gameDataId = jsonData.data.gameDataID;

            Console.WriteLine($"gameDataId : {gameDataId}");

            return gameDataId;
        }

        private async Task<long> GetGameDataId()
        {
            Console.WriteLine("[GetGameDataId에서 GameDataId GET 요청]");

            Task.Delay(3000).Wait();

            long gameDataId = 0;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string gameDataId_str = await RestClient.Get.RequestGameIdAsync("http://localhost:8080/gamedatas/gamemode", authentication);

            JsonData jsonData = JsonSerializer.Deserialize<JsonData>(gameDataId_str, options);

            gameDataId = jsonData.data.gameDataID;

            Console.WriteLine($"gameDataId : {gameDataId}");

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

        private void SetGameSaveTimer()
        {
            gameSaveTimer = new Timer(3000); // 300000
            gameSaveTimer.Elapsed += GameSaveTimedEvent;
        }

        private void SetTimer()
        {
            inGameTimer = new Timer(1000);
            inGameTimer.Elapsed += GameTimedEvent;
        }
        private async void GameSaveTimedEvent(object sender, EventArgs e)
        {
            GameData.Post postGameData = GameDataLoader.GetPostGameData(pid);
            host = SearchHostRoles();
            if (gameId == 0 && host)
            {
                // DB에 게임 등록 및 게임중 상태 업데이트
                Console.WriteLine("[게임 정보 POST 요청 및 GameId GET 요청]");
                await RestClient.Post.RequestAsync("http://localhost:8080/gamedatas", authentication, postGameData);
                await Task.Delay(5000);
                gameId = await GetGameDataId();
            }
            else
            {
                Console.WriteLine("[GameId GET 요청]");
                gameId = await GetGameDataId();
            }

            Console.WriteLine($"호스트 권한 : {host}");
            Console.WriteLine($"게임 아이디 : {gameId}");

            inGameTimer.Start();
            gameSaveTimer.Stop();
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
                    GameResultData gameResultData = GameResultLoader.GetGameResult(pid, member, lobbyData, gameId, win);
                    await SaveDB(gameResultData);
                    Console.WriteLine("[Win~]");
                    inGameTimer.Stop();
                }
                else
                {
                    GameResultData gameResultData = GameResultLoader.GetGameResult(pid, member, lobbyData, gameId, win);
                    await SaveDB(gameResultData);
                    Console.WriteLine("[lose]");
                    inGameTimer.Stop();
                }
            }
            else if (clientStatus[0] == 9 || clientStatus[0] == 27 || clientStatus[0] == 11)
            {
                int win = 0;
                GameResultData gameResultData = GameResultLoader.GetGameResult(pid, member, lobbyData, gameId, win);
                await SaveDB(gameResultData);
                Console.WriteLine("[you lose]");
                inGameTimer.Stop();
            }
            else if (clientStatus[0] == 0)
            {
                int win = 0;
                GameResultData gameResultData = GameResultLoader.GetGameResult(pid, member, lobbyData, gameId, win);
                await SaveDB(gameResultData);
                Console.WriteLine("[you lose]");
                inGameTimer.Stop();
            }
        }

        private int GetResult() // true : 승리, false : 패배
        {
            string filename = "D:\\[ESL]Syw2plus\\result.dat";
            byte[] bytess = System.IO.File.ReadAllBytes(filename);
            int win = BitConverter.ToInt32(bytess,128);

            return win;
        }

        private async Task SaveDB(GameResultData gameResult)
        {
            Console.WriteLine("[유저 게임 결과 저장]");
            await RestClient.Post.RequestAsync("http://localhost:8080/save/gameresultdatas", authentication, gameResult);
        }
    }
}
