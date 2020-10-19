using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;
using Playground.Core;
using Playground.Core.Diagnostics;

namespace Playground.WpfApp
{
    public partial class App
    {
        private void InitializeAndShowSplashScreen()
        {
            // set up the Dependency Injection container here
            //TODO
            
            /*
            //1:show splash screen
            const int minimumSplashTime = 2000; //2000 Milliseconds = 2 Seconds
            var viewModel = new SplashScreenViewModel();
            var view = new SplashScreenView {DataContext = viewModel};
            view.Show();

            //2: start the timer for splash screen
            Stopwatch timer = new Stopwatch();
            timer.Start();

            //3: Stop the timer and check how long to wait before showing the main window
            timer.Stop();
            */

            #region Initialize settings in the meantime

            //start the log file
            CoreConfig.CreateApplicationRelatedFolders("Playground.WpfApp");
            Logger.Info("Starting application " + ApplicationInfo.ProductName + " " + ApplicationInfo.Version + " ...");

            //set boolean flag in .Core Project
            CoreConfig.UseDevDatabase = () => false;

            #endregion Initialize settings in the meantime

            /*
            int remainingTimeToShowSplash = minimumSplashTime - (int)timer.ElapsedMilliseconds;
            if (remainingTimeToShowSplash > 0)
            {
                Thread.Sleep(remainingTimeToShowSplash);
            }

            //4: Close Splash Window
            view.Close();
            */

            //Show main window
            var mainView = new Forms.Main.MainWindowView { DataContext = new Forms.Main.MainWindowViewModel(DialogCoordinator.Instance) };
            mainView.Show();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            //Handle exceptions
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += (sender, args) => TaskSchedulerOnUnobservedTaskException(args);

            InitializeAndShowSplashScreen();
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            DisplayException(e.Exception);
        }

        private void TaskSchedulerOnUnobservedTaskException(UnobservedTaskExceptionEventArgs args)
        {
            DisplayException(args.Exception);
            args.SetObserved();
        }

        private void DisplayException(Exception ex)
        {
            Logger.Error(ex.Message, ex);
            var unhandledExceptionWindow = new UnhandledExceptionWindow(ex);
            unhandledExceptionWindow.ShowDialog();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Current.DispatcherUnhandledException -= Current_DispatcherUnhandledException;
            Logger.Info("Exiting application Playground.WpfApp");
            base.OnExit(e);
        }
    }
}
