using System.Windows;
using GameLauncher.Util;

namespace GameLauncher.View
{
    /// <summary>
    /// Interaction logic for AuthorizeWindow.xaml
    /// </summary>
    public partial class AuthorizeWindow : Window
    {
        private readonly AuthorizationManager _authorizer = new AuthorizationManager();

        public AuthorizeWindow()
        {
            InitializeComponent();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            if (Authorize())
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Неверный логин/пароль!");    
            }
        }

        private bool Authorize()
        {
            return LoginTextBox.Text == _authorizer.RetrieveLogin() && 
                   PasswordTextBox.Password == _authorizer.RetrievePassword();
        }
    }
}