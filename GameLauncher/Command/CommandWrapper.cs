using System;
using System.Windows;
using System.Windows.Input;

namespace GameLauncher.Command
{
    class CommandWrapper : Freezable, ICommand
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command",
                typeof(ICommand),
                typeof(CommandWrapper),
                new UIPropertyMetadata());

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return Command.CanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            Command.Execute(parameter);
        }

        #endregion

        #region Freezable Members

        protected override Freezable CreateInstanceCore()
        {
            return new CommandWrapper();
        }

        #endregion
    }
}
