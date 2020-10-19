using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Playground.WpfApp.WpfUtilities
{
    public sealed class DataGridRowDoubleClickHandler : FrameworkElement
    {
        public DataGridRowDoubleClickHandler(DataGrid dataGrid)
        {
            MouseButtonEventHandler handler = (sender, args) =>
            {
                var row = sender as DataGridRow;
                if (row != null && row.IsSelected)
                {
                    var methodName = GetMethodName(dataGrid);

                    var dataContextType = dataGrid.DataContext.GetType();
                    var method = dataContextType.GetMethod(methodName);
                    if (method == null)
                    {
                        throw new MissingMethodException(methodName);
                    }

                    method.Invoke(dataGrid.DataContext, null);
                }
            };

            dataGrid.LoadingRow += (s, e) =>
            {
                e.Row.MouseDoubleClick += handler;
            };

            dataGrid.UnloadingRow += (s, e) =>
            {
                e.Row.MouseDoubleClick -= handler;
            };
        }

        public static string GetMethodName(DataGrid dataGrid)
        {
            return (string)dataGrid.GetValue(MethodNameProperty);
        }

        public static void SetMethodName(DataGrid dataGrid, string value)
        {
            dataGrid.SetValue(MethodNameProperty, value);
        }

        public static readonly DependencyProperty MethodNameProperty = DependencyProperty.RegisterAttached("MethodName", typeof(string), typeof(DataGridRowDoubleClickHandler),
            new PropertyMetadata((o, e) =>
            {
                var dataGrid = o as DataGrid;
                if (dataGrid != null)
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    new DataGridRowDoubleClickHandler(dataGrid);
                }
            }));
    }
}