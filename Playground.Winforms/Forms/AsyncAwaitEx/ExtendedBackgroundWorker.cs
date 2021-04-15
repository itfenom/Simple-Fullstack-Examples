using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace Playground.Winforms.Forms.AsyncAwaitEx
{
    public class ExtendedBackgroundWorker : BackgroundWorker
    {
        public Thread CurrentThread
        {
            get;
            private set;
        }

        protected override void OnProgressChanged(ProgressChangedEventArgs e)
        {
            if (_threadAborted)
                return;

            base.OnProgressChanged(e);
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            _threadAborted = false;
            CurrentThread = Thread.CurrentThread;
            try
            {
                base.OnDoWork(e);
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
        }

        private bool _threadAborted;

        public void StopImmediately()
        {
            if (!IsBusy || CurrentThread == null)
            {
                return;
            }
            try
            {
                CancelAsync();
                Thread.SpinWait(10);
                CurrentThread.Abort();
            }
            catch (ThreadAbortException)
            {
                //swallow thread abort in this part,
            }

            _threadAborted = true;
            AsyncOperation op = GetPrivateFieldValue<AsyncOperation>("asyncOperation");
            SendOrPostCallback completionCallback = GetPrivateFieldValue<SendOrPostCallback>("operationCompleted");
            RunWorkerCompletedEventArgs completedArgs = new RunWorkerCompletedEventArgs(null, null, true);
            op.PostOperationCompleted(completionCallback, completedArgs);
        }

        //type safe reflection
        private TFieldType GetPrivateFieldValue<TFieldType>(string fieldName)
        {
            Type type = typeof(BackgroundWorker);
            FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            object fieldVal = field?.GetValue(this);

            return SafeCastTo<TFieldType>(fieldVal);
        }

        // ReSharper disable once UnusedMember.Local
        private void SetPrivateFieldValue<TFieldType>(string fieldName, TFieldType value)
        {
            Type type = typeof(BackgroundWorker);
            FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field?.SetValue(this, value);
            //Debug.Assert(field != null, nameof(field) + " != null");
            //Debug.Assert(field.GetValue(this).Equals(value));
        }

        /// <summary>
        /// Works like a strongly typed "as" operator
        /// If the object is not of the requested type,
        /// the default value for that type is returns instead of throwing an exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private T SafeCastTo<T>(object obj)
        {
            if (obj == null)
            {
                return default(T);
            }
            if (!(obj is T))
            {
                return default(T);
            }
            return (T)obj;
        }
    }
}
