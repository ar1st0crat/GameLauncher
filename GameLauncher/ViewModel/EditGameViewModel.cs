using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using GameLauncher.Command;
using GameLauncher.Model;
using Microsoft.Win32;

namespace GameLauncher.ViewModel
{
    class EditGameViewModel : INotifyPropertyChanged
    {
        private readonly Game _game;

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _exePath;
        public string ExePath
        {
            get { return _exePath; }
            set
            {
                _exePath = value;
                OnPropertyChanged("ExePath");
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                OnPropertyChanged("ImagePath");
            }
        }

        private int _duration;
        public int Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                OnPropertyChanged("Duration");
            }
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand BrowseExeCommand { get; private set; }
        public ICommand BrowseImageCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

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


        public EditGameViewModel(Game game = null)
        {
            SaveCommand = new RelayCommand(SaveGame);
            BrowseExeCommand = new RelayCommand(BrowseExePath);
            BrowseImageCommand = new RelayCommand(BrowseImagePath);
            CancelCommand = new RelayCommand(Cancel);

            if (game == null)
            {
                return;
            }

            _game = game;

            Name = game.Name;
            ImagePath = game.ImagePath;
            ExePath = game.ExePath;
            Duration = game.Duration;
        }

        public void BrowseExePath()
        {
            var ofd = new OpenFileDialog { RestoreDirectory = true };

            if (ofd.ShowDialog() == true)
            {
                ExePath = ofd.FileName;
            }
        }

        public void BrowseImagePath()
        {
            var ofd = new OpenFileDialog { RestoreDirectory = true };

            if (ofd.ShowDialog() == true)
            {
                ImagePath = ofd.FileName;
            }
        }

        public void SaveGame()
        {
            if (_name == "" || _exePath == "" || _duration <= 0)
            {
                MessageBox.Show("Заполните корректно поля!");
                return;
            }

            if (_game == null)
            {
                InsertGame();
            }
            else
            {
                UpdateGame();
            }

            DialogResult = true;
        }

        public void InsertGame()
        {
            var game = new Game
            {
                Name = Name,
                ExePath = ExePath,
                ImagePath = ImagePath,
                Duration = Duration
            };

            try
            {
                var db = new GameRepository();
                db.AddGame(game);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось добавить игру!\n" + ex.Message);
            }
        }

        public void UpdateGame()
        {
            _game.Name = Name;
            _game.ImagePath = ImagePath;
            _game.ExePath = ExePath;
            _game.Duration = Duration;

            try
            {
                var db = new GameRepository();
                db.UpdateGame(_game);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось обновить игру!\n" + ex.Message);
            }
        }

        private void Cancel()
        {
            DialogResult = false;
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
