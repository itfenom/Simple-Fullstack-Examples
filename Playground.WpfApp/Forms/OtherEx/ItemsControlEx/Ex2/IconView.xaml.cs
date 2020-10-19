using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Playground.WpfApp.Forms.OtherEx.ItemsControlEx.Ex2
{
    public partial class IconView : UserControl
    {
        private readonly IconViewModel _viewModel;

        public IconView()
        {
            InitializeComponent();
            _viewModel = (IconViewModel)rootGrid.DataContext;
        }

        public ObservableCollection<IconInfo> IconInfos
        {
            get => (ObservableCollection<IconInfo>)GetValue(IconInfosProperty);
            set => SetValue(IconInfosProperty, value);
        }

        public static readonly DependencyProperty IconInfosProperty =
            DependencyProperty.Register("IconInfos", typeof(ObservableCollection<IconInfo>),
                typeof(IconView), new PropertyMetadata(null, OnIconInfosSet));

        private static void OnIconInfosSet(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((IconView)d)._viewModel.IconInfos = e.NewValue as ObservableCollection<IconInfo>;
        }

        public double IconWidth
        {
            get => (double)GetValue(IconWidthProperty);
            set => SetValue(IconWidthProperty, value);
        }

        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register("IconWidth", typeof(double),
                typeof(IconView), new PropertyMetadata(-1.0));

        public Thickness IconMargin
        {
            get => (Thickness)GetValue(IconMarginProperty);
            set => SetValue(IconMarginProperty, value);
        }

        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness),
                typeof(IconView), new PropertyMetadata(new Thickness(0)));
    }
}