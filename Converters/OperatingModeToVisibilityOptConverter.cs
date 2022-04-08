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
    public class OperatingModeToVisibilityOptConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if((string)parameter == "Recoiling" && (PLCOperatingMode)value == PLCOperatingMode.Recoiling)
            {
                return "Visible";
            }
            else if ((string)parameter == "Sheeting" && (PLCOperatingMode)value == PLCOperatingMode.Sheeting)
            {
               return "Visible";
            }
            else
            {
                return "Hidden";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
