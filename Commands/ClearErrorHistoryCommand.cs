using BTApp.Models;
using BTApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BTApp.Commands
{
    public class ClearErrorHistoryCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public MainWindowViewModel vm;

        public ClearErrorHistoryCommand(MainWindowViewModel vm)
        {
            this.vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter != null)
            {
                List<PlcErrorLog> temp = parameter as List<PlcErrorLog>;
                if(temp.Count > 0)
                {
                    return true;
                }
                
            }

            return false;
        }

        public void Execute(object parameter)
        {
            vm.ClearErrorlog();
        }
    }
}
