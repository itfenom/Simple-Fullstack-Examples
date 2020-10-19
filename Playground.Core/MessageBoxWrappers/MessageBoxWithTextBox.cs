using System;

namespace Playground.Core.MessageBoxWrappers
{
    public static class MessageBoxWithTextBox
    {
        public static Func<string, DialogResult> DisplayText;
        public static Func<string, string, DialogResult> DisplayTextCaption;
        public static Func<string, string, MessageBoxButtons, DialogResult> DisplayTextCaptionButtons;
        public static Func<string, string, MessageBoxButtons, MessageBoxIcon, DialogResult> DisplayTextCaptionButtonsIcon;

        public static DialogResult Show(string text)
        {
            if (DisplayText != null)
            {
                return DisplayText(text);
            }

            return DialogResult.Ok;
        }

        public static DialogResult Show(string text, string caption)
        {
            if (DisplayTextCaption != null)
            {
                return DisplayTextCaption(text, caption);
            }

            return DialogResult.Ok;
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            if (DisplayTextCaptionButtons != null)
            {
                return DisplayTextCaptionButtons(text, caption, buttons);
            }

            return DialogResult.Ok;
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            if (DisplayTextCaptionButtonsIcon != null)
            {
                return DisplayTextCaptionButtonsIcon(text, caption, buttons, icon);
            }

            return DialogResult.Ok;
        }
    }
}
