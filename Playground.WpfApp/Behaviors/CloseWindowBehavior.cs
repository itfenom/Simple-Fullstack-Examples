using System.Windows;
using System.Windows.Interactivity;

namespace Playground.WpfApp.Behaviors
{
    public class CloseWindowBehavior : Behavior<Window>
    {
        public bool CloseTrigger
        {
            get => (bool)GetValue(CloseTriggerProperty);
            set => SetValue(CloseTriggerProperty, value);
        }

        public static readonly DependencyProperty CloseTriggerProperty =
            DependencyProperty.Register("CloseTrigger", typeof(bool), typeof(CloseWindowBehavior), new PropertyMetadata(false, OnCloseTriggerChanged));

        private static void OnCloseTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CloseWindowBehavior behavior)
            {
                behavior.OnCloseTriggerChanged();
            }
        }

        private void OnCloseTriggerChanged()
        {
            // when close-trigger is true, close the window
            if (CloseTrigger)
            {
                AssociatedObject.Close();
            }
        }
    }
}
/*
 * <Window x:Class="TestApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
        xmlns:local="clr-namespace:TestApp"
        Title="MainWindow" Height="350" Width="525">

        <i:Interaction.Behaviors>
            <local:CloseWindowBehavior CloseTrigger="{Binding CloseTrigger}" />
        </i:Interaction.Behaviors>

        <Grid>

        </Grid>
    </Window>

        private bool _closeTrigger;

        /// <summary>
        /// Gets or Sets if the window should be closed
        /// </summary>
        public bool CloseTrigger
        {
            get => _closeTrigger;
            set => SetPropertyValue(ref _closeTrigger, value);
        }
 */
