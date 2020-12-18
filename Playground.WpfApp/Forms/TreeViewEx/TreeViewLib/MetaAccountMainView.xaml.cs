using System;
using System.Windows;

namespace Playground.WpfApp.Forms.TreeViewEx.TreeViewLib
{
    public partial class MetaAccountMainView
    {
        public MetaAccountMainView()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as IDisposable;

            if (viewModel != null)
                viewModel.Dispose();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;

            var viewModel = new MetaAccountMainViewModel();
            DataContext = viewModel;

            await viewModel.LoadSampleDataAsync();
        }
    }
}
