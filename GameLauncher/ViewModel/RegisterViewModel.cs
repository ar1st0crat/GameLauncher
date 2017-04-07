using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using GameLauncher.Command;
using GameLauncher.Util;

namespace GameLauncher.ViewModel
{
    class RegisterViewModel : INotifyPropertyChanged
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

        private string _repeatPassword;
        public string RepeatPassword
        {
            get { return _repeatPassword; }
            set
            {
                _repeatPassword = value;
                OnPropertyChanged("RepeatPassword");
            }
        }

        public ICommand UpdateCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }
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


        public RegisterViewModel()
        {
            Login = _authorizer.RetrieveLogin();
            Password = _authorizer.RetrievePassword();
            RepeatPassword = _authorizer.RetrievePassword();

            UpdateCommand = new RelayCommand(UpdateAdminSettings);
            ResetCommand = new RelayCommand(ResetAdminSettings);
            CancelCommand = new RelayCommand(Cancel);
        }

        public void UpdateAdminSettings()
        {
            if (Password != RepeatPassword)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            if (Password == "" || Login == "")
            {
                MessageBox.Show("Логин и пароль не могут быть пустыми!");
                return;
            }

            _authorizer.UpdateLogin(Login);
            _authorizer.UpdatePassword(Password);

            DialogResult = true;
        }

        public void ResetAdminSettings()
        {
            if (MessageBox.Show("Вы уверены?", "Сброс настроек", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                _authorizer.ResetAdminSettings();
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сбросить настройки администратора! Попробуйте еще раз");
                return;
            }

            MessageBox.Show("Все настройки администратора сброшены!");
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
