using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsFocusViewer.Views
{
    /// <summary>
    /// CheckConsoleDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CheckConsoleDialog : UserControl
    {
        public CheckConsoleDialog()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            return;
            if (sender is FrameworkElement element)
            {
                var window = Window.GetWindow(element);
                window.Left = SystemParameters.WorkArea.Width / 2 - window.ActualWidth / 2;
                window.Top = SystemParameters.WorkArea.Height / 2 - window.ActualHeight / 2;
            }
        }

        private void WindowsServiceElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                RotateElement(element);
            }
        }

        private void RotateElement(FrameworkElement element)
        {
            var rotateTransform = element.RenderTransform as RotateTransform;
            if (null == rotateTransform)
            {
                rotateTransform = new RotateTransform();
                element.RenderTransform = rotateTransform;
            }
            element.RenderTransformOrigin = new Point(0.5, 0.5);
            var rotateAnimation = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(3)))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
        }
    }
}
