﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Playground.WpfApp.Behaviors
{
    public interface ICustomSorter : IComparer
    {
        ListSortDirection SortDirection { get; set; }
    }

    public class CustomSorter : ICustomSorter
    {
        public ListSortDirection SortDirection { get; set; }

        public int Compare(object x, object y)
        {
            //var materialPropX = x as MaterialPropertyModel;
            //var materialPropY = y as MaterialPropertyModel;
            //return materialPropX.PropertyName.CompareTo(materialPropY.PropertyName);
            return 0;
        }
    }

    public class CustomSortBehaviour
    {
        public static readonly DependencyProperty CustomSorterProperty =
    DependencyProperty.RegisterAttached("CustomSorter", typeof(ICustomSorter), typeof(CustomSortBehaviour));

        public static ICustomSorter GetCustomSorter(DataGridColumn gridColumn)
        {
            return (ICustomSorter)gridColumn.GetValue(CustomSorterProperty);
        }

        public static void SetCustomSorter(DataGridColumn gridColumn, ICustomSorter value)
        {
            gridColumn.SetValue(CustomSorterProperty, value);
        }

        public static readonly DependencyProperty AllowCustomSortProperty =
            DependencyProperty.RegisterAttached("AllowCustomSort", typeof(bool),
            typeof(CustomSortBehaviour), new UIPropertyMetadata(false, OnAllowCustomSortChanged));

        public static bool GetAllowCustomSort(DataGrid grid)
        {
            return (bool)grid.GetValue(AllowCustomSortProperty);
        }

        public static void SetAllowCustomSort(DataGrid grid, bool value)
        {
            grid.SetValue(AllowCustomSortProperty, value);
        }

        private static void OnAllowCustomSortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var existing = d as DataGrid;
            if (existing == null) return;

            var oldAllow = (bool)e.OldValue;
            var newAllow = (bool)e.NewValue;

            if (!oldAllow && newAllow)
            {
                existing.Sorting += HandleCustomSorting;
            }
            else
            {
                existing.Sorting -= HandleCustomSorting;
            }
        }

        private static void HandleCustomSorting(object sender, DataGridSortingEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null || !GetAllowCustomSort(dataGrid)) return;

            var listColView = dataGrid.ItemsSource as ListCollectionView;
            if (listColView == null)
                throw new Exception("The DataGrid's ItemsSource property must be of type, ListCollectionView");

            // Sanity check
            var sorter = GetCustomSorter(e.Column);
            if (sorter == null) return;

            // The guts.
            e.Handled = true;

            var direction = (e.Column.SortDirection != ListSortDirection.Ascending)
                                ? ListSortDirection.Ascending
                                : ListSortDirection.Descending;

            e.Column.SortDirection = sorter.SortDirection = direction;
            listColView.CustomSort = sorter;
        }
    }
}


/*
 Usage:

    Update Compare Method above!!!

    public int Compare(object x, object y)
     {
       //var materialPropX = x as MaterialPropertyModel;
       //var materialPropY = y as MaterialPropertyModel;
       //return materialPropX.PropertyName.CompareTo(materialPropY.PropertyName);
        return 0;
    }

    <DataGrid Name="dgvMaterials"
    Margin="2"
    ItemsSource="{Binding MaterialsView}"
    AutoGenerateColumns="False"
    CanUserAddRows="False"
    IsSynchronizedWithCurrentItem="True"
    SelectionMode="Single"
    GridLinesVisibility="All"
    VerticalScrollBarVisibility="Visible"
    IsReadOnly="{Binding IsReadOnlyRevision}"
    RowHeaderWidth="27"
    local:CustomSortBehaviour.AllowCustomSort="True">
</DataGrid>
 */
