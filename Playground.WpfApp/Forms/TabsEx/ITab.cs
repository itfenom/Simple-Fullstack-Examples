using System;
using System.Windows.Input;

namespace Playground.WpfApp.Forms.TabsEx
{
    public interface ITab
    {
        string Name { get; set; }
        ICommand CloseCommand { get; }

        event EventHandler CloseRequested;
    }
}