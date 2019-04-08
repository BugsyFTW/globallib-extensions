using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Extensions
{
    public static class ValidationsPT
    {
        /// <summary>
        /// Verifica se o NIB de uma conta bancária é válido
        /// </summary>
        /// <param name="nib">NIB em formato string sem espaços ou traços</param>
        /// <returns>Devolve True caso o NIB seja válido</returns>
        public static bool IsValidNIB(this string nib)
        {
            if (nib == null)
                return false;

            //remove os espaços vazios
            nib = nib.Replace(" ", string.Empty);
            // remove qq traço
            nib = nib.Replace("-", string.Empty);

            if (nib.Length != 21)
                return false;

            //guarda o check digit
            string digito = nib.Substring(nib.Length - 2, 2);

            //substitui o checkdigit por '00'
            nib = nib.Substring(0, 19);
            nib += "00";

            int peso = 0, res, a;
            bool resultado = false;

            for (int i = 1; i < nib.Length; i++)
            {
                a = int.Parse(nib.Substring(i - 1, 1));
                a = peso + a;
                peso = (a * 10) % 97;
            }

            res = 98 - peso;

            if (digito == string.Format("{0:00}", res))
            {
                resultado = true;
            }

            return resultado;
        }

        /// <summary>
        /// Verifica se o IBAN é válido
        /// </summary>
        /// <param name="iban">IBAN em formato string sem espaços</param>
        /// <returns>Devolve True caso o IBAN seja válido</returns>
        public static bool IsValidIBAN(this string iban)
        {
            if (iban == null)
                return false;

            if (iban.Length < 6)
                return false;

            //remove os espaços vazios
            iban = iban.Replace(" ", string.Empty);

            //troca para o fim o código do país e o check digit
            iban = iban.Substring(4) + iban.Substring(0, 4);

            char[] ibanArray = iban.ToCharArray();

            string aux = string.Empty;
            decimal finalIban;


            foreach (char c in ibanArray)
            {
                int res = 0;

                if (char.IsLetter(c))
                {
                    res = Convert.ToInt32(c) - 55;
                }
                else if (char.IsNumber(c))
                {
                    res = Convert.ToInt32(c.ToString());
                }

                aux += res;
            }

            finalIban = decimal.Parse(aux);

            if ((finalIban % 97) == 1)
                return true;
            else
                return false;
        }

        /// <summary>
        ///  Verifica se o NIF é válido de acordo com as regras portuguesas
        /// </summary>
        /// <param name="nif">NIF em formato string sem espaços</param>
        /// <param name="isInVATIN">true se contém a indicação ISO2 de portugal</param>
        /// <returns>Devolve True caso o NIF seja válido</returns>
        public static bool IsValidNIF_PT(this string nif, Boolean isInVATIN = false)
        {
            if ( (nif == null) || (nif.Trim().Length == 0))
                return false;

            if(isInVATIN)
            {
                string iso2 = nif.Substring(0, 2).ToUpperInvariant();
                
                if (iso2 != "PT")
                    return false;

                nif = nif.Replace(iso2, "");
            }

            int checkDigit;
            char firstNumber;

            //Verifica se é numerico e tem 9 digitos
            if (nif.isNumber() && nif.Length == 9)
            {
                //primeiro némero do NIF
                firstNumber = nif[0];

                //Verifica se o nif comeca por (1, 2, 5, 6, 8, 9) que séo os valores posséveis para os NIF's em PT
                if (firstNumber.Equals('1') || firstNumber.Equals('2') || firstNumber.Equals('5') || firstNumber.Equals('6') || firstNumber.Equals('8') || firstNumber.Equals('9'))
                {
                    //Calcula o CheckDigit
                    checkDigit = (Convert.ToInt16(firstNumber.ToString()) * 9);
                    for (int i = 2; i <= 8; i++)
                    {
                        checkDigit += Convert.ToInt16(nif[i - 1].ToString()) * (10 - i);
                    }
                    checkDigit = 11 - (checkDigit % 11);

                    //Se checkDigit for superior a 10 passa a 0
                    if (checkDigit >= 10)
                        checkDigit = 0;

                    //Compara o digito de controle com o éltimo numero do NIF
                    //Se igual, o NIF é vélido.
                    if (checkDigit.ToString() == nif[8].ToString())
                        return true;
                }
            }

            return false;
        }

    }
}
