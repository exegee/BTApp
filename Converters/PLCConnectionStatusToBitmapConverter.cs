using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BTApp.Converters
{
    [ValueConversion(typeof(PLCConnectionStatus), typeof(BitmapImage))]
    public class PLCConnectionStatusToBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            var bitmapImage = "";
            switch (value)
            {
                case PLCConnectionStatus.Connected:
                    bitmapImage = "Assets/Images/button_check_green.png";
                    break;
                case PLCConnectionStatus.NotConnected:
                    bitmapImage = "Assets/Images/loading_circle.gif";
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
