using System;

namespace Playground.WpfApp.Forms.AsyncEx.AsyncProgressbar
{
    public class WorkPerformedEventArgs : EventArgs
    {
        public WorkPerformedEventArgs(string data)
        {
            Data = data;
        }

        public string Data { get; set; }
    }
}
