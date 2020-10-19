using System;

namespace Playground.WpfApp.Mvvm
{
    public abstract class PropertyChangedBase : ObservableObject, IDisposable
    {
        public virtual string Title => "";

        public virtual void ReloadData() { }

        public virtual bool HasUnSavedChanges()
        {
            return false;
        }

        public virtual void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        public virtual bool IsBusy => false;

        #region IDispose Implementation

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed)
                {
                    if (disposing)
                    {
                        DisposeManagedResources();
                        // ReSharper disable once LocalizableElement
                        Console.WriteLine($"Disposing {GetType().Name} - {GetType().FullName}");
                    }

                    DisposeUnManagedResources();
                    IsDisposed = true;
                }
            }
            catch (Exception oEx)
            {
                Console.WriteLine(oEx.Message);
            }
        }

        protected virtual void DisposeManagedResources() { }

        protected virtual void DisposeUnManagedResources() { }

        #endregion IDispose Implementation
    }
}
