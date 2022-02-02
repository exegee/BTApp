using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BTApp.Converters
{
    [ValueConversion(typeof(PLCConnectionStatus), typeof(BitmapImage))]
    class MachineStatusToBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = new Machine().State.Find(item => item.Value == (string)value).Key;
            var bitmapImage = "";
            switch (state)
            {
                case 0:
                    bitmapImage = "Assets/Images/error.png";
                    break;
                case 101:
                    bitmapImage = "Assets/Images/warning.png";
                    break;
                case int v when (v > 101 && v <= 120 || v == 100):
                    bitmapImage = "Assets/Images/error.png";
                    break;
                default:
                    bitmapImage = "Assets/Images/ok.png";
                    break;
            }
            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
