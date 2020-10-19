using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Playground.WpfApp
{
    public partial class UnhandledExceptionWindow
    {
        public UnhandledExceptionWindow(Exception ex)
        {
            InitializeComponent();

            TxtMsg.Text = ex.Message;
            TxtStackTrace.Text = TranslateStack(ex);
        }

        private string TranslateStack(Exception exception)
        {
            var format = "{0};{1};{2};{3};{4}";
            var builder = new StringBuilder();
            var trace = new StackTrace(exception, true);
            var stackFrames = trace.GetFrames();
            if (stackFrames == null) return builder.ToString();
            foreach (var frame in stackFrames)
            {
                var name = frame.GetMethod().Name;
                var subBuilder = new StringBuilder();
                var fileName = frame.GetFileName();
                var str = frame.GetFileLineNumber().ToString(CultureInfo.InvariantCulture);
                if ((frame.GetMethod() == null) || (frame.GetMethod().DeclaringType == null)) continue;
                // ReSharper disable once PossibleNullReferenceException
                string fullName = frame.GetMethod().DeclaringType.FullName;
                foreach (var info in frame.GetMethod().GetParameters())
                {
                    if (subBuilder.Length != 0)
                    {
                        subBuilder.Append(", ");
                    }
                    subBuilder.Append(info.ParameterType.Name + " " + info.Name);
                }
                if (builder.Length != 0)
                {
                    builder.Append("\r\n");
                }
                builder.AppendFormat(CultureInfo.CurrentCulture, format, name, subBuilder, fullName, fileName, str);
            }
            return builder.ToString();
        }
    }
}
