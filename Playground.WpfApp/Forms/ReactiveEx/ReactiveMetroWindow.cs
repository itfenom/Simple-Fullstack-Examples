using System;
using System.Windows;
using MahApps.Metro.Controls;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx
{
    public class ReactiveMetroWindow<TViewModel> : MetroWindow, IViewFor<TViewModel>
           where TViewModel : class
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(TViewModel),
                typeof(ReactiveMetroWindow<TViewModel>),
                new PropertyMetadata(OnViewModelChanged));

        public ReactiveMetroWindow()
        {
            _ = new WindowEventLoggingObserver<ReactiveMetroWindow<TViewModel>>(this);

            DataContextChanged += OnDataContextChanged;
        }

        public TViewModel BindingRoot => ViewModel;

        public TViewModel ViewModel
        {
            get => (TViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }

        protected override void OnClosed(EventArgs e)
        {
            DataContextChanged -= OnDataContextChanged;
            base.OnClosed(e);
        }

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && !ReferenceEquals(element.DataContext, e.NewValue))
            {
                element.DataContext = e.NewValue;
            }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!ReferenceEquals(ViewModel, e.NewValue))
            {
                ViewModel = e.NewValue as TViewModel;
            }
        }
    }
}
