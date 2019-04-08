using System;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web;
using System.Net.Mail;

namespace GlobalLib.Extensions
{
    public static class StringExtensions
    {
        public static string Translate(this string Original, string CharFrom, string CharTo)
        {
            string result = String.Empty;

            foreach (Char c in Original)
            {
                int PosCharFrom = CharFrom.IndexOf(c);
                if (PosCharFrom >= 0)
                {
                    result += CharTo[PosCharFrom];
                }
                else
                {
                    result += c;
                }
            }

            return result;
        }

        public static string TranslateGSM(this string Original)
        {
            return Original.Translate("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!\"#%&/()=?áéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛãõÃÕçÇªº",
                                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!\"#%&/()=?aeiuoAEIOUaeiouAEIOUaeiouAEIOUaoAOcCao");
        }

        public static T SerializeFromString<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StringReader reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public static string SerializeToString(this string Origem, object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);

                return writer.ToString();
            }
        }


        /// <summary>
        /// Verfica se uma string é vazia ou nula
        /// Por defeito faz trim da string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Trimed"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string source, bool Trimed=true)
        {
            if (source != null)
            {
                if(Trimed)
                {
                    return source.Trim() == "" ? true :  false;
                }

                return source == "" ? true : false;
                
            }
            return true;
        }


        /// <summary>
        /// Helper functions for String not already found in C#.
        /// Inspired by PHP String Functions that are missing in .Net.
        /// </summary>

        /// <summary>
        /// Base64 encodes a string.
        /// </summary>
        /// <param name="input">A string</param>
        /// <returns>A base64 encoded string</returns>
        public static string Base64StringEncode(this string input)
        {
            byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(encbuff);
        }

        /// <summary>
        /// Base64 decodes a string.
        /// </summary>
        /// <param name="input">A base64 encoded string</param>
        /// <returns>A decoded string</returns>
        public static string Base64StringDecode(this string input)
        {
            byte[] decbuff = Convert.FromBase64String(input);
            return System.Text.Encoding.UTF8.GetString(decbuff);
        }

        /// <summary>
        /// A case insenstive replace function.
        /// </summary>
        /// <param name="input">The string to examine.</param>
        /// <param name="newValue">The value to replace.</param>
        /// <param name="oldValue">The new value to be inserted</param>
        /// <returns>A string</returns>
        public static string CaseInsenstiveReplace(this string input, string newValue, string oldValue)
        {
            Regex regEx = new Regex(oldValue, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regEx.Replace(input, newValue);
        }

        /// <summary>
        /// Removes all the words passed in the filter words parameters. The replace is NOT case
        /// sensitive.
        /// </summary>
        /// <param name="input">The string to search.</param>
        /// <param name="filterWords">The words to repace in the input string.</param>
        /// <returns>A string.</returns>
        public static string FilterWords(this string input, params string[] filterWords)
        {
            return input.FilterWords(char.MinValue, filterWords);
        }

        /// <summary>
        /// Removes all the words passed in the filter words parameters. The replace is NOT case
        /// sensitive.
        /// </summary>
        /// <param name="input">The string to search.</param>
        /// <param name="mask">A character that is inserted for each letter of the replaced word.</param>
        /// <param name="filterWords">The words to repace in the input string.</param>
        /// <returns>A string.</returns>
        public static string FilterWords(this string input, char mask, params string[] filterWords)
        {
            string stringMask = mask == char.MinValue ? string.Empty : mask.ToString();
            string totalMask = stringMask;

            foreach (string s in filterWords)
            {
                Regex regEx = new Regex(s, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                if (stringMask.Length > 0)
                {
                    for (int i = 1; i < s.Length; i++)
                        totalMask += stringMask;
                }

                input = regEx.Replace(input, totalMask);

                totalMask = stringMask;
            }

            return input;
        }

        /// <summary>
        /// Checks the passed string to see if has any of the passed words. Not case-sensitive.
        /// </summary>
        /// <param name="input">The string to check.</param>
        /// <param name="hasWords">The words to check for.</param>
        /// <returns>A collection of the matched words.</returns>
        public static MatchCollection HasWords(this string input, params string[] hasWords)
        {
            StringBuilder sb = new StringBuilder(hasWords.Length + 50);
            //sb.Append("[");

            foreach (string s in hasWords)
            {
                sb.AppendFormat("({0})|", s.Trim().HtmlSpecialEntitiesEncode());
            }

            string pattern = sb.ToString();
            pattern = pattern.TrimEnd('|'); // +"]";

            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regEx.Matches(input);
        }

        /// <summary>
        /// A wrapper around HttpUtility.HtmlEncode
        /// </summary>
        /// <param name="input">The string to be encoded</param>
        /// <returns>An encoded string</returns>
        public static string HtmlSpecialEntitiesEncode(this string input)
        {
            return HttpUtility.HtmlEncode(input);
        }

        /// <summary>
        /// A wrapper around HttpUtility.HtmlDecode
        /// </summary>
        /// <param name="input">The string to be decoded</param>
        /// <returns>The decode string</returns>
        public static string HtmlSpecialEntitiesDecode(this string input)
        {
            return HttpUtility.HtmlDecode(input);
        }

        /// <summary>
        /// MD5 encodes the passed string
        /// </summary>
        /// <param name="input">The string to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string MD5String(this string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// Verified a string against the passed MD5 hash.
        /// </summary>
        /// <param name="input">The string to compare.</param>
        /// <param name="hash">The hash to compare against.</param>
        /// <returns>True if the input and the hash are the same, false otherwise.</returns>
        public static bool MD5VerifyString(this string input, string hash)
        {
            // Hash the input.
            string hashOfInput = input.MD5String();

            // Create a StringComparer an comare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Left pads the passed input using the HTML non-breaking string entity (&nbsp;)
        /// for the total number of spaces.
        /// </summary>
        /// <param name="input">The string to pad.</param>
        /// <param name="totalSpaces">The total number to pad the string.</param>
        /// <returns>A padded string.</returns>
        public static string PadLeftHtmlSpaces(this string input, int totalSpaces)
        {
            string space = "&nbsp;";
            return PadLeft(input, space, totalSpaces * space.Length);
        }

        /// <summary>
        /// Left pads the passed input using the passed pad string
        /// for the total number of spaces.  It will not cut-off the pad even if it 
        /// causes the string to exceed the total width.
        /// </summary>
        /// <param name="input">The string to pad.</param>
        /// <param name="pad">The string to uses as padding.</param>
        /// <param name="totalSpaces">The total number to pad the string.</param>
        /// <returns>A padded string.</returns>
        public static string PadLeft(this string input, string pad, int totalWidth)
        {
            return input.PadLeft(pad, totalWidth, false);
        }

        /// <summary>
        /// Left pads the passed input using the passed pad string
        /// for the total number of spaces.  It will cut-off the pad so that  
        /// the string does not exceed the total width.
        /// </summary>
        /// <param name="input">The string to pad.</param>
        /// <param name="pad">The string to uses as padding.</param>
        /// <param name="totalSpaces">The total number to pad the string.</param>
        /// <returns>A padded string.</returns>
        public static string PadLeft(this string input, string pad, int totalWidth, bool cutOff)
        {
            if (input.Length >= totalWidth)
                return input;

            int padCount = pad.Length;
            string paddedString = input;

            while (paddedString.Length < totalWidth)
            {
                paddedString += pad;
            }

            // trim the excess.
            if (cutOff)
                paddedString = paddedString.Substring(0, totalWidth);

            return paddedString;
        }

        /// <summary>
        /// Right pads the passed input using the HTML non-breaking string entity (&nbsp;)
        /// for the total number of spaces.
        /// </summary>
        /// <param name="input">The string to pad.</param>
        /// <param name="totalSpaces">The total number to pad the string.</param>
        /// <returns>A padded string.</returns>
        public static string PadRightHtmlSpaces(this string input, int totalSpaces)
        {
            string space = "&nbsp;";
            return PadRight(input, space, totalSpaces * space.Length);
        }

        /// <summary>
        /// Right pads the passed input using the passed pad string
        /// for the total number of spaces.  It will not cut-off the pad even if it 
        /// causes the string to exceed the total width.
        /// </summary>
        /// <param name="input">The string to pad.</param>
        /// <param name="pad">The string to uses as padding.</param>
        /// <param name="totalSpaces">The total number to pad the string.</param>
        /// <returns>A padded string.</returns>
        public static string PadRight(this string input, string pad, int totalWidth)
        {
            return input.PadRight(pad, totalWidth, false);
        }

        /// <summary>
        /// Right pads the passed input using the passed pad string
        /// for the total number of spaces.  It will cut-off the pad so that  
        /// the string does not exceed the total width.
        /// </summary>
        /// <param name="input">The string to pad.</param>
        /// <param name="pad">The string to uses as padding.</param>
        /// <param name="totalSpaces">The total number to pad the string.</param>
        /// <returns>A padded string.</returns>
        public static string PadRight(this string input, string pad, int totalWidth, bool cutOff)
        {
            if (input.Length >= totalWidth)
                return input;

            string paddedString = string.Empty;

            while (paddedString.Length < totalWidth - input.Length)
            {
                paddedString += pad;
            }

            // trim the excess.
            if (cutOff)
                paddedString = paddedString.Substring(0, totalWidth - input.Length);

            paddedString += input;

            return paddedString;
        }

        /// <summary>
        /// Removes the new line (\n) and carriage return (\r) symbols.
        /// </summary>
        /// <param name="input">The string to search.</param>
        /// <returns>A string</returns>
        public static string RemoveNewLines(this string input)
        {
            return input.RemoveNewLines(false);
        }

        /// <summary>
        /// Removes the new line (\n) and carriage return (\r) symbols.
        /// </summary>
        /// <param name="input">The string to search.</param>
        /// <param name="addSpace">If true, adds a space (" ") for each newline and carriage
        /// return found.</param>
        /// <returns>A string</returns>
        public static string RemoveNewLines(this string input, bool addSpace)
        {
            string replace = string.Empty;
            if (addSpace)
                replace = " ";

            string pattern = @"[\r|\n]";
            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return regEx.Replace(input, replace);
        }

        /// <summary>
        /// Reverse a string.
        /// </summary>
        /// <param name="input">The string to reverse</param>
        /// <returns>A string</returns>
        public static string Reverse(this string input)
        {
            if (input.Length <= 1)
                return input;

            char[] c = input.ToCharArray();
            StringBuilder sb = new StringBuilder(c.Length);
            for (int i = c.Length - 1; i > -1; i--)
                sb.Append(c[i]);

            return sb.ToString();
        }

        /// <summary>
        /// Converts a string to sentence case.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>A string</returns>
        public static string SentenceCase(this string input)
        {
            if (input.Length < 1)
                return input;

            string sentence = input.ToLower();
            return sentence[0].ToString().ToUpper() + sentence.Substring(1);
        }

        /// <summary>
        /// Converts all spaces to HTML non-breaking spaces
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>A string</returns>
        public static string SpaceToNbsp(this string input)
        {
            string space = "&nbsp;";
            return input.Replace(" ", space);
        }

        /// <summary>
        /// Removes all HTML tags from the passed string
        /// </summary>
        /// <param name="input">The string whose values should be replaced.</param>
        /// <returns>A string.</returns>
        public static string StripTags(this string input)
        {
            Regex stripTags = new Regex("<(.|\n)+?>");
            return stripTags.Replace(input, "");
        }

        /// <summary>
        /// Converts a string to title case.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>A string.</returns>
        public static string TitleCase(this string input)
        {
            return TitleCase(input, true);
        }

        /// <summary>
        /// Converts a string to title case.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <param name="ignoreShortWords">If true, does not capitalize words like
        /// "a", "is", "the", etc.</param>
        /// <returns>A string.</returns>
        public static string TitleCase(this string input, bool ignoreShortWords)
        {
            List<string> ignoreWords = null;
            if (ignoreShortWords)
            {
                //TODO: Add more ignore words?
                ignoreWords = new List<string>();
                ignoreWords.Add("a");
                ignoreWords.Add("is");
                ignoreWords.Add("was");
                ignoreWords.Add("the");
            }

            string[] tokens = input.Split(' ');
            StringBuilder sb = new StringBuilder(input.Length);
            foreach (string s in tokens)
            {
                if (ignoreShortWords == true
                    && s != tokens[0]
                    && ignoreWords.Contains(s.ToLower()))
                {
                    sb.Append(s + " ");
                }
                else
                {
                    sb.Append(s[0].ToString().ToUpper());
                    sb.Append(s.Substring(1).ToLower());
                    sb.Append(" ");
                }
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Removes multiple spaces between words
        /// </summary>
        /// <param name="input">The string to trim.</param>
        /// <returns>A string.</returns>
        public static string TrimIntraWords(this string input)
        {
            Regex regEx = new Regex(@"[\s]+");
            return regEx.Replace(input, " ");
        }

        /// <summary>
        /// Converts new line(\n) and carriage return(\r) symbols to
        /// HTML line breaks.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>A string.</returns>
        public static string NewLineToBreak(this string input)
        {
            Regex regEx = new Regex(@"[\n|\r]+");
            return regEx.Replace(input, "<br />");
        }

        /// <summary>
        /// Wraps the passed string up the 
        /// until the next whitespace on or after the total charCount has been reached
        /// for that line.  Uses the environment new line
        /// symbol for the break text.
        /// </summary>
        /// <param name="input">The string to wrap.</param>
        /// <param name="charCount">The number of characters per line.</param>
        /// <returns>A string.</returns>
        public static string WordWrap(this string input, int charCount)
        {
            return input.WordWrap(charCount, false, Environment.NewLine);
        }

        /// <summary>
        /// Wraps the passed string up the total number of characters (if cuttOff is true)
        /// or until the next whitespace (if cutOff is false).  Uses the environment new line
        /// symbol for the break text.
        /// </summary>
        /// <param name="input">The string to wrap.</param>
        /// <param name="charCount">The number of characters per line.</param>
        /// <param name="cutOff">If true, will break in the middle of a word.</param>
        /// <returns>A string.</returns>
        public static string WordWrap(this string input, int charCount, bool cutOff)
        {
            return input.WordWrap(charCount, cutOff, Environment.NewLine);
        }

        /// <summary>
        /// Wraps the passed string up the total number of characters (if cuttOff is true)
        /// or until the next whitespace (if cutOff is false).  Uses the passed breakText
        /// for lineBreaks.
        /// </summary>
        /// <param name="input">The string to wrap.</param>
        /// <param name="charCount">The number of characters per line.</param>
        /// <param name="cutOff">If true, will break in the middle of a word.</param>
        /// <param name="breakText">The line break text to use.</param>
        /// <returns>A string.</returns>
        public static string WordWrap(this string input, int charCount, bool cutOff,
            string breakText)
        {
            StringBuilder sb = new StringBuilder(input.Length + 100);
            int counter = 0;

            if (cutOff)
            {
                while (counter < input.Length)
                {
                    if (input.Length > counter + charCount)
                    {
                        sb.Append(input.Substring(counter, charCount));
                        sb.Append(breakText);
                    }
                    else
                    {
                        sb.Append(input.Substring(counter));
                    }
                    counter += charCount;
                }
            }
            else
            {
                string[] strings = input.Split(' ');
                for (int i = 0; i < strings.Length; i++)
                {
                    counter += strings[i].Length + 1; // the added one is to represent the inclusion of the space.
                    if (i != 0 && counter > charCount)
                    {
                        sb.Append(breakText);
                        counter = 0;
                    }

                    sb.Append(strings[i] + ' ');
                }
            }
            return sb.ToString().TrimEnd(); // to get rid of the extra space at the end.
        }



        /// <summary>
        /// Function to test for Positive Integers. 
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns></returns>
        public static bool IsNaturalNumber(this String strNumber)
        {
            Regex objNotNaturalPattern = new Regex("[^0-9]");
            Regex objNaturalPattern = new Regex("0*[1-9][0-9]*");
            return !objNotNaturalPattern.IsMatch(strNumber) &&
            objNaturalPattern.IsMatch(strNumber);
        }


        /// <summary>
        /// Function to test for Positive Integers with zero inclusive  
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns></returns>
        public static bool IsWholeNumber(this String strNumber)
        {
            Regex objNotWholePattern = new Regex("[^0-9]");
            return !objNotWholePattern.IsMatch(strNumber);
        }


        /// <summary>
        /// Function to Test for Integers both Positive & Negative  
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns></returns>
        public static bool IsInteger(this String strNumber)
        {
            Regex objNotIntPattern = new Regex("[^0-9-]");
            Regex objIntPattern = new Regex("^-[0-9]+$|^[0-9]+$");
            return !objNotIntPattern.IsMatch(strNumber) && objIntPattern.IsMatch(strNumber);
        }


        /// <summary>
        /// Function to Test for Positive Number both Integer & Real  
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns></returns>
        public static bool IsPositiveNumber(this String strNumber)
        {
            Regex objNotPositivePattern = new Regex("[^0-9.]");
            Regex objPositivePattern = new Regex("^[.][0-9]+$|[0-9]*[.]*[0-9]+$");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            return !objNotPositivePattern.IsMatch(strNumber) &&
            objPositivePattern.IsMatch(strNumber) &&
            !objTwoDotPattern.IsMatch(strNumber);
        }


        /// <summary>
        /// Function to test whether the string is valid number or not
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns></returns>
        public static bool isNumber(this String strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.,-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.,][0-9]*[.,][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.,]|[-.,]|[0-9])[0-9]*[.,]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");
            return !objNotNumberPattern.IsMatch(strNumber) &&
            !objTwoDotPattern.IsMatch(strNumber) &&
            !objTwoMinusPattern.IsMatch(strNumber) &&
            objNumberPattern.IsMatch(strNumber) && (strNumber != "-") & (strNumber != "+");
        }

        /// <summary>
        /// Function To test for Alphabets. 
        /// </summary>
        /// <param name="strToCheck"></param>
        /// <returns></returns>
        public static bool IsAlpha(this String strToCheck)
        {
            Regex objAlphaPattern = new Regex("[^a-zA-Z]");
            return !objAlphaPattern.IsMatch(strToCheck);
        }

        /// <summary>
        /// Function to Check for AlphaNumeric. 
        /// </summary>
        /// <param name="strToCheck"></param>
        /// <returns></returns>
        public static bool IsAlphaNumeric(this String strToCheck)
        {
            Regex objAlphaNumericPattern = new Regex("[^a-zA-Z0-9]");
            return !objAlphaNumericPattern.IsMatch(strToCheck);
        }


        public static bool IsValidEmail(this string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

    }
}
