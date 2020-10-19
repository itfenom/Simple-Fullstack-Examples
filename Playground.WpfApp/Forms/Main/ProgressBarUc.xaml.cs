using System;
using System.ComponentModel;
using System.Threading;

namespace Playground.WpfApp.Forms.Main
{
    public partial class ProgressBarUc
    {
        public ProgressBarUc()
        {
            InitializeComponent();

            ShowProgress();
        }

        public void UpdateProgress(int percentage)
        {
            //Only populate up to 87%
            if (percentage <= 87)
            {
                // When progress is reported, update the progress bar control.
                PbLoad.Value = percentage;
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Notifying the progress bar window of the current progress.
            UpdateProgress(e.ProgressPercentage);
        }

        void ShowProgress()
        {
            try
            {
                // Using background worker to asynchronously run work method.
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += ProcessAsync;
                worker.ProgressChanged += worker_ProgressChanged;
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ProcessAsync(object sender, DoWorkEventArgs e)
        {
            for (int i = 1; i <= 100; i++)
            {
                // Simulates work being done
                Thread.Sleep(50);

                // Reports progress
                // ReSharper disable once PossibleNullReferenceException
                (sender as BackgroundWorker).ReportProgress(i);
            }
        }
    }
}
