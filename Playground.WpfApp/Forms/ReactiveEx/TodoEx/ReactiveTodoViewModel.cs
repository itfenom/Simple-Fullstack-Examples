using System.Reactive;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx.TodoEx
{
    public class ReactiveTodoViewModel : ReactiveObject
    {
        private SourceList<ReactiveTodoModel> _todos;

        public SourceList<ReactiveTodoModel> Todos
        {
            get => _todos;
            set => this.RaiseAndSetIfChanged(ref _todos, value);
        }

        private ReactiveTodoModel _selectedTodo;
        public ReactiveTodoModel SelectedTodo
        {
            get => _selectedTodo;
            set => this.RaiseAndSetIfChanged(ref _selectedTodo, value);
        }

        private ObservableAsPropertyHelper<bool> _canAdd;
        public bool CanAdd => _canAdd?.Value ?? false;

        private string _todoTitle;
        public string TodoTitle
        {
            get => _todoTitle;
            set => this.RaiseAndSetIfChanged(ref _todoTitle, value);
        }

        public ReactiveCommand<Unit, Unit> AddCommand { get; private set; }

        public ReactiveTodoViewModel()
        {
            this
                .WhenAnyValue(x => x.TodoTitle, title =>
                    !string.IsNullOrEmpty(title))
                .ToProperty(this, x => x.CanAdd, out _canAdd);

            AddCommand = ReactiveCommand.CreateFromTask(() =>
            {
                Todos.Add(new ReactiveTodoModel() { Title = TodoTitle });
                TodoTitle = string.Empty;
                return Task.CompletedTask;

            }, this.WhenAnyValue(x => x.CanAdd, canAdd => canAdd && canAdd));

            Todos = new SourceList<ReactiveTodoModel>();
            Todos.Add(new ReactiveTodoModel { IsDone = false, Title = "Go to Sleep" });
            Todos.Add(new ReactiveTodoModel { IsDone = false, Title = "Go get some dinner" });
            Todos.Add(new ReactiveTodoModel { IsDone = false, Title = "Watch GOT" });
            Todos.Add(new ReactiveTodoModel { IsDone = false, Title = "Code code and code!!!!" });
        }
    }

    public class ReactiveTodoModel : ReactiveObject
    {
        public string Title { get; set; }
        bool _isDone;
        public bool IsDone
        {
            get => _isDone;
            set => this.RaiseAndSetIfChanged(ref _isDone, value);
        }
        public bool IsEnabled => !IsDone;
    }
}
