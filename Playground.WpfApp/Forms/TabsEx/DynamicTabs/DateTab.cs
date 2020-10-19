using System;

namespace Playground.WpfApp.Forms.TabsEx.DynamicTabs
{
    public class DateTab : Tab
    {
        public DateTab()
        {
            Name = "Date: " + DateTime.Now.ToShortDateString();
        }
    }
}