using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.TabsEx.TabNavigation
{
    /// <summary>
    /// Represents an actionable item displayed by a View.
    /// </summary>
    public class CommandViewModel : PropertyChangedBase
    {
        public string DisplayName { get; protected set; }

        public CommandViewModel(string displayName, ICommand command)
        {
            // ReSharper disable once JoinNullCheckWithUsage
            if (command == null)
                throw new ArgumentNullException($@"null command passed!");

            DisplayName = displayName;
            Command = command;
        }

        public ICommand Command { get; set; }
    }
}
