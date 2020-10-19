using DynamicData.Binding;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;

namespace Playground.WpfApp.Forms.ReactiveEx.GettingStarted
{
    public partial class GettingStartedMain
    {
        public GettingStartedMain()
        {
            InitializeComponent();
            DataContext = new GettingStartedMainViewModel();
        }
    }

    public class GettingStartedMainViewModel : AbstractNotifyPropertyChanged
    {
        private AbstractNotifyPropertyChanged _selectedChild;

        public AbstractNotifyPropertyChanged SelectedChild
        {
            get => _selectedChild;
            set => SetAndRaise(ref _selectedChild, value);
        }

        public List<AbstractNotifyPropertyChanged> Children { get; protected set; }

        public GettingStartedMainViewModel()
        {
            Children = new List<AbstractNotifyPropertyChanged>();
            LoadSelectedItemsCommand = ReactiveCommand.Create(() => LoadSelectedItems());
            LoadAggregationCommand = ReactiveCommand.Create(() => LoadAggregation());
            LoadFilterObservableCommand = ReactiveCommand.Create(() => LoadFilterObservable());

            LoadSelectedItems();
        }

        public ReactiveCommand<Unit, Unit> LoadSelectedItemsCommand { get; }

        private void LoadSelectedItems()
        {
            var selectedItemsVm = Children.FirstOrDefault(vm => vm.GetType() == typeof(SelectableItemsViewModel));
            if (selectedItemsVm == null)
            {
                selectedItemsVm = new SelectableItemsViewModel();
                Children.Add(selectedItemsVm);
            }

            SelectedChild = selectedItemsVm;
        }

        public ReactiveCommand<Unit, Unit> LoadAggregationCommand { get; }

        private void LoadAggregation()
        {
            var aggregationVm = Children.FirstOrDefault(vm => vm.GetType() == typeof(AggregationViewModel));
            if (aggregationVm == null)
            {
                aggregationVm = new AggregationViewModel();
                Children.Add(aggregationVm);
            }

            SelectedChild = aggregationVm;
        }

        public ReactiveCommand<Unit, Unit> LoadFilterObservableCommand { get; }

        private void LoadFilterObservable()
        {
            var filterObservableVm = Children.FirstOrDefault(vm => vm.GetType() == typeof(FilterObservableViewModel));
            if (filterObservableVm == null)
            {
                filterObservableVm = new FilterObservableViewModel();
                Children.Add(filterObservableVm);
            }

            SelectedChild = filterObservableVm;
        }
    }


    public class SelectableItemCollection
    {
        public List<SampleItem> Items { get; }

        public SelectableItemCollection()
        {
            Items = new List<SampleItem>
            {

                new SampleItem("Selectable Items", new SelectableItemsViewModel(),
                    "Filter on an object which implements INotifyPropertyChanged",
                    "SelectableItemsViewModel.cs"),

                new SampleItem("Aggregations", new AggregationViewModel(),
                    "Aggregate over a collection which is filtered on a property"
                    ,"AggregationViewModel.cs"),

                //new SampleItem("Filter An Observable", new  FilterObservableViewModel(),
                //    "Filter observable which is a property of an object",
                //    "FilterObservableViewModel.cs"),

                //new SampleItem("One to many join", new  JoinManyViewModel(),
                //    "Join two observable caches which have a one to many relation",
                //    "JoinManyViewModel.cs")

            };
        }
    }

    public class SampleItem
    {
        public string Title { get; }
        public string Description { get; }
        public object Content { get; }
        public string CodeFileDisplay { get; }

        public SampleItem(string title, object content, string description, string codeFileDisplay)
        {
            Title = title;
            Description = description;
            CodeFileDisplay = codeFileDisplay;
            Content = content;
        }
    }
}
