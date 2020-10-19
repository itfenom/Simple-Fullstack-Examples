using System;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using Playground.Core.Diagnostics;
using Playground.Winforms.HelperForms;
using Playground.Winforms.Utilities;

// ReSharper disable once IdentifierTypo
namespace Playground.Winforms
{
    public static class ProgramExceptionManager
    {
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            DealWithUnhandledException((Exception) e.ExceptionObject,
                $"{nameof(ProgramExceptionManager)}.{nameof(CurrentDomain_UnhandledException)}().");
        }

        public static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            DealWithUnhandledException(e.Exception,
                $"{nameof(ProgramExceptionManager)}.{nameof(Application_ThreadException)}().");
        }

        public static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            DealWithUnhandledException(e.Exception,
                $"{nameof(ProgramExceptionManager)}.{nameof(OnDispatcherUnhandledException)}().");
        }

        public static void DealWithUnhandledException(Exception exception, string context = null)
        {
            try
            {
                Logger.Error(exception.Message, exception);

                EmailManager.EmailToTeam(
                    (context == null ? string.Empty : "This is being sent from " + context)
                    + "\n\nException Details:"
                    + "\n" + exception);

                ExceptionForm form = new ExceptionForm();
                form.Exception = exception;
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            finally
            {
                FileCleanup.StopThread();
                AppShutDownChecker.StopThread();
                Application.Exit();
            }

            Environment.Exit(-1);
        }
    }
}
