using Microsoft.Win32;

namespace GameLauncher.Util
{
    class AuthorizationManager
    {
        private const string REG_SUBKEY = @"SOFTWARE\WPFGameLauncher";

        public string RetrieveLogin()
        {
            string login = null;

            using (var key = Registry.CurrentUser.OpenSubKey(REG_SUBKEY))
            {
                if (key != null)
                {
                    login = key.GetValue("Login").ToString();
                }
            }

            return login;
        }

        public string RetrievePassword()
        {
            string password = null;

            using (var key = Registry.CurrentUser.OpenSubKey(REG_SUBKEY))
            {
                if (key != null)
                {
                    password = key.GetValue("Password").ToString().Decrypt();
                }
            }

            return password;
        }

        public void UpdateLogin(string login)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(REG_SUBKEY))
            {
                if (key != null)
                {
                    key.SetValue("Login", login);
                }
            }
        }

        public void UpdatePassword(string password)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(REG_SUBKEY))
            {
                if (key != null)
                {
                    key.SetValue("Password", password.Encrypt());
                }
            }
        }

        public void ResetAdminSettings()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(REG_SUBKEY))
            {
                if (key != null)
                {
                    Registry.CurrentUser.DeleteSubKey(REG_SUBKEY);
                }
            }
        }
    }
}
