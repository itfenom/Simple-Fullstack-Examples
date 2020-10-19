using System.Windows.Forms;
using Playground.Core.ColumnFilters.Filters;

namespace Playground.Core.ColumnFilters.Columns
{
    public class DataGridViewTextBoxColumnFiltered : DataGridViewTextBoxColumn
    {
        public readonly BaseFilter Filter;

        public DataGridViewTextBoxColumnFiltered()
            : base()
        {
            Filter = new BaseFilter(this);
        }

        public ColumnFilterType FilterType
        {
            get => Filter.FilterType;
            set => Filter.FilterType = value;
        }

        public SubstringType SubstringType
        {
            get => Filter.SubstringType;
            set => Filter.SubstringType = value;
        }
    }
}
