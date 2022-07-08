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
    internal class MachineStatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mode = new Machine().State.Find(item => item.Value == (string)value).Key;
            string result = "";

            switch (mode)
            {
                case 1: // manual
                    result = "Manual Mode";
                    break;
                case 2:
                    result = "Setting up";
                    break ;
                case 3:
                    result = "Auto Mode";
                    break;
                case 4:
                    result = "Auto Mode - recoiling without slitting";
                    break;
                case 5:
                    result = "Auto Mode - recoiling with slitting";
                    break;
                case 6:
                    result = "Auto Mode - sheeting without slitting";
                    break;
                case 7:
                    result = "Auto Mode - sheeting with slitting";
                    break;
                case 100:
                    result = "Emergency STOP";
                    break;
                default:
                    result = "no data";
                    break;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
