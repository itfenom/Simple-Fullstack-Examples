using System.Windows;
using System.Windows.Interactivity;

namespace Playground.WpfApp.Behaviors
{
    internal class CancelCloseWindowBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty CancelCloseProperty =
            DependencyProperty.Register("CancelClose", typeof(bool),
                typeof(CancelCloseWindowBehavior), new FrameworkPropertyMetadata(false));

        public bool CancelClose
        {
            get => (bool)GetValue(CancelCloseProperty);
            set => SetValue(CancelCloseProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.Closing += (sender, args) => args.Cancel = CancelClose;
        }
    }

    /*Usage:
       In XAML:
                <i:Interaction.Behaviors>
                  <local:CancelCloseWindowBehavior CancelClose="{Binding CancelClose}" />
                </i:Interaction.Behaviors>

      Where CancelClose is a bool property from the VM
      which indicates if the Closing event should be cancelled or not.
 */
}
