using System.Windows;

namespace WindowsFocusViewer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string ContentRegion = nameof(MainWindow) + nameof(ContentRegion);

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
