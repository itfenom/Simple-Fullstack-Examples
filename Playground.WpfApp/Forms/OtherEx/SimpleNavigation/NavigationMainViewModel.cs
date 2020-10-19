using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.OtherEx.SimpleNavigation
{
    public class NavigationMainViewModel : PropertyChangedBase
    {
        private readonly IDialogCoordinator _dialogCoordinator;

        public List<PropertyChangedBase> Children { get; protected set; }

        private PropertyChangedBase _selectedChild;

        public PropertyChangedBase SelectedChild
        {
            get => _selectedChild;
            set => SetPropertyValue(ref _selectedChild, value);
        }

        public NavigationMainViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
            Children = new List<PropertyChangedBase>();
            SelectedChild = null;
            NavigateCommand = new DelegateCommand<object>(param => ToggleViews(param));
            SayHelloCommand = new DelegateCommand(() => OnSayHello());
        }

        public ICommand NavigateCommand { get; }

        public ICommand SayHelloCommand { get; }

        private void  OnSayHello()
        {
            _dialogCoordinator.ShowMessageAsync(this, "Say Hello", "Hello from Navigation Main ViewModel/View!");
        }

        private void ToggleViews(object param)
        {
            if (param != null)
            {
                var viewToLoad = param.ToString();
                if (viewToLoad == "ViewA")
                {
                    GoToViewA();
                }
                else if (viewToLoad == "ViewB")
                {
                    GoToViewB();
                }
            }
        }

        private void GoToViewA()
        {
            var viewAvm = Children.FirstOrDefault(vm => vm.GetType() == typeof(NavigationAViewModel));
            if (viewAvm == null)
            {
                viewAvm = new NavigationAViewModel();
                Children.Add(viewAvm);
            }

            SelectedChild = null;
            SelectedChild = viewAvm;
            NotifyPropertyChanged("SelectedChild");
        }

        private void GoToViewB()
        {
            var viewBvm = Children.FirstOrDefault(vm => vm.GetType() == typeof(NavigationBViewModel));
            var viewAvm = Children.FirstOrDefault(vm => vm.GetType() == typeof(NavigationAViewModel));
            var messageToPass = string.Empty;

            if (viewAvm != null)
            {
                messageToPass = ((NavigationAViewModel)viewAvm).LastUpdated.ToString();
            }

            if (viewBvm == null)
            {
                viewBvm = new NavigationBViewModel(messageToPass);
                Children.Add(viewBvm);
            }
            else
            {
                ((NavigationBViewModel)viewBvm).Message = messageToPass;
            }

            SelectedChild = null;
            SelectedChild = viewBvm;
            NotifyPropertyChanged("SelectedChild");
        }
    }
}