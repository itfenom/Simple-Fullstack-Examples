using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Playground.WpfApp.WpfUtilities
{
    public class FormButtonSettings
    {
        #region public bool HideCloseButton (attached)

        public static readonly DependencyProperty HideCloseButtonProperty =
             DependencyProperty.RegisterAttached("HideCloseButton", typeof(bool), typeof(FormButtonSettings), new FrameworkPropertyMetadata(false, OnHideCloseButtonPropertyChanged));

        public static bool GetHideCloseButton(FrameworkElement element)
        {
            return (bool)element.GetValue(HideCloseButtonProperty);
        }

        public static void SetHideCloseButton(FrameworkElement element, bool hideCloseButton)
        {
            element.SetValue(HideCloseButtonProperty, hideCloseButton);
        }

        static void OnHideCloseButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = d as Window;

            if (window != null)
            {
                var hideCloseButton = (bool)e.NewValue;

                if (hideCloseButton && !GetIsCloseButtonHidden(window))
                {
                    if (!window.IsLoaded)
                        window.Loaded += OnWindowLoaded;
                    else
                        HideCloseButton(window);

                    SetIsCloseButtonHidden(window, true);
                }
                else if (!hideCloseButton && GetIsCloseButtonHidden(window))
                {
                    if (!window.IsLoaded)
                        window.Loaded -= OnWindowLoaded;
                    else
                        ShowCloseButton(window);

                    SetIsCloseButtonHidden(window, false);
                }
            }
        }

        static readonly RoutedEventHandler OnWindowLoaded = (s, e) => {

            if (s is Window)
            {
                Window window = s as Window;
                HideCloseButton(window);
                window.Loaded -= OnWindowLoaded;
            }

        };

        #endregion

        #region public bool IsCloseButtonHidden (readonly attached)

        static readonly DependencyPropertyKey IsHiddenCloseButtonKey =
            DependencyProperty.RegisterAttachedReadOnly("IsCloseButtonHidden", typeof(bool), typeof(FormButtonSettings), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsCloseButtonHiddenProperty =
             IsHiddenCloseButtonKey.DependencyProperty;

        public static bool GetIsCloseButtonHidden(FrameworkElement element)
        {
            return (bool)element.GetValue(IsCloseButtonHiddenProperty);
        }

        static void SetIsCloseButtonHidden(FrameworkElement element, bool isCloseButtonHidden)
        {
            element.SetValue(IsHiddenCloseButtonKey, isCloseButtonHidden);
        }

        #endregion

        #region Helper Methods

        static void HideCloseButton(Window w)
        {
            IntPtr hWnd = new WindowInteropHelper(w).Handle;
            SetWindowLong(hWnd, GWL_STYLE, GetWindowLong(hWnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        static void ShowCloseButton(Window w)
        {
            IntPtr hWnd = new WindowInteropHelper(w).Handle;
            SetWindowLong(hWnd, GWL_STYLE, GetWindowLong(hWnd, GWL_STYLE) | WS_SYSMENU);
        }

        #endregion

        #region Win32 Native Methods And Constants

        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once InconsistentNaming
        const int GWL_STYLE = -16;
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once InconsistentNaming
        const int WS_SYSMENU = 0x80000;

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion
    }
}

/*
 Usage:
 <Window x:Class="Parago.Windows.ProgressDialog" 
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
        xmlns:ui="clr-namespace:Parago.Windows.Controls" 
        Height="140" 
        Width="340" 
        MinHeight="140" 
        MinWidth="340" 
        Title="XtractBCS" 
        FontFamily="Segoe UI" 
        ResizeMode="CanResize" 
        WindowStyle="SingleBorderWindow" 
        WindowStartupLocation="CenterOwner" 
        Closing="OnClosing" ui:WindowSettings.HideCloseButton="True">
 */
