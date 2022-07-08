using BTApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BTApp.Converters
{
    [ValueConversion(typeof(List<ServiceGate>), typeof(string))]
    public class ServiceGatesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int numbrOfGatesOpened = ((List<ServiceGate>)value).Count;
            if (numbrOfGatesOpened > 0)
                return "Service gates opened!";
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
