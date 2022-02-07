using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WindowsFocusViewer.Views;
using Prism.Commands;
using Prism.Ioc;
using Prism.Services.Dialogs;

namespace WindowsFocusViewer.ViewModels
{
    public class CardViewModel : ViewModel
    {
        private ImageSource _Source;
        private static double _CardHeight = 200;
        private bool _IsCollected;
        private readonly string _ImagePath;

        private Visibility _LoadingVisibility;

        public Visibility LoadingVisibility
        {
            get => _LoadingVisibility;
            set => this.SetValue(ref _LoadingVisibility, value);
        }

        public bool IsCollected
        {
            get => _IsCollected;
            set
            {
               var changed= this.SetValue(ref _IsCollected, value);
               if (changed)
               {
                   if (value)
                   {
                       if (!This.MyCollected.Contains(_ImagePath))
                       {
                           This.MyCollected.Add(_ImagePath);
                       }
                   }
                   else
                   {
                       This.MyCollected.Remove(_ImagePath);
                   }
               }
            }
        }

        public static double CardHeight
        {
            get => _CardHeight;
            set => SetStaticValue(ref _CardHeight, value);
        }

        public ImageSource Source
        {
            get => _Source;
            set => this.SetValue(ref _Source, value);
        }

        public ICommand LoadedCommand { get; }

        public ICommand DownloadCommand { get; }

        public ICommand ShowCommand { get; }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        
        public CardViewModel(string file)
        {
            _ImagePath = file;
            _IsCollected = This.MyCollected.Contains(_ImagePath);
            LoadedCommand = new DelegateCommand(LoadedCommandExecuteMethod);
            DownloadCommand = new DelegateCommand<string>(DownloadCommandExecuteMethod);
            ShowCommand = new DelegateCommand(ShowCommandExecuteMethod,ShowCommandCanExecuteMethod);
            PropertyChanged +=OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(LoadingVisibility):
                    ShowCommand.RaiseCanExecuteChanged();
                    break;
            }
        }

        private bool ShowCommandCanExecuteMethod()
        {
            if (null == Source)
            {
                return false;
            }

            return LoadingVisibility == Visibility.Collapsed;
        }

        private void ShowCommandExecuteMethod()
        {
            var ioc = Application.Current.PrismIoc();
            var service = ioc.DialogService;
            var parameters = new DialogParameters();
            parameters.Add(nameof(Source), Source);
            service.Show(nameof(CardDialog), parameters, null);
        }

        private async void LoadedCommandExecuteMethod()
        {
            try
            {
                LoadingVisibility = Visibility.Visible;
                Source = await Task.Run(() =>
                {
                    var file = File.OpenRead(_ImagePath);
                    using (file)
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = file;
                        //bitmap.DecodePixelHeight = 300;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        return bitmap;
                    }
                });
            }
            finally
            {

                LoadingVisibility = Visibility.Collapsed;
            }
        }

        private void DownloadCommandExecuteMethod(string dir)
        {
            var name = Path.GetFileName(_ImagePath);
            var newPath = Path.Combine(dir, name);
            File.Copy(_ImagePath, newPath);
        }

        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private static void SetStaticValue<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            oldValue = newValue;
            OnStaticPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        
        private static void OnStaticPropertyChanged(PropertyChangedEventArgs e)
        {
            StaticPropertyChanged?.Invoke(null, e);
        }
    }
}