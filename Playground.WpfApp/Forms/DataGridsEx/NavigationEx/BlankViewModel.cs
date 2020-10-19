using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.DataGridsEx.NavigationEx
{
    public class BlankViewModel : ValidationPropertyChangedBase
    {
        public override string Title => "Current Selection: Blank";

        public BlankViewModel()
        {

        }

        public string Msg => "This is a blank View!";
    }
}
