using System;
using System.IO;
using Playground.Core.Diagnostics;
using Playground.MultiThreadedService.Base;

namespace Playground.MultiThreadedService.Threads
{
    public class MainWorkerThread : BaseTimerThread
    {
        /* NOTE: We'll get TNS exceptions on every heartbeat if the database goes down.  We only want to
         * log them every 30 minutes or so, so we'll track the last time it was logged.
        */
        // ReSharper disable once UnusedMember.Local
        private DateTime _lastTnsListenerException = DateTime.MinValue;
        // ReSharper disable once NotAccessedField.Local
        private readonly HeartBeatThread _heartBeatThread;

        internal MainWorkerThread(HeartBeatThread heartBeatThread)
        {
            _heartBeatThread = heartBeatThread;
        }

        internal MainWorkerThread(string name, int delay, int interval, HeartBeatThread heartBeatThread) : base(name, delay, interval)
        {
            _heartBeatThread = heartBeatThread;
        }

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
                Logger.Info($"{Name}: Working something in the main thread....");

                CleanUpTempFiles();

                // Set next cleanup time
                ServiceConfig.NextTempFileCleanUpTime = DateTime.Now + ServiceConfig.TempFileCleanUpInterval;
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            Logger.Info($"{Name} finished.");
        }

        private void LogError<TException>(TException ex) where TException : Exception
        {
            Logger.Error($"{Name}: Error processing MATLAB routine.", ex);
            EmailManager.EmailToTeam("Error processing MATLAB routine." + Environment.NewLine + ex.Message);
        }

        private void CleanUpTempFiles()
        {
            string[] tempFiles = Directory.GetFiles(@"C:\Temp", "*.*");
            if (tempFiles.Length >= 10)
            {
                Logger.Info($"{Name}: Cleaning up old files from temp folder...");

                foreach (string file in tempFiles)
                {
                    try
                    {
                        FileInfo fi = new FileInfo(file);
                        TimeSpan fileAge = DateTime.Now - File.GetLastWriteTime(file);
                        double fileAgeInDays = Math.Abs(fileAge.TotalDays);

                        if (fileAgeInDays > 0.5 && !IsFileLocked(fi))
                        {
                            File.Delete(file);
                            Logger.Info($"{Name}:\nTemp file: {fi.Name} deleted successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // We don't want a failure on any single file to cause all others to be
                        // skipped...so only logging it.
                        Logger.Error($"{Name}: {ex.Message}", ex);
                    }
                }

                Logger.Info($"{Name}: Cleaning up old files in temp folder completed.");
            }
        }

        private bool IsFileLocked(FileInfo file)
        {
            FileStream fileStream = null;

            try
            {
                fileStream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException ex)
            {
                // the file is unavailable because it is:
                // still being written to
                // or being processed by another thread
                // or does not exist (has already been processed)
                Console.WriteLine(ex.Message);
                return true;
            }
            finally
            {
                fileStream?.Close();
            }

            return false;
        }
    }
}
