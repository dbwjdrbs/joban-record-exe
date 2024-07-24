using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace joban_record_exe.Utilities
{
    internal static class PidLoader
    {
        private static Timer timer;
        private static int pid;
        public static async Task<int> SearchProcessId()
        {
            SetTimer();
            timer.Start();
            Console.WriteLine("타이머 시작");

            while (timer.Enabled)
            {
                await Task.Delay(100);
            }
            
            return pid;
        }

        private static void SetTimer() {
            timer = new Timer(3000); // 1초 간격으로 타이머 설정
            timer.Elapsed += OnTimedEvent;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            int currentPid = MemoryManager.ReadPid("syw2plus.exe");
            if (currentPid != 0)
            {
                Console.WriteLine($"프로세스 발견, PID: {currentPid}");
                pid = currentPid;
                timer.Stop();
            }
        }
    }
}
