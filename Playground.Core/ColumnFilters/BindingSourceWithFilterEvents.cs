using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Playground.Core.ColumnFilters
{
    public class BindingSourceWithFilterEvents : BindingSource
    {
        public event EventHandler FilterChanged;
        public bool RaiseFilterChangedEvents { get; set; } = true;

        public BindingSourceWithFilterEvents()
            : base()
        {
        }
        public BindingSourceWithFilterEvents(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        public BindingSourceWithFilterEvents(object dataSource, string dataMember)
            : base(dataSource, dataMember)
        {
        }

        public override string Filter
        {
            get
            {
                return base.Filter;
            }
            set
            {
                base.Filter = value;
                if (FilterChanged != null && RaiseFilterChangedEvents)
                {
                    FilterChanged(this, EventArgs.Empty);
                }
            }
        }

        public string GetFilterStatus()
        {
            if (DataSource == null || !SupportsFiltering)
            {
                return string.Empty;
            }

            // Retrieve the filtered row count. 
            int currentRowCount = Count;

            // Retrieve the unfiltered row count by 
            // temporarily unfiltering the data.
            RaiseListChangedEvents = false;
            RaiseFilterChangedEvents = false;
            string oldFilter = Filter;
            Filter = null;
            int unfilteredRowCount = Count;
            Filter = oldFilter;
            RaiseListChangedEvents = true;
            RaiseFilterChangedEvents = true;

            Debug.Assert(currentRowCount <= unfilteredRowCount,
                "current count is greater than unfiltered count");

            // Return String.Empty if the filtered and unfiltered counts
            // are the same, otherwise, return the status string. 
            return String.Format("{0} out of {1}", currentRowCount, unfilteredRowCount);
        }
    }
}
