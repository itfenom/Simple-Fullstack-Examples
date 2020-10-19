using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Playground.Winforms.Forms.BasicAsyncAwaitEx
{
    public partial class AsyncAndAwaitEx : BaseForm
    {
        public AsyncAndAwaitEx()
        {
            InitializeComponent();
        }

        private void BtnClickMe_Click(object sender, EventArgs e)
        {
            CallBigLongImportantMethod();
            label1.Text = @"Loading...";
        }


        private async void CallBigLongImportantMethod()
        {
            var result = await BigLongImportantMethodAsync(@"Kashif");
            label1.Text = result;
        }

        private Task<string> BigLongImportantMethodAsync(string name)
        {
            return Task.Factory.StartNew(() =>
                BigLongImportantMethod(name));
        }

        private string BigLongImportantMethod(string name)
        {
            Thread.Sleep(3000);
            return $"Hi, {name}!";
        }
    }
}
