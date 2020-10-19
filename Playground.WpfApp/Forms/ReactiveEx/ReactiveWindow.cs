﻿using System.Windows;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx
{
    public class ReactiveWindow<TViewModel> : BaseWindow, IViewFor<TViewModel>
        where TViewModel : class
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(TViewModel),
                typeof(ReactiveWindow<TViewModel>),
                new PropertyMetadata(null));

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
    }
}
