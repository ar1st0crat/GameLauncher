using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GameLauncher.Command;
using GameLauncher.Util;

namespace GameLauncher.ViewModel
{
    class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly AuthorizationManager _authorizer = new AuthorizationManager();

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

        public ICommand UpdateCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }


        public RegisterViewModel()
        {
            Login = _authorizer.RetrieveLogin();

            UpdateCommand = new RelayCommand(UpdateAdminSettings);
            ResetCommand = new RelayCommand(ResetAdminSettings);
            CancelCommand = new RelayCommand(Cancel);
        }

        // Well, I'm definitely one of the haters of PasswordBox control in WPF ((
        // I violate MVVM here (
        public void UpdateAdminSettings(object sender)
        {
            var window = sender as Window;
            var passwordBox = window.FindName("Password") as PasswordBox;
            var repeatPasswordBox = window.FindName("RepeatPassword") as PasswordBox;

            if (passwordBox.Password != repeatPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            if (passwordBox.Password == "" || Login == "")
            {
                MessageBox.Show("Логин и пароль не могут быть пустыми!");
                return;
            }

            _authorizer.UpdateLogin(Login);
            _authorizer.UpdatePassword(passwordBox.Password);

            window.DialogResult = true;
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

        public void Cancel(object sender)
        {
            var window = sender as Window;
            window.DialogResult = false;
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
