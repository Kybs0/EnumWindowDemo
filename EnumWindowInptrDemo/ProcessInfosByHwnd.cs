using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EnumWindowInptrDemo
{
    class ProcessInfosByHwnd
    {
        public static ProcessInfo GetInfo(IntPtr windowIntPtr)
        {
            GetWindowThreadProcessId(windowIntPtr, out var  processId);
            var process = Process.GetProcessById((int)processId);
            return new ProcessInfo(processId, process.ProcessName);
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
    }

    public class ProcessInfo
    {
        public uint ProcessId { get; }
        public string ProcessName { get; }

        public ProcessInfo(uint processId, string processName)
        {
            ProcessId = processId;
            ProcessName = processName;
        }
    }
}
