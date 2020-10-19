using System;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.TabsEx
{
    public abstract class Tab : ValidationPropertyChangedBase, ITab
    {
        public Tab()
        {
            CloseCommand = new DelegateCommand(() => CloseRequested?.Invoke(this, EventArgs.Empty));
        }

        public string Name { get; set; }

        public ICommand CloseCommand { get; }

        public event EventHandler CloseRequested;
    }
}