using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Playground.Core.Diagnostics
{
    public class Logger
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string LogPattern = "%date [%thread] %-5level - %message%newline";
        
        public string DefaultPattern => LogPattern;

        public Logger()
        {
            DefaultLayout.ConversionPattern = DefaultPattern;
            DefaultLayout.ActivateOptions();
        }

        public PatternLayout DefaultLayout
        {
            get;
        } = new PatternLayout();

        public void AddAppender(IAppender appender)
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.AddAppender(appender);
        }
        
        public static string CopiedLogsDirectory { get; }

        static Logger()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            TraceAppender tracer = new TraceAppender();
            PatternLayout patternLayout = new PatternLayout
            {
                ConversionPattern = LogPattern
            };

            patternLayout.ActivateOptions();

            tracer.Layout = patternLayout;
            tracer.ActivateOptions();
            hierarchy.Root.AddAppender(tracer);

            RollingFileAppender roller = new RollingFileAppender
            {
                Layout = patternLayout,
                AppendToFile = true,
                RollingStyle = RollingFileAppender.RollingMode.Size,
                MaxSizeRollBackups = Convert.ToInt32(ConfigurationManager.AppSettings["Log4Net.MaxSizeRollBackups"]),
                MaximumFileSize = "10000KB", //[10,000 KB = 10MB]
                StaticLogFileName = true,
                File = Path.Combine(CoreConfig.LogsDirectory, "Playground.log")
        };

            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;
        }
        
        public static ILog Create()
        {
            return LogManager.GetLogger("PlaygroundLog");
        }

        public static bool IsDebugEnabled => Log.IsDebugEnabled;

        public static bool IsInfoEnabled => Log.IsInfoEnabled;

        public static bool IsWarnEnabled => Log.IsWarnEnabled;

        public static bool IsErrorEnabled => Log.IsErrorEnabled;

        public static void Debug(string msg)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(msg);
            }
        }

        public static void Info(string msg)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info(msg);
            }
        }

        public static void Warn(string msg)
        {
            if (Log.IsWarnEnabled)
            {
                Log.Warn(msg);
            }
        }

        public static void Error(string msg)
        {
            if (Log.IsErrorEnabled)
            {
                Log.Error(msg);
            }
        }

        public static void Error(string msg, Exception ex)
        {
            if (Log.IsErrorEnabled)
            {
                Log.Error(msg, ex);
            }
        }
    }
}
