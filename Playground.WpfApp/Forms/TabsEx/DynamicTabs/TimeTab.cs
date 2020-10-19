using System;

namespace Playground.WpfApp.Forms.TabsEx.DynamicTabs
{
    public class TimeTab : Tab
    {
        public TimeTab()
        {
            Name = "Time: " + DateTime.Now.ToString("HH:mm:ss");
        }
    }
}