using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Prism.ViewModels;

namespace WindowsFocusViewer.ViewModels
{
    public class CheckConsoleDialogViewModel : DialogAwareViewModel
    {
        private readonly ConsoleStarter starter;
        private bool canExit;
        private readonly string name= $"[{This.Name}服务]";
        private readonly Run waitRun = new Run();
        private readonly TimeSpan delay = TimeSpan.FromSeconds(1);

        /// <summary>
        /// 检查 Windows 聚焦服务是否已运行
        /// </summary>
        public ICommand CheckCommand { get; }

        /// <summary>
        /// 未成功启动服务时，调用此命令退出
        /// </summary>
        public ICommand ExitCommand { get; }

        public CheckConsoleDialogViewModel()
        {
            Title = This.Name;
            var domain = AppDomain.CurrentDomain;
            var consolePath = Path.Combine(domain.BaseDirectory, domain.GetSetting("WindowsFocusConsolePath").Value);
            starter = new ConsoleStarter(consolePath);
            CheckCommand = new DelegateCommand<Paragraph>(CheckCommandExecuteMethod);
            ExitCommand = new DelegateCommand(ExitCommandExecuteMethod,ExitCommandCanExecuteMethod);
        }

        private bool ExitCommandCanExecuteMethod()
        {
            return canExit;
        }

        private void ExitCommandExecuteMethod()
        {
            OnRequestClose(ButtonResult.None);
        }

        private async void CheckCommandExecuteMethod(Paragraph paragraph)
        {
            paragraph.WriteLine($"请稍后，正在检测{name}");
            await Task.Delay(delay);
            if (!starter.IsRegistered())
            {
                paragraph.WriteLine($"未注册开机启动项：{name}");
                await Task.Delay(delay);
                paragraph.WriteLine($"请稍等，正在为您注册{name}");
                await Task.Delay(TimeSpan.FromSeconds(5));
                await Task.Delay(delay);
                paragraph.WaitStart(waitRun);
                var installResult = starter.Register();
                paragraph.WaitStop(waitRun);
                if (!installResult)
                {
                    paragraph.WriteLine($"注册{name}失败，请联系开发人员为您解决问题。", Brushes.Red);
                    await Task.Delay(delay);
                    RaiseExitCommandCanExecute();
                    paragraph.WriteLine($"点击窗口，可关闭。", Brushes.Yellow);
                    return;
                }
                paragraph.WriteLine($"{name}注册成功",Brushes.Green);
                await Task.Delay(delay);
                StartConsole(paragraph);
                return;
            }

            var started = await starter.IsStarted();
            if (!started)
            {
                StartConsole(paragraph);
                return;
            }
            paragraph.WriteLine($"{name}已启动", Brushes.Green);
            await Task.Delay(delay);
            OnRequestClose(ButtonResult.OK);
        }

        private async void StartConsole(Paragraph paragraph)
        {
            paragraph.WriteLine($"请稍等，正在为您启动{name}");
            await Task.Delay(delay);
            paragraph.WaitStart(waitRun);
            await Task.Delay(TimeSpan.FromSeconds(5));
            var startResult = starter.Start();
            paragraph.WaitStop(waitRun);
            if (!startResult)
            {
                paragraph.WriteLine($"启动{name}失败，请联系开发人员为您解决问题。", Brushes.Red);
                await Task.Delay(delay);
                RaiseExitCommandCanExecute();
                paragraph.WriteLine($"点击窗口，可关闭。", Brushes.Yellow);
                return;
            }
            paragraph.WriteLine($"{name}已启动", Brushes.Green);
            await Task.Delay(delay);
            OnRequestClose(ButtonResult.OK);
        }

        private void RaiseExitCommandCanExecute()
        {
            canExit = true;
            ExitCommand.RaiseCanExecuteChanged();
        }
    }
}