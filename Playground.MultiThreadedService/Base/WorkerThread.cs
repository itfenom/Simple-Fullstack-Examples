using System;
using System.Threading;

namespace Playground.MultiThreadedService.Base
{
    public abstract class WorkerThread
    {
        private string _name = string.Empty;

        /// <summary>
        /// Gets or sets the value indicating  whether the instance of a
        /// derived class should be performing the main operation.
        /// This flag can be used to suspend the thread execution.
        /// </summary>
        /// <value>
        /// <c>true</c> if the operation should be performed;
        /// otherwise, <c>false</c>.
        /// </value>
        protected bool IsDisabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value indicating  whether the instance of a
        /// derived class has been temporarily suspended.
        /// It's up to the derived class to handle this condition.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is suspended;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool IsPaused
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value indicating  whether the instance of a
        /// derived class is performing the main operation.
        /// This property is needed to avoid re-entrance,
        /// e.g. if a worker thread is set to be executed every minute,
        /// but it takes one execution five minutes to complete.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is executing;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool IsProcessing
        {
            get;
            protected set;
        }

        /// <summary>
        /// This property can be used to check whether the thread
        /// has started.
        /// </summary>
        /// <value>
        /// <c>true</c> if the thread has started;
        /// otherwise, <c>false</c>.
        /// </value>
        protected bool IsStarted => !ServiceHelper.IsEmpty(StartTime);

        /// <summary>
        /// This property can be used to check whether the
        /// thread received a signal to shut down.
        /// </summary>
        /// <value>
        /// <c>true</c> if the thread received a signal to shut
        /// down; otherwise, <c>false</c>.
        /// </value>
        public bool IsStopping
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the life span of this thread.
        /// The life span is calculated from the time of the first
        /// execution of the main operation performed by the thread.
        /// </summary>
        /// <value>
        /// The life span of the worker thread.
        /// </value>
        /// <remarks>
        /// When the service is stopped (normally or abnormally),
        /// this property will be reset.
        /// </remarks>
        public TimeSpan LifeSpan
        {
            get
            {
                if (ServiceHelper.IsEmpty(StartTime))
                {
                    return TimeSpan.Zero;
                }

                if (UseLocalTime)
                {
                    return DateTime.Now - StartTime;
                }

                return DateTime.UtcNow - StartTime;
            }
        }

        /// <summary>
        /// Gets or sets the name of the worker thread, which can be used
        /// for troubleshooting, logging, or reporting purposes.
        /// The name can be used to describe the task performed by
        /// the derived class.
        /// </summary>
        /// <value>
        /// Name of the worker thread.
        /// </value>
        /// <remarks>
        /// If the name has not been set explicitly,
        /// the name of the derived object type will be used.
        /// </remarks>
        public string Name
        {
            get
            {
                if (ServiceHelper.IsEmpty(_name))
                {
                    return GetType().Name;
                }

                return _name;
            }
            set => _name = value;
        }

        /// <summary>
        /// Gets or sets the date-time value indicating when the
        /// <see cref="Start"/> method was called for the first time
        /// during the lifetime of the service.
        /// </summary>
        /// <value>
        /// The date-time value of the first invocation of the thread.
        /// Depending on the <see cref="UseLocalTime"/> setting,
        /// this property can return a GMT (UTC) or local time value.
        /// </value>
        /// <remarks>
        /// When the service is stopped (normally or abnormally),
        /// this property will be reset.
        /// Normally, you should not use - and especially set -
        /// this property.
        /// </remarks>
        public DateTime StartTime
        {
            get;
            set;
        } = DateTime.MinValue;

        /// <summary>
        /// Gets or sets a value indicating whether local time must be used.
        /// </summary>
        /// <value>
        /// <c>true</c> if local time must be used;
        /// if GMT (UTC) is used, the value is <c>false</c>.
        /// </value>
        /// <remarks>
        /// If this property is not set explicitly, the GMT (UTC) will be used.
        /// </remarks>
        public static bool UseLocalTime
        {
            get;
            set;
        }

        /// <overloads>
        /// Sets the name of the physical thread (not the worker thread object).
        /// </overloads>
        ///
        /// <summary>
        /// Set the name of the running thread to the value
        /// defined in the <see cref="Name"/> property.
        /// </summary>
        /// <remarks>
        /// If the <see cref="Name"/> property was not explicitly set,
        /// the name of the thread will be set to an empty string.
        /// A thread name can be set only once.
        /// </remarks>
        public void SetThreadName()
        {
            SetThreadName(_name);
        }

        /// <summary>
        /// Set the name of the running thread to the specified value.
        /// </summary>
        /// <param name="name">
        /// Name of the thread that will be used.
        /// </param>
        /// <remarks>
        /// A thread name can be set only once.
        /// </remarks>
        public static void SetThreadName(string name)
        {
            if (ServiceHelper.IsEmpty(name))
            {
                name = string.Empty;
            }

            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = name;
            }
        }

        /// <summary>
        /// This method will be called by a
        /// <see cref="WindowsHost"/> object
        /// to start the thread.
        /// It must be implemented in derived classes.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// This method will be called by a
        /// <see cref="WindowsHost"/> object
        /// to stop the thread.
        /// It must be implemented in derived classes.
        /// </summary>
        public abstract void Stop();
    }
}
