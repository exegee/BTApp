using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BTApp.Converters
{
    [ValueConversion(typeof(PLCOperatingMode), typeof(string))]
    public class OperatingModeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var i = (PLCOperatingMode)value;
            switch (i)
            {
                case PLCOperatingMode.Recoiling:
                    return "Recoiling";
                case PLCOperatingMode.Sheeting:
                    return "Sheeting";
                default:
                    return "Indefinite";//No Data
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
