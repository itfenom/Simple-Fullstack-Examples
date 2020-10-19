using System;
using System.Configuration;
using Playground.Core.Diagnostics;
using System.Reflection;

namespace Playground.MultiThreadedService
{
    public class ServiceConfig
    {
        public static string Version => GetVersion();

        #region Service Settings from App.config

        public static string ServiceName => (GetConfigurationString("Service.Name")).Trim();

        public static string ServiceDescription => (GetConfigurationString("Service.Description")).Trim();

        public static string MainThreadName => (GetConfigurationString("Service.Thread.Main")).Trim();

        public static int MainThreadDelay => GetConfigurationInt32("Service.Thread.Main.Delay");

        public static int MainThreadInterval => GetConfigurationInt32("Service.Thread.Main.Interval");

        public static string HeartbeatThreadName => (GetConfigurationString("Service.Thread.Heartbeat")).Trim();

        public static int HeartbeatThreadDelay => GetConfigurationInt32("Service.Thread.Heartbeat.Delay");

        public static int HeartbeatThreadInterval => GetConfigurationInt32("Service.Thread.Heartbeat.Interval");

        public static int FileCleanupInterval => GetConfigurationInt32("FileCleanup.Interval");

        #endregion Service Settings from App.config

        #region Global Variables

        public static TimeSpan TempFileCleanUpInterval = new TimeSpan(FileCleanupInterval, 0, 0); //4 hours interval
        public static DateTime NextTempFileCleanUpTime = DateTime.Now + TempFileCleanUpInterval;
        public static DateTime NextLogsCleanUpTime = DateTime.Now - (new TimeSpan(2, 0, 0)); //Intentionally setting this time to past 2 hours so that we could run it at-least once at startup by comparing it to current time.

        #endregion Global Variables

        #region Methods

        private static string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
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
                string msg = "Configuration Error: Invalid on non-existent configuration parameter for '" + parameter + "': " + ConfigurationManager.AppSettings[parameter];
                Logger.Error($"{ServiceName}: {msg}", ex);
                throw new Exception(msg, ex);
            }
            if ((value == null) || (value.Length == 0))
            {
                string msg = "Configuration Error: Invalid on non-existent configuration parameter for '" + parameter + "': " + ConfigurationManager.AppSettings[parameter];
                Logger.Error($"{ServiceName}: {msg}");
                throw new Exception(msg);
            }
            return value;
        }

        public static string GetConfigurationStringEmptyAllowed(string parameter)
        {
            string value;
            try
            {
                value = ConfigurationManager.AppSettings[parameter];
            }
            catch (Exception ex)
            {
                string msg = "Configuration Error: Invalid on non-existent configuration parameter for '" + parameter + "': " + ConfigurationManager.AppSettings[parameter];
                Logger.Error($"{ServiceName}: {msg}", ex);
                throw new Exception(msg, ex);
            }
            if (value == null)
            {
                string msg = "Configuration Error: Invalid on non-existent configuration parameter for '" + parameter + "': " + ConfigurationManager.AppSettings[parameter];
                Logger.Error($"{ServiceName}: {msg}");
                throw new Exception(msg);
            }
            return value;
        }

        public static bool GetConfigurationBool(string parameter)
        {
            bool value;
            try
            {
                value = String.Compare(ConfigurationManager.AppSettings[parameter], "true", StringComparison.Ordinal) == 0;
            }
            catch (Exception ex)
            {
                string msg = "Configuration Error: Invalid on non-existent configuration parameter for '" + parameter + "': " + ConfigurationManager.AppSettings[parameter];
                Logger.Error($"{ServiceName}: {msg}", ex);
                throw new Exception(msg, ex);
            }
            return value;
        }

        public static Int32 GetConfigurationInt32(string parameter)
        {
            Int32 value;
            try
            {
                value = Int32.Parse(ConfigurationManager.AppSettings[parameter]);
            }
            catch (Exception ex)
            {
                string msg = "Configuration Error: Invalid on non-existent configuration parameter for '" + parameter + "': " + ConfigurationManager.AppSettings[parameter];
                Logger.Error($"{ServiceName}: {msg}", ex);
                throw new Exception(msg, ex);
            }
            return value;
        }

        #endregion Methods
    }
}
