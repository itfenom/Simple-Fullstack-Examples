using Playground.Winforms.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Playground.Winforms.Utilities
{
    public class AppShutDownChecker
    {
        private static Thread _thread;
        private static Main _mainForm;

        public static Main MainForm
        {
            get => _mainForm;
            set => _mainForm = value;
        }

        private static void ShutDownIfNecessary()
        {
            var status = Core.CoreConfig.GetAppShotDownStatus();
            if(status.Item1)
            {
                var msg = status.Item2;
                if(string.IsNullOrEmpty(msg))
                {
                    msg = "Application is undergoing maintenance. Please check back again later.";
                }

                var userDidConfirm = false;
                Task.Factory.StartNew(() =>
                {
                    _mainForm?.ShowMessageBoxFromAppShutDownCheckerThread(msg);
                    userDidConfirm = true;
                });

                var stopWatch = Stopwatch.StartNew();
                while (!userDidConfirm && stopWatch.Elapsed.TotalMinutes < 1)
                {
                    Thread.Sleep(1000);
                }

                Environment.Exit(-1);
            }
        }

        public static void StartThread()
        {
            if(_thread == null)
            {
                _thread = new Thread(AppShutDownCheckerThread)
                {
                    Name = "App Shut-Down thread."
                };
            }

            if(_thread.ThreadState == System.Threading.ThreadState.Unstarted)
            {
                _thread.Start();
            }
        }

        private static void AppShutDownCheckerThread()
        {
            while (true)
            {
                ShutDownIfNecessary();
                Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : Hello from AppShutDown thread.");
                Thread.Sleep(1000 * 60); // sleep for a minute
            }
        }

        public static void StopThread()
        {
            if(_thread != null)
            {
                _thread.Abort();
                Console.WriteLine("AppShutDown thread aborted.");
            }
        }
    }
}
