using System.Windows;
using System.Windows.Controls;

namespace GameLauncher.ViewHelpers
{
    /// <summary>
    /// BindablePasswordBox provides dependency property Password (unlike WPF standard PasswordBox)
    /// </summary>
    public class BindablePasswordBox : Decorator
    {
        public static readonly DependencyProperty PasswordProperty;

        private bool _ignoreCallback;
        
        static BindablePasswordBox()
        {
            PasswordProperty = DependencyProperty.Register(
                "Password",
                typeof(string),
                typeof(BindablePasswordBox),
                new FrameworkPropertyMetadata(
                    "", 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
                    OnPasswordPropertyChanged));
        }

        public BindablePasswordBox()
        {
            var passwordBox = new PasswordBox();
            passwordBox.PasswordChanged += PasswordEdited;
            Child = passwordBox;
        }

        public string Password
        {
            get { return GetValue(PasswordProperty) as string; }
            set { SetValue(PasswordProperty, value); }
        }

        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bindablePasswordBox = (BindablePasswordBox)d;
            var passwordBox = (PasswordBox)bindablePasswordBox.Child;

            if (bindablePasswordBox._ignoreCallback)
            {
                return;
            }

            passwordBox.Password = (e.NewValue != null) ? e.NewValue.ToString() : "";
        }

        private void PasswordEdited(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;

            _ignoreCallback = true;
            Password = passwordBox.Password;
            _ignoreCallback = false;
        }
    }
}