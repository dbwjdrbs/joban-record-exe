using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Utilities
{
    internal static class PidLoader
    {
        public static async Task<int> SearchProcessId(int pid)
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
    }
}
