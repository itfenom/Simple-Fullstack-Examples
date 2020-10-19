using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Playground.Core.ColumnFilters.Columns;

namespace Playground.Core.ColumnFilters.Filters
{
    public class ComboBoxFilterHeaderCell : FilteredHeaderCell
    {
        private static string allString = "(Show All)";

        private readonly ComboBox comboBox = new ComboBox();
        private DataTable itemTable = null;

        public ComboBoxFilterHeaderCell()
        {
        }
        public ComboBoxFilterHeaderCell(DataGridViewColumnHeaderCell template) : base(template)
        {
        }

        public override object Clone()
        {
            return new ComboBoxFilterHeaderCell(this);
        }

        public override Control FilterControl => comboBox;

        protected override void InitFilterControl()
        {
            UnhandledFilterControlEvents();

            PopulateComboBoxItems();

            comboBox.SelectionLength = 0;

            HandleFilterControlEvents();
        }

        protected override void HandleFilterControlEvents()
        {
            comboBox.Invalidated += new InvalidateEventHandler(comboBox_Invalidated);
            comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
            DataGridView.DataSourceChanged += new EventHandler(DataGridView_DataSourceChanged_forComboBox);
            if (UseCustomSorting())
            {
                DataGridView.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(DataGridView_ColumnHeaderMouseClick);
                OwningColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            }
        }
        protected override void UnhandledFilterControlEvents()
        {
            comboBox.Invalidated -= new InvalidateEventHandler(comboBox_Invalidated);
            comboBox.SelectedIndexChanged -= new EventHandler(comboBox_SelectedIndexChanged);
            DataGridView.DataSourceChanged -= new EventHandler(DataGridView_DataSourceChanged_forComboBox);
            DataGridView.ColumnHeaderMouseClick -= new DataGridViewCellMouseEventHandler(DataGridView_ColumnHeaderMouseClick);
        }
        protected override void DisplayErrorInFilterControl()
        {
            comboBox.Text = "ERROR";
        }
        private bool UseCustomSorting()
        {
            DataGridViewComboBoxColumnFiltered myCol = OwningColumn as DataGridViewComboBoxColumnFiltered;
            if (myCol == null)
            {
                return false;
            }

            if (myCol.ValueMember == myCol.DisplayMember || myCol.ValueMember == null || myCol.DisplayMember == null)
            {
                return false;
            }

            BindingSource bs = DataGridView.DataSource as BindingSource;
            if (bs == null)
            {
                return false;
            }

            DataTable dt = bs.DataSource as DataTable;
            if (dt == null)
            {
                return false;
            }

            if (!dt.Columns.Contains(myCol.DisplayMember))
            {
                return false;
            }

            return true;
        }

        protected override void ClearFilterValue()
        {
            filterValue = -1;
        }
        protected override bool FilterObjectEmpty(object filterObject)
        {
            return filterObject == null || (filterObject is int && ((int)filterObject) < 0);
        }
        protected override bool FilterObjectsEqual(object filterObject1, object filterObject2)
        {
            if (FilterObjectEmpty(filterObject1) && FilterObjectEmpty(filterObject2))
            {
                return true;
            }

            if (filterObject1 is int)
            {
                if (filterObject2 is int)
                {
                    return ((int)filterObject1) == ((int)filterObject2);
                }

                return false;
            }

            if (filterObject2 is int)
            {
                return false;
            }

            return true;
        }
        protected override string GetColumnFilter(object filterObject)
        {
            if (!(filterObject is int) || ((int)filterObject) < 0)
            {
                return string.Empty;
            }
            int filterIndex = (int)filterObject;

            // Declare a variable to store the filter string for this column.
            string newColumnFilter = null;

            // Store the column name in a form acceptable to the Filter property, 
            // using a backslash to escape any closing square brackets. 
            string columnProperty =
                OwningColumn.DataPropertyName.Replace("]", @"\]");

            object realFilterObject = null;
            DataGridViewComboBoxColumn cboCol = OwningColumn as DataGridViewComboBoxColumn;
            DataTable dt = null;
            if (cboCol != null)
            {
                dt = cboCol.DataSource as DataTable;
            }

            if (cboCol != null && dt != null)
            {
                //get realFilterObject from dt
                realFilterObject = itemTable.Rows[filterIndex + 1][cboCol.ValueMember];
            }
            else
            {
                //get realFilterObject from comboBox
                realFilterObject = comboBox.Items[filterIndex + 1];
            }

            if (realFilterObject == null || (realFilterObject is string && ((string)realFilterObject).Equals(string.Empty)))
            {
                newColumnFilter = string.Format("[{0}] IS NULL", columnProperty);
            }
            else
            {
                string formatString = "[{0}] = '{1}'";
                if (!(realFilterObject is string))
                {
                    formatString = "[{0}] = {1}";
                }

                // Determine the column filter string based on the user selection.
                newColumnFilter = string.Format(formatString,
                    columnProperty,
                    realFilterObject.ToString().Replace("'", "''"));
            }

            return newColumnFilter;
        }

