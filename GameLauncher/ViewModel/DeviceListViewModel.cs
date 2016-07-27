using System.Collections.Generic;
using System.ComponentModel;

namespace GameLauncher.ViewModel
{
    class DeviceListViewModel : INotifyPropertyChanged
    {
        public List<string> VideoDeviceList { get; set; }
        public List<string> AudioDeviceList { get; set; }

        public int SelectedAudioDevice { get; set; }
        public int SelectedVideoDevice { get; set; }
        
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
