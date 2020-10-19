using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Forms.AsyncEx.AdvAsyncAwaitEx
{
    public class AdvAsyncAwaitViewModel : PropertyChangedBase
    {
        public override string Title => "Advanced Task/Async and await";

        private readonly IDemoEmpRepository _repository;

        private ObservableCollection<DemoEmployeeModel> _employeesList;

        public ObservableCollection<DemoEmployeeModel> EmployeesList
        {
            get => _employeesList;
            set => SetPropertyValue(ref _employeesList, value);
        }

        private CancellationTokenSource _tokenSource;

        public AdvAsyncAwaitViewModel()
        {
            _repository = new DemoEmpRepository();
            _tokenSource = new CancellationTokenSource();
            _loadingDataWithTaskInProgress = false;
            _loadingDataWithAsyncInProgress = false;

            LoadDataWithTaskCommand = new DelegateCommand(() => OnLoadDataWithTask(), () => CanClickLoadDataWithTaskButton);
            LoadDataWithAsyncAwaitCommand = new DelegateCommand(() => OnLoadDataWithAsyncAwait(), () => CanClickLoadDataWithAsyncButton);
            CancelLoadingDataCommand = new DelegateCommand(() => OnCancelLoadingData());
        }

        private async Task<List<DemoEmployeeModel>> GetData(CancellationToken cancelToken = new CancellationToken())
        {
            await Task.Delay(3000); //wait for 3 seconds

            cancelToken.ThrowIfCancellationRequested();

            //throw new NotImplementedException("GetData Method is not implemented!");

            return _repository.GetAllEmployees();
        }

        #region Load data with Task

        public ICommand LoadDataWithTaskCommand { get; }

        private bool _loadingDataWithTaskInProgress;

        private bool CanClickLoadDataWithTaskButton => !_loadingDataWithTaskInProgress;

        private void OnLoadDataWithTask()
        {
            _tokenSource = new CancellationTokenSource();

            //Disable button to avoid double-clicks from user
            _loadingDataWithTaskInProgress = true;
            NotifyPropertyChanged("CanClickLoadDataWithTaskButton");

            //Clear ListBox
            _employeesList = new ObservableCollection<DemoEmployeeModel>();
            NotifyPropertyChanged("EmployeesList");

            var empTask = GetData(_tokenSource.Token);
            empTask.ContinueWith(
                task =>
                {
                    switch (task.Status)
                    {
                        case TaskStatus.RanToCompletion:
                            var list = empTask.Result;
                            _employeesList = new ObservableCollection<DemoEmployeeModel>(list);
                            NotifyPropertyChanged("EmployeesList");
                            break;

                        case TaskStatus.Canceled:
                            MessageBox.Show("Cancellation Requested.", "CANCELLED!");
                            break;

                        case TaskStatus.Faulted:
                            // ReSharper disable once PossibleNullReferenceException
                            foreach (var ex in task.Exception.Flatten().InnerExceptions)
                            {
                                MessageBox.Show(ex.Message, "ERROR!");
                            }
                            break;
                    }

                    //Enable the button
                    _loadingDataWithTaskInProgress = false;
                    NotifyPropertyChanged("CanClickLoadDataWithTaskButton");
                });
        }

        #endregion Load data with Task

        #region Load Data with AsyncAwait

        public ICommand LoadDataWithAsyncAwaitCommand { get; }

        private bool _loadingDataWithAsyncInProgress;

        private bool CanClickLoadDataWithAsyncButton => !_loadingDataWithAsyncInProgress;

        private async void OnLoadDataWithAsyncAwait()
        {
            _tokenSource = new CancellationTokenSource();
            _loadingDataWithAsyncInProgress = true;
            NotifyPropertyChanged("CanClickLoadDataWithAsyncButton");

            _employeesList = new ObservableCollection<DemoEmployeeModel>();
            NotifyPropertyChanged("EmployeesList");

            try
            {
                var empList = await GetData(_tokenSource.Token);
                _employeesList = new ObservableCollection<DemoEmployeeModel>(empList);
                NotifyPropertyChanged("EmployeesList");
            }
            catch (OperationCanceledException ex)
            {
                MessageBox.Show(ex.Message, "CANCELLED!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR!");
            }
            finally
            {
                _loadingDataWithAsyncInProgress = false;
                NotifyPropertyChanged("CanClickLoadDataWithAsyncButton");
            }
        }

        #endregion Load Data with AsyncAwait

        public ICommand CancelLoadingDataCommand { get; }

        private void OnCancelLoadingData()
        {
            _tokenSource.Cancel();
        }

        protected override void DisposeManagedResources()
        {
            _employeesList = null;
            _tokenSource = null;
        }
    }
}