using Microsoft.Win32;

namespace GameLauncher.Util
{
    class AuthorizationService
    {
        private const string RegSubkey = @"SOFTWARE\WPFGameLauncher";

        public string RetrieveLogin()
        {
            string login = null;

            using (var key = Registry.CurrentUser.OpenSubKey(RegSubkey))
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

            using (var key = Registry.CurrentUser.OpenSubKey(RegSubkey))
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
            using (var key = Registry.CurrentUser.CreateSubKey(RegSubkey))
            {
                if (key != null)
                {
                    key.SetValue("Login", login);
                }
            }
        }

        public void UpdatePassword(string password)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(RegSubkey))
            {
                if (key != null)
                {
                    key.SetValue("Password", password.Encrypt());
                }
            }
        }

        public void ResetAdminSettings()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegSubkey))
            {
                if (key != null)
                {
                    Registry.CurrentUser.DeleteSubKey(RegSubkey);
                }
            }
        }
    }
}
