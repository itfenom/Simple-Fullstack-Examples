using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Playground.Core.ColumnFilters.Filters;

namespace Playground.Core.ColumnFilters
{
    public class BaseFilter
    {
        public readonly DataGridViewColumn Column;

        /// <summary>
        /// Initializes a new instance of the FilteredColumn class.
        /// </summary>
        public BaseFilter(DataGridViewColumn column)
        {
            Column = column;
            FilterType = ColumnFilterType.TextBoxFilter;
        }

        //private Type _defaultHeaderCellType = typeof(TextBoxFilterHeaderCell);
        private ColumnFilterType _filterType;

        [
            Browsable(true),
            EditorBrowsable(EditorBrowsableState.Always),
            Category("Design"),
            ReadOnly(false),
        ]
        public ColumnFilterType FilterType
        {
            get => _filterType;
            set
            {
                if (Column.HeaderCell is FilteredHeaderCell oldHeaderCell)
                {
                    oldHeaderCell.Detach();
                }

                switch (value)
                {
                    case ColumnFilterType.TextBoxFilter:
                        Column.DefaultHeaderCellType = typeof(TextBoxFilterHeaderCell);
                        Column.HeaderCell = new TextBoxFilterHeaderCell(Column.HeaderCell);
                        _filterType = value;
                        break;
                    case ColumnFilterType.ComboBoxFilter:
                        Column.DefaultHeaderCellType = typeof(ComboBoxFilterHeaderCell);
                        Column.HeaderCell = new ComboBoxFilterHeaderCell(Column.HeaderCell);
                        _filterType = value;
                        break;
                }
            }
        }

        public SubstringType SubstringType
        {
            get
            {
                if (Column.HeaderCell is TextBoxFilterHeaderCell filteredHeaderCell)
                {
                    return filteredHeaderCell.SubstringType;
                }

                return SubstringType.Equals;
            }
            set
            {
                if (Column.HeaderCell is TextBoxFilterHeaderCell filteredHeaderCell)
                {
                    filteredHeaderCell.SubstringType = value;
                }
            }
        }
    }
}
