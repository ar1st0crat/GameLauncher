using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GameLauncher.ViewModel;

namespace GameLauncher.View
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _context;

        public MainWindow()
        {
            InitializeComponent();
            _context = (MainViewModel)DataContext;
        }

        private void OnAddGame(object sender, RoutedEventArgs e)
        {
            _context.AddGame();
        }

        private void OnEditGame(object sender, RoutedEventArgs e)
        {
            _context.EditGame();
        }

        private void OnDeleteGame(object sender, RoutedEventArgs e)
        {
            _context.DeleteGame();
        }

        private void OnPreviousPage(object sender, RoutedEventArgs e)
        {
            _context.ShowPreviousPage();
        }

        private void OnNextPage(object sender, RoutedEventArgs e)
        {
            _context.ShowNextPage();
        }

        private void PictogramClick(object sender, MouseButtonEventArgs e)
        {
            // the cast is always valid:
            var pic = sender as Image;
            var picIndex = int.Parse(pic.Tag.ToString());
            _context.SelectGame(picIndex);

            // process double-click : start game
            if (e.ClickCount == 2)
            {
                _context.StartGame();
            }
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Image)
            {
                return;
            }

            _context.SelectGame(-1);
        }

        private void OnStats(object sender, RoutedEventArgs e)
        {
            var statsWindow = new StatsWindow();
            statsWindow.ShowDialog();
        }

        private void OnDevices(object sender, RoutedEventArgs e)
        {
            _context.SetupDevices();
        }

        private void OnSettings(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow
            {
                Title = "Настройки администратора"
            };
            registerWindow.ShowDialog();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.CleanUp();
            base.OnClosed(e);
        }

        private void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                _context.StartGame();
            }
        }
    }
}
