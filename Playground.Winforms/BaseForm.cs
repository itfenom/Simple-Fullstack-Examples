using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Playground.Core.Diagnostics;
using Playground.Winforms.Utilities;

// ReSharper disable once IdentifierTypo
namespace Playground.Winforms
{
    public class BaseForm : Form
    {
        protected BaseForm()
        {
            LogMessage($"Creating Form. Instance = {UniqueId}; Type = {GetType()};");

            // hook into events to log opening and closing of the form
            Load += BaseForm_Load;
            FormClosed += BaseForm_FormClosed;

            // hook into activated/deactivated events so we can track which form has focus
            Activated += BaseForm_Activated;
            Deactivate += BaseForm_Deactivate;
        }

        public static bool IsInDesignMode
        {
            get
            {
                bool isInDesignMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime;
                if (!isInDesignMode)
                {
                    using (var process = Process.GetCurrentProcess())
                    {

                        // ReSharper disable once StringLiteralTypo
                        return process.ProcessName.ToLower().Contains(@"devenv");
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Create a unique id for this form instance.
        /// </summary>
        private Guid UniqueId { get; } = Guid.NewGuid();

        private void BaseForm_Activated(object sender, EventArgs e)
        {
            // log a message indicating that the form has been activated
            LogMessage($"Form Activated. Instance = {UniqueId}; Type = {GetType()};");
        }

        private void BaseForm_Deactivate(object sender, EventArgs e)
        {
            // log a message indicating that the form has been deactivated
            LogMessage($"Form Deactivated. Instance = {UniqueId}; Type = {GetType()};");
        }

        private void BaseForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // detach event handlers
            Load -= BaseForm_Load;
            FormClosed -= BaseForm_FormClosed;
            Activated -= BaseForm_Activated;
            Deactivate -= BaseForm_Deactivate;

            // log a message indicating that the form has closed
            LogMessage($"Form Closed. Instance = {UniqueId}; Type = {GetType()};");
        }

        private void BaseForm_Load(object sender, EventArgs e)
        {
            // log a message indicating that the form is loading
            LogMessage($"Form Loading. Instance = {UniqueId}; Type = {GetType()};");

            var dataGridViewControl = FindControl(this, typeof(DataGridView));

            if (dataGridViewControl != null)
            {
                Helpers.PositionMousePointerToTitleBarCenter(this);
            }
        }

        private Control FindControl(Control root, Type targetControlType)
        {
            if (root.GetType() == targetControlType)
            {
                return root;
            }

            foreach (Control child in root.Controls)
            {
                Control result = FindControl(child, targetControlType);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Log the given <paramref name="message" />.
        /// </summary>
        /// <param name="message">The message to log.</param>
        private void LogMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message) && !IsInDesignMode)
            {
                Logger.Info(message);
            }
        }
    }
}
