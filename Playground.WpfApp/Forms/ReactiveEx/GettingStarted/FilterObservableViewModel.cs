using DynamicData;
using DynamicData.Binding;
using Playground.WpfApp.Mvvm;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace Playground.WpfApp.Forms.ReactiveEx.GettingStarted
{
    public class FilterObservableViewModel : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;

        private readonly ReadOnlyObservableCollection<FootballPlayer> _availablePlayers;

        private readonly ReadOnlyObservableCollection<FootballPlayer> _myTeamPeople;

        public ReadOnlyObservableCollection<FootballPlayer> AvailablePlayers => _availablePlayers;
        public ReadOnlyObservableCollection<FootballPlayer> MyTeam => _myTeamPeople;

        public string Description => "Filter observable which is a property of an object";

        public FilterObservableViewModel()
        {
            var people = CreateFootballerList();
            var sharedDataSoure = people.Connect().Publish();

            //Load available players
            var allPeopleLoader = sharedDataSoure
                .FilterOnObservable(person => person.IncludedChanged, included => !included)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _availablePlayers)
                .Subscribe();

            //Load selected players
            var includedPeopleLoader = sharedDataSoure
                .FilterOnObservable(person => person.IncludedChanged, included => included)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _myTeamPeople)
                .Subscribe();

            _cleanUp = new CompositeDisposable(people, allPeopleLoader, includedPeopleLoader, sharedDataSoure.Connect());
        }

        private ISourceList<FootballPlayer> CreateFootballerList()
        {
            var people = new SourceList<FootballPlayer>();
            people.AddRange(new[]
            {
                new FootballPlayer("Hennessey"),
                new FootballPlayer("Chester"),
                new FootballPlayer("Williams"),
                new FootballPlayer("Davies"),
                new FootballPlayer("Gunter"),
                new FootballPlayer("Allen"),
                new FootballPlayer("Ledley"),
                new FootballPlayer("Ramsey"),
                new FootballPlayer("Taylor"),
                new FootballPlayer("Bale"),
                new FootballPlayer("King"),
                new FootballPlayer("Hennessey"),
                new FootballPlayer("Collins")
            });
            return people;
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }

    public class FootballPlayer : AbstractNotifyPropertyChanged
    {
        public string Name { get; }
        public ICommand IncludeCommand { get; }
        public ICommand ExcludeCommand { get; }
        public IObservable<bool> IncludedChanged { get; }

        public FootballPlayer(string name)
        {
            var includeChanged = new BehaviorSubject<bool>(false);

            Name = name;
            IncludeCommand = new DelegateCommand(() => includeChanged.OnNext(true));
            ExcludeCommand = new DelegateCommand(() => includeChanged.OnNext(false));
            IncludedChanged = includeChanged.AsObservable();
        }
    }

    public static class DynamicDataEx
    {
        public static IObservable<IChangeSet<TObject>> FilterOnObservable<TObject, TValue>(this IObservable<IChangeSet<TObject>> source,
            Func<TObject, IObservable<TValue>> observableSelector,
            Func<TValue, bool> predicate)
        {
            return Observable.Create<IChangeSet<TObject>>(observer =>
            {
                var locker = new object();

                //create a local list to store matching values
                var resultList = new SourceList<TObject>();

                //monitor whether the observable has changed and amend local list accordingly
                var observableChangedMonitor = source.SubscribeMany(item =>
                {
                    return observableSelector(item).Synchronize(locker)
                        .Subscribe(value =>
                        {
                            var isMatched = predicate(value);
                            if (isMatched)
                            {
                                //prevent duplicates with contains check - otherwise use a source cache
                                if (!resultList.Items.Contains(item))
                                    resultList.Add(item);
                            }
                            else
                            {
                                resultList.Remove(item);
                            }
                        });
                }).Subscribe(t => { }, observer.OnError);

                //publish results from the local list
                var publisher = resultList.Connect().SubscribeSafe(observer);

                return new CompositeDisposable(observableChangedMonitor, resultList, publisher);

            });
        }
    }
}
