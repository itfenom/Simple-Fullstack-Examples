using System;

namespace Playground.WpfApp.Forms.ReactiveEx
{
    /// <summary>
    /// Simple base implementation of a memento.
    /// </summary>
    public class Memento : IDisposable
    {
        /// <summary>
        /// Creates the memento with the given state.
        /// </summary>
        /// <param name="state">The state held by the memento.</param>
        public Memento(object state)
        {
            State = state;
        }

        /// <summary>
        /// Gets the empty memento.
        /// </summary>
        public static Memento Empty { get; } = new Memento(null);

        /// <summary>
        /// The state held by the memento.
        /// </summary>
        public object State { get; }

        #region IDisposable Support

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        protected bool IsDisposed
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
        /// Disposes the managed resources.
        /// </summary>
        protected virtual void DisposeManagedResources() { }

        /// <summary>
        /// Disposes the unmanaged resources.
        /// </summary>
        protected virtual void DisposeUnmanagedResources() { }

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
                    DisposeManagedResources();
                }

                DisposeUnmanagedResources();
                IsDisposed = true;
            }
        }

        #endregion
    }
}
