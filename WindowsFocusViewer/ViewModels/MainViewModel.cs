using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Paging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Prism.Commands;

namespace WindowsFocusViewer.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private ObservableCollection<CardViewModel> _CardCollection;
        private ObservableDictionary<string,string> _DirectoryDictionary;
        private ObservableDictionary<double, ImageSource> _ViewCategoryDictionary;
        private readonly string[] _ImageFiles;
        private ObservableDictionary<ShowCategroy, string> _CategoryDictionary;
        private ShowCategroy _ShowCategroy;
        private Paginator _Paginator;

        public PagingHandler Paging { get; }

        public ObservableDictionary<ShowCategroy,string> CategoryDictionary
        {
            get => _CategoryDictionary;
            set => this.SetValue(ref _CategoryDictionary, value);
        }
        
        public ObservableDictionary<double, ImageSource> ViewCategoryDictionary
        {
            get => _ViewCategoryDictionary;
            set => this.SetValue(ref _ViewCategoryDictionary, value);
        }

        public ObservableDictionary<string,string> DirectoryDictionary
        {
            get => _DirectoryDictionary;
            set => this.SetValue(ref _DirectoryDictionary, value);
        }

        public ObservableCollection<CardViewModel> CardCollection
        {
            get => _CardCollection;
            set => this.SetValue(ref _CardCollection, value);
        }

        public ICommand LoadedCommand { get; }

        public ICommand SelectedViewCategoryCommand { get; }

        public ICommand SelectedShowCategroyCommand { get; }

        public MainViewModel()
        {
            _ImageFiles = new DirectoryInfo(This.WindowsFocusSaveDirectory)
                .GetFiles()
                .OrderByDescending(p=>p.CreationTime)
                .Where(p => !p.Name.Contains(nameof(This.MyCollected)))
                .Select(p=>p.FullName)
                .ToArray();
            CategoryDictionary = GetCategoryDictionary();
            ViewCategoryDictionary = GetViewCategoryDictionary();
            Paging= new PagingHandler(OnPaging);
            LoadedCommand = new DelegateCommand<Paginator>(LoadedCommandExecuteMethod);
            SelectedViewCategoryCommand = new DelegateCommand<double?>(SelectedViewCategoryCommandExecuteMethod);
            SelectedShowCategroyCommand = new DelegateCommand<ShowCategroy?>(SelectedShowCategroyCommandExecuteMethod);
        }

        private void SelectedShowCategroyCommandExecuteMethod(ShowCategroy? categroy)
        {
            if (null == categroy)
                throw new NotImplementedException();
            _ShowCategroy = categroy.Value;
            _Paginator.PageIndex = 1;
            PaginatorCommands.GotoPage.Execute(null, _Paginator);
        }

        private async Task<IPagingResult> OnPaging(int pageindex, int pagesize)
        {
            var list = default(IList<string>);
            switch (_ShowCategroy)
            {
                case ShowCategroy.All:
                    list = _ImageFiles;
                    break;
                case ShowCategroy.MyCollected:
                    list = This.MyCollected;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CardCollection =
                await Task.Run(() => list
                    .Skip((pageindex - 1) * pagesize)
                    .Take(pagesize)
                    .Select(p => new CardViewModel(p))
                    .ToObservableCollection());
            return new PagingResult()
            {
                PageIndex = pageindex,
                PageSize = pagesize,
                PagingDataCount = list.Count
            };
            throw new NotImplementedException();
        }

        private void SelectedViewCategoryCommandExecuteMethod(double? height)
        {
            if (null == height)
            {
                throw new NotImplementedException();
            }

            CardViewModel.CardHeight = height.Value;
        }

        private void LoadedCommandExecuteMethod(Paginator paginator)
        {
            if(null == paginator)
                throw new NotImplementedException();
            _Paginator = paginator;
            //_Paginator.PageIndex = 1;
            //PaginatorCommands.GotoPage.Execute(null, _Paginator);
        }

        private ObservableDictionary<double, ImageSource> GetViewCategoryDictionary()
        {
            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\Icons\ViewCategroies");
            var dictionary = new Dictionary<double, ImageSource>();
            for (int i = 3; i > 0; i--)
            {
                var key = i * 100;
                var filePath = Path.Combine(directory, $"View{i}.png");
                var source = File.OpenRead(filePath).Create();
                dictionary.Add(key, source);
            }

            return new ObservableDictionary<double, ImageSource>(dictionary);
        }

        private ObservableDictionary<ShowCategroy, string> GetCategoryDictionary()
        {
            var dic = new Dictionary<ShowCategroy, string>();
            dic.Add(ShowCategroy.All,"全部");
            dic.Add(ShowCategroy.MyCollected,"我的收藏");
            return new ObservableDictionary<ShowCategroy, string>(dic);
        }
    }
}