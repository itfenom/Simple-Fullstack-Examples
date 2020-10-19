using System;
using System.Data;

namespace Playground.Core.Utilities
{
    public static class DataRowExtensions
    {
        public static bool GetBool(this DataRow row, string columnName)
        {
            return Convert.ToBoolean(row[columnName]);
        }

        public static bool GetBool(this DataRow row, string columnName, DataRowVersion version)
        {
            return Convert.ToBoolean(row[columnName, version]);
        }

        public static DateTime GetDateTime(this DataRow row, string columnName)
        {
            return Convert.ToDateTime(row[columnName]);
        }

        public static DateTime GetDateTime(this DataRow row, string columnName, DataRowVersion version)
        {
            return Convert.ToDateTime(row[columnName, version]);
        }

        public static decimal GetDecimal(this DataRow row, string columnName)
        {
            return Convert.ToDecimal(row[columnName]);
        }

        public static decimal GetDecimal(this DataRow row, string columnName, DataRowVersion version)
        {
            return Convert.ToDecimal(row[columnName, version]);
        }

        public static double GetDouble(this DataRow row, string columnName)
        {
            return Convert.ToDouble(row[columnName]);
        }

        public static double GetDouble(this DataRow row, string columnName, DataRowVersion version)
        {
            return Convert.ToDouble(row[columnName, version]);
        }

        public static int GetInt(this DataRow row, string columnName)
        {
            return Convert.ToInt32(row[columnName]);
        }

        public static int GetInt(this DataRow row, string columnName, DataRowVersion version)
        {
            return Convert.ToInt32(row[columnName, version]);
        }

        public static string GetString(this DataRow row, string columnName)
        {
            return Convert.ToString(row[columnName]);
        }

        public static string GetString(this DataRow row, string columnName, DataRowVersion version)
        {
            return Convert.ToString(row[columnName, version]);
        }

        public static void SetBool(this DataRow row, string columnName, bool value)
        {
            row[columnName] = value;
        }

        public static void SetDateTime(this DataRow row, string columnName, DateTime value)
        {
            row[columnName] = value;
        }

        public static void SetDecimal(this DataRow row, string columnName, decimal value)
        {
            row[columnName] = value;
        }

        public static void SetDouble(this DataRow row, string columnName, double value)
        {
            row[columnName] = value;
        }

        public static void SetInt(this DataRow row, string columnName, int value)
        {
            row[columnName] = value;
        }

        public static void SetString(this DataRow row, string columnName, string value)
        {
            row[columnName] = value;
        }
    }
}
