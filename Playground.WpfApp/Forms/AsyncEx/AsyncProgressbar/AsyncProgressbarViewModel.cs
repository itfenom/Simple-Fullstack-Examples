using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.AsyncEx.AsyncProgressbar
{
    public class AsyncProgressbarViewModel : PropertyChangedBase
    {
        public override string Title => "Async data retrieval while displaying progressbar";

        public ObservableCollection<string> ListBox { get; set; }

        private bool _isProcessing;

        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                if (_isProcessing == value) return;
                SetPropertyValue(ref _isProcessing, value);
            }
        }

        private bool _isProcessingPercentageBar;

        public bool IsProcessingPercentageBar
        {
            get => _isProcessingPercentageBar;
            set => SetPropertyValue(ref _isProcessingPercentageBar, value);
        }

        private string _percentageText;

        public string PercentageText
        {
            get => _percentageText;
            set => SetPropertyValue(ref _percentageText, value);
        }

        private readonly AsyncDataRetrieval _dataRetrieval;

        private readonly AsyncDataRepository _repository;

        public AsyncProgressbarViewModel()
        {
            ListBox = new ObservableCollection<string>();
            _dataRetrieval = new AsyncDataRetrieval();
            _dataRetrieval.WorkPerformed += OnMessageReceived;

            _repository = new AsyncDataRepository();
            _repository.WorkPerformed += OnDataRetrieved;

            LoadPercentageDataCommand = new AsyncDelegateCommand(OnLoadPercentageData);
            LoadDataCommand = new AsyncDelegateCommand(OnLoadDataCommandAsync);
            CloseCommand = new DelegateCommand(() => OnClose());
            ResetCommand = new DelegateCommand(() => OnReset());

            OnReset();
        }

        private void OnMessageReceived(object sender, WorkPerformedEventArgs e)
        {
            // ReSharper disable once PossibleNullReferenceException
            Application.Current.Dispatcher.Invoke(() =>
            {
                ListBox.Add(e.Data);
            });
        }

        private void OnDataRetrieved(object sender, WorkPerformedEvent e)
        {
            // ReSharper disable once PossibleNullReferenceException
            Application.Current.Dispatcher.Invoke(() =>
            {
                PercentageText = e.Data;
                NotifyPropertyChanged("PercentageText");
            });
        }

        #region Command/Buttons

        public ICommand LoadDataCommand { get; }

        private async Task OnLoadDataCommandAsync()
        {
            OnReset();

            IsProcessing = true;
            ListBox.Add("Starting demo!\n\n");

            try
            {
                string result = await _dataRetrieval.DoStuffAsync();
                ListBox.Add(result);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        public ICommand ResetCommand { get; }

        private void OnReset()
        {
            _isProcessingPercentageBar = false;
            NotifyPropertyChanged("IsProcessingPercentageBar");

            _isProcessing = false;
            NotifyPropertyChanged("IsProcessing");

            ListBox.Clear();
        }

        public ICommand LoadPercentageDataCommand { get; }

        private async Task OnLoadPercentageData()
        {
            OnReset();

            _isProcessingPercentageBar = true;
            NotifyPropertyChanged("IsProcessingPercentageBar");

            var result = await _repository.DoStuffAsync();

            MessageBox.Show(result);
        }

        public ICommand CloseCommand { get; }

        private void OnClose()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    _dataRetrieval.WorkPerformed -= OnMessageReceived;
                    window.Close();
                }
            }
        }
        #endregion
    }
}
