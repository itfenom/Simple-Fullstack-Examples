using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Playground.Core.ColumnFilters.Filters
{
    public enum SubstringType
    {
        Equals,
        StartsWith,
        Contains,
        EndsWith
    }

    public class TextBoxFilterHeaderCell : FilteredHeaderCell
    {
        public SubstringType SubstringType { get; set; }

        private readonly TextBox _textBox = new TextBox();

        public TextBoxFilterHeaderCell() : base()
        {
            Initialize();
        }

        public TextBoxFilterHeaderCell(DataGridViewColumnHeaderCell template) : base(template)
        {
            Initialize();
        }

        private void Initialize()
        {
            SubstringType = SubstringType.Contains;
        }

        public override object Clone()
        {
            return new TextBoxFilterHeaderCell(this);
        }

        public override Control FilterControl => _textBox;

        protected override void InitFilterControl()
        {
            string filterValueString = filterValue as string;
            if (string.IsNullOrEmpty(filterValueString))
            {
                _textBox.Text = @"Filter...";
                _textBox.ForeColor = SystemColors.GrayText;
            }
            else
            {
                _textBox.Text = filterValueString;
                _textBox.ForeColor = SystemColors.ControlText;
            }
        }

        protected override void HandleFilterControlEvents()
        {
            _textBox.Enter += textBox_Enter;
            _textBox.Leave += textBox_Leave;
            _textBox.KeyPress += textBox_KeyPress;
            _textBox.TextChanged += textBox_TextChanged;
        }

        protected override void UnhandledFilterControlEvents()
        {
            _textBox.Enter -= textBox_Enter;
            _textBox.Leave -= textBox_Leave;
            _textBox.KeyPress -= textBox_KeyPress;
            _textBox.TextChanged -= textBox_TextChanged;
        }

        protected override void DisplayErrorInFilterControl()
        {
        }

        protected override void ClearFilterValue()
        {
            filterValue = string.Empty;
        }

        protected override bool FilterObjectEmpty(object filterObject)
        {
            return string.IsNullOrEmpty(filterObject as string);
        }

        protected override bool FilterObjectsEqual(object filterObject1, object filterObject2)
        {
            string filterObject1String = filterObject1 as string;
            string filterObject2String = filterObject2 as string;

            if (filterObject1String == null)
            {
                return string.IsNullOrEmpty(filterObject2String);
            }

            if (filterObject2String == null)
            {
                return string.IsNullOrEmpty(filterObject1String);
            }

            return filterObject1String.Equals(filterObject2String);
        }

        public string AccountForCase(string text)
        {
            DataTable dataTable = GetMainTable();

            bool caseSensitive = dataTable?.CaseSensitive ?? false;

            return caseSensitive ? text : text.ToUpper();
        }

        public bool CompareSubstring(string largerString, string substring)
        {
            largerString = AccountForCase(largerString);
            substring = AccountForCase(substring);

            switch (SubstringType)
            {
                case SubstringType.Equals:
                    return largerString.Equals(substring);
                case SubstringType.StartsWith:
                    return largerString.StartsWith(substring);
                case SubstringType.Contains:
                    return largerString.Contains(substring);
                case SubstringType.EndsWith:
                    return largerString.EndsWith(substring);
                default:
                    throw new NotImplementedException($"The TextBoxFilterHeaderCell has a SubstringType with undefined behavior: {SubstringType}");
            }
        }

        public string GetLikeOrEqualsExpression(string columnName, string substring)
        {
            switch (SubstringType)
            {
                case SubstringType.Equals:
                    return $"[{columnName}] = '{substring}'";
                case SubstringType.StartsWith:
                    return $"[{columnName}] LIKE '{substring}%'";
                case SubstringType.Contains:
                    return $"[{columnName}] LIKE '%{substring}%'";
                case SubstringType.EndsWith:
                    return $"[{columnName}] LIKE '%{substring}'";
                default:
                    throw new NotImplementedException($"The TextBoxFilterHeaderCell has a SubstringType with undefined behavior: {SubstringType}");
            }
        }

        protected override string GetColumnFilter(object filterObject)
        {
            string filterObjectString = filterObject as string;
            if (string.IsNullOrEmpty(filterObjectString) || string.IsNullOrEmpty(OwningColumn?.DataPropertyName))
            {
                return string.Empty;
            }

            filterObjectString = filterObjectString.Replace("'", "''");

            // Store the column name in a form acceptable to the Filter property, using a backslash
            // to escape any closing square brackets.
            string columnProperty = OwningColumn.DataPropertyName.Replace("]", @"\]");

            // Determine the column filter string based on the user selection.
            string newColumnFilter;

            DataGridViewComboBoxColumn owningComboBoxColumn = OwningColumn as DataGridViewComboBoxColumn;
            DataTable dtColumnSource = owningComboBoxColumn?.DataSource as DataTable;
            if (dtColumnSource != null && owningComboBoxColumn.DisplayMember != owningComboBoxColumn.ValueMember)
            {
                //In this case, we need to translate between the DisplayMember and the ValueMember.

                //First, find all of the possible DisplayMember values that match our filter.
                EnumerableRowCollection<DataRow> matchingRows;
                if (dtColumnSource.Columns[owningComboBoxColumn.DisplayMember].DataType == typeof(string))
                {
                    matchingRows = dtColumnSource.AsEnumerable().Where(r => CompareSubstring(r[owningComboBoxColumn.DisplayMember].ToString(), filterObjectString));
                }
                else
                {
                    matchingRows = dtColumnSource.AsEnumerable().Where(r => r[owningComboBoxColumn.DisplayMember].ToString() == filterObjectString);
                }

                //Now, get all the corresponding ValueMember values.
                EnumerableRowCollection<string> valueMemberStrings;
                if (dtColumnSource.Columns[owningComboBoxColumn.ValueMember].DataType == typeof(string))
                {
                    valueMemberStrings = matchingRows.Select(r => $"'{r[owningComboBoxColumn.ValueMember].ToString()}'");
                }
                else
                {
                    valueMemberStrings = matchingRows.Select(r => r[owningComboBoxColumn.ValueMember].ToString());
                }

                //We will have to write out those ValueMember values in a comma-separated list behind the scenes.
                if (valueMemberStrings.Any())
                {
                    newColumnFilter = $"[{columnProperty}] IN ({string.Join(", ", valueMemberStrings)})";
                }
                else
                {
                    //There were no ValueMember values that matched, so we need to apply a filter that will prevent any rows from being shown.
                    newColumnFilter = $"[{columnProperty}] IS NULL AND [{columnProperty}] IS NOT NULL";
                }
            }
            else
            {
                if (OwningColumn.ValueType == typeof(string))
                {
                    newColumnFilter = GetLikeOrEqualsExpression(columnProperty, filterObjectString);
                }
                else
                {
                    newColumnFilter = $"[{columnProperty}] = {filterObjectString}";
                }
            }

            return newColumnFilter;
        }

        #region Filter Control events

        private void textBox_Enter(object sender, EventArgs e)
        {
            if (_textBox.ForeColor == SystemColors.GrayText)
            {
                _textBox.Text = "";
                _textBox.ForeColor = SystemColors.ControlText;
            }
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            if (_textBox.Text == "" || _textBox.ForeColor == SystemColors.GrayText)
            {
                _textBox.ForeColor = SystemColors.GrayText;
                _textBox.Text = @"Filter...";
                UpdateFilter(string.Empty, false);
            }
            else
            {
                UpdateFilter(_textBox.Text, false);
            }
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13 && !FilterObjectsEqual(_textBox.Text, filterValue))
            {
                UpdateFilter(_textBox.Text, false);
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (_textBox.ForeColor == SystemColors.ControlText && !FilterObjectsEqual(_textBox.Text, filterValue))
            {
                UpdateFilter(_textBox.Text, false);
            }
        }

        #endregion Filter Control events
    }
}
