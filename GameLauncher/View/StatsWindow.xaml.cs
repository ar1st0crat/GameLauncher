using System;
using System.Windows;
using System.Windows.Forms;
using GameLauncher.ViewModel;

namespace GameLauncher.View
{
    /// <summary>
    /// Interaction logic for StatsWindow.xaml
    /// </summary>
    public partial class StatsWindow : Window
    {
        public StatsWindow()
        {
            InitializeComponent();

            CalendarStartPeriod.ShowTodayCircle = false;
            CalendarStartPeriod.ShowToday = false;

            var now = DateTime.Now;
            var startPeriod = now.AddDays(-7);
            CalendarStartPeriod.SetSelectionRange(startPeriod, startPeriod);

            ((StatsViewModel)DataContext).FillDataInPeriod(
                CalendarStartPeriod.SelectionEnd, CalendarEndPeriod.SelectionEnd);
        }

        private void CalendarStartPeriod_OnMouseDown(object sender, MouseEventArgs e)
        {
            ((StatsViewModel)DataContext).FillDataInPeriod(
                CalendarStartPeriod.SelectionEnd, CalendarEndPeriod.SelectionEnd);
        }

        private void CalendarEndPeriod_OnMouseDown(object sender, MouseEventArgs e)
        {
            ((StatsViewModel)DataContext).FillDataInPeriod(
                CalendarStartPeriod.SelectionEnd, CalendarEndPeriod.SelectionEnd);
        }

        private void LastWeekClick(object sender, RoutedEventArgs e)
        {
            var now = DateTime.Now;
            var lastWeekDate = now.AddDays(1 - (int)now.DayOfWeek);

            CalendarStartPeriod.SetSelectionRange(lastWeekDate, lastWeekDate);

            ((StatsViewModel)DataContext).FillDataInPeriod(lastWeekDate, now);
        }

        private void LastMonthClick(object sender, RoutedEventArgs e)
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            CalendarStartPeriod.SetSelectionRange(startOfMonth, startOfMonth);

            ((StatsViewModel)DataContext).FillDataInPeriod(startOfMonth, now);
        }
    }
}
