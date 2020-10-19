using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Playground.WpfApp.WpfUtilities
{
    /// <summary>
    /// Extensions for WPF-related items.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Determines whether all items in the collection are selected. If <c>true</c>, all items
        /// are selected. If <c>false</c>, there are no items selected. If <c>null</c>, some items
        /// are selected.
        /// </summary>
        /// <param name="items">The collection of items.</param>
        /// <returns>A value indicating whether all, none, or some of the items are selected.</returns>
        public static bool? AreAllItemsSelected(this IEnumerable<bool> items)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            int checkedItemCount = items?.Count(x => x) ?? 0;
            if (checkedItemCount == 0)
            {
                return false;
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once PossibleMultipleEnumeration
            if (checkedItemCount == items.Count())
            {
                return true;
            }

            return null;
        }

        public static bool DetachCollectionAndView(this ICollectionView cv)
        {
            var ncc = cv.SourceCollection as System.Collections.Specialized.INotifyCollectionChanged;
            if (ncc == null)
            {
                return false;
            }

            // Get the method that subscribes to OnCollectionChanged
            var mi = cv.GetType().GetMethod("OnCollectionChanged",
               System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null,
               // ReSharper disable once RedundantExplicitArrayCreation
               new Type[] { typeof(object), typeof(System.Collections.Specialized.NotifyCollectionChangedEventArgs) },
               null);

            // ReSharper disable once AssignNullToNotNullAttribute
            var handler = (System.Collections.Specialized.NotifyCollectionChangedEventHandler)Delegate.CreateDelegate(typeof(System.Collections.Specialized.NotifyCollectionChangedEventHandler), cv, mi);
            cv.CollectionChanged -= handler;

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether or not the object is currently in design mode.
        /// </summary>
        /// <param name="this">The object the property value is read from.</param>
        /// <returns><c>true</c> if the object is in design mode; otherwise, <c>false</c>.</returns>
        public static bool IsInDesignMode(this System.Windows.DependencyObject @this)
        {
            // NOTE: The check for Location contains VisualStudio is necessary for WPF controls when
            //       running in a Windows Forms application
            return DesignerProperties.GetIsInDesignMode(@this)
                || System.Reflection.Assembly.GetExecutingAssembly().Location.Contains("VisualStudio");
        }
    }
}
