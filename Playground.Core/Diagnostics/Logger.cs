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
    public class DILogger
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public const string LogPattern = "%date [%thread] %-5level - %message%newline";

        public string DefaultPattern
        {
            get { return LogPattern; }
        }

        public DILogger(string fileName = "PlaygroundLog")
        {

            DefaultLayout.ConversionPattern = DefaultPattern;
            DefaultLayout.ActivateOptions();

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            TraceAppender tracer = new TraceAppender();
            PatternLayout patternLayout = new PatternLayout();

            patternLayout.ConversionPattern = LogPattern;
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
                File = Path.Combine(CoreConfig.LogsDirectory, fileName)
            };

            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;
        }

        public PatternLayout DefaultLayout { get; } = new PatternLayout();

        public void AddAppender(IAppender appender)
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.AddAppender(appender);
        }

        public ILog Create()
        {
            return LogManager.GetLogger("PlaygroundLog");
        }

        public bool IsDebugEnabled => _log.IsDebugEnabled;

        public bool IsInfoEnabled => _log.IsInfoEnabled;

        public bool IsWarnEnabled => _log.IsWarnEnabled;

        public bool IsErrorEnabled => _log.IsErrorEnabled;

        public void Debug(string msg)
        {
            if (_log.IsDebugEnabled)
            {
                _log.Debug(msg);
            }
        }

        public void Info(string msg)
        {
            if (_log.IsInfoEnabled)
            {
                _log.Info(msg);
            }
        }

        public void Warn(string msg)
        {
            if (_log.IsWarnEnabled)
            {
                _log.Warn(msg);
            }
        }

        public void Error(string msg)
        {
            if (_log.IsErrorEnabled)
            {
                _log.Error(msg);
            }
        }

        public void Error(string msg, Exception ex)
        {
            if (_log.IsErrorEnabled)
            {
                _log.Error(msg, ex);
            }
        }
    }

    public class NonNullReference<T> where T : class
    {
        private Func<T> _defaultCreator;
        private T _theReference;
        // because the NonNullReference<T> may be a static object, we're going to add a double check lock during default construction.
        private object _theLock = new object();

        public T Ref
        {
            get
            {
                if (_theReference == null)
                {
                    lock (_theLock)
                    {
                        if (_theReference == null)
                        {
                            _theReference = _defaultCreator();
                        }
                    }
                }

                return _theReference;
            }

            set
            {
                _theReference = value;
            }
        }

        public NonNullReference(Func<T> defaultCreator)
        {
            _defaultCreator = defaultCreator;
        }
    }

    public class Logger
    {
        private static NonNullReference<DILogger> _logger = new NonNullReference<DILogger>(() => new DILogger());

        public string DefaultPattern
        {
            get { return DILogger.LogPattern; }
        }

        public static void Initialize(DILogger injectedDiLogger = null)
        {
            _logger.Ref = injectedDiLogger;
        }

        public static PatternLayout DefaultLayout => _logger.Ref.DefaultLayout;

        public static void AddAppender(IAppender appender)
        {
            _logger.Ref.AddAppender(appender);
        }

        public static ILog Create()
        {
            return _logger.Ref.Create();
        }

        public static bool IsDebugEnabled => _logger.Ref.IsDebugEnabled;

        public static bool IsInfoEnabled => _logger.Ref.IsInfoEnabled;

        public static bool IsWarnEnabled => _logger.Ref.IsWarnEnabled;

        public static bool IsErrorEnabled => _logger.Ref.IsErrorEnabled;

        public static void Debug(string msg)
        {
            if (_logger.Ref.IsDebugEnabled)
            {
                _logger.Ref.Debug(msg);
            }
        }

        public static void Info(string msg)
        {
            if (_logger.Ref.IsInfoEnabled)
            {
                _logger.Ref.Info(msg);
            }
        }

        public static void Warn(string msg)
        {
            if (_logger.Ref.IsWarnEnabled)
            {
                _logger.Ref.Warn(msg);
            }
        }

        public static void Error(string msg)
        {
            if (_logger.Ref.IsErrorEnabled)
            {
                _logger.Ref.Error(msg);
            }
        }

        public static void Error(string msg, Exception ex)
        {
            if (_logger.Ref.IsErrorEnabled)
            {
                _logger.Ref.Error(msg, ex);
            }
        }
    }

    /* Old class
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
    */
}
