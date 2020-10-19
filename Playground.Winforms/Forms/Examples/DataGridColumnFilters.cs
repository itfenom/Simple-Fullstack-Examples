using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Playground.Core.ColumnFilters.Columns;
using Playground.Core.ColumnFilters.Filters;
using Playground.Winforms.Utilities;

// ReSharper disable PossibleNullReferenceException

namespace Playground.Winforms.Forms.Examples
{
    public partial class DataGridColumnFilters : Form
    {
        // ReSharper disable once NotAccessedField.Local
        private DialogResult _iAnswer;
        private DataGridViewTextBoxColumnFiltered _textBoxCol;
        private DataGridViewComboBoxColumnFiltered _comboBoxCol;
        private DataGridViewButtonColumn _btnCol;
        private DataGridViewCheckBoxColumn _checkBoxCol;
        private DataGridViewTextBoxColumn _txtCol;
        private DataTable _mainDt;
        private DataTable _yesNoDt;
        private BindingSource _bs;

        public DataGridColumnFilters()
        {
            InitializeComponent();
        }

        public void ShowForm(Form oParent)
        {
            LoadDataTables();
            SetupDataGridViewColumns();

            _iAnswer = ShowDialog(oParent);
        }

        private void LoadDataTables()
        {
            _mainDt = new DataTable();
            _mainDt.Columns.Add("ID", typeof(Int32));
            _mainDt.Columns.Add("CHECK", typeof(bool));
            _mainDt.Columns.Add("NAME", typeof(string));
            _mainDt.Columns.Add("COMPANY", typeof(string));
            _mainDt.Columns.Add("EMAIL", typeof(string));
            _mainDt.Columns.Add("ACTIVE", typeof(Int32));
            _mainDt.Columns.Add("DISPLAY_ORDER", typeof(Int32));
            _mainDt.Columns.Add("DETAILS_II", typeof(string));

            var row = _mainDt.NewRow();
            row["ID"] = 1;
            row["CHECK"] = true;
            row["NAME"] = "Kashif Mubarak";
            row["COMPANY"] = "Qorvo";
            row["EMAIL"] = "kashif.mubarak@qorvo.com";
            row["ACTIVE"] = 1;
            row["DISPLAY_ORDER"] = 1;
            row["DETAILS_II"] = "(Mouse Over...)";
            _mainDt.Rows.Add(row);

            row = _mainDt.NewRow();
            row["ID"] = 2;
            row["CHECK"] = false;
            row["NAME"] = "Kashif M";
            row["COMPANY"] = "CapitalOne Bank";
            row["EMAIL"] = "kashif.mubarak@capitalone.com";
            row["ACTIVE"] = 0;
            row["DISPLAY_ORDER"] = 2;
            row["DETAILS_II"] = "(Mouse Over...)";
            _mainDt.Rows.Add(row);

            row = _mainDt.NewRow();
            row["ID"] = 3;
            row["CHECK"] = false;
            row["NAME"] = "Kashif";
            row["COMPANY"] = "Bank of America";
            row["EMAIL"] = "kashif.mubarak@bankofamerica.com";
            row["ACTIVE"] = 0;
            row["DISPLAY_ORDER"] = 3;
            row["DETAILS_II"] = "(Mouse Over...)";
            _mainDt.Rows.Add(row);

            row = _mainDt.NewRow();
            row["ID"] = 4;
            row["CHECK"] = false;
            row["NAME"] = "Mubarak, Kashif";
            row["COMPANY"] = "eBay";
            row["EMAIL"] = "kmubarak@ebay.com";
            row["ACTIVE"] = 0;
            row["DISPLAY_ORDER"] = 4;
            row["DETAILS_II"] = "(Mouse Over...)";
            _mainDt.Rows.Add(row);

            row = _mainDt.NewRow();
            row["ID"] = 5;
            row["CHECK"] = true;
            row["NAME"] = "David F";
            row["COMPANY"] = "TriQuint";
            row["EMAIL"] = "d.f@tqs.com";
            row["ACTIVE"] = 1;
            row["DISPLAY_ORDER"] = 5;
            row["DETAILS_II"] = "(Mouse Over...)";
            _mainDt.Rows.Add(row);

            row = _mainDt.NewRow();
            row["ID"] = 6;
            row["CHECK"] = true;
            row["NAME"] = "David Seppi";
            row["COMPANY"] = "eBay";
            row["EMAIL"] = "dseppi@ebay.com";
            row["ACTIVE"] = 1;
            row["DISPLAY_ORDER"] = 6;
            row["DETAILS_II"] = "(Mouse Over...)";
            _mainDt.Rows.Add(row);

            row = _mainDt.NewRow();
            row["ID"] = 7;
            row["CHECK"] = true;
            row["NAME"] = "Billy Hall";
            row["COMPANY"] = "CapitalOne Bank";
            row["EMAIL"] = "billy.hall@capitalone.com";
            row["ACTIVE"] = 1;
            row["DISPLAY_ORDER"] = 7;
            row["DETAILS_II"] = "(Mouse Over...)";
            _mainDt.Rows.Add(row);

            row = _mainDt.NewRow();
            row["ID"] = 8;
            row["CHECK"] = false;
            row["NAME"] = "Bill Hardy";
            row["COMPANY"] = "CapitalOne Bank";
            row["EMAIL"] = "bill.hardy@capitalone.com";
            row["ACTIVE"] = 0;
            row["DISPLAY_ORDER"] = 8;
            row["DETAILS_II"] = "(Mouse Over...)";
            _mainDt.Rows.Add(row);

            _mainDt.AcceptChanges();

            _yesNoDt = new DataTable();
            _yesNoDt.Columns.Add("INT_VALUE", typeof(Int32));
            _yesNoDt.Columns.Add("STRING_VALUE", typeof(string));

            row = _yesNoDt.NewRow();
            row["INT_VALUE"] = 0;
            row["STRING_VALUE"] = "NO";
            _yesNoDt.Rows.Add(row);

            row = _yesNoDt.NewRow();
            row["INT_VALUE"] = 1;
            row["STRING_VALUE"] = "YES";
            _yesNoDt.Rows.Add(row);

            _yesNoDt.AcceptChanges();
        }

