using System;
using System.Configuration;
using Playground.Core.Diagnostics;

namespace Playground.WinService.Common
{
    public class ServiceConfig
    {
        #region Constants

        public const string ProgramVersion = "1.0.1";
        public const string ProgramReleaseDate = "Aug-027-2019";

        #endregion Constants

        #region Service Program Options

        public static bool DebugProgramEnabled => (GetConfigurationBool("Debug.Program"));

        public static string ProgramName => (GetConfigurationString("Program.Name"));

        public static string ServiceName => (GetConfigurationString("Service.Name"));

        public static Int32 ServiceSleeps => (GetConfigurationInt32("Service.Sleeps"));

        public static string SchedulerRunDays => (GetConfigurationString("Scheduler.Run.Days"));

        public static string SchedulerTimeStart => (GetConfigurationString("Scheduler.Time.Start"));

        public static string SchedulerTimeStop => (GetConfigurationString("Scheduler.Time.Stop"));

        public static Int32 SchedulerTimeInterval => (GetConfigurationInt32("Scheduler.Time.Interval"));

        #endregion Service Program Options

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
                Logger.Error(msg, ex);
                throw new Exception(msg, ex);
            }
            if ((value == null) || (value.Length == 0))
            {
                string msg = "Configuration Error: Invalid on non-existent configuration parameter for '" + parameter + "': " + ConfigurationManager.AppSettings[parameter];
                Logger.Error(msg);
                throw new Exception(msg);
            }
            return (value);
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
                Logger.Error(msg, ex);
                throw new Exception(msg, ex);
            }
            if (value == null)
            {
                string msg = "Configuration Error: Invalid on non-existent configuration parameter for '" + parameter + "': " + ConfigurationManager.AppSettings[parameter];
                Logger.Error(msg);
                throw new Exception(msg);
            }
            return (value);
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
                string msg = "Configuration Error: Invalid on non-existent configuration parameter for '" + parameter + "': " + ConfigurationManager.AppSettings[parameter];
                Logger.Error(msg, ex);
                throw new Exception(msg, ex);
            }
            return (value);
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
                Logger.Error(msg, ex);
                throw new Exception(msg, ex);
            }
            return (value);
        }
    }
}
