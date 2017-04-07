using System;
using System.Windows;
using GameLauncher.View;
using GameLauncher.Util;
using GameLauncher.Model;

namespace GameLauncher
{
    public partial class App : Application
    {
        private void ApplicationStart(object sender, StartupEventArgs e)
        {
            //Disable shutdown when the dialog is closed
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            
            // step 1
            var authorizer = new AuthorizationService();

            if (authorizer.RetrieveLogin() == null)
            {
                var registerWindow = new RegisterWindow();
                registerWindow.ShowDialog();
            }

            // step 2 - create database if it doesn't exist
            try
            {
                GameRepository.Prepare();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Проблемы с базой данных:\n" + ex.Message);
                Shutdown();
            }

            // step 3 - run authorization
            var authorizeWindow = new AuthorizeWindow();
            if (authorizeWindow.ShowDialog() == true)
            {
                // step 4 - show main window
                var mainWindow = new MainWindow();
                mainWindow.ShowDialog();
            }

            Shutdown();
        }
    }
}