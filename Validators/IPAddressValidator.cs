using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BTApp.Validators
{
    public class IPAddressValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string[] input = ((string)value).Split('.');
            int adresByte = 0;
            bool AreAllBytesCorrect = true;

            //adres must have 4 bytes 
            if (input.Length == 4)
            {
                foreach(string s in input)
                {
                    //try parse single byte to number
                    if(Int32.TryParse(s, out adresByte))
                    {
                        //check single byte range
                        if(adresByte < 0 || adresByte > 255)
                        {
                            AreAllBytesCorrect = false;
                        }
                    }
                    else
                    {
                        AreAllBytesCorrect= false;
                    }
                }
            }
            else
            {
                AreAllBytesCorrect = false;
            }

            if (AreAllBytesCorrect)
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, "Bad Address format");
            }

        }
    }
}
