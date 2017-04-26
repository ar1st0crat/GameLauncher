using GameLauncher.Model;
using System.ComponentModel;

namespace GameLauncher.ViewModel
{
    class GameViewModel : INotifyPropertyChanged
    {
        public int GameNo { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public Game Game { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
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
