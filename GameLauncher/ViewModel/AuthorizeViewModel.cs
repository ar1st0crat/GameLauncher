using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using GameLauncher.Command;
using GameLauncher.Util;

namespace GameLauncher.ViewModel
{
    class AuthorizeViewModel : INotifyPropertyChanged
    {
        private readonly AuthorizationService _authorizer = new AuthorizationService();

        private string _login;
        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
                OnPropertyChanged("Login");
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        public ICommand LogInCommand { get; private set; }
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


        public AuthorizeViewModel()
        {
            LogInCommand = new RelayCommand(LogIn);
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool AuthorizeSuccess()
        {
            return Login == _authorizer.RetrieveLogin() && 
                   Password == _authorizer.RetrievePassword();
        }

        private void LogIn()
        {
            if (AuthorizeSuccess())
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Неверный логин/пароль!");
            }
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
