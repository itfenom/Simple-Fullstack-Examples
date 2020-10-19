using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Playground.Core.Utilities;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;
using Playground.WpfApp.WpfUtilities;

namespace Playground.WpfApp.Forms.OtherEx.CachingEx
{
    public class CachingViewModel : PropertyChangedBase
    {
        public override string Title => "Caching example";

        private readonly IDemoEmpRepository _repository;

        private string _key;

        private ObservableCollection<DemoJobTitleModel> _jobsList;

        public ObservableCollection<DemoJobTitleModel> JobsList
        {
            get => _jobsList;
            set => SetPropertyValue(ref _jobsList, value);
        }


        public CachingViewModel()
        {
            _repository = new DemoEmpRepository();
            _key = "Data_Key";

            LoadFromDbCommand = new DelegateCommand(() => OnLoadFromDb());
            LoadFromCacheCommand = new DelegateCommand(() => OnLoadFromCache());
            CloseCommand = new DelegateCommand(() => OnClose());
        }

        private void LoadData(List<DemoJobTitleModel> jobs)
        {
            UIHelper.SetBusyState();
            _jobsList = new ObservableCollection<DemoJobTitleModel>(jobs);
            NotifyPropertyChanged("JobsList");
        }

        #region Commands/Buttons
        public ICommand LoadFromDbCommand { get; }

        private void OnLoadFromDb()
        {
            var data = _repository.GetAllJobTitles();

            //remove from cache and insert into cache for later retrieval
            CacheEngine.Instance.RemoveItem(_key);
            CacheEngine.Instance.AddItem(_key, data);
            LoadData(data);
        }

        private List<DemoJobTitleModel> GetJobs()
        {
            return _repository.GetAllJobTitles();
        }

        public ICommand LoadFromCacheCommand { get; }

        private void OnLoadFromCache()
        {
            var cachedData = CacheEngine.Instance.GetItem<List<DemoJobTitleModel>>(_key);
            if (cachedData == null || cachedData.Count == 0)
            {
                var data = GetJobs();
                CacheEngine.Instance.AddItem(_key, data);
                LoadData(data);
            }
            else
            {
                LoadData(cachedData);
            }
        }

        public ICommand CloseCommand { get; }

        private void OnClose()
        {
            foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                }
            }
        }

        #endregion

        protected override void DisposeManagedResources()
        {
            _jobsList = null;
            CacheEngine.Instance.RemoveAllItems();
        }
    }
}
