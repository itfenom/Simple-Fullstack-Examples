using System;
using System.Text;

namespace Playground.Core.Utilities
{
    public class StringOperations
    {
        public static string RemoveLastIndexOfCharacter(string inputString, char characterToRemove)
        {
            return inputString.Remove(inputString.LastIndexOf(characterToRemove), 1);
        }
        public static string CountCharacterInString(string inputVal)
        {
            var retVal = new StringBuilder();
            var value = inputVal;
            var length = Convert.ToInt32(value.Length);

            while (length != 0)
            {
                char character = value[0];
                int count = CharCounter(character, value);
                value = value.Replace(character.ToString(), "");
                retVal.Append("'" + character + "'=" + count + " times, ");
                length = length - count;
            }

            return retVal.ToString();
        }

        private static int CharCounter(char c, string s)
        {
            int retVal = 0;
            foreach (char chr in s)
            {
                if (chr == c)
                {
                    retVal++;
                }
            }

            return retVal;
        }

        public static string RemoveSpecialCharacters(string inputString)
        {
            char[] specialCharacters = {'`', '~', '!', '@', '#', '$', '%', '^', '&', '(',
                                                       ')', '_', '-', '=', '+', '|', '\\', '{', '}',
                                                       '[', ']', '"', '\'', ':', ';', '<', '>', '?',
                                                       '/', ',', '.'};
            foreach (char chr in specialCharacters)
            {
                inputString = inputString.Replace(chr.ToString(), "");
            }

            return inputString.Trim();
        }

        public static bool HasSpecialCharacters(string inputString)
        {
            bool retVal = false;
            char[] specialCharacters = {'`', '~', '!', '@', '#', '$', '%', '^', '&', '(',
                                                       ')', '_', '-', '=', '+', '|', '\\', '{', '}',
                                                       '[', ']', '"', '\'', ':', ';', '<', '>', '?',
                                                       '/', ','};
            for (int i = 0; i < inputString.Length; i++)
            {
                if (inputString.Contains(specialCharacters[i].ToString()))
                {
                    retVal = true;
                    break;
                }
            }
            return retVal;
        }
    }
}
