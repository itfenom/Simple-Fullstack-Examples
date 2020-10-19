using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Playground.Core.ColumnFilters.Filters
{
    public enum ColumnFilterType
    {
        TextBoxFilter,
        ComboBoxFilter,
    }

    public abstract class FilteredHeaderCell : DataGridViewColumnHeaderCell
    {
        protected object filterValue = null;
        private string currentColumnFilter = string.Empty;
        private bool filteringEnabledValue = true;

        [DefaultValue(true)]
        public bool FilteringEnabled
        {
            get
            {
                // Return filteringEnabledValue if (there is no DataGridView or if (its DataSource
                // property has not been set.
                if (DataGridView == null || DataGridView.DataSource == null)
                {
                    return filteringEnabledValue;
                }

                // if (the DataSource property has been set, return a value that combines the
                // filteringEnabledValue and BindingSource.SupportsFiltering values.
                BindingSource data = DataGridView.DataSource as BindingSource;
                Debug.Assert(data != null);
                return filteringEnabledValue && data.SupportsFiltering;
            }
            set => filteringEnabledValue = value;
        }

        protected FilteredHeaderCell()
        {
        }

        protected FilteredHeaderCell(DataGridViewColumnHeaderCell template)
        {
            ContextMenuStrip = template.ContextMenuStrip;
            ErrorText = template.ErrorText;
            Tag = template.Tag;
            ToolTipText = template.ToolTipText;
            Value = template.Value;
            ValueType = template.ValueType;

            // Use HasStyle to avoid creating a new style object when the Style property has not
            // previously been set.
            if (template.HasStyle)
            {
                Style = template.Style;
            }

            // Copy this type's properties if the old cell is an auto-filter cell. This enables the
            // Clone method to reuse this constructor.
            if (template is FilteredHeaderCell filterCell)
            {
                FilteringEnabled = filterCell.FilteringEnabled;
            }
        }

        public void Detach()
        {
            UnhandleDataGridViewEvents(DataGridView);
            HideFilterControl();
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates dataGridViewElementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            ResetFilterControl();
        }

        #region DataGridView & DataGridView events

        protected override void OnDataGridViewChanged()
        {
            // Continue only if there is a DataGridView.
            if (DataGridView == null)
            {
                return;
            }

            // Disable sorting and filtering for columns that can't make effective use of them.
            if (OwningColumn != null)
            {
                if (OwningColumn is DataGridViewImageColumn ||
                (OwningColumn is DataGridViewButtonColumn &&
                ((DataGridViewButtonColumn)OwningColumn).UseColumnTextForButtonValue) ||
                (OwningColumn is DataGridViewLinkColumn &&
                ((DataGridViewLinkColumn)OwningColumn).UseColumnTextForLinkValue))
                {
                    FilteringEnabled = false;
                }
            }

            // Confirm that the data source meets requirements.
            VerifyDataSource();

            // Call the OnDataGridViewChanged method on the base class to raise the
            // DataGridViewChanged event.
            base.OnDataGridViewChanged();

            HandleDataGridViewEvents();

            // Initialize the filter control bounds so that any initial column autosizing will
            // accommodate the filter control width.
            ResetFilterControl();
            if (FilteringEnabled)
            {
                ShowFilterControl();
                ResetFilterControl();
            }
        }

        private void VerifyDataSource()
        {
            // Continue only if there is a DataGridView and its DataSource has been set.
            if (DataGridView == null || DataGridView.DataSource == null)
            {
                return;
            }

            // Throw an exception if the data source is not a BindingSource.
            BindingSource data = DataGridView.DataSource as BindingSource;
            if (data == null)
            {
                throw new NotSupportedException(
                    "The DataSource property of the containing DataGridView control " +
                    "must be set to a BindingSource.");
            }
        }

        private void HandleDataGridViewEvents()
        {
            BindingSourceWithFilterEvents bs = DataGridView.DataSource as BindingSourceWithFilterEvents;
            if (bs != null)
            {
                bs.FilterChanged += new EventHandler(BindingSource_FilterChanged);
            }

            DataGridView.ColumnDisplayIndexChanged += new DataGridViewColumnEventHandler(DataGridView_ColumnDisplayIndexChanged);
            DataGridView.ColumnHeadersHeightChanged += new EventHandler(DataGridView_ColumnHeadersHeightChanged);
            DataGridView.ColumnRemoved += new DataGridViewColumnEventHandler(DataGridView_ColumnRemoved);
            DataGridView.ColumnWidthChanged += new DataGridViewColumnEventHandler(DataGridView_ColumnWidthChanged);

            //this.DataGridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(DataGridView_DataBindingComplete);
            DataGridView.DataSourceChanged += new EventHandler(DataGridView_DataSourceChanged);
            DataGridView.ParentChanged += new EventHandler(DataGridView_ParentChanged);
            DataGridView.RowHeadersWidthChanged += new EventHandler(DataGridView_RowHeadersWidthChanged);
            DataGridView.Scroll += new ScrollEventHandler(DataGridView_Scroll);
            DataGridView.SizeChanged += new EventHandler(DataGridView_SizeChanged);
        }

        private void UnhandleDataGridViewEvents(DataGridView dataGridView)
        {
            if (dataGridView == null)
            {
                return;
            }

            BindingSourceWithFilterEvents bs = dataGridView.DataSource as BindingSourceWithFilterEvents;
            if (bs != null)
            {
                bs.FilterChanged -= new EventHandler(BindingSource_FilterChanged);
            }

            dataGridView.ColumnDisplayIndexChanged -= new DataGridViewColumnEventHandler(DataGridView_ColumnDisplayIndexChanged);
            dataGridView.ColumnHeadersHeightChanged -= new EventHandler(DataGridView_ColumnHeadersHeightChanged);
            dataGridView.ColumnRemoved -= new DataGridViewColumnEventHandler(DataGridView_ColumnRemoved);
            dataGridView.ColumnWidthChanged -= new DataGridViewColumnEventHandler(DataGridView_ColumnWidthChanged);

            //dataGridView.DataBindingComplete -= new DataGridViewBindingCompleteEventHandler(DataGridView_DataBindingComplete);
            dataGridView.DataSourceChanged -= new EventHandler(DataGridView_DataSourceChanged);
            dataGridView.ParentChanged -= new EventHandler(DataGridView_ParentChanged);
            dataGridView.RowHeadersWidthChanged -= new EventHandler(DataGridView_RowHeadersWidthChanged);
            dataGridView.Scroll -= new ScrollEventHandler(DataGridView_Scroll);
            dataGridView.SizeChanged -= new EventHandler(DataGridView_SizeChanged);
        }

        private void BindingSource_FilterChanged(object sender, EventArgs e)
        {
            BindingSource bs = DataGridView.DataSource as BindingSource;
            if (bs != null)
            {
                if (string.IsNullOrEmpty(bs.Filter))
                {
                    ResetFilter();
                }
            }
        }

        private void DataGridView_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            ResetFilterControl();
        }

        private void DataGridView_ColumnHeadersHeightChanged(object sender, EventArgs e)
        {
            ResetFilterControl();
        }

        private void DataGridView_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            if (e.Column == OwningColumn)
            {
                DataGridView dgvSender = sender as DataGridView;
                if (dgvSender != null)
                {
                    UnhandleDataGridViewEvents(dgvSender);
                }
                HideFilterControl();
            }
            else
            {
                if (DataGridView != null)
                {
                    DataGridView.InvalidateCell(this);
                }
            }
        }

        private void DataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            ResetFilterControl();
        }

        private void DataGridView_DataSourceChanged(object sender, EventArgs e)
        {
            VerifyDataSource();

            object filterValueTemp = filterValue;
            UpdateFilter(null, false);
            UpdateFilter(filterValueTemp, false);
        }

        private void DataGridView_ParentChanged(object sender, EventArgs e)
        {
            HideFilterControl();
            if (DataGridView != null && DataGridView.Parent != null)
            {
                ShowFilterControl();
            }
        }

        private void DataGridView_RowHeadersWidthChanged(object sender, EventArgs e)
        {
            ResetFilterControl();
        }

        private void DataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                ResetFilterControl();
            }
        }

        private void DataGridView_SizeChanged(object sender, EventArgs e)
        {
            ResetFilterControl();
        }

        #endregion DataGridView & DataGridView events

        public void ResetFilter()
        {
            if (DataGridView == null) return;
            BindingSource source = DataGridView.DataSource as BindingSource;
            if (source == null || string.IsNullOrEmpty(source.Filter))
            {
                ClearFilterValue();
                currentColumnFilter = string.Empty;
            }
            ResetFilterControl();
        }

        protected void ResetFilterControl()
        {
            if (DataGridView == null)
            {
                HideFilterControl();
                return;
            }

            // Initialize a variable to store the text box height, setting its initial value based on
            // the font height.
            int textBoxHeight = InheritedStyle.Font.Height + 5;

            bool visualStylesEnabled =
                Application.RenderWithVisualStyles &&
                DataGridView.EnableHeadersVisualStyles;

            int topOffset = visualStylesEnabled ? 4 : 1;
            int paddingOffset = textBoxHeight + topOffset;

            // Determine the difference between the new and current padding adjustment.
            int heightChange = paddingOffset - Style.Padding.Bottom;

            // If the padding needs to change, store the new value and make the change.
            if (heightChange != 0)
            {
                // Create a new Padding using the adjustment amount, then add it to the cell's
                // existing Style.Padding property value.
                Padding paddingChange = new Padding(0, 0, 0, paddingOffset - Style.Padding.Bottom);
                Style.Padding = new Padding(0, 0, 0, heightChange);
            }

            // Retrieve the cell display rectangle, which is used to set the position of the filter control.
            Rectangle cellBounds;
            try
            {
                cellBounds = DataGridView.GetCellDisplayRectangle(ColumnIndex, -1, false);
            }
            catch (ArgumentOutOfRangeException)
            {
                FilterControl.Invalidate();
                return;
            }

            // Calculate the location of the text box, with adjustments based on whether visual
            // styles are enabled.
            int top = DataGridView.Top + cellBounds.Bottom - textBoxHeight - topOffset;
            int leftOffset = visualStylesEnabled ? 3 : 1;
            int left = DataGridView.Left + cellBounds.Left + leftOffset;
            int rightOffset = visualStylesEnabled ? 3 : 1;
            int right = DataGridView.Left + cellBounds.Right - rightOffset;

            cellBounds = new Rectangle(left, top, right - left, textBoxHeight);

            Rectangle dgvRect = DataGridView.Bounds;
            Control dgvParent = DataGridView.Parent as ContainerControl;
            if (DataGridView.Dock == DockStyle.Fill && dgvParent != null)
            {
                dgvRect = new Rectangle(
                    dgvParent.ClientRectangle.Left + dgvParent.Padding.Left,
                    dgvParent.ClientRectangle.Top + dgvParent.Padding.Top,
                    dgvParent.ClientRectangle.Width - dgvParent.Padding.Horizontal,
                    dgvParent.ClientRectangle.Height - dgvParent.Padding.Vertical);
            }
            cellBounds.Intersect(dgvRect);
            if (cellBounds.Width > 0 && cellBounds.Height > 0)
            {
                foreach (Control ctrl in DataGridView.Controls)
                {
                    if (!ctrl.Visible) { continue; }

                    if (ctrl is VScrollBar)
                    {
                        if (cellBounds.Right > DataGridView.Left + ctrl.Bounds.Left)
                        {
                            cellBounds.Width = (DataGridView.Left + ctrl.Bounds.Left) - cellBounds.Left;
                        }
                    }
                    if (ctrl is HScrollBar)
                    {
                        if (cellBounds.Bottom > DataGridView.Top + ctrl.Bounds.Bottom)
                        {
                            cellBounds.Height = (DataGridView.Top + ctrl.Bounds.Bottom) - cellBounds.Top;
                        }
                    }
                }
            }

            // Set the text box bounds using the calculated values, and adjust the cell padding accordingly.
            FilterControl.Bounds = cellBounds;

            FilterControl.Invalidate();
        }

        protected void ShowFilterControl()
        {
            Debug.Assert(DataGridView != null, "DataGridView is null");

            // Ensure that the current row is not the row for new records. This prevents the new row
            // from affecting the filter list and also prevents the new row from being added when the
            // filter changes.
            if (DataGridView.CurrentRow != null && DataGridView.CurrentRow.IsNewRow)
            {
                DataGridView.CurrentCell = null;
            }

            InitFilterControl();

            // Add handlers to filter control events.
            HandleFilterControlEvents();

            // Set the size and location of filter control, then display it.
            ResetFilterControl();
            FilterControl.Visible = true;

            Debug.Assert(FilterControl.Parent == null, "ShowFilterControl has been called multiple times before HideFilterControl");

            // Add filter control to the DataGridView.
            if (DataGridView.Parent != null)
            {
                DataGridView.Parent.Controls.Add(FilterControl);
            }
            FilterControl.BringToFront();

            // Invalidate the cell so that it will repaint
            DataGridView.InvalidateCell(this);
        }

        protected void HideFilterControl()
        {
            //Debug.Assert(this.DataGridView != null, "DataGridView is null");

            // Hide filter control, remove handlers from its events, and remove it from the
            // DataGridView control.
            FilterControl.Visible = false;
            if (DataGridView != null)
            {
                UnhandledFilterControlEvents();

                //FilterControl.Parent.Controls.Remove(FilterControl);
                if (DataGridView.Parent != null)
                {
                    DataGridView.Parent.Controls.Remove(FilterControl);
                }

                // Invalidate the cell so that it will repaint
                DataGridView.InvalidateCell(this);
            }
        }

        protected string FilterWithoutCurrentColumn(string filter)
        {
            // If there is no filter in effect, return String.Empty.
            if (string.IsNullOrEmpty(filter))
            {
                return string.Empty;
            }

            // If the column is not filtered, return the filter string unchanged.
            if (string.IsNullOrEmpty(currentColumnFilter))
            {
                return filter;
            }

            if (filter.IndexOf(currentColumnFilter) >= 1)
            {
                // If the current column filter is not the first filter, return the specified filter
                // value without the current column filter and without the preceding " AND ".
                return filter.Replace(
                    " AND " + currentColumnFilter, string.Empty);
            }
            else
            {
                if (filter.Length > currentColumnFilter.Length)
                {
                    // If the current column filter is the first of multiple filters, return the
                    // specified filter value without the current column filter and without the
                    // subsequent " AND ".
                    return filter.Replace(
                        currentColumnFilter + " AND ", string.Empty);
                }
                else
                {
                    // If the current column filter is the only filter, return the empty string.
                    return string.Empty;
                }
            }
        }

        protected void UpdateFilter(object newFilterValue, bool force)
        {
            // Continue only if the selection has changed.
            if (!force && FilterObjectsEqual(newFilterValue, filterValue))
            {
                return;
            }

            // Store the new selection value.
            filterValue = newFilterValue;

            // Cast the data source to an IBindingListView.
            IBindingListView data = DataGridView.DataSource as IBindingListView;

            if (data == null || !data.SupportsFiltering || !FilteringEnabled)
            {
                return;
            }

            // If the user selection is empty, remove any filter currently in effect for the column.
            if (FilterObjectEmpty(filterValue))
            {
                data.Filter = FilterWithoutCurrentColumn(data.Filter);
                currentColumnFilter = string.Empty;
                return;
            }

            // Declare a variable to store the filter string for this column.
            string newColumnFilter = GetColumnFilter(filterValue);

            // Determine the new filter string by removing the previous column filter string from the
            // BindingSource.Filter value, then appending the new column filter string, using " AND "
            // as appropriate.
            string newFilter = FilterWithoutCurrentColumn(data.Filter);
            if (string.IsNullOrEmpty(newFilter))
            {
                newFilter += newColumnFilter;
            }
            else
            {
                newFilter += " AND " + newColumnFilter;
            }

            // Set the filter to the new value.
            try
            {
                data.Filter = newFilter;
            }
            catch (InvalidExpressionException)
            {
                filterValue = string.Empty;
                currentColumnFilter = string.Empty;
                data.Filter = string.Empty;
                DisplayErrorInFilterControl();
                return;
            }

            // Indicate that the column is currently filtered and store the new column filter for use
            // by subsequent calls to the FilterWithoutCurrentColumn method.
            currentColumnFilter = newColumnFilter;
        }

        public DataTable GetMainTable()
        {
            return (DataGridView?.DataSource as BindingSource)?.DataSource as DataTable;
        }

        #region abstract elements

        public abstract override object Clone();

        public abstract Control FilterControl
        {
            get;
        }

        protected abstract void InitFilterControl();

        protected abstract void HandleFilterControlEvents();

        protected abstract void UnhandledFilterControlEvents();

        protected abstract void DisplayErrorInFilterControl();

        protected abstract void ClearFilterValue();

        protected abstract bool FilterObjectEmpty(object filterObject);

        protected abstract bool FilterObjectsEqual(object filterObject1, object filterObject2);

        protected abstract string GetColumnFilter(object filterObject);

        #endregion abstract elements
    }
}
