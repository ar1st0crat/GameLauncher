using System.Windows;

namespace GameLauncher.View
{
    /// <summary>
    /// Interaction logic for DeviceListWindow.xaml
    /// </summary>
    public partial class DeviceListWindow : Window
    {
        public DeviceListWindow()
        {
            InitializeComponent();
        }

        // IMHO, No need to use command binding to ViewModel here just to set DialogResult

        private void OnOk(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}