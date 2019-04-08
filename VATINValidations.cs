using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GlobalLib.Extensions
{
    public static class VATINValidations
    {
        public static bool IsValidVATIN(this string value, string pais)
        {
            if (value.IsNullOrEmpty()) return true;

            string[] paises = { "AT", "BE", "BG", "CY", "CZ", "DE", "DK", "EE", "EL", "GR", "ES", "FI", "FR"
                    , "GB", "HU", "IE", "IT", "LT", "LU", "LV", "MT", "NL", "PL", "RO", "SE", "SI", "SK" };

            var valueISO2 = value.Substring(0, 2);

            if (paises.Contains(valueISO2) && pais == valueISO2)
            {
                try
                {
                    Regex regex = new Regex(@"^((AT)?U[0-9]{8}|(BE)?0[0-9]{9}|(BG)?[0-9]{9,10}|(CY)?[0-9]{8}L|
                                        (CZ)?[0-9]{8,10}|(DE)?[0-9]{9}|(DK)?[0-9]{8}|(EE)?[0-9]{9}|
                                        (EL|GR)?[0-9]{9}|(ES)?[0-9A-Z][0-9]{7}[0-9A-Z]|(FI)?[0-9]{8}|
                                        (FR)?[0-9A-Z]{2}[0-9]{9}|(GB)?([0-9]{9}([0-9]{3})?|[A-Z]{2}[0-9]{3})|
                                        (HU)?[0-9]{8}|(IE)?[0-9]S[0-9]{5}L|(IT)?[0-9]{11}|
                                        (LT)?([0-9]{9}|[0-9]{12})|(LU)?[0-9]{8}|(LV)?[0-9]{11}|(MT)?[0-9]{8}|
                                        (NL)?[0-9]{9}B[0-9]{2}|(PL)?[0-9]{10}|(PT)?[0-9]{9}|(RO)?[0-9]{2,10}|
                                        (SE)?[0-9]{12}|(SI)?[0-9]{8}|(SK)?[0-9]{10})$");

                    if (regex.Match(value).Success)
                    {
                        return true;
                    }
                    return false;
                }
                catch (ArgumentException)
                {
                    return false;
                }

            }
            return false;
        }
    }
}
