using System.ComponentModel;
using System.Windows;

namespace Playground.WpfApp
{
    public static class Config
    {
        // ReSharper disable once InconsistentNaming
        private static readonly bool _isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
#if (DEBUG)
        private static bool debug = true;
#else
        private static bool debug = false;
#endif

        /// <summary>
        /// Gets a value indicating whether the code is running in design mode.
        /// </summary>
        /// <value><c>true</c> if the code is running in design mode; otherwise, <c>false</c>.</value>
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once ConvertToAutoProperty
        public static bool IsInDesignMode => _isInDesignMode;
    }
}
