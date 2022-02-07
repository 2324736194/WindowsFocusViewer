using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace WindowsFocusService
{
    public partial class Service1 : ServiceBase
    {
        private readonly ILogger _Logger;
        private readonly Process _Process;

        public Service1()
        {
            InitializeComponent();
            _Logger = LogManager.GetCurrentClassLogger();
            _Process= new Process();
            AppDomain.CurrentDomain.UnhandledException +=OnUnhandledException;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _Logger.Error(e.ExceptionObject);
        }

        protected override async void OnStart(string[] args)
        {
            //await Task.Delay(10000);
            var exePath = AppDomain.CurrentDomain.GetSetting("WindowsFocusConsole").Value;
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, exePath);
            _Process.StartInfo.FileName = fileName;
            _Process.StartInfo.CreateNoWindow = true;
            _Process.Start();
            _Logger.Info(_Process.Id);
        }

        protected override void OnStop()
        {
            base.OnStop();
            
            _Process.Close();
            _Process.Dispose();
        }
    }
}
