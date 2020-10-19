using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Timers;
using Playground.Core.Diagnostics;
using Playground.WinService.Common;

namespace Playground.WinService
{
    public partial class SingleThreadedService : ServiceBase
    {
        public static readonly string ClassName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType?.ToString();
        private Thread _workThread;
        private System.Timers.Timer _timer;
        private bool _keepWorking = true;
        public bool Debug => ServiceConfig.DebugProgramEnabled;

        private string _lastRunTime = "00:00";
        // ReSharper disable once ConvertToAutoProperty
        public string LastRunTime
        {
            get => _lastRunTime;
            set => _lastRunTime = value;
        }

        public SingleThreadedService()
        {
            try
            {
                ServiceName = ServiceConfig.ServiceName;
            }
            catch
            {
                ServiceName = Program.CommandLineArguments.ProgramName;
            }
            if ((ServiceName == null) || (ServiceName.Length == 0))
            {
                ServiceName = Program.CommandLineArguments.ProgramName;
            }

            Logger.Info(ServiceName + " Service Initializing");

            InitializeComponent();
        }

        #region Timer Functions

        private void SetTimer()
        {
            _timer?.Dispose();
            _timer = new System.Timers.Timer();
            _timer.Elapsed += TimerAlarm;

            //       1000 =  1 sec
            //      60000 =  1 minute
            // 10 * 60000 = 10 minutes
            // 60 * 60000 =  1 hour
            var interval = 60000;
            try
            {
                interval = ServiceConfig.ServiceSleeps * 60000;
            }
            catch
            {
                // ignored
            }

            _timer.Interval = interval;
            _timer.Enabled = true;
        }

        private void TimerAlarm(Object source, ElapsedEventArgs e)
        {
#if DEBUG
            Logger.Debug(ServiceName + " Timer Activated");
            EventLog.WriteEntry(ServiceName + " Timer Activated");
#endif
            StartProcess();
        }

        #endregion Timer Functions

        #region Check Scheduler

