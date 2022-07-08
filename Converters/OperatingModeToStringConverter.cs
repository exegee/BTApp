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
                case PLCOperatingMode.Recoiling_NoCutting:
                    return "Recoiling without cutting";
                case PLCOperatingMode.Recoiling_Cutting:
                    return "Recoiling with cutting";
                case PLCOperatingMode.Sheeting_NoCutting:
                    return "Sheeting without cutting";
                case PLCOperatingMode.Sheeting_Cutting:
                    return "Sheeting with cutting";
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
