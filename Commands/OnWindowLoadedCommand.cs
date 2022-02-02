using System;
using System.Windows.Input;

namespace BTApp.Commands
{
    public class OnWindowLoadedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action _execute;

        public OnWindowLoadedCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke();
        }
    }
}
