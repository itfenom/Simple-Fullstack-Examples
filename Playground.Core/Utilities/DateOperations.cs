using System;
using System.Text;

namespace Playground.Core.Utilities
{
    public class DateOperations
    {
        public static string DisplayDate(DateTime value)
        {
            string str = "";
            if (string.CompareOrdinal(value.ToString("MM/dd/yyyy HH:mm:ss"), DateTime.MinValue.ToString("MM/dd/yyyy HH:mm:ss")) != 0 & string.CompareOrdinal(value.ToString("MM/dd/yyyy HH:mm:ss"), DateTime.MaxValue.ToString("MM/dd/yyyy HH:mm:ss")) != 0)
                str = value.ToString("MM/dd/yyyy");
            return str;
        }

        public static string DisplayDateTime(DateTime value)
        {
            string str = "";
            if (string.CompareOrdinal(value.ToString("MM/dd/yyyy HH:mm:ss"), DateTime.MinValue.ToString("MM/dd/yyyy HH:mm:ss")) != 0 & string.CompareOrdinal(value.ToString("MM/dd/yyyy HH:mm:ss"), DateTime.MaxValue.ToString("MM/dd/yyyy HH:mm:ss")) != 0)
                str = value.ToString("MM/dd/yyyy HH:mm:ss");
            return str;
        }

        public static DateTime CalculateMonthEnd(DateTime dtDate)
        {
            StringBuilder builder = new StringBuilder();
            DateTime time3 = dtDate.AddMonths(1);
            builder.AppendFormat("{0}/1/{1}", time3.Month, time3.Year);
            return Convert.ToDateTime(builder.ToString()).AddDays(-1.0);
        }

        public static string ConvertToMonth(byte bytValue)
        {
            EnumMonth month = (EnumMonth)bytValue;
            return month.ToString();
        }

        public enum EnumMonth : byte
        {
            April = 4,
            August = 8,
            December = 12,
            February = 2,
            January = 1,
            July = 7,
            June = 6,
            March = 3,
            May = 5,
            November = 11,
            October = 10,
            September = 9
        }
    }
}
