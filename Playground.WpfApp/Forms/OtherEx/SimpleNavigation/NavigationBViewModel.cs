using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.OtherEx.SimpleNavigation
{
    public class NavigationBViewModel : ValidationPropertyChangedBase
    {
        public NavigationBViewModel(string message)
        {
            _message = string.IsNullOrEmpty(message) ? "<Empty>" : message;

            NotifyPropertyChanged("Message");
        }

        private string _message;

        public string Message
        {
            get => _message;
            set => SetPropertyValue(ref _message, value);
        }
    }
}