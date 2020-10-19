using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interactivity;
using ControlzEx;
using Playground.WpfApp.WpfUtilities;

namespace Playground.WpfApp.Behaviors
{
    public class SynchronizeWithSelectedValueBehavior : Behavior<Selector>
    {
        private ICollectionView _collectionView;
        private PropertyChangeNotifier _notifier;
        private bool _wasCollectionViewCreatedLocally;

        /// <summary>
        /// Called when attached to the target object.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject_Unloaded(AssociatedObject, new RoutedEventArgs());
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;

            base.OnDetaching();
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.SelectionChanged += OnAssociatedObjectSelectionChanged;

            // NOTE: Use the PropertyChangeNotifier here rather than a TypeDescriptor so that we
            //       don't root the associated object.
            _notifier = new PropertyChangeNotifier(AssociatedObject, nameof(Selector.ItemsSource));
            _notifier.ValueChanged += notifier_ValueChanged;

            // fire the "value changed" event manually when we're first hooking things up
            notifier_ValueChanged(_notifier, EventArgs.Empty);
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.SelectionChanged -= OnAssociatedObjectSelectionChanged;

            _notifier.ValueChanged -= notifier_ValueChanged;
            _notifier.Dispose();

            DetachCollection();
        }

        private void DetachCollection()
        {
            if (_wasCollectionViewCreatedLocally && _collectionView != null)
            {
                if (_collectionView is ListCollectionView listCollection)
                {
                    listCollection.DetachFromSourceCollection();
                }
                else
                {
                    _collectionView.DetachCollectionAndView();
                }
            }

            _collectionView = null;
        }

        private void notifier_ValueChanged(object sender, EventArgs e)
        {
            DetachCollection();

            if (AssociatedObject != null)
            {
                if (AssociatedObject.ItemsSource is ICollectionView collectionView)
                {
                    _collectionView = collectionView;
                    _wasCollectionViewCreatedLocally = false;
                }
                else
                {
                    _collectionView = CollectionViewSource.GetDefaultView(AssociatedObject);
                    _wasCollectionViewCreatedLocally = true;
                }
            }
        }

        /// <summary>
        /// Called when the selection changes on the associated object.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">
        /// The <see cref="SelectionChangedEventArgs" /> instance containing the event data.
        /// </param>
        private void OnAssociatedObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_collectionView == null)
            {
                return;
            }

            var item = _collectionView?.CurrentItem;
            if (AssociatedObject.IsSynchronizedWithCurrentItem == true && AssociatedObject.SelectedItem != item)
            {
                AssociatedObject.IsSynchronizedWithCurrentItem = false;
                // ReSharper disable once PossibleNullReferenceException
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    AssociatedObject.SelectedItem = item;
                    AssociatedObject.IsSynchronizedWithCurrentItem = true;
                }));
            }
        }
    }
}
