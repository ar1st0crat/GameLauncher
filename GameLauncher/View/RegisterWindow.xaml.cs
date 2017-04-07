using System.Windows;
using GameLauncher.Util;

namespace GameLauncher.View
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly AuthorizationService _authorizer = new AuthorizationService();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            if (PasswordTextBox.Password != PasswordRepeatTextBox.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            if (PasswordTextBox.Password == "" || LoginTextBox.Text == "")
            {
                MessageBox.Show("Логин и пароль не могут быть пустыми!");
                return;
            }

            _authorizer.UpdateLogin(LoginTextBox.Text);
            _authorizer.UpdatePassword(PasswordTextBox.Password);

            DialogResult = true;
        }

        private void OnReset(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены?", "Сброс настроек", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            // TODO: add check if reset was succesful
            _authorizer.ResetAdminSettings();

            MessageBox.Show("Все настройки администратора сброшены!");
        }
    }
}
