using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using GameLauncher.Command;
using GameLauncher.Model;

namespace GameLauncher.ViewModel
{
    class StatsViewModel : INotifyPropertyChanged
    {
        // Collections are element-wise static, so there's no need for using the ObservableCollection
        private List<GameStats> _games;
        public List<GameStats> Games
        { 
            get { return _games; }
            set
            {
                _games = value;
                OnPropertyChanged("Games");
            }
        }

        private List<LaunchInfo> _launches;
        public List<LaunchInfo> Launches
        {
            get { return _launches; }
            set
            {
                _launches = value;
                OnPropertyChanged("Launches");
            }
        }

        // number of displayed launches of each game
        private const int RecentListCount = 5;

        // filters
        private DateTime _startPeriod;
        public DateTime StartPeriod
        {
            get { return _startPeriod; }
            set
            {
                _startPeriod = value;
                OnPropertyChanged("StartPeriod");
                
                // re-fill data if ONLY StartPeriod was changed
                if (!_dateGroupChanging)
                {
                    FillDataInPeriod();
                }
                // otherwise, data will be re-filled in EndPeriod Setter
            }
        }

        private DateTime _endPeriod;
        public DateTime EndPeriod
        {
            get { return _endPeriod; }
            set
            {
                _endPeriod = value;
                OnPropertyChanged("EndPeriod");
                FillDataInPeriod();
            }
        }

        private bool _dateGroupChanging;

        public RelayCommand ShowLastWeekCommand { get; private set; }
        public RelayCommand ShowLastMonthCommand { get; private set; }
        public RelayCommand ShowLogCommand { get; private set; }


        public StatsViewModel()
        {
            ShowLastWeekCommand = new RelayCommand(LastWeekData);
            ShowLastMonthCommand = new RelayCommand(LastMonthData);
            ShowLogCommand = new RelayCommand(FillGameData);

            _dateGroupChanging = true;

            StartPeriod = DateTime.Now.AddDays(-7);
            EndPeriod = DateTime.Now;

            _dateGroupChanging = false;
        }

        private void FillDataInPeriod()
        {
            // set start time to 00:00:00 and end time to 23:59:59
            _startPeriod = _startPeriod.Date + new TimeSpan(0, 0, 0);
            _endPeriod = _endPeriod.Date + new TimeSpan(23, 59, 59);

            try
            {
                var db = new GameRepository();
                Launches = db.LoadLaunches(_startPeriod, _endPeriod);
                Games = db.LoadGamesStats(_startPeriod, _endPeriod, RecentListCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось получить информацию по запускам!\n" + ex.Message);
            }
        }

        private void FillGameData(object id)
        {
            try
            {
                var db = new GameRepository();
                Launches = db.LoadGameLaunchData((int)id, _startPeriod, _endPeriod);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось получить информацию по запускам игры!\n" + ex.Message);
            }
        }

        private void LastWeekData()
        {
            var now = DateTime.Now;
            _dateGroupChanging = true;

            StartPeriod = now.AddDays(1 - (int)now.DayOfWeek);
            EndPeriod = now;

            _dateGroupChanging = false;
        }

        private void LastMonthData()
        {
            var now = DateTime.Now;
            _dateGroupChanging = true;

            StartPeriod = new DateTime(now.Year, now.Month, 1);
            EndPeriod = now;

            _dateGroupChanging = false;
        }

        #region INPC-related code

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}