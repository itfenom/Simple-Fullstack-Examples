using System;
using System.Threading;
using System.IO;

namespace Playground.Winforms.Utilities
{
    /**********************************************************************************************
     * This class cleans up the temp folder is Special Directory Structure.
     * C:\Users\<USERNAME>\AppData\Roaming\Temp
     * This class runs as a thread every 5 min to delete files that are 7 days older
     * - Kashif Mubarak (02/15/2016)
     * ********************************************************************************************/
    internal static class FileCleanup
    {
        private static Thread _thread;
        private static readonly string TempFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Temp\";
        private static int _fiveMinsInterval = 60000;
        // One minutes equals to 60000 milliseconds
        // One hour equals to (1000 * 60 * 60)

        public static void StartThread()
        {
            if (_thread == null)
            {
                _thread = new Thread(CleanupTempFolder);
            }
            if (_thread.ThreadState == ThreadState.Unstarted)
            {
                _thread.IsBackground = true; // Making this thread as Background thread as we want this thread to end when application ends.
                _thread.Start();
            }
        }

        public static void StopThread()
        {
            if(_thread != null)
            {
                _thread.Abort();
            }
        }

        private static void CleanupTempFolder()
        {
            while (true)
            {
                Console.WriteLine(@"File Cleanup thread started at: " + DateTime.Now);
                string[] files = null;

                try
                {
                    files = Directory.GetFiles(TempFolderPath, "*.*");
                }
                catch (Exception ex)
                {
                    if (ex.GetType().Name == "DirectoryNotFoundException")
                    {
                        Directory.CreateDirectory(TempFolderPath);
                        files = Directory.GetFiles(TempFolderPath, "*.*");
                    }
                }

                // ReSharper disable once PossibleNullReferenceException
                if (files.Length > 0)
                {
                    foreach (string file in files)
                    {
                        var fi = new FileInfo(file);
                        TimeSpan fileAge = File.GetLastWriteTime(file) - DateTime.Now;
                        // ReSharper disable once InconsistentNaming
                        var _fileAge = fileAge.Days;

                        if (_fileAge < 0)
                        {
                            _fileAge = (fileAge.Days * -1);
                        }

                        if (_fileAge <= 7) continue;

                        if (_fileAge > 7 && IsFileLocked(fi) == false)
                        {
                            File.Delete(file);
                        }
                    }
                }

                //Thread.Sleep(_fiveMinsInterval);
                Thread.Sleep(10000); //wait for 10 seconds
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static bool IsFileLocked(FileInfo file)
        {
            FileStream fileStream = null;

            try
            {
                fileStream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException ex)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                Console.WriteLine(ex.Message);
                return true;
            }
            finally
            {
                // ReSharper disable once UseNullPropagation
                if (fileStream != null)
                    fileStream.Close();
            }

            return false;
        }

    }
}
