using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace WindowsFocusConsole
{
    class Program
    {
        public static readonly ILogger _Logger;
        private static readonly Mutex _Mutex;
        private static readonly AppDomain _Domain;

        static Program()
        {
            _Logger = LogManager.GetCurrentClassLogger();
            _Mutex = new Mutex(true, This.WindowsFocusHandler);
            _Domain = AppDomain.CurrentDomain;
            _Domain.UnhandledException += OnUnhandledException;
        }

        static void Main(string[] args)
        {
            var consoleHide = _Domain.GetSetting("ConsoleHide");
            if (bool.TryParse(consoleHide.Value, out var hide) && hide)
            {
                Hide();
            }
            var result = _Mutex.WaitOne(TimeSpan.FromSeconds(3));
            if (!result)
            {
                Console.WriteLine("已启动 {0}", This.WindowsFocusHandler);
                return;
            }
            new WindowsFocusHandler().Handle();
            Console.ReadLine();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _Logger.Error(e.ExceptionObject);
        }

        private static void Hide()
        {
            Console.Title = "隐藏控制台";
            var window = FindWindow(null, "隐藏控制台");
            ShowWindow(window, 0);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化
        }

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "ShowWindow")] 
        private static extern bool ShowWindow(IntPtr hWnd, int type);
    }
}
