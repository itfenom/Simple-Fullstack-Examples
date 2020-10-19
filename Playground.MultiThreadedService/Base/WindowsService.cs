using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using Playground.Core.Diagnostics;
using Playground.Core.Utilities;
using Playground.MultiThreadedService.Base;

// ReSharper disable once CheckNamespace
//Intentional namespace to a higher level for compilation
namespace Playground.MultiThreadedService
{
    public class WindowsService : ServiceBase
    {
        private string _displayName = string.Empty;

        public string DisplayName
        {
            get
            {
                if (ServiceHelper.IsEmpty(_displayName))
                {
                    return ServiceName;
                }
                return _displayName;
            }
            set => _displayName = value;
        }

        public WorkerThread[] WorkerThreads
        {
            get;
            set;
        }

        protected static bool IsDebugMode => Environment.UserInteractive;

        public static bool IsRunning(string serviceName)
        {
            try
            {
                using (ServiceController scm = new ServiceController(serviceName))
                {
                    return scm.Status == ServiceControllerStatus.Running;
                }
            }
            catch
            {
                return false;
            }
        }

        protected override void OnContinue()
        {
            // Make sure that there is at least one worker thread running.
            if (ServiceHelper.IsEmpty(WorkerThreads))
                return;

            for (int i = 0; i < WorkerThreads.Length; i++)
            {
                // Indicate to the worker thread that the service is being suspended.
                WorkerThreads[i].IsPaused = false;
            }

            base.OnContinue();
        }

        protected override void OnCustomCommand(int command)
        {
            base.OnCustomCommand(command);

            // make sure the command is something we expect to receive
            if (!Enum.IsDefined(typeof(ServiceCommands), command))
            {
                if (!Enum.IsDefined(typeof(ServiceControllerStatus), command))
                {
                    // if the command we've received is not one of our known commands, and is not one
                    // of the built in commands, log it
                    Logger.Info($"Received unknown custom command: {command}");
                }

                return;
            }

            ServiceCommands commandValue = (ServiceCommands)command;

            try
            {
                using (var sc = new ServiceController(ServiceName))
                {
                    if (commandValue == ServiceCommands.ShutDownWhenPossible
                        && sc.Status != ServiceControllerStatus.Stopped
                        && sc.Status != ServiceControllerStatus.StopPending)
                    {
                        Logger.Info($"Received custom command: {commandValue}");
                        StopThreads();

                        Logger.Info("Waiting for threads to stop");

                        // wait while there are any threads still processing
                        while (WorkerThreads.Any(thread => thread.IsProcessing))
                        {
                            Thread.Sleep(250);
                        }

                        // all threads should be done running now.  Shut down the service
                        Logger.Info("All threads stopped");
                        Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        protected override void OnPause()
        {
            // Make sure that there is at least one worker thread.
            if (ServiceHelper.IsEmpty(WorkerThreads))
                return;

            for (int i = 0; i < WorkerThreads.Length; i++)
            {
                // Indicate to the worker thread that the
                // service is being suspended.
                WorkerThreads[i].IsPaused = true;
            }

            base.OnPause();
        }

        protected override void OnStart(string[] args)
        {
            Start(args);
        }

        protected override void OnStop()
        {
            StopThreads();
        }

        protected static void Run(ServiceBase[] services, string[] args)
        {
            // Make sure we got services.
            if (ServiceHelper.IsEmpty(services))
                return;

            // Check a if the process was launched in Debug mode.
            if (IsDebugMode)
            {
                MessageBox.Show("Running in Debug mode!", "Service Mode", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Start individual services manually.
                foreach (ServiceBase service in services)
                {
                    if (service is WindowsService windowsService)
                    {
                        windowsService.Start(args);
                    }
                }

                // We just need to keep this thread running.
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                // Loop indefinitely until the user stops
                while (true)
                {
                    Thread.Sleep(100);
                }
            }
            else
            {
                // Execute services from Service Control Manager (SCM).
                ServiceBase.Run(services);
            }
        }

        public virtual void Start(string[] args)
        {
            // Make sure that there is at least one worker thread.
            if (ServiceHelper.IsEmpty(WorkerThreads))
                return;

            // Start all worker threads.
            for (int i = 0; i < WorkerThreads.Length; i++)
            {
                WorkerThreads[i].Start();
            }
        }

        public static void StartService(string serviceName)
        {
            StartService(serviceName, 0);
        }

        /// <summary>
        /// Starts the specified service using Service Control Manager (SCM).
        /// </summary>
        /// <param name="serviceName">
        /// Name of the service to be started.
        /// </param>
        /// <param name="timeout">
        /// A timeout (in seconds) used to wait while checking whether the
        /// service is started.
        /// A negative value indicates an infinite timeout.
        /// The value of 0 (zero) indicates that the timeout will not be used,
        /// i.e. the service status will not be checked.
        /// </param>
        /// <remarks>
        /// If the service is already running, it will not be restarted.
        /// </remarks>
        public static void StartService(string serviceName, int timeout)
        {
            ServiceController scm = new ServiceController(serviceName);

            if (scm.Status == ServiceControllerStatus.Running)
                return;

            if (scm.Status != ServiceControllerStatus.StartPending)
            {
                try
                {
                    scm.Start();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Cannot start the Windows service '{0}'.", serviceName), ex);
                }
            }

            if (timeout == 0)
                return;

            if (timeout > 0)
                timeout *= 1000;    // msec

            int sleepTime = 100;                    // msec

            while (scm.Status != ServiceControllerStatus.Running && timeout != 0)
            {
                Thread.Sleep(sleepTime);

                if (timeout > 0)
                    timeout -= sleepTime;
            }
        }

        public virtual void StopThreads()
        {
            // Make sure that there is at least one worker thread.
            if (ServiceHelper.IsEmpty(WorkerThreads))
                return;

            for (int i = 0; i < WorkerThreads.Length; i++)
            {
                // Indicate to the worker thread that the
                // service is stopping. This can be handy
                // for long-running operations.
                WorkerThreads[i].IsStopping = true;

                // Stop the thread.
                WorkerThreads[i].Stop();
            }
        }

        public static void StopService(string serviceName)
        {
            StopService(serviceName, 0);
        }

        /// <summary>
        /// Stops the specified service using Service Control Manager (SCM).
        /// </summary>
        /// <param name="serviceName">
        /// Name of the service to be stopped.
        /// </param>
        /// <param name="timeout">
        /// A timeout (in seconds) used to wait while checking whether the
        /// service is stopped.
        /// A negative value indicates an infinite timeout.
        /// The value of 0 (zero) indicates that the timeout will not be used,
        /// i.e. the service status will not be checked.
        /// </param>
        public static void StopService(string serviceName, int timeout)
        {
            ServiceController scm = new ServiceController(serviceName);

            if (scm.Status == ServiceControllerStatus.Stopped)
                return;

            if (scm.Status != ServiceControllerStatus.StopPending)
            {
                try
                {
                    scm.Stop();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Cannot stop the Windows service '{0}'.", serviceName), ex);
                }
            }

            if (timeout == 0)
                return;

            if (timeout > 0)
                timeout *= 1000;    // msec

            int sleepTime = 100;    // msec

            while (scm.Status != ServiceControllerStatus.Stopped && timeout != 0)
            {
                Thread.Sleep(sleepTime);

                if (timeout > 0)
                    timeout -= sleepTime;
            }
        }
    }
}