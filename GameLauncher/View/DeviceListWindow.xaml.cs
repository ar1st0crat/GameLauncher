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
