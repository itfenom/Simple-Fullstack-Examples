using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.ListViewEx.ListViewPaging
{
    public class ListViewPagingViewModel : PropertyChangedBase
    {
        private readonly int _itemPerPage = 10;
        private readonly int _itemCount;

        public override string Title => "Paging in ListView";

        public ListViewPagingViewModel()
        {
            PopulateList();

            PagingObjectsViewList = new CollectionViewSource {Source = PagingObjects};
            PagingObjectsViewList.Filter += View_Filter;

            CurrentPageIndex = 0;
            _itemCount = PagingObjects.Count;
            CalculateTotalPages();

            NextCommand = new DelegateCommand(() => ShowNextPage());
            PreviousCommand = new DelegateCommand(() => ShowPreviousPage());
            FirstCommand = new DelegateCommand(() => ShowFirstPage());
            LastCommand = new DelegateCommand(() => ShowLastPage());
        }

        private void View_Filter(object sender, FilterEventArgs e)
        {
            int index = ((PagingObject)e.Item).Id - 1;
            if (index >= _itemPerPage * CurrentPageIndex && index < _itemPerPage * (CurrentPageIndex + 1))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        private void PopulateList()
        {
            _pagingObjects = new ObservableCollection<PagingObject>();
            for (int i = 0; i < 96; i++)
            {
                if (i % 2 == 0)
                {
                    _pagingObjects.Add(new PagingObject(i, "Person: " + i.ToString(), true));
                }
                else
                {
                    _pagingObjects.Add(new PagingObject(i, "Person: " + i.ToString(), false));
                }
            }
        }

        private void CalculateTotalPages()
        {
            if (_itemCount % _itemPerPage == 0)
            {
                TotalPages = (_itemCount / _itemPerPage);
            }
            else
            {
                TotalPages = (_itemCount / _itemPerPage) + 1;
            }
        }

        #region Properties

        private ObservableCollection<PagingObject> _pagingObjects;

        public ObservableCollection<PagingObject> PagingObjects => _pagingObjects;

        public CollectionViewSource PagingObjectsViewList { get; set; }

        private int _currentPageIndex;

        public int CurrentPageIndex
        {
            get => _currentPageIndex;
            set
            {
                SetPropertyValue(ref _currentPageIndex, value);
                NotifyPropertyChanged("CurrentPage");
            }
        }

        public int CurrentPage => _currentPageIndex + 1;

        private int _totalPages;

        public int TotalPages
        {
            get => _totalPages;
            set => SetPropertyValue(ref _totalPages, value);
        }

        private bool _showHideSelectColumn;

        public bool ShowHideSelectColumn
        {
            get => _showHideSelectColumn;
            set => SetPropertyValue(ref _showHideSelectColumn, value);
        }

        private bool _showHideNameColumn;

        public bool ShowHideNameColumn
        {
            get => _showHideNameColumn;
            set => SetPropertyValue(ref _showHideNameColumn, value);
        }

        private bool _showHideIdColumn;

        public bool ShowHideIdColumn
        {
            get => _showHideIdColumn;
            set => SetPropertyValue(ref _showHideIdColumn, value);
        }

        #endregion Properties

        #region Commands

        public ICommand NextCommand { get; }

        private void ShowNextPage()
        {
            CurrentPageIndex++;
            PagingObjectsViewList.View.Refresh();
        }

        public ICommand PreviousCommand { get; }

        private void ShowPreviousPage()
        {
            if (CurrentPageIndex == 0) return;
            CurrentPageIndex--;
            PagingObjectsViewList.View.Refresh();
        }

        public ICommand FirstCommand { get; }

        private void ShowFirstPage()
        {
            CurrentPageIndex = 0;
            PagingObjectsViewList.View.Refresh();
        }

        public ICommand LastCommand { get; }

        private void ShowLastPage()
        {
            CurrentPageIndex = TotalPages - 1;
            PagingObjectsViewList.View.Refresh();
        }

        #endregion Commands

}

    public class PagingObject
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsSelected { get; set; }

        public PagingObject(int id, string name, bool isSelected)
        {
            Id = id;
            Name = name;
            IsSelected = isSelected;
        }
    }
}