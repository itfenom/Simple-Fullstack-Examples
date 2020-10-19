using Playground.Core.AdoNet;
using Playground.Core.Diagnostics;
using Playground.WinService.Common;
using System;
using System.Threading;

namespace Playground.WinService
{
    public class ProcessWorkThread
    {
        private string _processName = "PLAYGROUND_SERVICE_PROCESS_THREAD";
        private bool _keepWorking = true;
        private WorkerThread _workerThread;

        public string ProcessName
        {
            get => (_processName);
            set => _processName = value;
        }

        public bool KeepWorking
        {
            get => (_keepWorking);
            set => _keepWorking = value;
        }

        public WorkerThread WorkThread => (_workerThread);

        public void WaitForWorkThreadToComplete()
        {
            _keepWorking = true;
            while (_keepWorking && (_workerThread != null))
            {
                Thread.Sleep(1000);
                /*
                  0 = indicates that this thread should be suspended to allow other waiting threads to execute;
                  1000 =  1 sec
                  60000 =  1 minute
                  10 * 60000 = 10 minutes
                  60 * 60000 =  1 hour
                 */
            }
        }

        public void ProcessWork(bool synchronous)
        {
            _workerThread = new WorkerThread {Name = _processName};
            // ReSharper disable once RedundantDelegateCreation
            _workerThread.CreateThread(this, new WorkerThread.CallbackProcessWork(ProcessControllerCallback), true);

            if (synchronous)
            {
                // Wait for the worker thread to finish before returning.
                // Prevents more that one job from being started if the "work" takes a long enough time to complete.
                WaitForWorkThreadToComplete();
            }
        }

        public void ProcessControllerCallback()
        {
            //Briefly release control
            Thread.Sleep(0);

            //Begining of work!
            Logger.Info("Begin process!");
            Logger.Info("------------------------------------------------------------");

            try
            {
                //Code to process in the service go here!!! - Kashif Mubarak

                #region TestRegion
                string output = DAL.Seraph.ExecuteScalar("SELECT CURRENT_TIMESTAMP FROM DUAL").ToString();

                Logger.Info("Current Time Oracle TIME_STAMP: " + output);

                #endregion TestRegion

                Logger.Info("End process!");
                Logger.Info("------------------------------------------------------------");
                //End of Work
            }
            catch (Exception ex)
            {
                Logger.Error("Error processing", ex);
            }
            finally
            {
                _workerThread = null;
            }
        }
    }
}
