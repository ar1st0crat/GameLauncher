using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using GameLauncher.Command;
using GameLauncher.Model;
using GameLauncher.Util;

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
        private const int RECENT_LIST_COUNT = 5;

        // filters
        private DateTime _startPeriod;
        private DateTime _endPeriod;

        public RelayCommand ShowLogCommand { get; private set; }


        public StatsViewModel()
        {
            ShowLogCommand = new RelayCommand(FillGameData);
        }

        public void FillDataInPeriod(DateTime startPeriod, DateTime endPeriod)
        {
            // set time to 00:00:00
            _startPeriod = startPeriod.Date + new TimeSpan(0, 0, 0);
            _endPeriod = endPeriod.AddDays(1).Date + new TimeSpan(0, 0, 0);

            try
            {
                var db = new DatabaseManager();
                Launches = db.LoadLaunches(_startPeriod, _endPeriod);
                Games = db.LoadGamesStats(_startPeriod, _endPeriod, RECENT_LIST_COUNT);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось получить информацию по запускам!\n" + ex.Message);
            }
        }

        public void FillGameData(object id)
        {
            try
            {
                var db = new DatabaseManager();
                Launches = db.LoadGameLaunchData((int)id, _startPeriod, _endPeriod);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось получить информацию по запускам игры!\n" + ex.Message);
            }
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