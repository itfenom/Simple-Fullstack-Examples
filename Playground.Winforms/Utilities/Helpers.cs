using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

// ReSharper disable once IdentifierTypo
namespace Playground.Winforms.Utilities
{
    public static class Helpers
    {
        public static void DisplayException(Form oParent, Exception oEx, string sTitle = "Exception")
        {
            // ReSharper disable once UnusedVariable
            int num = (int)MessageBox.Show(oParent, oEx.Message + @"\r\n\r\n" + oEx, sTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        public static void DisplayInfo(Form oParent, string sMsg, string sTitle = "Info")
        {
            // ReSharper disable once UnusedVariable
            int num = (int)MessageBox.Show(oParent, sMsg, sTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        public static void DisplayError(IWin32Window owner, string message, string title = "Error")
        {
            if (owner == null)
            {
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                MessageBox.Show(owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        public static DialogResult DisplayYesNoMessageBoxOnTop(string sMessage, string sTitle = "Info")
        {
            DialogResult dr = MessageBox.Show(new Form { TopMost = true }, sMessage, sTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return dr;
        }

        public static void DisplayMultipleErrors(Form oParent, ArrayList oErrors, string sTitle = "Form Validation")
        {
            if (oErrors.Count <= 0)
                return;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Error(s) List:");
            int num1 = 0;
            int num2 = checked(oErrors.Count - 1);
            int index = num1;
            while (index <= num2)
            {
                stringBuilder.AppendFormat("{0} - {1}", @"\r\n", RuntimeHelpers.GetObjectValue(oErrors[index]));
                checked { ++index; }
            }
            DisplayError(oParent, stringBuilder.ToString(), sTitle);
        }

        public static void DisplayMultipleErrors(IWin32Window owner, IEnumerable<string> errors, string sTitle = "Form Validation")
        {
            // if there are no errors, just exit the method
            // ReSharper disable once PossibleMultipleEnumeration
            if (errors == null || !errors.Any())
            {
                return;
            }

            // create the message and display it
            string separator = Environment.NewLine + " - ";
            // ReSharper disable once PossibleMultipleEnumeration
            string errorMessage = "Error(s) List: " + separator + string.Join(separator, errors);
            DisplayError(owner, errorMessage, sTitle);
        }

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            var form = new Form();
            var label = new Label();
            var textBox = new TextBox();
            var buttonOk = new Button();
            var buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = @"OK";
            buttonCancel.Text = @"Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        public static DialogResult YesNoRedTextMessageBox(string title, string messageToDisplay)
        {
            Form form = new Form();
            Label label = new Label();
            Button buttonYes = new Button();
            Button buttonNo = new Button();

            label.Text = messageToDisplay;
            label.ForeColor = Color.Red;
            label.Font = new Font("Arial", 9, FontStyle.Bold);

            buttonYes.Text = @"Yes";
            buttonNo.Text = @"No";
            buttonYes.DialogResult = DialogResult.Yes;
            buttonNo.DialogResult = DialogResult.No;

            label.SetBounds(9, 20, 500, 20);
            buttonYes.SetBounds(228, 72, 75, 23);
            buttonNo.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            buttonYes.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonNo.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.Text = title;
            form.BackColor = Color.Beige;
            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, buttonYes, buttonNo });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonYes;
            form.CancelButton = buttonNo;

            DialogResult dialogResult = form.ShowDialog();
            return dialogResult;
        }

        public static DialogResult DisplayMultiLineFormattedMessage(string title, string textToDisplay)
        {
            Form form = new Form();
            RichTextBox textBox = new RichTextBox();
            Button buttonOk = new Button();

            //Set location and size of RichTextBox!
            textBox.Name = "richTextBox1";
            textBox.Text = textToDisplay;
            textBox.Multiline = true;
            textBox.BorderStyle = BorderStyle.Fixed3D;
            textBox.Size = new Size(290, 170);
            textBox.MaxLength = 2147483647;
            textBox.Location = new Point(5, 15);
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            textBox.ReadOnly = true;

            //Set Location/Size of OK button
            buttonOk.Text = @"OK";
            buttonOk.DialogResult = DialogResult.OK;
            buttonOk.Size = new Size(75, 25);
            buttonOk.Location = new Point(125, 195);
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            //Setup Form Properties
            form.Text = title;
            form.ClientSize = new Size(300, 225);
            form.Controls.AddRange(new Control[] { textBox, buttonOk });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;

            //Set formatting in the rich Text Box.
            if (textToDisplay.Contains("<") && textToDisplay.Contains("</"))
            {
                FormatRichTextBox(textBox, textToDisplay);
            }

            DialogResult dialogResult = form.ShowDialog();
            return dialogResult;
        }

        private static void FormatRichTextBox(RichTextBox textBox, string textToDisplay)
        {
            textBox.Clear();
            string accumStr = "";
            int indx = 0;
            char tagValue = '\0';
            while (indx <= textToDisplay.Length - 1)
            {
                if (textToDisplay[indx] == '<')
                { // start of tag?
                    //=================  START TAG =====================
                    if (textToDisplay[indx + 2] == '>') // starting tags only
                    {
                        tagValue = textToDisplay[indx + 1];
                        if (tagValue == 'b') // bold
                        {
                            if (!string.IsNullOrEmpty(accumStr))
                            {
                                textBox.AppendText(accumStr);
                                accumStr = "";
                            }
                            textBox.SelectionFont = new Font("Arial", (float)9, FontStyle.Bold);
                            indx += 3;
                        }
                        if (tagValue == 'u') //underline
                        {
                            if (!string.IsNullOrEmpty(accumStr))
                            {
                                textBox.AppendText(accumStr);
                                accumStr = "";
                            }
                            textBox.SelectionFont = new Font("Arial", (float)9, FontStyle.Underline);
                            indx += 3;
                        }
                        if (tagValue == 'i') //underline
                        {
                            if (!string.IsNullOrEmpty(accumStr))
                            {
                                textBox.AppendText(accumStr);
                                accumStr = "";
                            }
                            textBox.SelectionFont = new Font("Arial", (float)9, FontStyle.Italic);
                            indx += 3;
                        }
                    }
                    //=================  END TAG =====================
                    if (textToDisplay[indx + 1] == '/') // ending tag start?
                    {
                        if (textToDisplay[indx + 2] == tagValue)
                        {
                            if (!string.IsNullOrEmpty(accumStr))
                            {
                                textBox.AppendText(accumStr);
                                accumStr = "";
                            }
                            textBox.SelectionFont = new Font("Arial", (float)9, FontStyle.Regular);
                            indx += 4;
                        }
                    }
                }
                else
                {
                    accumStr += textToDisplay[indx];
                    indx++;
                }
            }

            if (!string.IsNullOrEmpty(accumStr))
            {
                textBox.AppendText(accumStr);
                accumStr = "";
            }
        }


        /// <summary>
        /// While creating DataGridView, if the mouse is suddenly 
        /// on the position where the Column header is, 
        /// DataGridView would try to handle the CellEnter event, 
        /// and try to resize the columns, but since the DataGridView 
        /// is still loading (being painted) As a result, 
        /// an InvalidOperation Exception is thrown:
        /// "This operation cannot be performed while an auto-filled column is being resized"
        ///   
        /// The following code places the mouse pointer in the center of the title bar of the window.
        /// Call this method in the constructor of the form after InitializeComponent() method!!!
        /// </summary>
        /// <param name="form"></param>
        public static void PositionMousePointerToTitleBarCenter(Form form)
        {
            Cursor.Position = form.PointToScreen(new Point(form.Width / 2, -10));
        }

        public static void MoveDisplayOrderUp(DataGridView dgv, BindingSource bs, string clickedColumnName, string columnPrefix)
        {
            if (dgv.Rows.Count == 0) return;

            //Get selected Cell Count to make sure that user has selected some cell!
            var selectedCellCount = dgv.GetCellCount(DataGridViewElementStates.Selected);

            if (selectedCellCount == 0)
            {
                MessageBox.Show(@"To Move Row Up, you must Select '{" + clickedColumnName.Replace(columnPrefix, "") + @"}' Cell!", @"Move Display Order Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount > 1)
            {
                MessageBox.Show(@"To Move Row Up, you must only Select One Cell!", @"Move Display Order Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount == 1)
            {
                //Now find out which column cell was selected, if it was clickedColumnName then
                // ReSharper disable once PossibleNullReferenceException
                if (dgv.SelectedCells[0].ColumnIndex == dgv.Columns[clickedColumnName].Index)
                {
                    //Now make sure that this was not already at the top!
                    var rowIndex = dgv.CurrentCell.RowIndex;
                    var selectedColumnIndex = dgv.Columns[clickedColumnName].Index;

                    if (rowIndex == 0)
                    {
                        MessageBox.Show(@"Display Order is already at the Top!", @"Move Display Order Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (dgv.Rows[rowIndex].DataBoundItem is DataRowView selectedRow)
                    {
                        var selectedRowDisplayOrder = Convert.ToInt32(selectedRow["DISPLAY_ORDER"]);

                        //Get the above row 
                        DataRowView aboveRow = dgv.Rows[rowIndex - 1].DataBoundItem as DataRowView;
                        if (aboveRow != null)
                        {
                            var aboveRowDisplayOrder = Convert.ToInt32(aboveRow["DISPLAY_ORDER"]);

                            //Change the row above to the displayOrder of the moving row
                            selectedRow["DISPLAY_ORDER"] = aboveRowDisplayOrder;
                        }

                        if (aboveRow != null) aboveRow["DISPLAY_ORDER"] = selectedRowDisplayOrder;
                    }

                    if (rowIndex != 0)
                    {
                        dgv.CurrentCell = dgv[selectedColumnIndex, rowIndex - 1];
                    }

                    dgv.EndEdit();
                    bs.EndEdit();
                }
                else
                {
                    MessageBox.Show(@"To Move Row Up, you must Select '{" + clickedColumnName.Replace(columnPrefix, "") + @"}' Cell!", @"Move Display Order Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public static void MoveDisplayOrderDown(DataGridView dgv, BindingSource bs, string clickedColumnName, string columnPrefix)
        {
            if (dgv.Rows.Count == 0) return;

            //Get selected Cell Count to make sure that user has selected some cell!
            var selectedCellCount = dgv.GetCellCount(DataGridViewElementStates.Selected);

            if (selectedCellCount == 0)
            {
                MessageBox.Show(@"To Move Display Order Down, you must Select '{" + clickedColumnName.Replace(columnPrefix, "") + @"}' Cell!", @"Move Display Order Down", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount > 1)
            {
                MessageBox.Show(@"To Move Display Order Down, you must only Select One Cell!", @"Move Display Order Down", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount == 1)
            {
                //Now find out which column cell was selected, if it was clickedColumnName then
                // ReSharper disable once PossibleNullReferenceException
                if (dgv.SelectedCells[0].ColumnIndex == dgv.Columns[clickedColumnName].Index)
                {
                    //Now make sure that this was not already at the bottom!
                    var rowIndex = dgv.CurrentCell.RowIndex;
                    var selectedColumnIndex = dgv.Columns[clickedColumnName].Index;

                    if (rowIndex == dgv.Rows.Count - 1)
                    {
                        MessageBox.Show(@"Display Order is already at the Bottom!", @"Move Display Order Down", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (dgv.Rows[rowIndex].DataBoundItem is DataRowView selectedRow)
                    {
                        var selectedRowDisplayOrder = Convert.ToInt32(selectedRow["DISPLAY_ORDER"]);

                        //Get the above below 
                        DataRowView belowRow = dgv.Rows[rowIndex + 1].DataBoundItem as DataRowView;
                        if (belowRow != null)
                        {
                            var belowRowDisplayOrder = Convert.ToInt32(belowRow["DISPLAY_ORDER"]);

                            //Change the row above to the displayOrder of the moving row
                            selectedRow["DISPLAY_ORDER"] = belowRowDisplayOrder;
                        }

                        if (belowRow != null) belowRow["DISPLAY_ORDER"] = selectedRowDisplayOrder;
                    }

                    if (rowIndex != dgv.Rows.Count - 1)
                    {
                        dgv.CurrentCell = dgv[selectedColumnIndex, rowIndex + 1];
                    }

                    dgv.EndEdit();
                    bs.EndEdit();
                }
                else
                {
                    MessageBox.Show(@"To Move Row Down, you must Select '{" + clickedColumnName.Replace(columnPrefix, "") + @"}' Cell!", @"Move Display Order Down", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public static void MoveDisplayOrderToTop(DataGridView dgv, BindingSource bs, string clickedColumnName, string columnPrefix)
        {
            if (dgv.Rows.Count == 0) return;

            //Get selected Cell Count to make sure that user has selected some cell!
            int selectedCellCount = dgv.GetCellCount(DataGridViewElementStates.Selected);

            if (selectedCellCount == 0)
            {
                MessageBox.Show(@"To Move Display Order to Top, you must Select '{" + clickedColumnName.Replace(columnPrefix, "") + @"}' Cell!", @"Move Display Order to Top", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount > 1)
            {
                MessageBox.Show(@"To Move Display Order to Top, you must only Select One Cell!", @"Move Display Order to Top", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount == 1)
            {
                //Now find out which column cell was selected, if it was clickedColumnName then
                // ReSharper disable once PossibleNullReferenceException
                if (dgv.SelectedCells[0].ColumnIndex == dgv.Columns[clickedColumnName].Index)
                {
                    //Now make sure that this was not already at the top!
                    var rowIndex = dgv.CurrentCell.RowIndex;
                    var selectedColumnIndex = dgv.Columns[clickedColumnName].Index;

                    if (rowIndex == 0)
                    {
                        MessageBox.Show(@"Display Order is already at the Top!", @"Move Display Order to Top", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    while (rowIndex >= 1)
                    {
                        DataRowView selectedRow = dgv.Rows[rowIndex].DataBoundItem as DataRowView;
                        if (selectedRow != null)
                        {
                            var selectedRowDisplayOrder = Convert.ToInt32(selectedRow["DISPLAY_ORDER"]);

                            //Get the above row 
                            DataRowView aboveRow = dgv.Rows[rowIndex - 1].DataBoundItem as DataRowView;
                            if (aboveRow != null)
                            {
                                var aboveRowDisplayOrder = Convert.ToInt32(aboveRow["DISPLAY_ORDER"]);

                                //Change the row above to the displayOrder of the moving row
                                selectedRow["DISPLAY_ORDER"] = aboveRowDisplayOrder;
                            }

                            if (aboveRow != null) aboveRow["DISPLAY_ORDER"] = selectedRowDisplayOrder;
                        }

                        if (rowIndex != 0)
                        {
                            dgv.CurrentCell = dgv[selectedColumnIndex, rowIndex - 1];
                        }

                        dgv.EndEdit();
                        rowIndex--;
                    }

                    bs.EndEdit();
                }
                else
                {
                    MessageBox.Show(@"To Move Display Order to Top, you must Select '{" + clickedColumnName.Replace(columnPrefix, "") + @"}' Cell!", @"Move Display Order to Top", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public static void MoveDisplayOrderToBottom(DataGridView dgv, BindingSource bs, string clickedColumnName, string columnPrefix)
        {
            if (dgv.Rows.Count == 0) return;

            //Get selected Cell Count to make sure that user has selected some cell!
            int selectedCellCount = dgv.GetCellCount(DataGridViewElementStates.Selected);

            if (selectedCellCount == 0)
            {
                MessageBox.Show(@"To Move Display Order to Bottom, you must Select '{" + clickedColumnName.Replace(columnPrefix, "") + @"}' Cell!", @"Move Display Order to Bottom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount > 1)
            {
                MessageBox.Show(@"To Move Display Order to Bottom, you must only Select one Cell!", @"Move Display Order to Bottom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCellCount == 1)
            {
                //Now find out which column cell was selected, if it was clickedColumnName then
                // ReSharper disable once PossibleNullReferenceException
                if (dgv.SelectedCells[0].ColumnIndex == dgv.Columns[clickedColumnName].Index)
                {
                    //Now make sure that this was not already at the bottom!
                    var rowIndex = dgv.CurrentCell.RowIndex;
                    var selectedColumnIndex = dgv.Columns[clickedColumnName].Index;
                    var lastRowsIndex = dgv.Rows.Count - 1;

                    if (rowIndex == dgv.Rows.Count - 1)
                    {
                        MessageBox.Show(@"Display order is already at the Bottom!", @"Move Display Order to Bottom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    while (rowIndex < lastRowsIndex)
                    {
                        if (dgv.Rows[rowIndex].DataBoundItem is DataRowView selectedRow)
                        {
                            var selectedRowDisplayOrder = Convert.ToInt32(selectedRow["DISPLAY_ORDER"]);

                            //Get the above below 
                            DataRowView belowRow = dgv.Rows[rowIndex + 1].DataBoundItem as DataRowView;
                            if (belowRow != null)
                            {
                                var belowRowDisplayOrder = Convert.ToInt32(belowRow["DISPLAY_ORDER"]);

                                //Change the row above to the displayOrder of the moving row
                                selectedRow["DISPLAY_ORDER"] = belowRowDisplayOrder;
                            }

                            if (belowRow != null) belowRow["DISPLAY_ORDER"] = selectedRowDisplayOrder;
                        }

                        if (rowIndex != dgv.Rows.Count - 1)
                        {
                            dgv.CurrentCell = dgv[selectedColumnIndex, rowIndex + 1];
                        }

                        dgv.EndEdit();

                        rowIndex++;
                    }

                    bs.EndEdit();
                }
                else
                {
                    MessageBox.Show(@"To Move Display Order to Bottom, you must Select '{" + clickedColumnName.Replace(columnPrefix, "") + @"}' Cell!", @"Move Display Order to Bottom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
