using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Playground.Core.Utilities
{
    public static class HelperTools
    {
        public static string ExceptionToString(Exception e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("An exception of type " + e.GetType().Name + "occurred.");
            sb.AppendLine();
            sb.AppendLine("Message: " + e.Message);
            sb.AppendLine();
            sb.AppendLine("StackTrace:");
            sb.Append(e.StackTrace);
            sb.AppendLine();

            e = e.InnerException;

            while (e != null)
            {
                sb.AppendLine();
                sb.AppendLine("That exception contained an InnerException of type " + e.GetType().Name + ".");
                sb.AppendLine();
                sb.AppendLine("Message: " + e.Message);
                sb.AppendLine();
                sb.AppendLine("StackTrace:");
                sb.Append(e.StackTrace);
                sb.AppendLine();

                e = e.InnerException;
            }
            sb.AppendLine();

            return sb.ToString();
        }

        public static string FormatSqlString(string sqlString)
        {
            return sqlString.Replace("'", "''");
        }

        public static string RemoveSpecialCharacters(string inputString, string replaceWithCharacter = "")
        {
            char[] specialCharacters = new char[] { '/', '\\', ':', '*', '?', '"', '<', '>', '|' };

            foreach (char chr in specialCharacters)
            {
                inputString = inputString.Replace(chr.ToString(), replaceWithCharacter);
            }

            string result = inputString.Trim();
            return result;
        }

        public static bool HasSpecialCharacters(string inputString)
        {
            char[] specialCharacters = {'`', '~', '!', '@', '#', '$', '%', '^', '&', '(',
                                                       ')', '_', '-', '=', '+', '|', '\\', '{', '}',
                                                       '[', ']', '"', '\'', ':', ';', '<', '>', '?',
                                                       '/', ','};

            return inputString.Where((t, i) => inputString.Contains(specialCharacters[i].ToString())).Any();
        }

        public static bool IsNumeric(string inputStringVal)
        {
            return double.TryParse(inputStringVal, out _);
        }

        public static string Encode(string source)
        {
            var result = source;
            var invalidChars = Path.GetInvalidFileNameChars().ToList();
            invalidChars.Remove('%');
            invalidChars.Insert(0, '%');

            for (int i = 0; i < invalidChars.Count; i++)
            {
                result = result.Replace(invalidChars[i] + "", "%" + $"{Convert.ToInt32(invalidChars[i]):X}");
            }
            return result;
        }

        public static string Decode(string encodedString)
        {
            var result = encodedString;
            var i = 0;
            while (i < result.Length)
            {
                if (result[i] == '%')
                {
                    string hex = result.Substring(i + 1, 2);
                    string hexReplace = Char.ConvertFromUtf32(Convert.ToInt32(hex, 16));
                    result = result.Remove(i, 3);
                    result = result.Insert(i, hexReplace);
                }
                i++;
            }
            return result;
        }

        public static string RemoveLastIndex(string inputString, char characterToRemove)
        {
            string retVal = string.Empty;
            int count = inputString.Split(characterToRemove).Length - 1;

            if (count == 0)
            {
                retVal = inputString;
            }
            else if (count != 0)
            {
                retVal = inputString.Remove(inputString.LastIndexOf(characterToRemove), 1);
            }

            return retVal;
        }

        public static IEnumerable<string> SplitLargeStringIntoSmallerChunks(string str, int maxChunkSize)
        {
            for (var i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        private static IEnumerable<IEnumerable<T>> Chunk<T>(IEnumerable<T> items, int chunkSize)
        {
            return items
                .Select((item, index) => new { Index = index, Item = item })
                .GroupBy(grp => grp.Index / chunkSize)
                .Select(grp => grp.Select(v => v.Item));
        }

        /// <summary>
        /// Sanitize the given string value so that it's safe to use in a "LIKE" query on a data-table.
        /// </summary>
        /// <param name="value">The value to be sanitized.</param>
        /// <returns>The sanitized string.</returns>
        public static string EscapeLikeValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            StringBuilder sb = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                switch (c)
                {
                    case ']':
                    case '[':
                    case '%':
                    case '*':
                        sb.Append("[").Append(c).Append("]");
                        break;

                    case '\'':
                        sb.Append("''");
                        break;

                    default:
                        sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }

        public static void LogEntriesToFileLocally(List<string> instructionList)
        {
            var tempDir = @"C:\Temp";
            var fileName = Path.Combine(tempDir, $@"PlaygroundLogEntries_{DateTime.Now:MM_dd_yyyy_HH_mm_ss}.txt");

            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            if (!File.Exists(fileName))
            {
                using (StreamWriter streamWriter = File.CreateText(fileName))
                {
                    foreach (string instruction in instructionList)
                    {
                        streamWriter.WriteLine(instruction);
                    }
                }
            }
        }

        public static IEnumerable<string> SplitLargeString(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        private static DataTable LinqToDataTable<T>(IEnumerable<T> varlist)
        {
            var retValDt = new DataTable();
            // column names
            PropertyInfo[] oProps = null;

            if (varlist == null) return retValDt;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        retValDt.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = retValDt.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }
                retValDt.Rows.Add(dr);
            }
            return retValDt;
        }

        public static Int32 BoolToInt(params bool[] bools)
        {
            //return bools.Select((x, i) => Convert.ToByte(x) << i).Sum();
            return bools.Select((t, i) => (t ? 1 : 0) << i).Sum();
        }

        public static double ConvertCelsiusToFahrenheit(double c)
        {
            return ((9.0 / 5.0) * c) + 32;
        }

        public static double ConvertFahrenheitToCelsius(double f)
        {
            return (5.0 / 9.0) * (f - 32);
        }

        public static List<List<string>> GetAllCombinationsInCollection(List<List<string>> categories)
        {
            var combinations = new List<List<string>>();

            var loopCounters = new List<int>();
            for (int i = 0; i < categories.Count; i++)
            {
                loopCounters.Add(0);
            }

            bool stillGoing = true;

            while (stillGoing)
            {
                var combination = new List<string>();
                for (int i = 0; i < loopCounters.Count; i++)
                {
                    combination.Add(categories[i][loopCounters[i]]);
                }
                combinations.Add(combination);

                stillGoing = false;
                for (int i = loopCounters.Count - 1; i >= 0; i--)
                {
                    loopCounters[i]++;
                    if (loopCounters[i] < categories[i].Count)
                    {
                        stillGoing = true;
                        break;
                    }

                    loopCounters[i] = 0;
                }
            }

            return combinations;
        }

        public static List<string> RemoveDuplicatesFromCollection(List<string> inputList)
        {
            var finalList = new List<string>();
            foreach (var val in inputList)
            {
                if (!finalList.Contains(val))
                {
                    finalList.Add(val);
                }
            }
            return finalList;
        }
    }
}
