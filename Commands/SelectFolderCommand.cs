using BTApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace BTApp.Commands
{
    public class SelectFolderCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public MainWindowViewModel vm;

        public SelectFolderCommand(MainWindowViewModel vm)
        {
            this.vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            string param = parameter as string;

            FolderBrowserDialog dialog = new FolderBrowserDialog();

            dialog.ShowDialog();
            Console.WriteLine(dialog.SelectedPath);

            vm.ChangeFilePath(param , dialog.SelectedPath);
        }
    }
}
