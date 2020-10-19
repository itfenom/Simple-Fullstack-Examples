using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Forms.DataGridsEx.NavigationEx
{
    public class JobTitleDialogViewModel : PropertyChangedBase
    {
        public override string Title => "Select Job Title";

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly IDemoEmpRepository _repository;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private ObservableCollection<DemoJobTitleModel> _jobTitles;

        public CollectionView JobTitlesView { get; set; }

        private DemoJobTitleModel _selectedJobTitle;

        public DemoJobTitleModel SelectedJobTitle
        {
            get => _selectedJobTitle;
            set => SetPropertyValue(ref _selectedJobTitle, value);
        }

        private bool? _dialogResultDependencyPropertyVal;

        public bool? DialogResultDependencyPropertyVal
        {
            get => _dialogResultDependencyPropertyVal;
            set => SetPropertyValue(ref _dialogResultDependencyPropertyVal, value);
        }

        public JobTitleDialogViewModel(IDemoEmpRepository repository)
        {
            _repository = repository;

            //Load available job-titles
            var jobTitleList = new List<DemoJobTitleModel>();
            foreach (var item in _repository.GetAllJobTitles())
            {
                item.PropertyChanged += NavigationJobTitleDialogViewModel_PropertyChanged;
                jobTitleList.Add(item);
            }

            OkCommand = new DelegateCommand(() => SelectJobTitle(), () => (SelectedJobTitle != null));
            CancelCommand = new DelegateCommand(() => Cancel());

            _jobTitles = new ObservableCollection<DemoJobTitleModel>(jobTitleList);
            JobTitlesView = (CollectionView)new CollectionViewSource { Source = _jobTitles }.View;
            NotifyPropertyChanged("JobTitlesView");
        }

        private void NavigationJobTitleDialogViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        public void SelectJobTitle()
        {
            DialogResultDependencyPropertyVal = true;
        }

        public DelegateCommand OkCommand { get; }

        public DelegateCommand CancelCommand { get; }

        private void Cancel()
        {
            SelectedJobTitle = null;
            DialogResultDependencyPropertyVal = false;
        }

        protected override void DisposeManagedResources()
        {
            _jobTitles = null;
            JobTitlesView = null;
            _selectedJobTitle = null;
        }
    }
}
