using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData.Binding;


namespace Playground.WpfApp.Forms.ReactiveEx.TodoEx
{
    public class TodoListViewModel : BindableBase
    {
        public TodoListViewModel(IEnumerable<TodoItem> items)
        {
            Items = new ObservableCollection<TodoItem>(items);
        }

        public ObservableCollection<TodoItem> Items { get; }
    }

    public class TodoItem : AbstractNotifyPropertyChanged
    {
        private string _desc;

        public string Description
        {
            get => _desc;
            set => SetAndRaise(ref _desc, value);
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set => SetAndRaise(ref _isChecked, value);
        }
    }
}
