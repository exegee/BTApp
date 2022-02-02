using System;
using System.Windows.Input;

namespace BTApp.Commands
{
    public class LastOrderCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action _execute;

        public LastOrderCommand(Action execute)
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
