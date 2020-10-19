using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;


namespace Playground.WpfApp.Forms.ReactiveEx.GettingStarted
{
    public class SelectableItemsViewModel : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<SimpleItemViewModel> _selected;
        private readonly ReadOnlyObservableCollection<SimpleItemViewModel> _notSelected;
        public ReadOnlyObservableCollection<SimpleItemViewModel> Selected => _selected;
        public ReadOnlyObservableCollection<SimpleItemViewModel> NotSelected => _notSelected;

        public string Description => "Filter on an object which implements INotifyPropertyChanged.";

        public SelectableItemsViewModel()
        {
            var sourceList = new SourceList<SimpleItem>();

            sourceList.AddRange(Enumerable.Range(1, 10).Select(i => new SimpleItem(i)));

            //create a shared list of view models
            var viewModels = sourceList
                .Connect()
                .Transform(simpleItem => new SimpleItemViewModel(simpleItem))
                .Publish();

            //filter on items which are selected and populate into an observable collection
            var selectedLoader = viewModels
                .AutoRefresh()
                .Filter(v => v.IsSelected == true)
                .Sort(SortExpressionComparer<SimpleItemViewModel>.Ascending(vm => vm.Number))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _selected)
                .Subscribe();

            //filter on items which are not selected and populate into an observable collection
            var notSelectedLoader = viewModels
                .AutoRefresh()
                .Filter(vm => vm.IsSelected == false)
                .Sort(SortExpressionComparer<SimpleItemViewModel>.Ascending(vm => vm.Number))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _notSelected)
                .Subscribe();

            _cleanUp = new CompositeDisposable(sourceList, selectedLoader, notSelectedLoader, viewModels.Connect());
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }

    public class SimpleItemViewModel : AbstractNotifyPropertyChanged
    {
        private bool _isSelected;
        public SimpleItem Item { get; }

        public int Number => Item.Id;

        public SimpleItemViewModel(SimpleItem item)
        {
            Item = item;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetAndRaise(ref _isSelected, value);
        }
    }

    public class SimpleItem
    {
        public int Id { get; }

        public SimpleItem(int id)
        {
            Id = id;
        }
    }
}
