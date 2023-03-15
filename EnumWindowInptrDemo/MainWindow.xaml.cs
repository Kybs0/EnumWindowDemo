using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EnumWindowInptrDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AllWindowsButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowsTextBlock.Text = string.Empty;
            var currentIntPtr = new WindowInteropHelper(this).Handle;
            var windowInfos=WindowEnumerator.FindAll(I => I.IsVisible && !I.IsMinimized && I.Bounds.Width > 1 &&
                                                          I.Bounds.Height > 1
                                                          && I.Hwnd != currentIntPtr).ToList();
            foreach (var windowInfo in windowInfos)
            {
                WindowsTextBlock.Text +=
                    $"【Process:{windowInfo.ProcessInfo.ProcessId}/{windowInfo.ProcessInfo.ProcessName},Window:{windowInfo.Hwnd}/{windowInfo.Title}】,IsMinimized:{windowInfo.IsMinimized},IsVisible:{windowInfo.IsVisible},ClassName:{windowInfo.ClassName},Bounds:{windowInfo.Bounds}\r\n";
                Debug.WriteLine($"Process:{windowInfo.ProcessInfo.ProcessId}/{windowInfo.ProcessInfo.ProcessName},Window:{windowInfo.Hwnd}/{windowInfo.Title},IsMinimized:{windowInfo.IsMinimized},IsVisible:{windowInfo.IsVisible},ClassName:{windowInfo.ClassName},Bounds:{windowInfo.Bounds}\r\n");
            }
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            WindowsTextBlock.Text = string.Empty;
            var currentIntPtr = new WindowInteropHelper(this).Handle;
            var windows = WindowEnumerator.GetAllAboveWindows(currentIntPtr);
            var windowInfos = windows.Where(I => I.IsVisible && !I.IsMinimized && I.Bounds.Width > 1 && I.Bounds.Height > 1
                                            && I.Hwnd != currentIntPtr
                                            && !(I.ClassName.StartsWith("Shell_") && I.ClassName.EndsWith("TrayWnd"))).ToList();
            foreach (var windowInfo in windowInfos)
            {
                WindowsTextBlock.Text +=
                    $"【Process:{windowInfo.ProcessInfo.ProcessId}/{windowInfo.ProcessInfo.ProcessName},Window:{windowInfo.Hwnd}/{windowInfo.Title}】,IsMinimized:{windowInfo.IsMinimized},IsVisible:{windowInfo.IsVisible},ClassName:{windowInfo.ClassName},Bounds:{windowInfo.Bounds}\r\n";
                Debug.WriteLine($"Process:{windowInfo.ProcessInfo.ProcessId}/{windowInfo.ProcessInfo.ProcessName},Window:{windowInfo.Hwnd}/{windowInfo.Title},IsMinimized:{windowInfo.IsMinimized},IsVisible:{windowInfo.IsVisible},ClassName:{windowInfo.ClassName},Bounds:{windowInfo.Bounds}\r\n");
            }
        }
    }
}
