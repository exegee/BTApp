using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BTApp.Converters
{
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class BoolToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush color;
            switch (value)
            {
                case true:
                    color = new SolidColorBrush(Colors.DarkGreen);
                    break;
                case false:
                    color = new SolidColorBrush(Colors.DarkRed);
                    break;
                default:
                    color = new SolidColorBrush(Colors.Black);
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
