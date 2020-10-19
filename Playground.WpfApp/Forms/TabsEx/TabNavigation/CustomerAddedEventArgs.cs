using System;

namespace Playground.WpfApp.Forms.TabsEx.TabNavigation
{
    public class CustomerAddedEventArgs : EventArgs
    {
        public CustomerAddedEventArgs(TabNavigationCustomer newCustomer)
        {
            NewCustomer = newCustomer;
        }

        public TabNavigationCustomer NewCustomer { get; private set; }
    }
}