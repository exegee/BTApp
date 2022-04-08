using BTApp.Common;
using BTApp.Helpers;
using BTApp.Services;
using BTApp.ViewModels;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace BTApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                DataContext = new MainWindowViewModel();
                InitializeComponent();
                DebugMode.WriteErrorToLogFile("Main window initialized");
            }
            catch (Exception ex)
            {
                DebugMode.WriteErrorToLogFile(ex.Message);
            }

        }
    }
}
