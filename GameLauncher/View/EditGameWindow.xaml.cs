using System.Windows;
using GameLauncher.ViewModel;

namespace GameLauncher.View
{
    /// <summary>
    /// Interaction logic for EditGameWindow.xaml
    /// </summary>
    partial class EditGameWindow : Window
    {
        private readonly EditGameViewModel _context;

        public EditGameWindow(EditGameViewModel context)
        {
            DataContext = context;
            _context = context;
            InitializeComponent();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            _context.SaveGame();
            DialogResult = true;
        }

        private void OnBrowseExePath(object sender, RoutedEventArgs e)
        {
            _context.BrowseExePath();
        }

        private void OnBrowseImagePath(object sender, RoutedEventArgs e)
        {
            _context.BrowseImagePath();
        }
    }
}
