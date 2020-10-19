using System.Collections.ObjectModel;
using System.Linq;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Forms.TreeViewEx.TreeViewSelectItem
{
    public class TreeViewSelectItemViewModel : PropertyChangedBase
    {
        public override string Title => "TreeView selectedItem/Filtering";

        private ObservableCollection<ContinentNode> _continentNodes;

        public ObservableCollection<ContinentNode> ContinentNodes
        {
            get => _continentNodes;
            set => SetPropertyValue(ref _continentNodes, value);
        }

        private ObservableCollection<IWorld> _selectedTreeViewItems;

        public ObservableCollection<IWorld> SelectedTreeViewItems
        {
            get => _selectedTreeViewItems;
            set => SetPropertyValue(ref _selectedTreeViewItems, value);
        }

        private IWorld _selectedItem;

        public IWorld SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value == _selectedItem) return;
                SetPropertyValue(ref _selectedItem, value);
                UpdateSelectedItemInListView(_selectedItem);
            }
        }

        public TreeViewSelectItemViewModel()
        {
            _selectedTreeViewItems = new ObservableCollection<IWorld>();
            try
            {
                _continentNodes = new ObservableCollection<ContinentNode>(
                (from continent in WorldRepository.GetContinents()
                 select new ContinentNode(continent))
                .ToList());
            }
            finally
            {
                ContinentNodes = _continentNodes;

                //Select the first item on load
                var itemToSelect = ContinentNodes[0];
                itemToSelect.IsExpanded = true;
                itemToSelect.IsSelected = true;
                SelectedItem = itemToSelect;
            }
        }

        private void UpdateSelectedItemInListView(IWorld selectedItem)
        {
            _selectedTreeViewItems.Clear();
            _selectedTreeViewItems.Add(selectedItem);
            NotifyPropertyChanged("SelectedTreeViewItems");
        }

        private bool _isStringContainedSearchOption;

        public bool IsStringContainedSearchOption
        {
            get => _isStringContainedSearchOption;
            set
            {
                if (_isStringContainedSearchOption == value) return;
                SetPropertyValue(ref _isStringContainedSearchOption, value);
            }
        }
    }

    public interface IWorld
    {
        string Name { get; }
        bool IsSelected { get; }
        string ToolTipText { get; }
        string Type { get; }
    }
}