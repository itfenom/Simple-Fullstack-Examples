using System;
using System.Reactive.Disposables;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx
{
    public abstract class BindableBase : ReactiveObject, IDisposable
    {
        public virtual string Title => "";

        protected BindableBase()
        {
            Disposables = new Lazy<CompositeDisposable>(() =>
            {
                var disposable = new CompositeDisposable();
                if (IsDisposed)
                {
                    // if this instance has already been disposed, and it's the first time the
                    // disposable group has been created, we want to immediately dispose of the group.
                    disposable.Dispose();
                }

                return disposable;
            });
        }

        /// <summary>
        /// Gets a disposable that will be disposed when this instance is disposed. All disposables
        /// added to this disposable will also be disposed.
        /// </summary>
        protected Lazy<CompositeDisposable> Disposables { get; }

        #region IDisposable Support

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        /// unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    if (Disposables.IsValueCreated && !Disposables.Value.IsDisposed)
                    {
                        Disposables.Value.Dispose();
                    }

                    DisposeManagedResources();
                }

                DisposeUnmanagedResources();
                IsDisposed = true;
            }
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected virtual void DisposeManagedResources() { }

        /// <summary>
        /// Disposes the unmanaged resources.
        /// </summary>
        protected virtual void DisposeUnmanagedResources() { }

        #endregion
    }
}
