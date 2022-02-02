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
                    message = "Połączono";
                    break;
                case PLCConnectionStatus.NotConnected:
                    message = "Brak połączenia z linią!";
                    break;
                case PLCConnectionStatus.Connecting:
                    message = "Łączenie...";
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
