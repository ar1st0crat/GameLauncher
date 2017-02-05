using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GameLauncher.Command;
using GameLauncher.Model;
using GameLauncher.View;
using GameLauncher.Util;

namespace GameLauncher.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        // games:
        private List<Game> _gameList = new List<Game>();
        public List<Game> GameList
        {
            get { return _gameList; }
            set
            {
                if (value != null) _gameList = value;
                OnPropertyChanged("GameList");
            }
        }
        
        private Game _selectedGame;
        public Game SelectedGame
        {
            get { return _selectedGame; }
            set
            {
                _selectedGame = value;
                OnPropertyChanged("SelectedGame");
            }
        }

        private int _selectedIndex = -1;
        private int _playingGameId;

        // pagination:
        private const int GamesPerPage = 6;
        private int _pageStartPosition = 0;

        // message to show while some operation is pending
        private string _dialogMessage = "";
        public string DialogMessage
        {
            get { return _dialogMessage; }
            set
            {
                _dialogMessage = value;
                OnPropertyChanged("DialogMessage");
            }
        }

        // web cam:
        private readonly WebCamRecorder _webCamRecorder = new WebCamRecorder();
        
        // game process (exe)
        private Process _gameProcess;
        
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // WINAPI-like constants:
        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        //private const int SW_SHOWMINIMIZED = 2;

        // timer:
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private int _elapsedTime;
        private int _duration;
        
        // commands:
        public ICommand PrevPageCommand { get; private set; }
        public ICommand NextPageCommand { get; private set; }
        public ICommand PrevGameCommand { get; private set; }
        public ICommand NextGameCommand { get; private set; }
        public ICommand StartGameCommand { get; private set; }
        public ICommand AddGameCommand { get; private set; }
        public ICommand EditGameCommand { get; private set; }
        public ICommand DeleteGameCommand { get; private set; }
        public ICommand DeviceListCommand { get; private set; }
        public ICommand StatsCommand { get; private set; }
        public ICommand SettingsCommand { get; private set; }


        public MainViewModel()
        {
            PrevPageCommand = new RelayCommand(ShowPreviousPage);
            NextPageCommand = new RelayCommand(ShowNextPage);
            PrevGameCommand = new RelayCommand(SelectPreviousGame);
            NextGameCommand = new RelayCommand(SelectNextGame);
            StartGameCommand = new RelayCommand(StartGame);
            AddGameCommand = new RelayCommand(AddGame);
            EditGameCommand = new RelayCommand(EditGame);
            DeleteGameCommand = new RelayCommand(DeleteGame);
            DeviceListCommand = new RelayCommand(SetupDevices);
            StatsCommand = new RelayCommand(Stats);
            SettingsCommand = new RelayCommand(EditSettings);
            
            if (!_webCamRecorder.Initialize())
            {
                MessageBox.Show(
                    "FFmpeg не установлен или не удалось обнаружить устройство записи! Запись не будет производиться",
                    "Внимание!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += timer_Tick;

            LoadGamesFromDatabase();
        }

        #region settings and stats

        public void SetupDevices()
        {
            var deviceListWindowContext = new DeviceListViewModel
            {
                VideoDeviceList = _webCamRecorder.VideoDeviceList,
                AudioDeviceList = _webCamRecorder.AudioDeviceList,
                SelectedVideoDevice = _webCamRecorder.SelectedVideoDevice,
                SelectedAudioDevice = _webCamRecorder.SelectedAudioDevice
            };
            var deviceWindow = new DeviceListWindow { DataContext = deviceListWindowContext };
            if (deviceWindow.ShowDialog() == true)
            {
                _webCamRecorder.SelectedVideoDevice = deviceListWindowContext.SelectedVideoDevice;
                _webCamRecorder.SelectedAudioDevice = deviceListWindowContext.SelectedAudioDevice;
            }
        }

        private void Stats()
        {
            var statsWindow = new StatsWindow();
            statsWindow.ShowDialog();
        }

        private void EditSettings()
        {
            var registerWindow = new RegisterWindow
            {
                Title = "Настройки администратора"
            };
            registerWindow.ShowDialog();
        }

        #endregion

        #region CRUD games

        private bool LoadGamesFromDatabase()
        {
            try
            {
                var db = new DatabaseManager();
                GameList = db.LoadGames(_pageStartPosition, GamesPerPage + 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось получить информацию по играм!\n" + ex.Message);
                return false;
            }

            return true;
        }

        public void ShowNextPage()
        {
            if (GameList.Count <= GamesPerPage)
            {
                return;
            }

            _pageStartPosition += GamesPerPage;

            if (!LoadGamesFromDatabase())
            {
                _pageStartPosition -= GamesPerPage;
            }
        }

        public void ShowPreviousPage()
        {
            if (_pageStartPosition < GamesPerPage)
            {
                return;
            }

            _pageStartPosition -= GamesPerPage;

            if (!LoadGamesFromDatabase())
            {
                _pageStartPosition += GamesPerPage;
            }
        }

        public void LoadLastPage()
        {
            try
            {
                var db = new DatabaseManager();
                GameList = db.LoadLastGames(GamesPerPage, out _pageStartPosition);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось получить информацию по играм!\n" + ex.Message);
            }
        }
        
        public void AddGame()
        {
            var viewmodel = new EditGameViewModel
            {
                Name = "Неизвестная", 
                Duration = 30
            };

            var editGameWindow = new EditGameWindow
            {
                Title = "Новая игра",
                DataContext = viewmodel
            };

            if (editGameWindow.ShowDialog() == true)
            {
                LoadLastPage();
            }
        }

        public void EditGame()
        {
            if (SelectedGame == null)
            {
                return;
            }

            var viewmodel = new EditGameViewModel(SelectedGame);
            var editGameWindow = new EditGameWindow { DataContext = viewmodel };
            editGameWindow.ShowDialog();

            OnPropertyChanged("GameList");
        }

        public void DeleteGame()
        {
            if (SelectedGame == null)
            {
                return;
            }

            var question = string.Format(
                "Вы уверены, что хотите удалить игру {0} и всю статистику по ней?",
                SelectedGame.Name);

            if (MessageBox.Show(question, "Удаление игры", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            
            // deletion of all launches of the game can take a while:
            ThreadPool.QueueUserWorkItem(DeleteGameOperation);
        }

        private void DeleteGameOperation(object state)
        {
            ShowMessage("Подождите, пока удалится вся история запусков игры!");

            try
            {
                var db = new DatabaseManager();
                db.DeleteGame(SelectedGame.Id);
            }
            catch (Exception ex)
            {
                StopShowingMessage();
                MessageBox.Show("Не удалось удалить игру и статистику по ней!\n" + ex.Message);
                return;
            }

            StopShowingMessage();

            SelectGame(-1);

            // if we've just deleted last game on the current page
            if (GameList.Count == 1)
            {
                ShowPreviousPage();
                return;
            }

            LoadGamesFromDatabase();
        }

        private void ShowMessage(string text)
        {
            DialogMessage = text;
        }

        private void StopShowingMessage()
        {
            DialogMessage = "";
        }

        #endregion

        #region game navigation

        public void SelectGame(int picIndex)
        {
            if (picIndex == -1)
            {
                SelectedGame = null;
                _selectedIndex = -1;
            }
            else
            {
                SelectedGame = GameList[picIndex];
                _selectedIndex = _pageStartPosition + picIndex;
            }
        }

        public void SelectNextGame()
        {
            if (_selectedIndex % GamesPerPage == GameList.Count - 1 
                && GameList.Count <= GamesPerPage)
            {
                return;
            }

            // check if the selection is currently on other page
            if (_selectedIndex < _pageStartPosition || 
                _selectedIndex >= _pageStartPosition + GamesPerPage)
            {
                return;
            }
            
            _selectedIndex++;

            if (_selectedIndex % GamesPerPage == 0)
            {
                if (_selectedIndex != 0)
                {
                    ShowNextPage();
                }
            }

            SelectGame(_selectedIndex % GamesPerPage);
        }

        public void SelectPreviousGame()
        {
            if (_selectedIndex <= 0)
            {
                return;
            }

            // check if the selection is currently on other page
            if (_selectedIndex < _pageStartPosition ||
                _selectedIndex >= _pageStartPosition + GamesPerPage)
            {
                return;
            }

            if (_selectedIndex % GamesPerPage == 0)
            {
                ShowPreviousPage();
            }

            _selectedIndex--;

            SelectGame(_selectedIndex % GamesPerPage);
        }

        #endregion

        /// <summary>
        /// TODO: comment entire workflow
        /// </summary>
        public void StartGame()
        {
            if (SelectedGame == null)
            {
                return;
            }

            if (_gameProcess != null)
            {
                MessageBox.Show("Остановите сначала уже запущенную игру!");
                return;
            }

            // try starting new game
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = SelectedGame.ExePath,
                    UseShellExecute = true
                };
                _gameProcess = new Process { StartInfo = startInfo };
                _gameProcess.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Игра не может быть запущена!\nПроверьте путь к файлу!");

                if (_gameProcess != null)
                {
                    _gameProcess.Dispose();
                    _gameProcess = null;
                }
                return;
            }

            _playingGameId = SelectedGame.Id;

            // =============================================== prepare for recording
            // TODO: introduce PathManager
            var filename = String.Format("{0}_{1}.avi",
                DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"),
                SelectedGame.Name);

            // guarantee that the filename is valid
            filename = Path.GetInvalidFileNameChars()
                .Aggregate(filename, (current, c) => current.Replace(c, '_'));

            filename = @"video\" + filename;

            _webCamRecorder.Start(filename);
            // =====================================================================

            // prepare and start timer
            _elapsedTime = 0;
            _duration = SelectedGame.Duration;
            _timer.Start();
        }

        private void StopGame()
        {
            if (_gameProcess == null)
            {
                return;
            }

            // 1
            _timer.Stop();
            
            // 2
            if (!_gameProcess.HasExited)
            {
                // first try killing process gently
                if (!_gameProcess.CloseMainWindow())
                {
                    /*
                     * Win32Exception	
                        The associated process could not be terminated.
                        -or-
                        The process is terminating.
                        -or-
                        The associated process is a Win16 executable.
                                             * 
                      InvalidOperationException	
                        The process has already exited.
                        -or-
                        There is no process associated with this Process object.
                     */
                    _gameProcess.Kill();
                }
            }
            _gameProcess.Close();
            _gameProcess = null;

            // 3
            _webCamRecorder.Stop();

            // 4) update database ('launch' table)
            try
            {
                var db = new DatabaseManager();
                db.AddLaunch(_playingGameId, _elapsedTime);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось добавить информацию о новом запуске игры!\n" + ex.Message);
            }
            
            _elapsedTime = 0;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (_elapsedTime == _duration)
            {
                NotifyStopTrial();
            }

            if (_gameProcess != null && _gameProcess.HasExited)
            {
                StopGame();
            }

            _elapsedTime++;
        }

        private void NotifyStopTrial()
        {
            ShowWindow(_gameProcess.MainWindowHandle, SW_HIDE);
            //ShowWindow(_gameProcess.MainWindowHandle, SW_SHOWMINIMIZED);

            if (MessageBox.Show("Тестовый период завершен! Продолжить?", "Внимание!", 
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Information) != MessageBoxResult.Yes)
            {
                StopGame();
                return;
            }

            ShowWindow(_gameProcess.MainWindowHandle, SW_SHOWNORMAL); 
        }

        public void CleanUp()
        {
            if (_gameProcess != null)
            {
                return;
            }

            StopGame();

            _timer.Tick -= timer_Tick;
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