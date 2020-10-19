using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.ConsoleApp.DelegateEx
{
    /// <summary>
    /// Event Args class
    /// </summary>
    public class ProcessEventsArgs : EventArgs
    {
        public bool IsSuccessful { get; set; }
        public DateTime CompletionTime { get; set; }
    }

    /// <summary>
    /// Event Publisher class
    /// </summary>
    public class EventPublisher
    {
        public event EventHandler<ProcessEventsArgs> ProcessCompleted;

        public void StartProcess()
        {
            var data = new ProcessEventsArgs();

            try
            {
                data.IsSuccessful = true;
                data.CompletionTime = DateTime.Now;
                OnProcessCompleted(data);
            }
            catch (Exception e)
            {
                data.IsSuccessful = false;
                data.CompletionTime = DateTime.Now;
                OnProcessCompleted(data);
                Console.WriteLine(e.Message);
            }
        }

        protected virtual void OnProcessCompleted(ProcessEventsArgs e)
        {
            ProcessCompleted?.Invoke(this, e);
        }
    }

    public class EventSubscriber
    {
        public void SubscribeEvent()
        {
            var publisher = new EventPublisher();
            publisher.ProcessCompleted += OnComplete;
        }

        public void OnComplete(object sender, ProcessEventsArgs e)
        {
            Console.WriteLine("Process " + (e.IsSuccessful? " completed successfully" : " failed"));
            Console.WriteLine($"Completion time: {e.CompletionTime.ToLongTimeString()}");
        }
    }
}
