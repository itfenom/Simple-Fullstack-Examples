using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx.TeapotCheckBoxes
{
    public class TeapotsViewModel : ReactiveObject
    {
        State.TeapotState _teapotState;
        public State.TeapotState TeapotCurrentState
        {
            get => _teapotState;
            set => this.RaiseAndSetIfChanged(ref _teapotState, value);
        }
        State.TeapotState _teapotState2;
        public State.TeapotState TeapotCurrentState2
        {
            get => _teapotState2;
            set => this.RaiseAndSetIfChanged(ref _teapotState2, value);
        }

        string _teapotMessage;
        public string TeapotMessage
        {
            get => _teapotMessage;
            set => this.RaiseAndSetIfChanged(ref _teapotMessage, value);
        }

        public bool TupleContainsAtLeastOne(State.TeapotState state, Tuple<State.TeapotState, State.TeapotState> teapotState)
        {
            return (teapotState.Item1 == state || teapotState.Item2 == state);
        }

        public bool TupleContainsTwo(State.TeapotState state, Tuple<State.TeapotState, State.TeapotState> teapotState)
        {
            return (teapotState.Item1 == state && teapotState.Item2 == state);
        }

        public TeapotsViewModel()
        {
            TeapotCurrentState = State.TeapotState.Whistling;
            CheckWhistlingCommand = ReactiveCommand.Create(() => TeapotCurrentState = State.TeapotState.Whistling);
            CheckNotWhistlingCommand = ReactiveCommand.Create(() => TeapotCurrentState = State.TeapotState.NotWhistling);
            CheckBoilingOverCommand = ReactiveCommand.Create(() => TeapotCurrentState = State.TeapotState.BoilingOver);
            CheckWhistlingCommand2 = ReactiveCommand.Create(() => TeapotCurrentState2 = State.TeapotState.Whistling);
            CheckNotWhistlingCommand2 = ReactiveCommand.Create(() => TeapotCurrentState2 = State.TeapotState.NotWhistling);
            CheckBoilingOverCommand2 = ReactiveCommand.Create(() => TeapotCurrentState2 = State.TeapotState.BoilingOver);

            var teapotOneObservable = this.WhenAnyValue(x => x.TeapotCurrentState);
            var teapotTwoObservable = this.WhenAnyValue(x => x.TeapotCurrentState2);

            Observable.CombineLatest(teapotOneObservable, teapotTwoObservable, (teapot1, teapot2) => Tuple.Create(teapot1, teapot2)).Subscribe(tuple =>
            {
                TeapotMessage = State.EnumToStringDictionary[tuple];
            });
        }

        public ReactiveCommand<Unit, State.TeapotState> CheckWhistlingCommand { get; private set; }
        public ReactiveCommand<Unit, State.TeapotState> CheckNotWhistlingCommand { get; private set; }
        public ReactiveCommand<Unit, State.TeapotState> CheckBoilingOverCommand { get; private set; }
        public ReactiveCommand<Unit, State.TeapotState> CheckWhistlingCommand2 { get; private set; }
        public ReactiveCommand<Unit, State.TeapotState> CheckNotWhistlingCommand2 { get; private set; }
        public ReactiveCommand<Unit, State.TeapotState> CheckBoilingOverCommand2 { get; private set; }
    }
}
