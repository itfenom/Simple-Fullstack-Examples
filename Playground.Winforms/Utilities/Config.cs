using System;
using System.Configuration;

namespace Playground.Winforms.Utilities
{
    public static class Config
    {
        public static string OracleHost => GetConfigurationString("ORACLE.HOST");

        public static string OraclePort => GetConfigurationString("ORACLE.PORT");

        public static string OracleServiceName => GetConfigurationString("ORACLE.SERVICE.NAME");

        public static string OracleUserId => GetConfigurationString("ORACLE.USER.ID");

        public static string OracleUserPassword => GetConfigurationString("ORACLE.USER.PASSWORD");

        public static string GetOracleConnString()
        {
            return $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={OracleHost})" +
                   $"(PORT={OraclePort}))(CONNECT_DATA=(SERVICE_NAME={OracleServiceName})));User Id={OracleUserId};Password={OracleUserPassword};";
        }

        public static int GetConfigurationInt32(string parameter)
        {
            int value;
            try
            {
                value = int.Parse(ConfigurationManager.AppSettings[parameter]);
            }
            catch (Exception ex)
            {
                var msg = "Configuration Error: Invalid on non-existent configuration parameter for '" + parameter + "': " + ConfigurationManager.AppSettings[parameter];
                throw new Exception(msg, ex);
            }
            return value;
        }

        public static bool GetConfigurationBool(string parameter)
        {
            bool value;
            try
            {
                value = string.Compare(ConfigurationManager.AppSettings[parameter], "true", StringComparison.Ordinal) == 0;
            }
            catch (Exception ex)
            {
                var msg = "Configuration Error: Invalid on non-existent configuration parameter for '" + parameter + "': " + ConfigurationManager.AppSettings[parameter];
                throw new Exception(msg, ex);
            }
            return value;
        }

        public static string GetConfigurationString(string parameter)
        {
            string value;
            try
            {
                value = ConfigurationManager.AppSettings[parameter];
            }
            catch (Exception ex)
            {
                var msg = "Configuration Error: Invalid on non-existent configuration parameter for '" + parameter + "': " + ConfigurationManager.AppSettings[parameter];
                throw new Exception(msg, ex);
            }
            if ((value == null) || (value.Length == 0))
            {
                var msg = "Configuration Error: Invalid on non-existent configuration parameter for '" + parameter + "': " + ConfigurationManager.AppSettings[parameter];
                throw new Exception(msg);
            }
            return value;
        }
    }
}
