using System;
using System.Threading;

namespace Playground.MultiThreadedService.Base
{
    public abstract class BaseTimerThread : WorkerThread
    {
        private readonly object _lockObject = new object();
        private int _delay;
        private int _interval;
        private Timer _timer;

        protected BaseTimerThread()
        {
        }

        protected BaseTimerThread(string name, int delay, int interval)
        {
            Name = name;
            Delay = delay;

            _interval = interval < 0 ? DefaultInterval : interval;
        }

        /// <summary>
        /// Gets the default value of the initial delay (in milliseconds)
        /// </summary>
        public int DefaultDelay => 0;

        /// <summary>
        /// Returns the default interval (in milliseconds) between the invocations of the <see cref="Run(object)" />
        /// </summary>
        public int DefaultInterval => 1000;

        /// <summary>
        /// Gets or sets an initial delay (in milliseconds) before the first invocation of the <see cref="TimerThreadCallback(object)" />
        /// </summary>
        public int Delay
        {
            get => _delay;
            set => _delay = value < 0 ? DefaultDelay : value;
        }

        /// <summary>
        /// Gets or sets the interval (in milliseconds) between the invocations of the <see cref="Run(object)" />
        /// </summary>
        public int Interval
        {
            get => _interval;
            set
            {
                lock (_lockObject)
                {
                    if (_interval == value)
                    {
                        return;
                    }

                    _interval = value < 0 ? DefaultInterval : value;
                }

                // If the timer was already initialized, we need to reset it.
                if (IsStarted)
                {
                    Start();
                }
            }
        }

        public DateTime LastExecutionTime
        {
            get;
            private set;
        } = DateTime.MinValue;

        public override void Start()
        {
            // In case this thread is already running, stop it.
            Stop();

            lock (_lockObject)
            {
                // Set the start time.
                if (ServiceHelper.IsEmpty(StartTime))
                {
                    StartTime = UseLocalTime ? DateTime.Now : DateTime.UtcNow;
                }

                // Initialize timer.
                if (_timer == null)
                {
                    // ReSharper disable once RedundantDelegateCreation
                    _timer = new Timer(new TimerCallback(TimerThreadCallback), null, _delay, _interval);
                }
            }
        }

        public override void Stop()
        {
            lock (_lockObject)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }

        protected abstract void Run(object source);

        protected virtual void TimerThreadCallback(object source)
        {
            // If the operation takes longer to complete than the timer interval, we need to exit the
            // callback immediately; otherwise, we'll have a number of queued callbacks waiting on
            // the lock.
            if (!Monitor.TryEnter(_lockObject))
            {
                return;
            }

            try
            {
                // If we are already processing or suspended, skip the rest.
                if (IsProcessing || IsDisabled || IsPaused)
                {
                    return;
                }

                // Set the flag to indicate that the thread is being processed
                IsProcessing = true;

                // Execute the main method.
                Run(source);

                // Save the last execution time.
                LastExecutionTime = UseLocalTime ? DateTime.Now : DateTime.UtcNow;
            }
            finally
            {
                IsProcessing = false;

                // Allow the next callback to execute.
                Monitor.Exit(_lockObject);
            }
        }
    }
}
