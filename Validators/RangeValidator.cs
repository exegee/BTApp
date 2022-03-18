using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BTApp.Validators
{
    public class RangeValidator: ValidationRule
    {
        public int Min { get; set; } = 0;
        public int Max { get; set; } = 100000;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string input = value as string;
            int number = 0;

            if(Int32.TryParse(input, out number))
            {
                if(number >= Min && number <= Max)
                {
                    return ValidationResult.ValidResult;
                }
            }
            return new ValidationResult(false, "Number out of range");
        }
    }
}
