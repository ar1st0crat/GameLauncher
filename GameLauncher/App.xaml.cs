using System;
using System.Windows;
using GameLauncher.View;
using GameLauncher.Util;
using GameLauncher.Model;

namespace GameLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void ApplicationStart(object sender, StartupEventArgs e)
        {
            //Disable shutdown when the dialog closes
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            
            // step 1
            var authorizer = new AuthorizationService();
            if (authorizer.RetrieveLogin() == null)
            {
                RegisterAdmin();
            }

            // step 2
            try
            {
                GameRepository.Prepare();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Проблемы с базой данных:\n" + ex.Message);
                Shutdown();
            }

            // step 3
            var authorizeWindow = new AuthorizeWindow();
            if (authorizeWindow.ShowDialog() == true)
            {
                // step 4
                var mainWindow = new MainWindow();
                mainWindow.ShowDialog();
            }

            Shutdown();
        }

        private void RegisterAdmin()
        {
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}