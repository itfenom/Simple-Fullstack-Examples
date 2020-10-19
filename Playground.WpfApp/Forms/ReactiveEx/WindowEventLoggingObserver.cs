using System;
using System.Windows;
using Playground.Core.Diagnostics;

namespace Playground.WpfApp.Forms.ReactiveEx
{
    /// <summary>
    /// Decorator that handles events on a window to perform logging when window events are fired.
    /// </summary>
    /// <typeparam name="TWindow">The type of the window being observed.</typeparam>
    /// <remarks>
    /// Some windows will inherit <see cref="Window" />, while others will directly inherit from
    /// <see cref="MahApps.Metro.Controls.MetroWindow" />. This class pulls out common logging logic
    /// that can simply be "attached" to anything that derives from <see cref="Window" /> (including <see cref="MahApps.Metro.Controls.MetroWindow" />).
    /// </remarks>
    public class WindowEventLoggingObserver<TWindow> where TWindow : Window
    {
        private TWindow _decoratedWindow;

        public WindowEventLoggingObserver(TWindow window)
        {
            _decoratedWindow = window ?? throw new ArgumentNullException(nameof(window));

            LogMessage("Decorating Window.");

            // hook into events to log opening and closing of the window
            _decoratedWindow.Loaded += Window_Loaded;
            _decoratedWindow.Closing += Window_Closing;
            _decoratedWindow.Closed += Window_Closed;

            // hook into activated/deactivated events so we can track which window has focus
            _decoratedWindow.Activated += Window_Activated;
            _decoratedWindow.Deactivated += Window_Deactivated;

            _decoratedWindow.DataContextChanged += Window_DataContextChanged;
        }

        /// <summary>
        /// Create a unique id for this window instance.
        /// </summary>
        private Guid UniqueId { get; } = Guid.NewGuid();

        private void DataContext_RequestClose(object sender, EventArgs e)
        {
            _decoratedWindow.Close();
        }

        /// <summary>
        /// Log the given <paramref name="message" />.
        /// </summary>
        /// <param name="message">The message to log.</param>
        private void LogMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Logger.Info($"{message} Instance = {UniqueId}; Type = {_decoratedWindow.GetType()};");
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            LogMessage("Window Activated.");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // detach event handlers
            _decoratedWindow.Loaded -= Window_Loaded;
            _decoratedWindow.Closing -= Window_Closing;
            _decoratedWindow.Closed -= Window_Closed;
            _decoratedWindow.Activated -= Window_Activated;
            _decoratedWindow.Deactivated -= Window_Deactivated;
            _decoratedWindow.DataContextChanged -= Window_DataContextChanged;

            if (_decoratedWindow.DataContext is ICloseable closeable)
            {
                closeable.RequestClose -= DataContext_RequestClose;
            }

#pragma warning disable S1215 // "GC.Collect" should not be called

            // force garbage collection. Especially useful when WPF windows are closed because the
            // XAML object may hold on to more memory than they should...
            GC.Collect();
#pragma warning restore S1215 // "GC.Collect" should not be called

            // log a message indicating that the window has closed
            LogMessage("Window Closed.");

            _decoratedWindow = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_decoratedWindow.DataContext is ICloseable closeable && !closeable.CanClose())
            {
                // if the view-model says we can't close, cancel the closing
                e.Cancel = true;
            }
        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ICloseable oldCloseable)
            {
                oldCloseable.RequestClose -= DataContext_RequestClose;
            }

            if (e.NewValue is ICloseable newCloseable)
            {
                newCloseable.RequestClose += DataContext_RequestClose;
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            LogMessage("Window Deactivated.");
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            LogMessage("Window Loading.");
        }
    }
}
