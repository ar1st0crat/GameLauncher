using System.Windows;

namespace GameLauncher.ViewHelpers
{
    /// <summary>
    /// Class provides some useful attached properties
    /// </summary>
    public static class DialogResultHelper
    {
        // DialogResult
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached(
                "DialogResult",
                typeof(bool?),
                typeof(DialogResultHelper),
                new PropertyMetadata(DialogResultChanged));

        private static void DialogResultChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window != null)
            {
                window.DialogResult = e.NewValue as bool?;
            }
        }

        public static void SetDialogResult(Window target, bool? value)
        {
            target.SetValue(DialogResultProperty, value);
        }
    }
}