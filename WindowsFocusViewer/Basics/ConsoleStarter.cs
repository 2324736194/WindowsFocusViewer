using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using NLog;
using File = System.IO.File;

namespace WindowsFocusViewer
{
    public sealed class ConsoleStarter
    {
        private readonly string _ExePath;
        private readonly string _StartupDirectory;
        private readonly ILogger _Logger;
        
        public ConsoleStarter(string exePath)
        {
            _Logger = LogManager.CreateNullLogger();
            _ExePath = exePath;
            _StartupDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Microsoft\Windows\Start Menu\Programs\Startup");
        }

        /// <summary>
        /// 启动控制台程序
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            try
            {
                // todo:隐藏控制台
                var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _ExePath);
                var process = new Process();
                using (process)
                {
                    process.StartInfo.FileName = fileName;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                }
                return true;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 是否已启动
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsStarted()
        {
            try
            {
                var result = await Task.Run(() =>
                {
                    var mutex = new Mutex(true, This.WindowsFocusHandler, out var createdNew);
                    using (mutex)
                    {
                        // 如果已创建，则代表已启动
                        return !createdNew;
                    }
                });
                return result;

            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
            }

            return false;
        }

        /// <summary>
        /// 是否已注册
        /// </summary>
        /// <returns></returns>
        public bool IsRegistered()
        {
            try
            {
                var shortcutPath = Path.Combine(_StartupDirectory, $"{This.WindowsFocusHandler}.lnk");
                var result = File.Exists(shortcutPath);
                return result;
            }
            catch (Exception e)
            {
                _Logger.Error(e);
            }

            return true;
        }

        /// <summary>
        /// 在开始菜单启动项内创建快捷方式
        /// </summary>
        /// <returns></returns>
        public bool Register()
        {
            try
            {
                var shortcutPath = Path.Combine(_StartupDirectory, $"{This.WindowsFocusHandler}.lnk");
                var shell = new WshShell();
                var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,_ExePath);
                shortcut.WorkingDirectory = Path.GetDirectoryName(shortcut.TargetPath);
                shortcut.Save();
                return true;
            }
            catch (Exception e)
            {
                _Logger.Error(e);
            }

            return false;
        }
    }
}