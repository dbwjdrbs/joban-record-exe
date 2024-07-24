using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace joban_record_exe.Utilities
{
    internal class MemoryManager
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);

        private const uint PROCESS_QUERY_INFORMATION = 0x0400;
        private const uint PROCESS_VM_READ = 0x0010;
        private const uint PROCESS_VM_WRITE = 0x0020;
        private const uint PROCESS_VM_OPERATION = 0x0008;

        public static int ReadPid(string targetOriginalFileName) // "syw2plus.exe"
        {
            // 모든 실행 중인 프로세스를 가져옵니다.
            int pid = 0;
            Process[] processes = Process.GetProcesses();
            string pname = "";

            foreach (Process process in processes)
            {
                try
                {
                    // 각 프로세스의 실행 파일 경로를 확인합니다.
                    string processFilePath = process.MainModule.FileName;

                    // 실행 파일의 파일 버전 정보를 가져옵니다.
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(processFilePath);

                    // 원본 파일 이름이 타겟 원본 파일 이름과 일치하는지 확인합니다.
                    if (string.Equals(fileVersionInfo.OriginalFilename, targetOriginalFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"프로세스 ID: {process.Id}, 프로세스 이름: {process.ProcessName}");
                        pid = process.Id;
                        pname = process.ProcessName;
                    }
                }
                catch (Exception ex)
                {
                    // 접근할 수 없는 프로세스에 대한 예외 처리
                    Console.WriteLine($"프로세스 {process.ProcessName} 접근 실패: {ex.Message}");
                }
            }

            Console.WriteLine($"최종 결과 : PID = {pid} ProcessName = {pname}");
            Console.WriteLine("-------------------------------------------------");

            return pid;
        }

        public static byte[] ReadMemory(IntPtr baseAddress, int bytesToRead, int pid)
        {
            // 프로세스 핸들을 엽니다.
            IntPtr hProcess = OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, pid);

            if (hProcess == IntPtr.Zero)
            {
                Console.WriteLine("프로세스를 열 수 없습니다.");
                return null;
            }

            byte[] buffer = new byte[bytesToRead];
            int bytesRead;

            // 메모리 공간을 읽어옵니다.
            if (ReadProcessMemory(hProcess, baseAddress, buffer, buffer.Length, out bytesRead) && bytesRead == bytesToRead)
            {
                // 프로세스 핸들을 닫습니다.
                CloseHandle(hProcess);
                return buffer;
            }
            else
            {
                Console.WriteLine("메모리 읽기 실패.");
                // 프로세스 핸들을 닫습니다.
                CloseHandle(hProcess);
                return null;
            }
        }
        private static readonly object _lock = new object();

        public static async Task<bool> WriteMemoryAsync(IntPtr address, int pid, byte[] data)
        {
            IntPtr hProcess = OpenProcess(PROCESS_VM_WRITE | PROCESS_VM_OPERATION, false, pid);
            IntPtr bytesWritten;

            return await Task.Run(() =>
            {
                lock (_lock)
                {
                    bool result = WriteProcessMemory(hProcess, address, data, (uint)data.Length, out bytesWritten);
                    if (!result)
                    {
                        Console.WriteLine("Failed to write process memory. Error: " + Marshal.GetLastWin32Error());
                        return false;
                    }
                    return true;
                }
            }).ConfigureAwait(false);
        }


        public static async Task<bool> CheckMemoryUntilMatchAsync(IntPtr baseAddress, int size, int pid, string value1)
        {
            IntPtr processHandle = OpenProcess(PROCESS_VM_READ, false, pid);

            if (processHandle == IntPtr.Zero)
            {
                CloseHandle(processHandle);
                return false;
            }

            byte[] buffer = new byte[size];
            byte[] previousBuffer = new byte[size];

            int bytesRead;
            bool memoryMatch = false;

            System.Timers.Timer timer = new System.Timers.Timer(1000); // 1초 간격으로 타이머 설정
            timer.Elapsed += (sender, e) =>
            {
                if (ReadProcessMemory(processHandle, baseAddress, buffer, size, out bytesRead) && bytesRead == size)
                {
                    if (!CompareArrays(buffer, previousBuffer))
                    {
                        string message = BitConverter.ToString(buffer);
                        if (message.Equals(value1)) 
                        {
                            memoryMatch = true;
                            timer.Stop();
                        }
                        Array.Copy(buffer, previousBuffer, size);
                    }
                    else
                    {
                        memoryMatch = false;
                        timer.Stop();
                    }
                }
                else
                {
                    CloseHandle(processHandle);
                    Console.WriteLine("메모리 읽기에 실패했습니다.");
                    timer.Stop();
                }
            };

            timer.Start();

            // 타이머가 멈출 때까지 기다립니다.
            while (timer.Enabled)
            {
                await Task.Delay(100);
            }
            // try catch 처리
            CloseHandle(processHandle);
            return memoryMatch;
        }

        public static async Task<bool> CheckMemoryAsync(IntPtr baseAddress, int size, int pid, string value)
        {
            IntPtr processHandle = OpenProcess(PROCESS_VM_READ, false, pid);

            if (processHandle == IntPtr.Zero)
            {
                CloseHandle(processHandle);
                return false;
            }

            byte[] buffer = new byte[size];
            byte[] previousBuffer = new byte[size];

            int bytesRead;
            bool memoryMatch = false;

            System.Timers.Timer timer = new System.Timers.Timer(1000); // 1초 간격으로 타이머 설정
            timer.Elapsed += (sender, e) =>
            {
                Console.WriteLine($"{value} 이 될때까지 대기중.");

                if (ReadProcessMemory(processHandle, baseAddress, buffer, size, out bytesRead) && bytesRead == size)
                {
                    if (!CompareArrays(buffer, previousBuffer))
                    {
                        string message = BitConverter.ToString(buffer);

                        if (message.Equals(value))
                        {
                            memoryMatch = true;
                            timer.Stop();
                        }
                        Array.Copy(buffer, previousBuffer, size);
                    }
                }
                else
                {
                    CloseHandle(processHandle);
                    Console.WriteLine("메모리 읽기에 실패했습니다.");
                    timer.Stop();
                }
            };

            timer.Start();

            // 타이머가 멈출 때까지 기다립니다.
            while (timer.Enabled)
            {
                await Task.Delay(100);
            }
            // try catch 처리
            CloseHandle(processHandle);
            return memoryMatch;
        }

        public static async Task<bool> CheckMemoryAsync(IntPtr baseAddress, int size, int pid, string[] values)
        {
            IntPtr processHandle = OpenProcess(PROCESS_VM_READ, false, pid);

            if (processHandle == IntPtr.Zero)
            {
                CloseHandle(processHandle);
                return false;
            }

            byte[] buffer = new byte[size];
            byte[] previousBuffer = new byte[size];

            int bytesRead;
            bool memoryMatch = false;

            System.Timers.Timer timer = new System.Timers.Timer(1000); // 1초 간격으로 타이머 설정
            timer.Elapsed += (sender, e) =>
            {
                if (ReadProcessMemory(processHandle, baseAddress, buffer, size, out bytesRead) && bytesRead == size)
                {
                    if (!CompareArrays(buffer, previousBuffer))
                    {
                        string message = BitConverter.ToString(buffer);
                        foreach(string str in values)
                        {
                            if (message.Equals(str))
                            {
                                memoryMatch = true;
                                timer.Stop();
                                break;
                            }
                            Array.Copy(buffer, previousBuffer, size);
                        }
                        
                    }
                }
                else
                {
                    CloseHandle(processHandle);
                    Console.WriteLine("메모리 읽기에 실패했습니다.");
                    timer.Stop();
                }
            };

            timer.Start();

            // 타이머가 멈출 때까지 기다립니다.
            while (timer.Enabled)
            {
                await Task.Delay(100);
            }
            // try catch 처리
            CloseHandle(processHandle);
            return memoryMatch;
        }

        // 배열 비교 함수
        private static bool CompareArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }

            return true;
        }
    }
}
