using System.Windows;

namespace Playground.WpfApp.Forms.ReactiveEx
{
    /// <summary>
    /// Window used as the base for other windows in the application.
    /// </summary>
    public abstract class BaseWindow : Window
    {
        protected BaseWindow()
        {
            // by default set the style to the base window style
            SetResourceReference(StyleProperty, typeof(Window));
            _ = new WindowEventLoggingObserver<BaseWindow>(this);
        }
    }
}
