using System.Text.RegularExpressions;

namespace Playground.Core.Utilities
{
    public class RegExOperations
    {
        public static bool IsPasswordSecured(string inputVal)
        {
            bool retVal = false;
            string regularExpression = @"^(?=.*?\d.*?\d)(?=.*?\w.*?\w)[\d\w]{6,8}$";
            if (Regex.IsMatch(inputVal, regularExpression))
            {
                retVal = true;
            }
            return retVal;
        }

        public static bool HasSpecialCharactersRegEx(string inputString)
        {
            bool retVal = true;
            string str = @"[^\w\.\,!""$%^&*\(\)-_+=::@']";
            Regex specialCharRegEx = new Regex(str);

            if ((specialCharRegEx.Matches(inputString.Trim()).Count) == 0)
            {
                retVal = false;
            }

            return retVal;
        }

        public static bool HasUpperCase(string inputString)
        {
            bool retVal = true;
            Regex upperCase = new Regex("[A-Z]");

            if (upperCase.Matches(inputString.Trim()).Count == 0)
            {
                retVal = false;
            }

            return retVal;
        }

        public static bool HasLowerCase(string inputString)
        {
            bool retVal = true;
            Regex lowerCase = new Regex("[a-z]");

            if (lowerCase.Matches(inputString.Trim()).Count == 0)
            {
                retVal = false;
            }
            return retVal;
        }

        public static bool HasNumber(string inputString)
        {
            bool retVal = true;
            Regex numeric = new Regex("[0-9]");

            if (numeric.Matches(inputString.Trim()).Count == 0)
            {
                retVal = false;
            }
            return retVal;
        }
    }
}
