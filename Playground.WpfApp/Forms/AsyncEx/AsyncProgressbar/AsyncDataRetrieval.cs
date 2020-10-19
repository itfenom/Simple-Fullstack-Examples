using System;
using System.Threading;
using System.Threading.Tasks;

namespace Playground.WpfApp.Forms.AsyncEx.AsyncProgressbar
{
    public class AsyncDataRetrieval
    {
        /// <summary>
        /// Event handler to route arguments to any listeners 
        /// </summary>
        public event EventHandler<WorkPerformedEventArgs> WorkPerformed;

        /// <summary>
        /// The main helper async task that does things.
        /// </summary>
        /// <returns>The returned string from sub-tasks</returns>
        public async Task<string> DoStuffAsync()
        {

            // keep calling back with updates. This calls back to viewmodel and updates
            // the output each tome it loops
            await DoLongRunningTaskWithCallbacks();

            // This string returns back to the user interface after completion
            // this is only returned after continuation from the above await call
            return "\nDoStuffAsync returns after callbacks!";
        }

        /// <summary>
        /// Loop and execute the OnWorkPerformed each time work is performed.
        /// </summary>
        /// <returns></returns>
        private async Task DoLongRunningTaskWithCallbacks()
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    OnWorkPerformed($"Hello # {i} from task loop!\n");

                    Thread.Sleep(500);
                }
            });
        }


        /// <summary>
        /// Callback fired to send data back to the viewmodel
        /// </summary>
        /// <param name="data">The string being sent in the callback</param>
        private void OnWorkPerformed(string data)
        {
            WorkPerformed?.Invoke(this, new WorkPerformedEventArgs(data));
        }
    }
}
