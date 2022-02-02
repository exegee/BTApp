using System.Windows;

namespace BTApp.Services
{
    public class WindowService : IWindowService
    {
        /// <summary>
        /// Metoda otwiera nowe okno z przypisanym do niego modelem widoku
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="title"></param>
        public void ShowWindow(object viewModel, string title)
        {
            var win = new Window();
            win.Content = viewModel;
            win.ResizeMode = ResizeMode.NoResize;
            win.Title = title;
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.SizeToContent = SizeToContent.WidthAndHeight;
            win.Show();
        }
    }
}
