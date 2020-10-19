using System;
using ReactiveUI;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Playground.WpfApp.Forms.ReactiveEx.TeapotCheckBoxes
{
    public partial class TeapotsView : IViewFor<TeapotsViewModel>, INotifyPropertyChanged
    {
        private TeapotsViewModel viewModel;
        public TeapotsView()
        {
            InitializeComponent();

            viewModel = new TeapotsViewModel();
            DataContext = viewModel;
            ViewModel = viewModel;

            var whistlingImage = new BitmapImage(new Uri("../../../Images/TeapotBoring.png", UriKind.Relative));
            var notWhistlingImage = new BitmapImage(new Uri("../../../Images/TeapotBoring.png", UriKind.Relative));
            var boilingImage = new BitmapImage(new Uri("../../../Images/TeapotBoring.png", UriKind.Relative));

            var whistlingImageGreen = new BitmapImage(new Uri("../../../Images/TeapotBoringGreen.png", UriKind.Relative));
            var notWhistlingImageGreen = new BitmapImage(new Uri("../../../Images/TeapotBoringGreen.png", UriKind.Relative));
            var boilingImageGreen = new BitmapImage(new Uri("../../../Images/TeapotBoringGreen.png", UriKind.Relative));


            this.WhenActivated(d =>
            {
                d(this.OneWayBind(ViewModel, vm => vm.CheckWhistlingCommand, v => v.Whistling.Command));
                d(this.OneWayBind(ViewModel, vm => vm.CheckNotWhistlingCommand, v => v.NotWhistling.Command));
                d(this.OneWayBind(ViewModel, vm => vm.CheckBoilingOverCommand, v => v.BoilingOver.Command));

                d(this.OneWayBind(ViewModel, vm => vm.CheckWhistlingCommand2, v => v.Whistling2.Command));
                d(this.OneWayBind(ViewModel, vm => vm.CheckNotWhistlingCommand2, v => v.NotWhistling2.Command));
                d(this.OneWayBind(ViewModel, vm => vm.CheckBoilingOverCommand2, v => v.BoilingOver2.Command));

                d(this.OneWayBind(ViewModel, vm => vm.TeapotCurrentState, v => v.Whistling.IsChecked, (teapotState) => teapotState == State.TeapotState.Whistling));
                d(this.OneWayBind(ViewModel, vm => vm.TeapotCurrentState, v => v.NotWhistling.IsChecked, (teapotState) => teapotState == State.TeapotState.NotWhistling));
                d(this.OneWayBind(ViewModel, vm => vm.TeapotCurrentState, v => v.BoilingOver.IsChecked, (teapotState) => teapotState == State.TeapotState.BoilingOver));
                d(this.OneWayBind(ViewModel, vm => vm.TeapotCurrentState, v => v.TeapotImage.Source, (teapotState) => teapotState == State.TeapotState.BoilingOver ? boilingImage : (teapotState == State.TeapotState.Whistling ? whistlingImage : notWhistlingImage)));

                d(this.OneWayBind(ViewModel, vm => vm.TeapotCurrentState2, v => v.Whistling2.IsChecked, (teapotState) => teapotState == State.TeapotState.Whistling));
                d(this.OneWayBind(ViewModel, vm => vm.TeapotCurrentState2, v => v.NotWhistling2.IsChecked, (teapotState) => teapotState == State.TeapotState.NotWhistling));
                d(this.OneWayBind(ViewModel, vm => vm.TeapotCurrentState2, v => v.BoilingOver2.IsChecked, (teapotState) => teapotState == State.TeapotState.BoilingOver));
                d(this.OneWayBind(ViewModel, vm => vm.TeapotCurrentState2, v => v.TeapotImage2.Source, (teapotState) => teapotState == State.TeapotState.BoilingOver ? boilingImageGreen : (teapotState == State.TeapotState.Whistling ? whistlingImageGreen : notWhistlingImageGreen)));

                d(this.OneWayBind(ViewModel, vm => vm.TeapotMessage, v => v.TeapotMessage.Text));
            });
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(TeapotsViewModel), typeof(TeapotsView), new PropertyMetadata(null));

        public TeapotsViewModel ViewModel
        {
            get => (TeapotsViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TeapotsViewModel)value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
