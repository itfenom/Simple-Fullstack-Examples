using System.Reactive;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx.TodoEx
{
    public class AddItemViewModel : BindableBase
    {
        public AddItemViewModel()
        {
            var canClickOk = this.WhenAnyValue(
                x => x.Description,
                (d) => !string.IsNullOrEmpty(d) &&
                       d.Length > 0);

            Ok = ReactiveCommand.Create(
                () => new TodoItem { Description = Description },
                canClickOk);
            Cancel = ReactiveCommand.Create(() => { });
        }

        private string _description;

        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        public ReactiveCommand<Unit, TodoItem> Ok { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
    }
}
