using BTApp.Models;
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
    [ValueConversion(typeof(List<ServiceGate>), typeof(SolidColorBrush))]
    public class ServiceGatesToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int numbrOfGatesOpened = ((List<ServiceGate>)value).Count;
            if (numbrOfGatesOpened > 0)
                return new SolidColorBrush(Color.FromRgb(0xb8, 0, 0));
            return new SolidColorBrush(Colors.DarkGreen);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
