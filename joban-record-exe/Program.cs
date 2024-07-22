using joban_record_exe.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe
{
    /** 이 애플리케이션의 목적은 유저 전적 기록과 그에 따른 환경을 만드는데 있다.
     * 기능
     * 1. 게임 기록 - 클라이언트(syw2plus.exe) 의 상태 메모리가 03일때 작동
     * 2. 닉네임 동기화 - 상태 메모리 0D 일때, 0066966E -> Member.name (16진수로 변환한 후), 0066966C -> 00 으로 변경 **/
    
    internal class Program
    {
        static async Task Main(string[] args)
        {
            int pid = 0;
            pid = await PidLoader.SearchProcessId(pid);
            

        }
    }
}
