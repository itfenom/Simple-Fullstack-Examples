using System;
using System.Threading.Tasks;

namespace Playground.WpfApp.Forms.AsyncEx.AsyncProgressbar
{
    public class WorkPerformedEvent : EventArgs
    {
        public string Data { get; set; }

        public WorkPerformedEvent(string data)
        {
            Data = data;
        }
    }

    public class AsyncDataRepository
    {
        /// <summary>
        /// Event handler to route arguments to any listeners 
        /// </summary>
        public event EventHandler<WorkPerformedEvent> WorkPerformed;

        /// <summary>
        /// The main helper async task that does things.
        /// </summary>
        /// <returns>The returned string from sub-tasks</returns>
        public async Task<string> DoStuffAsync()
        {
            long sum = 0;
            long total = 100000;

            await Task.Run(() =>
            {
                for (long i = 1; i <= total; i++)
                {
                    sum += i;
                    int percentage = Convert.ToInt32(((double)i / total) * 100);

                    //Keep calling back with updates. This calls back to viewmodel and updates
                    OnWorkPerformed(percentage.ToString());
                }
            });

            // This string returns back to the user interface after completion
            // this is only returned after continuation from the above await call
            return $"Sum: {sum}";
        }

        /// <summary>
        /// Callback fired to send data back to the viewmodel
        /// </summary>
        /// <param name="data">The string being sent in the callback</param>
        private void OnWorkPerformed(string data)
        {
            WorkPerformed?.Invoke(this, new WorkPerformedEvent(data));
        }
    }
}
