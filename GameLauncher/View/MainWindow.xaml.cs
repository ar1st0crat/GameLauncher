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