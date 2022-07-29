using System;
using System.Globalization;
using System.Windows.Data;

namespace BTApp.Converters
{
    [ValueConversion(typeof(PLCConnectionStatus), typeof(string))]
    public class PLCConnectionStatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string message = "";
            switch (value)
            {
                case PLCConnectionStatus.Connected:
                    message = "Connected";
                    break;
                case PLCConnectionStatus.NotConnected:
                    message = "Connection failed";
                    break;
                case PLCConnectionStatus.Connecting:
                    message = "Connecting...";
                    break;
            }
            return message;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
