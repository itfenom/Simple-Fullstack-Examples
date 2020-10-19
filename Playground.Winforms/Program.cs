using System;
using System.Windows.Forms;
using Playground.Core;
using Playground.Winforms.Forms;
using Playground.Winforms.HelperForms;

namespace Playground.Winforms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            var shutDownStatus = CoreConfig.GetAppShotDownStatus();
            if (shutDownStatus.Item1)
            {
                MessageBox.Show("Application is undergoing maintenance. Please check back again later.");
                Environment.Exit(-1);
                return;
            }

            CoreConfig.UseDevDatabase = () => false;

            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += ProgramExceptionManager.Application_ThreadException;

            // Set the unhandled exception mode to force all Windows Forms errors to go through our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event.
            AppDomain.CurrentDomain.UnhandledException += ProgramExceptionManager.CurrentDomain_UnhandledException;

            //Initialize callback functions in .Core project.
            SetupMessageBox();
            SetupMessageBoxWithTextBox();

            //Setup Application related folder structure
            Core.CoreConfig.CreateApplicationRelatedFolders("Playground.Winforms");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        private static void SetupMessageBox()
        {
            // ReSharper disable once RedundantLambdaParameterType
            Core.MessageBoxWrappers.MessageBox.ShowText = (string text) =>
            {
                // ReSharper disable once ConvertToLambdaExpression
                // ReSharper disable once RedundantNameQualifier
                return (Core.MessageBoxWrappers.DialogResult)(int)System.Windows.Forms.MessageBox.Show(new Form { TopMost = true }, text);
            };

            Core.MessageBoxWrappers.MessageBox.ShowTextCaption = (string text, string caption) =>
            {
                // ReSharper disable once ConvertToLambdaExpression
                return (Core.MessageBoxWrappers.DialogResult)(int)System.Windows.Forms.MessageBox.Show(new Form { TopMost = true }, text, caption);
            };

            Core.MessageBoxWrappers.MessageBox.ShowTextCaptionButtons = (string text, string caption, Core.MessageBoxWrappers.MessageBoxButtons buttons) =>
            {
                // ReSharper disable once ConvertToLambdaExpression
                return (Core.MessageBoxWrappers.DialogResult)(int)System.Windows.Forms.MessageBox.Show
                (
                    new Form { TopMost = true },
                    text,
                    caption,
                    (System.Windows.Forms.MessageBoxButtons)(int)buttons
                );
            };

            Core.MessageBoxWrappers.MessageBox.ShowTextCaptionButtonsIcon = (string text, string caption, Core.MessageBoxWrappers.MessageBoxButtons buttons, Core.MessageBoxWrappers.MessageBoxIcon icon) =>
            {
                // ReSharper disable once ConvertToLambdaExpression
                return (Core.MessageBoxWrappers.DialogResult)(int)System.Windows.Forms.MessageBox.Show
                (
                    new Form { TopMost = true },
                    text,
                    caption,
                    (System.Windows.Forms.MessageBoxButtons)(int)buttons,
                    (System.Windows.Forms.MessageBoxIcon)(int)icon
                 );
            };
        }
        
        private static void SetupMessageBoxWithTextBox()
        {
            Core.MessageBoxWrappers.MessageBoxWithTextBox.DisplayText = (string text) =>
            {
                // ReSharper disable once ConvertToLambdaExpression
                return (Core.MessageBoxWrappers.DialogResult)(int)MessageBoxWithTextBox.Show(text);
            };

            Core.MessageBoxWrappers.MessageBoxWithTextBox.DisplayTextCaption = (string text, string caption) =>
            {
                // ReSharper disable once ConvertToLambdaExpression
                return (Core.MessageBoxWrappers.DialogResult)(int)MessageBoxWithTextBox.Show(text, caption);
            };

            Core.MessageBoxWrappers.MessageBoxWithTextBox.DisplayTextCaptionButtons = (string text, string caption, Core.MessageBoxWrappers.MessageBoxButtons buttons) =>
            {
                // ReSharper disable once ConvertToLambdaExpression
                return (Core.MessageBoxWrappers.DialogResult)(int)MessageBoxWithTextBox.Show
                (
                    text,
                    caption,
                    (MessageBoxButtons)(int)buttons
                );
            };

            Core.MessageBoxWrappers.MessageBoxWithTextBox.DisplayTextCaptionButtonsIcon = (string text, string caption, Core.MessageBoxWrappers.MessageBoxButtons buttons, Core.MessageBoxWrappers.MessageBoxIcon icon) =>
            {
                // ReSharper disable once ConvertToLambdaExpression
                return (Core.MessageBoxWrappers.DialogResult)(int)MessageBoxWithTextBox.Show
                (
                    text,
                    caption,
                    (MessageBoxButtons)(int)buttons,
                    (MessageBoxIcon)(int)icon
                 );
            };
        }

    }
}
