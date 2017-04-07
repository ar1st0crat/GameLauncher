using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using GameLauncher.Command;

namespace GameLauncher.ViewModel
{
    class DeviceListViewModel : INotifyPropertyChanged
    {
        public List<string> VideoDeviceList { get; set; }
        public List<string> AudioDeviceList { get; set; }

        public int SelectedAudioDevice { get; set; }
        public int SelectedVideoDevice { get; set; }

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set
            {
                _dialogResult = value;
                OnPropertyChanged("DialogResult");
            }
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public DeviceListViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        public void Save()
        {
            DialogResult = true;
        }

        public void Cancel()
        {
            DialogResult = false;
        }

        #region INPC-related code

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

        }

        #endregion
    }
}
