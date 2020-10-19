using System;
using Playground.Core.Diagnostics;
using Playground.Core.AdoNet;
using Playground.MultiThreadedService.Base;

namespace Playground.MultiThreadedService.Threads
{
    public class HeartBeatThread : BaseTimerThread
    {
        /* NOTE: We'll get TNS exceptions on every heartbeat if the database goes down.  We only want to
         * log them every 30 minutes or so, so we'll track the last time it was logged.
        */
        private DateTime _lastTnsListenerException = DateTime.MinValue;

        protected override void Run(object source)
        {
            if (IsStopping)
            {
                Logger.Info($"{Name} is stopping...");
                return;
            }

            Logger.Info($"{Name} started...");

            try
            {
                Logger.Info($"{Name}: Sending Heartbeat...");
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            Logger.Info($"{Name} finished.");
        }

        private void LogError<TException>(TException ex) where TException : Exception
        {
            Logger.Error($"{Name}: Error sending heartbeat.", ex);
            EmailManager.EmailToTeam($"Error sending heartbeat. {Environment.NewLine}{ex.Message}");
        }
    }
}
