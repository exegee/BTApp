using BTApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BTApp.Commands
{
    public class RestoreDefaultSettingsCommand : ICommand
    {
        public MainWindowViewModel vm { get; set; }
        public event EventHandler CanExecuteChanged;
        public RestoreDefaultSettingsCommand(MainWindowViewModel vm)
        {
            this.vm = vm;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            vm.RestoreDefaultSettings();
        }
    }
}
