using System;

namespace Playground.Core.MessageBoxWrappers
{
    public class InputBox
    {
        public static Func<string, string, string, string> DisplayInputBox;

        public static string Show(string message, string title, string defaultInput)
        {
            return DisplayInputBox?.Invoke(message, title, defaultInput);
        }
        public static string Show(string message, string title)
        {
            return Show(message, title, string.Empty);
        }
        public static string Show(string message)
        {
            return Show(message, string.Empty, string.Empty);
        }
    }
}
