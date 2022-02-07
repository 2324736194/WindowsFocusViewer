using System.Windows;
using System.Windows.Input;
using WindowsFocusViewer.Views;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;

namespace WindowsFocusViewer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _Title = "Windows 聚焦查看器";

        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }

        public ICommand LoadedCommand { get; }

        public MainWindowViewModel()
        {
            LoadedCommand = new DelegateCommand(LoadedCommandExecuteMethod);
        }

        private void LoadedCommandExecuteMethod()
        {
            var ioc = Application.Current.PrismIoc();
            var manager = ioc.RegionManager;
            manager.RequestNavigate(MainWindow.ContentRegion,nameof(MainView));
        }
    }
}
