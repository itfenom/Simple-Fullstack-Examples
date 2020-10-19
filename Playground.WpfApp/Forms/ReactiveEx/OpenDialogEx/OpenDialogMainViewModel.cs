using System;
using System.Reactive;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx.OpenDialogEx
{
    public class OpenDialogMainViewModel : ReactiveObject
    {
        private string _selectedValue;

        public string SelectedValue
        {
            get => _selectedValue;
            set => this.RaiseAndSetIfChanged(ref _selectedValue, value);
        }

        public ReactiveCommand<Unit, Unit> OpenAsDialog { get; }
        public Interaction<Unit, Unit> OpenAsDialogInteraction { get; } = new Interaction<Unit, Unit>();

        public OpenDialogMainViewModel()
        {
            OpenAsDialog = ReactiveCommand.Create(() =>
            {
                OpenAsDialogInteraction.Handle(Unit.Default).Subscribe();
            });
        }
    }
}
