using System;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace joban_record_exe.Utilities
{
    internal class InGameNameChanger
    {
        private Timer timer;
        private int pid;
        private string name;

        public InGameNameChanger(string name)
        {
            this.name = name;
        }

        public async Task<bool> ChangeInGameName(int cureentPid)
        {
            pid = cureentPid;
            if (pid != 0)
            {
                Console.WriteLine("네임 체인저 실행");
                await ChangeName(pid);
                SetTimer();
                timer.Start();

                while (timer.Enabled)
                {
                    await Task.Delay(100);
                }
            }

            return true;
        }

        private void SetTimer()
        {
            timer = new Timer(1000);
            timer.Elapsed += OnTimedEvent;
        }

        private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            await ChangeName(pid);

            if (await MemoryManager.CheckMemoryUntilMatchAsync((IntPtr)EBaseAddressList.CLIENT_STATUS, (int)EBaseAddressList.CLIENT_STATUS_READSIZE, pid, "03"))
            {
                Console.WriteLine("ChangeInGameName Timer Stop");
                timer.Stop();
            }

        }

        private async Task ChangeName(int pid)
        {
            byte[] nameToByteArray = FillEmptyBuffer((int)EBaseAddressList.NAME_DATA_READSIZE, StringToByteArray(name));
            byte[] seletedNameValue = { 0X00 };
            Console.WriteLine("이름 변경 " + name);
            await MemoryManager.WriteMemoryAsync((IntPtr)EBaseAddressList.NAME_DATA, pid, nameToByteArray);
            await MemoryManager.WriteMemoryAsync((IntPtr)EBaseAddressList.SELECTED_NAME_DATA, pid, seletedNameValue);
        }

        private byte[] FillEmptyBuffer(int size, byte[] buffer)
        {
            byte[] currentBuffer = new byte[size];

            for (int i = 0; i < size; i++)
            {
                currentBuffer[i] = 0x00;
            }

            // 입력 버퍼의 내용을 현재 버퍼에 복사
            for (int i = 0; i < buffer.Length && i < size; i++)
            {
                currentBuffer[i] = buffer[i];
            }

            return currentBuffer;
        }

        private byte[] StringToByteArray(string name)
        {
            int euckrCodePage = 51949;  // euc-kr 코드 번호
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding euckr = Encoding.GetEncoding(euckrCodePage);

            return euckr.GetBytes(name);
        }
    }
}
