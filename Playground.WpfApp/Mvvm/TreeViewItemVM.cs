using System.Collections.ObjectModel;

namespace Playground.WpfApp.Mvvm
{
    public class TreeViewItemVm : ObservableObject
    {
        /// <summary>
        /// Base class for all ViewModel classes displayed by TreeViewItems.
        /// This acts as an adapter between a raw data object and a TreeViewItem.
        /// </summary>

        #region Data

        private static readonly TreeViewItemVm DummyChild = new TreeViewItemVm();

        private readonly ObservableCollection<TreeViewItemVm> _children;
        private readonly TreeViewItemVm _parent;

        private bool _isExpanded;
        private bool _isSelected;

        #endregion Data

        #region Constructors

        protected TreeViewItemVm(TreeViewItemVm parent, bool lazyLoadChildren)
        {
            _parent = parent;

            _children = new ObservableCollection<TreeViewItemVm>();

            if (lazyLoadChildren)
                _children.Add(DummyChild);
        }

        // This is used to create the DummyChild instance.
        private TreeViewItemVm() { }

        #endregion Constructors

        #region Children

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<TreeViewItemVm> Children => _children;

        #endregion Children

        #region HasLoadedChildren

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild => Children.Count == 1 && Children[0] == DummyChild;

        #endregion HasLoadedChildren

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (value != _isExpanded)
                {
                    SetPropertyValue(ref _isExpanded, value);
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (HasDummyChild)
                {
                    Children.Remove(DummyChild);
                    LoadChildren();
                }
            }
        }

        #endregion IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value != _isSelected)
                {
                    SetPropertyValue(ref _isSelected, value);
                }
            }
        }

        #endregion IsSelected

        #region LoadChildren

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LoadChildren() { }

        #endregion LoadChildren

        #region Parent

        public TreeViewItemVm Parent => _parent;

        #endregion Parent
    }
}
