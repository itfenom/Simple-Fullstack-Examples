using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx.Simple
{
    public class SimpleViewModel : BindableBase
    {
        public override string Title => "Reactive simple binding";

        public ReactiveCommand<Unit, Unit> CloseCommand { get; }

        public SimpleViewModel()
        {
            //Simple binding
            _fullName = this
                .WhenAnyValue(x => x.FirstName, x => x.LastName, (f, l) => $"{f}  {l}")
                .ToProperty(this, x => x.FullName);

            var canClickMe = this
                .WhenAnyValue(x => x.FirstName, x => x.LastName, (f, l) => !string.IsNullOrEmpty(f) && !string.IsNullOrEmpty(l));
            ClickMeCommand = ReactiveCommand.CreateFromObservable(() => DummyAsync(), canClickMe);
            ClickMeCommand.Subscribe(_ =>
            {
                Message = "Finished";
                StatusBarContentMsg = "Ready";
            });

            //Greetings
            Greeting = "Greeting";
            var greetings = new Dictionary<string, string>() {
                { "English", "Hello World!" },
                { "French", "Bonjour le monde!" },
                { "German", "Hallo Welt!" },
                { "Japanese", "Kon'nichiwa sekai!" },
                { "Spanish", "¡Hola Mundo!" },
            };

            string[] keys = greetings.Keys.ToArray();

            // select next language every 3 seconds (100 times)
            _lang =
            Observable.Interval(TimeSpan.FromSeconds(3))
                .Take(100)
                .Select(_ => keys[(Array.IndexOf(keys, Lang) + 1) % keys.Count()])
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.Lang, "Language");

            // update Greeting when language changes
            this.WhenAnyValue(x => x.Lang)
                .Where(lang => keys.Contains(lang))
                .Select(x => greetings[x])
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => Greeting = x);

            //WordCount
            this.WhenAnyValue(x => x.FullName)
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.Trim().ToCharArray().Length - 1)
                .ToProperty(this, vm => vm.WordCount, out _wordCount);

            //CurrentTime
            _currentTime = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(_ => DateTime.Now.ToLongTimeString())
                .ToProperty(this, x => x.CurrentTime, out _currentTime);

            //Closing
            CloseCommand = ReactiveCommand.Create(() => OnClose());
            StatusBarContentMsg = "Ready";
        }

        private readonly ObservableAsPropertyHelper<string> _fullName;
        public string FullName => _fullName.Value;

        private string _firstName;

        public string FirstName
        {
            get => _firstName;
            set => this.RaiseAndSetIfChanged(ref _firstName, value);
        }

        private string _lastName;

        public string LastName
        {
            get => _lastName;
            set => this.RaiseAndSetIfChanged(ref _lastName, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public ReactiveCommand<Unit, Unit> ClickMeCommand { get; }

        private IObservable<Unit> DummyAsync()
        {
            StatusBarContentMsg = "Running...";
            return Observable
                .Return(new Random().Next(0, 10))
                .Delay(TimeSpan.FromMilliseconds(500))
                .Do(
                    result =>
                    {
                        if (result > 10)
                        {
                            throw new InvalidOperationException("Failed to proceed...");
                        }
                    }
                ).Select(_ => Unit.Default);
        }


        private string _statusBarContentMsg;

        public string StatusBarContentMsg
        {
            get => _statusBarContentMsg;
            set => this.RaiseAndSetIfChanged(ref _statusBarContentMsg, value);
        }

        //WordCount
        private readonly ObservableAsPropertyHelper<int> _wordCount;

        public int WordCount
        {
            get
            {
                if (_wordCount == null) return 0;
                if (_wordCount.Value < 0) return 0;
                return _wordCount.Value;
            }
        }

        //Current Time

        private readonly ObservableAsPropertyHelper<string> _currentTime;
        public string CurrentTime => _currentTime.Value;

        //Greetings
        private string _greetings;

        public string Greeting
        {
            get => _greetings;
            set => this.RaiseAndSetIfChanged(ref _greetings, value);
        }

        private readonly ObservableAsPropertyHelper<string> _lang;
        public string Lang => _lang.Value;


        private void OnClose()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Title == Title)
                {
                    window.Close();
                }
            }
        }
    }
}


/*
 private ObservableAsPropertyHelper<bool> _canAdd;
 public bool CanAdd => _canAdd?.Value ?? false;
 */
