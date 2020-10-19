using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Playground.WpfApp.Forms.ReactiveEx.GettingStarted
{
    public class AggregationViewModel : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<AggregationItem> _items;
        public ReadOnlyObservableCollection<AggregationItem> Items => _items;

        public string Description => "Aggregate over a collection which is filtered on a property.";

        private int _count;
        private int _max;
        private double _stdDev;
        private double _avg;
        private int _min;
        private double _sumOfOddNumbers;
        private int _sum;
        private int _countIncluded;

        public AggregationViewModel()
        {
            var sourceList = new SourceList<AggregationItem>();

            sourceList.AddRange(Enumerable.Range(1, 15).Select(i => new AggregationItem(i)));

            //Load items to display to user and allow them to include items or not

            var listLoader = sourceList.Connect()
                .Sort(SortExpressionComparer<AggregationItem>.Ascending(vm => vm.Number))
                .ObserveOnDispatcher()
                .Bind(out _items)
                .Subscribe();

            // share the connection because we are doing multiple aggregations
            var aggregatable = sourceList.Connect()
                .AutoRefresh()
                .Filter(vm => vm.IncludeInTotal)
                .Publish();

            //Do a custom aggregation (ToCollection() produces a readonly collection of underlying data)
            var sumOfOddNumbers = aggregatable.ToCollection()
                .Select(collection => collection.Where(i => i.Number % 2 == 1).Select(ai => ai.Number).Sum())
                .Subscribe(sum => SumOfOddNumbers = sum);

            _cleanUp = new CompositeDisposable(sourceList,
                listLoader,
                aggregatable.Count().Subscribe(count => Count = count),
                aggregatable.Sum(ai => ai.Number).Subscribe(sum => Sum = sum),
                aggregatable.Avg(ai => ai.Number).Subscribe(average => Avg = Math.Round(average, 2)),
                aggregatable.Minimum(ai => ai.Number).Subscribe(max => Max = max),
                aggregatable.Maximum(ai => ai.Number).Subscribe(min => Min = min),
                aggregatable.StdDev(ai => ai.Number).Subscribe(std => StdDev = Math.Round(std, 2)),
                sumOfOddNumbers,
                aggregatable.Connect());
        }

        public int Count
        {
            get => _count;
            set => SetAndRaise(ref _count, value);
        }

        public int CountIncluded
        {
            get => _countIncluded;
            set => SetAndRaise(ref _countIncluded, value);
        }

        public int Sum
        {
            get => _sum;
            set => SetAndRaise(ref _sum, value);
        }

        public int Min
        {
            get => _min;
            set => SetAndRaise(ref _min, value);
        }

        public int Max
        {
            get => _max;
            set => SetAndRaise(ref _max, value);
        }

        public double StdDev
        {
            get => _stdDev;
            set => SetAndRaise(ref _stdDev, value);
        }

        public double Avg
        {
            get => _avg;
            set => SetAndRaise(ref _avg, value);
        }

        public double SumOfOddNumbers
        {
            get => _sumOfOddNumbers;
            set => this.SetAndRaise(ref _sumOfOddNumbers, value);
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }

    public class AggregationItem : AbstractNotifyPropertyChanged
    {
        public int Number { get; }

        private bool _includeInTotal = true;

        public AggregationItem(int number)
        {
            Number = number;
        }

        public bool IncludeInTotal
        {
            get => _includeInTotal;
            set => SetAndRaise(ref _includeInTotal, value);
        }
    }
}
