using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx.TodoEx
{
    public class TodoMainViewModel : BindableBase
    {
        public override string Title => "Simple Todo using ReactiveUI";

        private BindableBase _content;

        public BindableBase Content
        {
            get => _content;
            private set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        private bool _isAddButtonVisible;

        public bool IsAddButtonVisible
        {
            get => _isAddButtonVisible;
            set => this.RaiseAndSetIfChanged(ref _isAddButtonVisible, value);
        }

        public TodoListViewModel List { get; }

        public TodoMainViewModel()
        {
            IsAddButtonVisible = true;
            var items = new[]
            {
                new TodoItem {Description = "Walk the dog"},
                new TodoItem {Description = "Buy some milk"},
                new TodoItem {Description = "Learn ReactiveUI", IsChecked = true}
            };

            Content = List = new TodoListViewModel(items);
            AddNewItemCommand = ReactiveCommand.Create(() => AddItem());
        }

        public ReactiveCommand<Unit, Unit> AddNewItemCommand { get; }

        public void AddItem()
        {
            IsAddButtonVisible = false;
            var vm = new AddItemViewModel();

            Observable.Merge(
                    vm.Ok,
                    vm.Cancel.Select(_ => (TodoItem)null))
                .Take(1)
                .Subscribe(model =>
                {
                    if (model != null)
                    {
                        List.Items.Add(model);
                    }

                    IsAddButtonVisible = true;
                    Content = List;
                });

            Content = vm;
        }
    }
}