        private void SetupDataGridViewColumns()
        {
            _txtCol = new DataGridViewTextBoxColumn();
            _txtCol.Name = "colID";
            _txtCol.HeaderText = @"ID";
            _txtCol.DataPropertyName = "ID";
            dataGridView1.Columns.Add(_txtCol);
            dataGridView1.Columns["colID"].Visible = false;

            _checkBoxCol = new DataGridViewCheckBoxColumn();
            _checkBoxCol.Name = "colCheck";
            _checkBoxCol.HeaderText = @"SELECT";
            _checkBoxCol.DataPropertyName = "CHECK";
            dataGridView1.Columns.Add(_checkBoxCol);

            _btnCol = new DataGridViewButtonColumn();
            _btnCol.Name = "colDetails";
            _btnCol.FillWeight = 22.67f;
            _btnCol.Frozen = false;
            _btnCol.MinimumWidth = 5;
            _btnCol.Width = 45;
            _btnCol.ToolTipText = @"Click to get report for this record.";
            _btnCol.HeaderText = @"Details";
            _btnCol.DefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            _btnCol.UseColumnTextForButtonValue = true;
            _btnCol.Text = "-->";
            dataGridView1.Columns.Add(_btnCol);

            _txtCol = new DataGridViewTextBoxColumn();
            _txtCol.Name = "colDetailsII";
            _txtCol.HeaderText = @"Details II";
            _txtCol.DataPropertyName = "DETAILS_II";
            dataGridView1.Columns.Add(_txtCol);

            _textBoxCol = new DataGridViewTextBoxColumnFiltered();
            _textBoxCol.FilterType = ColumnFilterType.TextBoxFilter;
            _textBoxCol.Name = "colName";
            _textBoxCol.HeaderText = @"Name";
            _textBoxCol.DataPropertyName = "NAME";
            dataGridView1.Columns.Add(_textBoxCol);

            _textBoxCol = new DataGridViewTextBoxColumnFiltered();
            _textBoxCol.FilterType = ColumnFilterType.TextBoxFilter;
            _textBoxCol.Name = "colCompany";
            _textBoxCol.HeaderText = @"Company";
            _textBoxCol.DataPropertyName = "COMPANY";
            dataGridView1.Columns.Add(_textBoxCol);

            _textBoxCol = new DataGridViewTextBoxColumnFiltered();
            _textBoxCol.FilterType = ColumnFilterType.TextBoxFilter;
            _textBoxCol.Name = "colEmail";
            _textBoxCol.HeaderText = @"Email";
            _textBoxCol.DataPropertyName = "EMAIL";
            dataGridView1.Columns.Add(_textBoxCol);

            _textBoxCol = new DataGridViewTextBoxColumnFiltered();
            _textBoxCol.FilterType = ColumnFilterType.TextBoxFilter;
            _textBoxCol.Name = "colDisplayOrder";
            _textBoxCol.HeaderText = @"Display Order";
            _textBoxCol.DataPropertyName = "DISPLAY_ORDER";
            dataGridView1.Columns.Add(_textBoxCol);

            _comboBoxCol = new DataGridViewComboBoxColumnFiltered();
            _comboBoxCol.FilterType = ColumnFilterType.ComboBoxFilter;
            _comboBoxCol.Name = "colYesNo";
            _comboBoxCol.HeaderText = @"Active?";
            _comboBoxCol.DataPropertyName = "ACTIVE";
            _comboBoxCol.SortMode = DataGridViewColumnSortMode.Automatic;
            _comboBoxCol.ValueMember = "INT_VALUE";
            _comboBoxCol.DisplayMember = "STRING_VALUE";
            _comboBoxCol.DataSource = _yesNoDt;
            dataGridView1.Columns.Add(_comboBoxCol);

            dataGridView1.AutoGenerateColumns = false;

        }

