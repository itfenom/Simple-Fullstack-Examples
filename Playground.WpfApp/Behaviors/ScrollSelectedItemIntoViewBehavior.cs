using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Playground.WpfApp.Behaviors
{
    /// <summary>
    /// A behavior for data-grids that scrolls an item into view when the selected item changes.
    /// </summary>
    public class ScrollSelectedItemIntoViewBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid grid)
            {
                var selectedItem = grid.SelectedItem;
                if (selectedItem != null)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    grid.Dispatcher.InvokeAsync(() =>
                    {
                        grid.UpdateLayout();
                        grid.ScrollIntoView(selectedItem, null);
                    });
                }
            }
        }
    }
}
