using System;
using System.Windows.Input;

namespace BTApp.Commands
{
    public class OnWindowUnloadedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action _execute;

        public OnWindowUnloadedCommand(Action execute)
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
