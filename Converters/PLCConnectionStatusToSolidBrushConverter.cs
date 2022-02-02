using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BTApp.Converters
{
    [ValueConversion(typeof(PLCConnectionStatus), typeof(SolidColorBrush))]
    public class PLCConnectionStatusToSolidBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush color;
            switch (value)
            {
                case PLCConnectionStatus.Connected:
                    color = new SolidColorBrush(Colors.Green);
                    break;
                case PLCConnectionStatus.NotConnected:
                    color = new SolidColorBrush(Colors.Red);
                    break;
                default:
                    color = new SolidColorBrush(Colors.Orange);
                    break;
            }
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
