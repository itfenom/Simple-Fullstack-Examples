using Playground.Core.Diagnostics;
using Playground.MultiThreadedService.Base;
using System;
using System.ServiceProcess;
using Playground.MultiThreadedService.Threads;

namespace Playground.MultiThreadedService
{
    partial class ServiceHost : WindowsService
    {
        public ServiceHost()
        {
            InitializeComponent();

            DisplayName = ServiceConfig.ServiceName;
            ServiceName = ServiceConfig.ServiceName;
        }

        private static void Main(string[] args)
        {
            try
            {
                Core.CoreConfig.CreateApplicationRelatedFolders("Playground.MultiThreadedService");
                Logger.Info(ServiceConfig.ServiceName + " started... [Version: " + ServiceConfig.Version + "]");

                //Initialize callback functions in Seraph.Core project.
                InitializeCoreProjectCallBackFunc();

                // Initialize all worker threads
                var heartBeatThread = new HeartBeatThread
                {
                    Name = ServiceConfig.HeartbeatThreadName
                };
                heartBeatThread.SetThreadName();
                heartBeatThread.Delay = ServiceConfig.HeartbeatThreadDelay * 1000;
                heartBeatThread.Interval = ServiceConfig.HeartbeatThreadInterval * 60 * 1000;

                var mainWorkerThread = new MainWorkerThread(heartBeatThread)
                {
                    Name = ServiceConfig.MainThreadName
                };
                mainWorkerThread.SetThreadName();
                mainWorkerThread.Delay = ServiceConfig.MainThreadDelay;
                mainWorkerThread.Interval = ServiceConfig.MainThreadInterval * 60 * 1000;

                //Initialize service
                WindowsService service = new WindowsService();
                service.ServiceName = ServiceConfig.ServiceName;

                //Associate 2 worker threads {Main Thread and Heartbeat thread} to Windows service.
                service.WorkerThreads = new WorkerThread[]
                 {
                     mainWorkerThread,
                     heartBeatThread,
                 };

                // Add services to an array.
                ServiceBase[] services = new ServiceBase[]
                {
                     service
                };

                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += UnknownExceptionHandler;

                // Launch service(s).
                Run(services, args);
            }
            catch (Exception ex)
            {
                Core.CoreConfig.CreateLogsLocally(ex.Message);
                Core.CoreConfig.CreateLogsLocally(ex.StackTrace);

                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Core.CoreConfig.CreateLogsLocally(innerEx.Message);

                    Core.CoreConfig.CreateLogsLocally(innerEx.StackTrace);
                    innerEx = innerEx.InnerException;
                }

                Logger.Error("Error during service execution", ex);
                EmailManager.EmailToTeam("Error during service execution in Playground.MultiThreadedService Project.");
            }
        }

        private static void UnknownExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;
            Logger.Error(exception.Message, exception);
            EmailManager.EmailToTeam("An unhandled exception occurred in Playground.MultiThreadedService Project." + exception.Message);
        }

        private static void InitializeCoreProjectCallBackFunc()
        {
            //Set database flag (Dev or Production)
            Core.CoreConfig.UseDevDatabase = () => false;

            //MessageBoxes
            // ReSharper disable once RedundantLambdaParameterType
            Core.MessageBoxWrappers.MessageBox.ShowText = (string text) =>
            {
                Logger.Info("MessageBox.Show:\n" + text);

                return Core.MessageBoxWrappers.DialogResult.Ok;
            };

            // ReSharper disable once RedundantLambdaParameterType
            Core.MessageBoxWrappers.MessageBox.ShowTextCaption = (string text, string caption) =>
            {
                Logger.Info("MessageBox.Show:\n" + text);

                return Core.MessageBoxWrappers.DialogResult.Ok;
            };

            // ReSharper disable once RedundantLambdaParameterType
            Core.MessageBoxWrappers.MessageBox.ShowTextCaptionButtons = (string text, string caption, Core.MessageBoxWrappers.MessageBoxButtons buttons) =>
            {
                Logger.Info("MessageBox.Show:\n" + text);

                return GetMessageBoxResponse(buttons);
            };

            // ReSharper disable once RedundantLambdaParameterType
            Core.MessageBoxWrappers.MessageBox.ShowTextCaptionButtonsIcon = (string text, string caption, Core.MessageBoxWrappers.MessageBoxButtons buttons, Core.MessageBoxWrappers.MessageBoxIcon icon) =>
            {
                Logger.Info("MessageBox.Show:\n" + text);

                return GetMessageBoxResponse(buttons);
            };

            // ReSharper disable once RedundantLambdaParameterType
            Core.MessageBoxWrappers.MessageBox.ShowTextCaptionButtons = (string text, string caption, Core.MessageBoxWrappers.MessageBoxButtons buttons) =>
            {
                Logger.Info("MessageBox.Show:\n" + text);

                return GetMessageBoxResponse(buttons);
            };

            // ReSharper disable once RedundantLambdaParameterType
            Core.MessageBoxWrappers.MessageBox.ShowTextCaptionButtonsIcon = (string text, string caption, Core.MessageBoxWrappers.MessageBoxButtons buttons, Core.MessageBoxWrappers.MessageBoxIcon icon) =>
            {
                Logger.Info("MessageBox.Show:\n" + text);

                return GetMessageBoxResponse(buttons);
            };
        }

        private static Core.MessageBoxWrappers.DialogResult GetMessageBoxResponse(Core.MessageBoxWrappers.MessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case Core.MessageBoxWrappers.MessageBoxButtons.AbortRetryIgnore:
                    return Core.MessageBoxWrappers.DialogResult.Ignore;

                case Core.MessageBoxWrappers.MessageBoxButtons.Ok:
                case Core.MessageBoxWrappers.MessageBoxButtons.OkCancel:
                    return Core.MessageBoxWrappers.DialogResult.Ok;

                case Core.MessageBoxWrappers.MessageBoxButtons.RetryCancel:
                    return Core.MessageBoxWrappers.DialogResult.Cancel;

                case Core.MessageBoxWrappers.MessageBoxButtons.YesNo:
                case Core.MessageBoxWrappers.MessageBoxButtons.YesNoCancel:
                    return Core.MessageBoxWrappers.DialogResult.Yes;

                default:
                    return Core.MessageBoxWrappers.DialogResult.Ok;
            }
        }
    }
}
