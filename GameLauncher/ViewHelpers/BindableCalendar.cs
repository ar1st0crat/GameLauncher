using System;
using System.Globalization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace GameLauncher.ViewHelpers
{
    public class BindableCalendar : WindowsFormsHost
    {
        public static readonly DependencyProperty DateProperty = 
            DependencyProperty.Register("Date",
            typeof(DateTime),
            typeof(BindableCalendar),
            new PropertyMetadata(OnDatePropertyChanged));

        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        public BindableCalendar()
        {
            var calendar = new MonthCalendar
            {
                ShowTodayCircle = false, 
                ShowToday = false
            };
            calendar.DateSelected += (sender, args) => Date = calendar.SelectionEnd;
            Child = calendar;
        }

        private static void OnDatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = (sender as WindowsFormsHost).Child as MonthCalendar;
            calendar.SetDate((DateTime)e.NewValue);
        }

        public bool ShowToday {
            set
            {
                (Child as MonthCalendar).ShowToday = value;
                (Child as MonthCalendar).ShowTodayCircle = value;
            }
        }
    }
}
