using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.ConsoleApp.AutoFacEx
{
    public class PrintingNotifier : IMemoDueNotifier
    {
        private TextWriter _writer;

        public PrintingNotifier(TextWriter writer)
        {
            _writer = writer;
        }

        public void MemoIsDue(Memo memo)
        {
            _writer.WriteLine("Memo '{0}' is due!", memo.Title);
        }
    }
}
