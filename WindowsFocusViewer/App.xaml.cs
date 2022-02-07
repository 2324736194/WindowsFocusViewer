using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using WindowsFocusViewer.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using System.Windows.Threading;
using NLog;
using Prism.Services.Dialogs;

namespace WindowsFocusViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly ILogger _Logger;
        public App()
        {
            _Logger = LogManager.GetCurrentClassLogger();
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException+=OnDispatcherUnhandledException;
        }


        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _Logger.Error(e.Exception);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _Logger.Error(e.ExceptionObject);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(this);
        }

        protected override void OnInitialized()
        {
            var ioc = Application.Current.PrismIoc();
            var dialogService = ioc.DialogService;
            dialogService.Show(nameof(CheckConsoleDialog), null, CheckServiceDialogCallback);
        }

        private void CheckServiceDialogCallback(IDialogResult obj)
        {
            base.OnInitialized();
            //if (obj.Result == ButtonResult.OK)
            //{
            //    base.OnInitialized();
            //    return;
            //}
            //Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            This.MyCollectedSave();
        }
    }
}
