using System.ComponentModel;
using System.Windows.Media;
using Prism.Services.Dialogs;
using Prism.ViewModels;

namespace WindowsFocusViewer.ViewModels
{
    public class CardDialogViewModel : DialogAwareViewModel
    {
        private ImageSource _Source;

        public ImageSource Source
        {
            get => _Source;
            private set => this.SetValue(ref _Source, value);
        }
        
        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            Source = parameters.GetValue<ImageSource>(nameof(Source));
        }
    }
}