        private void PopulateComboBoxItems()
        {
            DataGridViewComboBoxColumn cboCol = OwningColumn as DataGridViewComboBoxColumn;
            DataTable dt = null;
            if (cboCol != null)
            {
                dt = cboCol.DataSource as DataTable;
            }

            if (cboCol != null && dt != null)
            {
                bool diffCols = !cboCol.ValueMember.Equals(cboCol.DisplayMember);

                itemTable = new DataTable();

                DataColumn col = new DataColumn(cboCol.DisplayMember, typeof(string));
                itemTable.Columns.Add(col);
                if (diffCols)
                {
                    col = new DataColumn(cboCol.ValueMember, dt.Columns[cboCol.ValueMember].DataType);
                    itemTable.Columns.Add(col);
                }

                DataRow row = itemTable.NewRow();
                row[cboCol.DisplayMember] = allString;
                itemTable.Rows.Add(row);

                foreach (DataRow dtRow in dt.Select(string.Empty, cboCol.DisplayMember))
                {
                    row = itemTable.NewRow();
                    row[cboCol.DisplayMember] = dtRow[cboCol.DisplayMember].ToString();
                    if (diffCols)
                    {
                        row[cboCol.ValueMember] = dtRow[cboCol.ValueMember];
                    }
                    itemTable.Rows.Add(row);
                }

                comboBox.DataSource = itemTable;
                comboBox.DisplayMember = cboCol.DisplayMember;
                comboBox.ValueMember = cboCol.ValueMember;
            }
            else
            {
                List<string> items = new List<string>();

                BindingSource bs = DataGridView.DataSource as BindingSource;
                if (bs != null)
                {
                    BindingSourceWithFilterEvents bindingSourceWithFilterEvents = bs as BindingSourceWithFilterEvents;
                    if (bindingSourceWithFilterEvents != null)
                    {
                        bindingSourceWithFilterEvents.RaiseFilterChangedEvents = false;
                    }

                    string oldFilter = bs.Filter;
                    bs.Filter = string.Empty;
                    foreach (object item in bs)
                    {
                        object value = null;
                        ICustomTypeDescriptor ictd = item as ICustomTypeDescriptor;
                        if (ictd != null)
                        {
                            value = ictd.GetProperties()[OwningColumn.DataPropertyName].GetValue(item);
                        }
                        else
                        {
                            value = item.GetType()
                                .GetProperty(OwningColumn.DataPropertyName)
                                .GetValue(item, null /*property index*/);
                        }

                        if (!items.Contains(value.ToString()))
                        {
                            items.Add(value.ToString());
                        }
                    }

                    bs.Filter = oldFilter;
                    if (bindingSourceWithFilterEvents != null)
                    {
                        bindingSourceWithFilterEvents.RaiseFilterChangedEvents = true;
                    }
                }

                items.Sort();

                comboBox.Items.Clear();
                comboBox.Items.AddRange(items.ToArray());
                comboBox.Items.Insert(0, allString);
            }
        }

        #region Filter Control events
        private void comboBox_Invalidated(object sender, InvalidateEventArgs e)
        {
            if (!comboBox.Focused)
            {
                comboBox.SelectionLength = 0;
            }
        }
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFilter(comboBox.SelectedIndex - 1, false);
        }
        private void DataGridView_DataSourceChanged_forComboBox(object sender, EventArgs e)
        {
            UnhandledFilterControlEvents();
            PopulateComboBoxItems();
            comboBox.SelectedIndex = 0;
            UpdateFilter(-1, false);
            HandleFilterControlEvents();
        }
        private void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == OwningColumn.Index)
            {
                if (!UseCustomSorting())
                {
                    return;
                }

                DataGridViewComboBoxColumnFiltered myCol = OwningColumn as DataGridViewComboBoxColumnFiltered;
                BindingSource bs = DataGridView.DataSource as BindingSource;

                if (SortGlyphDirection == SortOrder.Ascending)
                {
                    bs.Sort = myCol.DisplayMember + " DESC";
                    SortGlyphDirection = SortOrder.Descending;
                }
                else
                {
                    bs.Sort = myCol.DisplayMember + " ASC";
                    SortGlyphDirection = SortOrder.Ascending;
                }
            }
        }
        #endregion Filter Control events
    }
}