        private bool CheckScheduler()
        {
            var today = DateTime.Now;

            try
            {
                // Check which days of the week to run on: "Sunday Monday Tuesday Wednesday Thursday Friday Saturday"
                var tRunDays = ServiceConfig.SchedulerRunDays;
                if (tRunDays != null)
                {
                    tRunDays = tRunDays.ToUpper();
                    var tDay = DateTime.Now.DayOfWeek.ToString().ToUpper();

                    if (!tRunDays.Contains(tDay))
                    {
                        if (Debug) Logger.Debug("CheckScheduler(): " + tDay + " not found in " + tRunDays);
                        LastRunTime = "00:00";			// clear 'last run time' for next day;
                        return (false);
                    }
                    if (Debug) Logger.Debug("CheckScheduler(): " + tDay + " was found in " + tRunDays);
                }
                else
                {
                    Logger.Error(ServiceName + " Invalid configuration setting: 'Scheduler.Run.Days' parameter not found");
                }

                var tNow = today.ToString("HH:mm");
                var tStart = ServiceConfig.SchedulerTimeStart;
                var tStop = ServiceConfig.SchedulerTimeStop;
                int interval;
                try
                {
                    interval = ServiceConfig.SchedulerTimeInterval;
                }
                catch
                {
                    Logger.Error(ServiceName + " Invalid configuration setting: " + ServiceConfig.SchedulerTimeInterval);
                    EventLog.WriteEntry(ServiceName + " Invalid configuration setting: " + ServiceConfig.SchedulerTimeInterval);
                    return (false);
                }

                if (Debug)
                    Logger.Debug("CheckScheduler(): interval: " + interval);
                if (Debug)
                    Logger.Debug("CheckScheduler(): t_now: " + tNow);
                if (Debug)
                    Logger.Debug("CheckScheduler(): t_now: " + tStart);
                if (Debug)
                    Logger.Debug("CheckScheduler(): t_now: " + tStop);
                if (Debug)
                    Logger.Debug("CheckScheduler(): (t_now.CompareTo(t_start) >= 0): " + string.Compare(tNow, tStart, StringComparison.Ordinal));
                if (Debug)
                    Logger.Debug("CheckScheduler(): (t_now.CompareTo(t_stop) <= 0): " + string.Compare(tNow, tStop, StringComparison.Ordinal));

                if ((string.Compare(tNow, tStart, StringComparison.Ordinal) >= 0) && (string.Compare(tNow, tStop, StringComparison.Ordinal) <= 0))
                {
                    DateTime next = DateTime.ParseExact(LastRunTime, "HH:mm", null);
                    next = next.AddMinutes(interval);
                    var tNext = next.ToString("HH:mm");

                    if (Debug)
                        Logger.Debug("CheckScheduler(): t_next.CompareTo(t_now) <= 0: " + String.Compare(tNext, tNow, StringComparison.Ordinal));
                    if (String.Compare(tNext, tNow, StringComparison.Ordinal) <= 0)
                    {
                        LastRunTime = DateTime.Now.ToString("HH:mm");	// update LastRunTime;
                        return (true);				// time to process work load;
                    }
                }
                else
                {
                    LastRunTime = "00:00";			// clear 'last run time' for tomorrow;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ServiceName + " Error calculating valid run time", ex);
                EventLog.WriteEntry(ServiceName + "::Error calculating valid run time   \r\n" + ex.Message);
            }
            return (false);							// not time to process work load;
        }

        #endregion Check Scheduler

        #region Start/Stop Service (Create Worker Thread)

        public void OnStart()
        {
            ServiceName = ServiceConfig.ServiceName;
            OnStart(null);
            Debugger.Launch();
        }
        
        protected override void OnStart(string[] args)
        {
            Logger.Info(ServiceName + " Starting Service");
            EventLog.WriteEntry("Starting Service " + ServiceName);
            // ReSharper disable once RedundantDelegateCreation
            _workThread = new Thread(new ThreadStart(StartThread));
            _workThread.Start();

            StartProcess();
        }

        protected override void OnStop()
        {
            Logger.Info(ServiceName + " Stopping Service");
            EventLog.WriteEntry("Stopping Service " + ServiceName);
            _keepWorking = false;
            _timer?.Dispose();
        }

        #endregion Start/Stop Service (Create Worker Thread)

        #region StartThread (Worker Thread Entry Point)

        private void StartThread()
        {
            Logger.Info(ServiceName + " - Service scheduler thread started");
            EventLog.WriteEntry("Service scheduler thread started");
            SetTimer();

            _keepWorking = true;
            while (_keepWorking && (_timer != null))
            {
                //          0 = indicates that this thread should be suspended to allow other waiting threads to execute;
                //       1000 =  1 sec
                //      60000 =  1 minute
                // 10 * 60000 = 10 minutes
                // 60 * 60000 =  1 hour
                Thread.Sleep(1000);
            }
            Logger.Info(ServiceName + " - Service scheduler thread finished");
            EventLog.WriteEntry("Service scheduler thread finished");
        }

        #endregion StartThread (Worker Thread Entry Point)

        #region StartProcess()

        private void StartProcess()
        {
            try
            {
                if (_timer != null)
                {
                    _timer.Enabled = false;
                }
                ConfigurationManager.RefreshSection("appSettings");

                if (CheckScheduler())
                {
                    ProcessWork();
                    // Force garbage collection before going back to sleep.
                    // Depending on the type of "work" done, this helps reduce drain on system/server resources.
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                if (_timer != null)
                {
                    _timer.Enabled = true;
                }
            }
        }

        #endregion StartProcess()

        #region ProcessWork()

        public bool ProcessWork()
        {
            try
            {
                ProcessWorkThread work = new ProcessWorkThread();
                work.ProcessWork(true);
                return (true);
            }
            catch (Exception ex)
            {
                Logger.Error(ServiceName + " Error calling 'ProcessWork' code", ex);
                EventLog.WriteEntry(ServiceName + " Error calling 'ProcessWork' code  \n\r" + ex.Message);
            }
            return (false);
        }

        #endregion ProcessWork()

    }
}
