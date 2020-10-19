using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Playground.WpfApp.Mvvm
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        // ReSharper disable once RedundantAssignment
        protected void SetPropertyValue<T>(ref T target, T value, [CallerMemberName] string caller = null)
        {
            //if (object.Equals(target, value))
            //    return;
            target = value;
            NotifyPropertyChanged(caller);
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            // ReSharper disable once UseNullPropagation
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
