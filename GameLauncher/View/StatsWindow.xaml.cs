using System.Windows;
using System.Windows.Controls;
using ListView = System.Windows.Controls.ListView;

namespace GameLauncher.View
{
    public partial class StatsWindow : Window
    {
        public StatsWindow()
        {
            InitializeComponent();
        }

        // THIS COULD NOT BE ACHIEVED WITH XAML IN .NET 3.5.
        // YOU CAN ELIMINATE THE FOLLOWING CODE IF YOU WILL:

        // Auto-compute the width of the last column and auto-resize it when the gridview resizes:
        // subtract the width of vertical scrollbar and widths of other columns (total: 220 pixels)
        private void LaunchListView_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView == null)
                return;
            var gridView = listView.View as GridView;
            gridView.Columns[3].Width = 
                listView.ActualWidth - SystemParameters.VerticalScrollBarWidth - 220;
        }
    }
}
