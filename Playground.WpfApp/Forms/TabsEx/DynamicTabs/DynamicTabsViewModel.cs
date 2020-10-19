using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.TabsEx.DynamicTabs
{
    public class DynamicTabsViewModel : PropertyChangedBase
    {
        private readonly ObservableCollection<ITab> _tabs;

        public ICollection<ITab> Tabs { get; }

        public DynamicTabsViewModel()
        {
            _title = "Create Tabs Dynamically";
            NewTabCommand = new DelegateCommand(() => NewTab());
            NewTimeTabCommand = new DelegateCommand(() => NewTimeTab());

            _tabs = new ObservableCollection<ITab>();
            _tabs.CollectionChanged += Tabs_CollectionChanged;

            Tabs = _tabs;
        }

        private void Tabs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ITab tab;
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                tab = (ITab)e.NewItems[0];
                tab.CloseRequested += OnCloseRequested;
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                tab = (ITab)e.OldItems[0];
                tab.CloseRequested -= OnCloseRequested;
            }
        }

        private void OnCloseRequested(object sender, EventArgs e)
        {
            Tabs.Remove((ITab)sender);
        }

        public ICommand NewTabCommand { get; }

        private void NewTab()
        {
            _tabs.Add(new DateTab());
        }

        public ICommand NewTimeTabCommand { get; }

        private void NewTimeTab()
        {
            _tabs.Add(new TimeTab());
        }

        private string _title;

        public override string Title
        {
            get { return _title; }
        }
    }
}