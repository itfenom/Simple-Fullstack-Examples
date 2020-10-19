using System.Windows;
using System.Windows.Controls;

namespace Playground.WpfApp.Forms.OtherEx.DataTemplating
{
    public class TaskListDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // ReSharper disable once UsePatternMatching
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is DataTemplatingModel)
            {
                // ReSharper disable once TryCastAlwaysSucceeds
                var taskItem = item as DataTemplatingModel;

                if (taskItem.Priority == 1)
                {
                    return element.FindResource("ImportantTaskTemplate") as DataTemplate;
                }

                return element.FindResource("MyTaskTemplate") as DataTemplate;
            }

            return null;
        }
    }
}
