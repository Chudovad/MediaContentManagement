using System.Windows;

namespace MediaContentManagementClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ScrollToTop(object sender, RoutedEventArgs e)
        {
            myScrollViewer.ScrollToTop();
        }
    }
}