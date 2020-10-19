using System;
using System.Threading;
using System.Threading.Tasks;
using MahApps.Metro.Controls;

namespace Playground.WpfApp.Forms.AsyncEx.BasicAsyncAwaitEx
{
    public partial class BasicAsyncAwaitExView : MetroWindow
    {
        public BasicAsyncAwaitExView()
        {
            InitializeComponent();
        }

        private async void BtnClickMe_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var progress = new Progress<int>(value =>
            {
                MyProgressBar.Value = value;
                lblMsg.Content = $"{value}%";
            });

            await Task.Run(() => LoopThroughNumbers(100, progress));
            lblMsg.Content = "Completed!";


            //CallBigLongImportantMethod();
            //lblMsg.Content = "Loading...";
        }

        private async void CallBigLongImportantMethod()
        {
            var result = await BigLongImportantMethodAsync("Kashif");
            lblMsg.Content = result;
        }

        private Task<string> BigLongImportantMethodAsync(string name)
        {
            return Task.Factory.StartNew(() =>
                        BigLongImportantMethod(name));
        }

        private string BigLongImportantMethod(string name)
        {
            Thread.Sleep(3000);
            return string.Format("Hi, {0}!", name);
        }

        //----
        private void LoopThroughNumbers(int count, IProgress<int> progress)
        {
            for (int x = 0; x < count; x++)
            {
                Thread.Sleep(100);
                var percentageComplete = (x * 100) / count;
                progress.Report(percentageComplete);
            }
        }
    }
}