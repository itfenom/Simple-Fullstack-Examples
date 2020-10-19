using System;

namespace Playground.Core.MessageBoxWrappers
{
    public enum DialogResult : byte
    {
        Abort = 3,
        Cancel = 2,
        Ignore = 5,
        No = 7,
        None = 0,
        Ok = 1,
        Retry = 4,
        Yes = 6
    }

    public enum MessageBoxButtons : byte
    {
        Ok = 0,
        OkCancel = 1,
        AbortRetryIgnore = 2,
        YesNoCancel = 3,
        YesNo = 4,
        RetryCancel = 5
    }

    public enum MessageBoxIcon : byte
    {
        Asterisk = 64,
        Error = 16,
        Exclamation = 48,
        Hand = 16,
        Information = 64,
        None = 0,
        Question = 32,
        Stop = 16,
        Warning = 48
    }

    public static class MessageBox
    {
        public static Func<string, DialogResult> ShowText;
        public static Func<string, string, DialogResult> ShowTextCaption;
        public static Func<string, string, MessageBoxButtons, DialogResult> ShowTextCaptionButtons;
        public static Func<string, string, MessageBoxButtons, MessageBoxIcon, DialogResult> ShowTextCaptionButtonsIcon;

        public static DialogResult Show(string text)
        {
            if (ShowText != null)
            {
                return ShowText(text);
            }

            return DialogResult.Ok; 
        }

        public static DialogResult Show(string text, string caption)
        {
            if (ShowTextCaption != null)
            {
                return ShowTextCaption(text, caption);
            }

            return DialogResult.Ok; 
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            if (ShowTextCaptionButtons != null)
            {
                return ShowTextCaptionButtons(text, caption, buttons);
            }

            return DialogResult.Ok; 
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            if (ShowTextCaptionButtonsIcon != null)
            {
                return ShowTextCaptionButtonsIcon(text, caption, buttons, icon);
            }

            return DialogResult.Ok; 
        }
    }
}