        private void DataGridColumnFilters_Load(object sender, EventArgs e)
        {
            if (_mainDt.Rows.Count > 0)
            {
                //bind data to gridView
                _bs = new BindingSource();
                _bs.DataSource = _mainDt;
                dataGridView1.DataSource = _bs;


                //Setup Tooltip for the button control!
                foreach (DataRow row in _mainDt.Rows)
                {
                    var toolTipText = "Info:\n\n"
                                          + (string.IsNullOrEmpty(row["NAME"].ToString()) ? "NULL" : "{" + row["NAME"] + "}") + "\n"
                                          + (string.IsNullOrEmpty(row["COMPANY"].ToString()) ? "NULL" : "{" + row["COMPANY"] + "}") + "\n"
                                          + (string.IsNullOrEmpty(row["EMAIL"].ToString()) ? "NULL" : "{" + row["EMAIL"] + "}") + "\n"
                                          + (string.IsNullOrEmpty(row["ACTIVE"].ToString()) ? "NULL" : "{" + row["ACTIVE"] + "}") + "\n"
                                          + "-----------------------------";
                    foreach (DataGridViewRow r in dataGridView1.Rows)
                    {
                        if (r.Cells["colID"].Value.ToString() == row["ID"].ToString())
                        {
                            r.Cells["colDetails"].ToolTipText = toolTipText;
                            break;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show(@"No data to load!", @"Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["colDetails"].Index && e.RowIndex >= 0)
            {
                var msg = "Info:";
                msg += "\r\n<b>NAME:</b> " + dataGridView1.CurrentRow.Cells["colName"].FormattedValue;
                msg += "\r\n<i>COMPANY:</i> " + dataGridView1.CurrentRow.Cells["colCompany"].FormattedValue;
                msg += "\r\n<u>EMAIL:</u> " + dataGridView1.CurrentRow.Cells["colEmail"].FormattedValue;
                msg += "\r\n<b>ACTIVE:</b> " + dataGridView1.CurrentRow.Cells["colYesNo"].FormattedValue;

                Helpers.DisplayMultiLineFormattedMessage("Employee Details", msg);
            }
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 3)
            {
                var msg = "Details:";
                msg += "\nNAME: " + dataGridView1.Rows[e.RowIndex].Cells["colName"].FormattedValue;
                msg += "\nCOMPANY: " + dataGridView1.Rows[e.RowIndex].Cells["colCompany"].FormattedValue;
                msg += "\nEMAIL: " + dataGridView1.Rows[e.RowIndex].Cells["colEmail"].FormattedValue;
                msg += "\nACTIVE: " + dataGridView1.Rows[e.RowIndex].Cells["colYesNo"].FormattedValue;
                msg += "\nDISPLAY ORDER: " + dataGridView1.Rows[e.RowIndex].Cells["colDisplayOrder"].FormattedValue;

                var cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cell.ToolTipText = msg;
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var drv = dataGridView1.Rows[e.RowIndex].DataBoundItem as DataRowView;

            if (drv == null) return;

            var isChecked = Convert.ToBoolean(drv["CHECK"]);

            if (isChecked == false)
            {
                EnableDisableCell(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex], Convert.ToBoolean(drv.Row["CHECK"]));
            }
        }

        private void EnableDisableCell(DataGridViewCell dc, bool enabled)
        {
            //toggle read-only state
            dc.ReadOnly = !enabled;
            if (enabled)
            {
                //restore cell style to the default value
                dc.Style.BackColor = dc.OwningColumn.DefaultCellStyle.BackColor;
                dc.Style.ForeColor = dc.OwningColumn.DefaultCellStyle.ForeColor;
            }
            else
            {
                //gray out the cell
                dc.Style.BackColor = Color.LightGray;
                dc.Style.ForeColor = Color.DarkGray;
            }
        }

        private void BtnClearFilters_Click(object sender, EventArgs e)
        {
            for (int i = 4; i < dataGridView1.Columns.Count - 1; i++) //start at 5th column and skip the last column as last column is of type comboBox 
            {
                if (((TextBox)((TextBoxFilterHeaderCell)dataGridView1.Columns[i].HeaderCell).FilterControl).Text != @"Filter...")
                {
                    ((TextBox)((TextBoxFilterHeaderCell)dataGridView1.Columns[i].HeaderCell).FilterControl).Text = "";
                }
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MoveDisplayOrderUp(DataGridView dgv, BindingSource bs)
        {
            if (dgv.Rows.Count == 0) return;

            //Get selected Cell Count to make sure that user has selected some cell!
            int selectedCellCount = dgv.GetCellCount(DataGridViewElementStates.Selected);

            if (selectedCellCount == 0)
            {
                MessageBox.Show(@"To Move Display Order Up, you must Select Display Order Cell!", @"Move Display Order Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount > 1)
            {
                MessageBox.Show(@"To Move Display Order Up, you must only Select one Cell!", @"Move Display Order Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount == 1)
            {
                //Now find out which column cell was selected, if it was Dislay Order then
                if (dgv.SelectedCells[0].ColumnIndex == dgv.Columns["colDisplayOrder"].Index)
                {
                    //Now make sure that this was not already at the top!
                    int rowIndex = dgv.CurrentCell.RowIndex;
                    int dispOrderColumnIndex = dgv.Columns["colDisplayOrder"].Index;

                    if (rowIndex == 0)
                    {
                        MessageBox.Show(@"Display Order already at the Top!", @"Move Display Order Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        var selectedRow = dgv.Rows[rowIndex].DataBoundItem as DataRowView;
                        var selectedRowDisplayOrder = Convert.ToInt32(selectedRow["DISPLAY_ORDER"]);

                        //Get the above row 
                        var aboveRow = dgv.Rows[rowIndex - 1].DataBoundItem as DataRowView;
                        int aboveRowDisplayOrder = Convert.ToInt32(aboveRow["DISPLAY_ORDER"]);

                        //Change the row above to the displayOrder of the moving row
                        selectedRow["DISPLAY_ORDER"] = aboveRowDisplayOrder;
                        aboveRow["DISPLAY_ORDER"] = selectedRowDisplayOrder;

                        if (rowIndex != 0)
                        {
                            dgv.CurrentCell = dgv[dispOrderColumnIndex, rowIndex - 1];
                        }

                        dgv.EndEdit();
                        bs.EndEdit();
                    }
                }
                else
                {
                    MessageBox.Show(@"To Move Display Order Up, you must Select Display Order Cell!", @"Move Display Order Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void MoveDisplayOrderDown(DataGridView dgv, BindingSource bs)
        {
            if (dgv.Rows.Count == 0) return;

            //Get selected Cell Count to make sure that user has selected some cell!
            var selectedCellCount = dgv.GetCellCount(DataGridViewElementStates.Selected);

            if (selectedCellCount == 0)
            {
                MessageBox.Show(@"To Move Display Order Down, you must Select Display Order Cell!", @"Move Display Order Down", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount > 1)
            {
                MessageBox.Show(@"To Move Display Order Down, you must only Select one Cell!", @"Move Display Order Down", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount == 1)
            {
                //Now find out which column cell was selected, if it was Dislay Order then
                if (dgv.SelectedCells[0].ColumnIndex == dgv.Columns["colDisplayOrder"].Index)
                {
                    //Now make sure that this was not already at the bottom!
                    var rowIndex = dgv.CurrentCell.RowIndex;
                    var dispOrderColumnIndex = dgv.Columns["colDisplayOrder"].Index;

                    if (rowIndex == dgv.Rows.Count - 1)
                    {
                        MessageBox.Show(@"Display Order already at the Bottom!", @"Move Display Order Down", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        var selectedRow = dgv.Rows[rowIndex].DataBoundItem as DataRowView;
                        var selectedRowDisplayOrder = Convert.ToInt32(selectedRow["DISPLAY_ORDER"]);

                        //Get the above below 
                        DataRowView belowRow = dgv.Rows[rowIndex + 1].DataBoundItem as DataRowView;
                        var belowRowDisplayOrder = Convert.ToInt32(belowRow["DISPLAY_ORDER"]);

                        //Change the row above to the displayOrder of the moving row
                        selectedRow["DISPLAY_ORDER"] = belowRowDisplayOrder;
                        belowRow["DISPLAY_ORDER"] = selectedRowDisplayOrder;

                        if (rowIndex != dgv.Rows.Count - 1)
                        {
                            dgv.CurrentCell = dgv[dispOrderColumnIndex, rowIndex + 1];
                        }

                        dgv.EndEdit();
                        bs.EndEdit();
                    }
                }
                else
                {
                    MessageBox.Show(@"To Move Display Order Down, you must Select Display Order Cell!", @"Move Display Order Down", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void MoveDisplayOrderToTop(DataGridView dgv, BindingSource bs)
        {
            if (dgv.Rows.Count == 0) return;

            //Get selected Cell Count to make sure that user has selected some cell!
            var selectedCellCount = dgv.GetCellCount(DataGridViewElementStates.Selected);

            if (selectedCellCount == 0)
            {
                MessageBox.Show(@"To Move Display Order to Top, you must Select Display Order Cell!", @"Move Display Order to Top", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount > 1)
            {
                MessageBox.Show(@"To Move Display Order to Top, you must only Select one Cell!", @"Move Display Order to Top", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount == 1)
            {
                //Now find out which column cell was selected, if it was Dislay Order then
                if (dgv.SelectedCells[0].ColumnIndex == dgv.Columns["colDisplayOrder"].Index)
                {
                    //Now make sure that this was not already at the top!
                    var rowIndex = dgv.CurrentCell.RowIndex;
                    var dispOrderColumnIndex = dgv.Columns["colDisplayOrder"].Index;

                    if (rowIndex == 0)
                    {
                        MessageBox.Show(@"Display Order already at the Top!", @"Move Display Order to Top", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        while (rowIndex >= 1)
                        {
                            var selectedRow = dgv.Rows[rowIndex].DataBoundItem as DataRowView;
                            var selectedRowDisplayOrder = Convert.ToInt32(selectedRow["DISPLAY_ORDER"]);

                            //Get the above row 
                            var aboveRow = dgv.Rows[rowIndex - 1].DataBoundItem as DataRowView;
                            int aboveRowDisplayOrder = Convert.ToInt32(aboveRow["DISPLAY_ORDER"]);

                            //Change the row above to the displayOrder of the moving row
                            selectedRow["DISPLAY_ORDER"] = aboveRowDisplayOrder;
                            aboveRow["DISPLAY_ORDER"] = selectedRowDisplayOrder;

                            if (rowIndex != 0)
                            {
                                dgv.CurrentCell = dgv[dispOrderColumnIndex, rowIndex - 1];
                            }

                            dgv.EndEdit();
                            rowIndex--;
                        }

                        bs.EndEdit();
                    }
                }
                else
                {
                    MessageBox.Show(@"To Move Display Order to Top, you must Select Display Order Cell!", @"Move Display Order to Top", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void MoveDisplayOrderToBottom(DataGridView dgv, BindingSource bs)
        {
            if (dgv.Rows.Count == 0) return;

            //Get selected Cell Count to make sure that user has selected some cell!
            var selectedCellCount = dgv.GetCellCount(DataGridViewElementStates.Selected);

            if (selectedCellCount == 0)
            {
                MessageBox.Show(@"To Move Display Order to Bottom, you must Select Display Order Cell!", @"Move Display Order to Bottom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount > 1)
            {
                MessageBox.Show(@"To Move Display Order to Bottom, you must only Select one Cell!", @"Move Display Order to Bottom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount == 1)
            {
                //Now find out which column cell was selected, if it was Dislay Order then
                if (dgv.SelectedCells[0].ColumnIndex == dgv.Columns["colDisplayOrder"].Index)
                {
                    //Now make sure that this was not already at the bottom!
                    var rowIndex = dgv.CurrentCell.RowIndex;
                    var dispOrderColumnIndex = dgv.Columns["colDisplayOrder"].Index;
                    var lastRowsIndex = dgv.Rows.Count - 1;

                    if (rowIndex == dgv.Rows.Count - 1)
                    {
                        MessageBox.Show(@"Display Order already at the Bottom!", @"Move Display Order to Bottom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        while (rowIndex < lastRowsIndex)
                        {
                            var selectedRow = dgv.Rows[rowIndex].DataBoundItem as DataRowView;
                            var selectedRowDisplayOrder = Convert.ToInt32(selectedRow["DISPLAY_ORDER"]);

                            //Get the above below 
                            var belowRow = dgv.Rows[rowIndex + 1].DataBoundItem as DataRowView;
                            int belowRowDisplayOrder = Convert.ToInt32(belowRow["DISPLAY_ORDER"]);

                            //Change the row above to the displayOrder of the moving row
                            selectedRow["DISPLAY_ORDER"] = belowRowDisplayOrder;
                            belowRow["DISPLAY_ORDER"] = selectedRowDisplayOrder;

                            if (rowIndex != dgv.Rows.Count - 1)
                            {
                                dgv.CurrentCell = dgv[dispOrderColumnIndex, rowIndex + 1];
                            }

                            dgv.EndEdit();

                            rowIndex++;
                        }

                        bs.EndEdit();
                    }
                }
                else
                {
                    MessageBox.Show(@"To Move Display Order to Bottom, you must Select Display Order Cell!", @"Move Display Order to Bottom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnMoveDispOrderToDown_Click_1(object sender, EventArgs e)
        {
            MoveDisplayOrderDown(dataGridView1, _bs);
        }

        private void BtnMoveDispOrderToUp_Click_1(object sender, EventArgs e)
        {
            MoveDisplayOrderUp(dataGridView1, _bs);
        }

        private void BtnMoveDispOrderToBottom_Click_1(object sender, EventArgs e)
        {
            MoveDisplayOrderToBottom(dataGridView1, _bs);
        }

        private void BtnMoveDispOrderToTop_Click_1(object sender, EventArgs e)
        {
            MoveDisplayOrderToTop(dataGridView1, _bs);
        }
    }
}
