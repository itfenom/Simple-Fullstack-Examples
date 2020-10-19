using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Playground.WinService.Common
{
    public class WorkerThread
    {
        private CallbackFinished _callbackFinished;
        private CallbackMessage _callbackMessage;
        private CallbackProcessWork _callbackProcessWork;
        private CallbackStarted _callbackStarted;
        private CallbackStatus _callbackStatus;
        private readonly ManualResetEvent _eventStopThread = new ManualResetEvent(false);
        private readonly ManualResetEvent _eventThreadStopped = new ManualResetEvent(false);
        private ISynchronizeInvoke _objInvoke;
        private string _threadName = "Worker Thread";
        private Thread _workerThread;
        public const int PostMessage = 1;
        public const int SendMessage = 2;

        public Thread CreateThread(object objInvoke, CallbackProcessWork pfunct, bool autoStart)
        {
            if (_workerThread == null)
            {
                _objInvoke = objInvoke as ISynchronizeInvoke;
                _callbackProcessWork = pfunct;
                _workerThread = new Thread(ThreadEntryPoint)
                {
                    Name = _threadName, IsBackground = true
                };
                if (autoStart)
                {
                    Start();
                }
            }
            return _workerThread;
        }

        public void Dispose()
        {
            Stop();
        }

        public void Finished(int mode)
        {
            if (_callbackFinished != null)
            {
                try
                {
                    if ((_objInvoke != null) && _objInvoke.InvokeRequired)
                    {
                        switch (mode)
                        {
                            case 1:
                                _objInvoke.Invoke(_callbackFinished, null);
                                return;

                            case 2:
                                _objInvoke.BeginInvoke(_callbackFinished, null);
                                return;
                        }
                    }
                    else
                    {
                        _callbackFinished();
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        public void Join()
        {
            if (_workerThread.IsAlive)
            {
                _workerThread.Join();
            }
        }

        // ReSharper disable once UnusedMember.Global
        public bool Join(int millisecondsTimeout)
        {
            TimeSpan timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);
            return Join(timeout);
        }

        public bool Join(TimeSpan timeout)
        {
            return (!_workerThread.IsAlive || _workerThread.Join(timeout));
        }

        public void Message(int type, object msg, int mode)
        {
            if (_callbackMessage != null)
            {
                try
                {
                    if ((_objInvoke != null) && _objInvoke.InvokeRequired)
                    {
                        switch (mode)
                        {
                            case 1:
                                _objInvoke.Invoke(_callbackMessage, new object[] { type, msg });
                                return;

                            case 2:
                                _objInvoke.BeginInvoke(_callbackMessage, new object[] { type, msg });
                                return;
                        }
                    }
                    else
                    {
                        _callbackMessage(type, msg);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        public void Start()
        {
            _eventStopThread.Reset();
            _eventThreadStopped.Reset();
            _workerThread.Start();
        }

        public void Started(int mode)
        {
            if (_callbackStarted != null)
            {
                try
                {
                    if ((_objInvoke != null) && _objInvoke.InvokeRequired)
                    {
                        switch (mode)
                        {
                            case 1:
                                _objInvoke.Invoke(_callbackStarted, null);
                                return;

                            case 2:
                                _objInvoke.BeginInvoke(_callbackStarted, null);
                                return;
                        }
                    }
                    else
                    {
                        _callbackStarted();
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        public void Status(string msg, int mode)
        {
            if (_callbackStatus != null)
            {
                try
                {
                    if ((_objInvoke != null) && _objInvoke.InvokeRequired)
                    {
                        switch (mode)
                        {
                            case 1:
                                _objInvoke.Invoke(_callbackStatus, new object[] { msg });
                                return;

                            case 2:
                                _objInvoke.BeginInvoke(_callbackStatus, new object[] { msg });
                                return;
                        }
                    }
                    else
                    {
                        _callbackStatus(msg);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        public void Stop()
        {
            if ((_workerThread != null) && _workerThread.IsAlive)
            {
                _eventStopThread.Set();
                while (_workerThread.IsAlive)
                {
                    // ReSharper disable once RedundantExplicitArrayCreation
                    // ReSharper disable once CoVariantArrayConversion
                    if (WaitHandle.WaitAll(new ManualResetEvent[] { _eventThreadStopped }, 100, true))
                    {
                        return;
                    }
                    Application.DoEvents();
                }
            }
        }

        private bool StopWorking()
        {
            if (_eventStopThread.WaitOne(0, true))
            {
                _eventThreadStopped.Set();
                return true;
            }
            return false;
        }

        private void ThreadEntryPoint()
        {
            Started(1);
            _callbackProcessWork();
            Finished(1);
        }

        public ManualResetEvent EventStopThread => _eventStopThread;

        public ManualResetEvent EventThreadStopped => _eventThreadStopped;

        public bool ExitThread => StopWorking();

        public string Name
        {
            get => _threadName;
            set
            {
                _threadName = value;
                if (_workerThread != null)
                {
                    _workerThread.Name = _threadName;
                }
            }
        }

        public CallbackFinished SetCallbackFinished
        {
            set => _callbackFinished = (CallbackFinished)Delegate.Combine(_callbackFinished, value);
        }

        public CallbackMessage SetCallbackMessage
        {
            set => _callbackMessage = (CallbackMessage)Delegate.Combine(_callbackMessage, value);
        }

        public CallbackStarted SetCallbackStarted
        {
            set => _callbackStarted = (CallbackStarted)Delegate.Combine(_callbackStarted, value);
        }

        public CallbackStatus SetCallbackStatus
        {
            set => _callbackStatus = (CallbackStatus)Delegate.Combine(_callbackStatus, value);
        }

        public Thread Worker => _workerThread;

        public delegate void CallbackFinished();

        public delegate void CallbackMessage(int type, object msg);

        public delegate void CallbackProcessWork();

        public delegate void CallbackStarted();

        public delegate void CallbackStatus(string msg);
    }
}
