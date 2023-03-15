using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace EnumWindowInptrDemo
{
    /// <summary>
    /// 包含枚举当前用户空间下所有窗口的方法。
    /// </summary>
    public class WindowEnumerator
    {
        /// <summary>
        /// 查找当前用户空间下所有符合条件的窗口（仅查找顶层窗口）。如果不指定条件，将返回所有窗口。
        /// </summary>
        /// <param name="match">过滤窗口的条件。</param>
        /// <returns>找到的所有窗口信息。</returns>
        public static IReadOnlyList<WindowInfo> FindAll(Predicate<WindowInfo> match = null)
        {
            var windowList = new List<WindowInfo>();
            User32.EnumWindows(OnWindowEnum, 0);
            return match == null ? windowList : windowList.FindAll(match);

            bool OnWindowEnum(IntPtr hWnd, int lparam)
            {
                // 仅查找顶层窗口。
                if (User32.GetParent(hWnd) == IntPtr.Zero)
                {
                    var windowDetail = GetWindowDetail(hWnd);
                    // 添加到已找到的窗口列表。
                    windowList.Add(windowDetail);
                }

                return true;
            }
        }

        private static WindowInfo GetWindowDetail(IntPtr hWnd)
        {
            // 获取窗口类名。
            var lpString = new StringBuilder(512);
            User32.GetClassName(hWnd, lpString, lpString.Capacity);
            var className = lpString.ToString();

            // 获取窗口标题。
            var lptrString = new StringBuilder(512);
            User32.GetWindowText(hWnd, lptrString, lptrString.Capacity);
            var title = lptrString.ToString().Trim();

            // 获取窗口可见性。
            var isVisible = User32.IsWindowVisible(hWnd);

            // 获取窗口位置和尺寸。
            User32.LPRECT rect = default;
            User32.GetWindowRect(hWnd, ref rect);
            var bounds = new Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

            // 获取窗口所在进程信息
            var processInfo = ProcessInfosByHwnd.GetInfo(hWnd);
            return new WindowInfo(hWnd, className, title, isVisible, bounds, processInfo);
        }
        public static List<WindowInfo> GetAllAboveWindows(IntPtr hwnd)
        {
            var windowInfos = new List<WindowInfo>();
            var intPtr = User32.GetWindow(hwnd, 3);
            if (intPtr == IntPtr.Zero)
            {
                return windowInfos;
            }
            var windowDetail = GetWindowDetail(intPtr);
            windowInfos.AddRange(GetAllAboveWindows(intPtr));
            windowInfos.Add(windowDetail);
            return windowInfos;
        }
        public static List<WindowInfo> GetAllBelowWindows(IntPtr hwnd)
        {
            var windowInfos = new List<WindowInfo>();
            var intPtr = User32.GetWindow(hwnd, 2);
            if (intPtr == IntPtr.Zero)
            {
                return windowInfos;
            }
            var windowDetail = GetWindowDetail(intPtr);
            windowInfos.AddRange(GetAllBelowWindows(intPtr));
            windowInfos.Add(windowDetail);
            return windowInfos;
        }
    }

    public static class User32
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hwnd, uint windowType);

        public delegate bool WndEnumProc(IntPtr hWnd, int lParam);
        [DllImport("user32")]
        public static extern bool EnumWindows(WndEnumProc lpEnumFunc, int lParam);

        [DllImport("user32")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lptrString, int nMaxCount);

        [DllImport("user32")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        [DllImport("user32")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref LPRECT rect);


        [StructLayout(LayoutKind.Sequential)]
        public readonly struct LPRECT
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;
        }
    }
    /// <summary>
    /// 获取 Win32 窗口的一些基本信息。
    /// </summary>
    public readonly struct WindowInfo
    {
        public WindowInfo(IntPtr hWnd, string className, string title, bool isVisible, Rect bounds, ProcessInfo processInfo) : this()
        {
            Hwnd = hWnd;
            ClassName = className;
            Title = title;
            IsVisible = isVisible;
            Bounds = bounds;
            ProcessInfo = processInfo;
        }

        /// <summary>
        /// 进程信息
        /// </summary>
        public ProcessInfo ProcessInfo { get; }

        /// <summary>
        /// 获取窗口句柄。
        /// </summary>
        public IntPtr Hwnd { get; }

        /// <summary>
        /// 获取窗口类名。
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// 获取窗口标题。
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// 获取当前窗口是否可见。
        /// </summary>
        public bool IsVisible { get; }

        /// <summary>
        /// 获取窗口当前的位置和尺寸。
        /// </summary>
        public Rect Bounds { get; }

        /// <summary>
        /// 获取窗口当前是否是最小化的。
        /// </summary>
        public bool IsMinimized => Bounds.Left == -32000 && Bounds.Top == -32000;
    }
}