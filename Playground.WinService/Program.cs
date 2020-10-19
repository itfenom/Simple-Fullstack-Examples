using Playground.Core.Diagnostics;
using Playground.WinService.Common;
using System;
using System.Windows.Forms;

namespace Playground.WinService
{
    internal static class Program
    {
        public static readonly string ClassName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType?.ToString();

        public const int ModeDebug = 1;
        public const int ModeRunOnce = 2;
        public const int ModeService = 4;

        public const string CmdlineValueDebug = "Debug";
        public const string CmdlineValueRunOnce = "RunOnce";
        public const string CmdlineValueRunService = "Service";

        private static ClParser _cmdLineArgs = new ClParser();

        public static ClParser CommandLineArguments
        {
            get => (_cmdLineArgs);
            set => _cmdLineArgs = value;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            //Initialize callback functions in Seraph.Core project.
            InitializeCoreProjectCallBackFunc();

            int mode;
            string cmd;

#if DEBUG
            MessageBox.Show("Running in Debug mode!", "Warning");
            mode = ModeDebug;
            cmd = CmdlineValueDebug;
#endif
            Core.CoreConfig.CreateApplicationRelatedFolders("Playground.WinService");
            Logger.Info("Program Started...(Release Date: " + ServiceConfig.ProgramReleaseDate + " Version: " + ServiceConfig.ProgramVersion + ")");

            // Determine what mode program is running in.
            if (_cmdLineArgs[CmdlineValueRunOnce] != null)
            {
                mode = ModeRunOnce;
                cmd = CmdlineValueRunOnce;
            }
            else if (_cmdLineArgs[CmdlineValueRunService] != null)
            {
                mode = ModeService;
                cmd = CmdlineValueRunService;
            }

            Logger.Info("Program Running in '" + cmd + "' mode");

            if (((mode & ModeDebug) == ModeDebug) || ((mode & ModeRunOnce) == ModeRunOnce))
            {
                SingleThreadedService svc = new SingleThreadedService();
                svc.ProcessWork();  						// run now (manually);
                svc.OnStart();  							// use timer;
            }
            else
            {
                var servicesToRun = new System.ServiceProcess.ServiceBase[] { new SingleThreadedService() };
                try
                {
                    System.ServiceProcess.ServiceBase.Run(servicesToRun);
                }
                catch (Exception ex)
                {
                    Logger.Error("Program error during service execution", ex);
                }
            }
        }

        private static void InitializeCoreProjectCallBackFunc()
        {
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

            //MessageBoxWithTextBox
            // ReSharper disable once RedundantLambdaParameterType
            Core.MessageBoxWrappers.MessageBoxWithTextBox.DisplayText = (string text) =>
            {
                Logger.Info("MessageBox.Show:\n" + text);

                return Core.MessageBoxWrappers.DialogResult.Ok;
            };

            // ReSharper disable once RedundantLambdaParameterType
            Core.MessageBoxWrappers.MessageBoxWithTextBox.DisplayTextCaption = (string text, string caption) =>
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
