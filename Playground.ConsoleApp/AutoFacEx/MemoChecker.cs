using System;
using System.Collections.Generic;
using System.Linq;

namespace Playground.ConsoleApp.AutoFacEx
{
    public class MemoChecker
    {
        private readonly List<Memo> _memos;
        private readonly IMemoDueNotifier _notifier;

        public MemoChecker(List<Memo> memos, IMemoDueNotifier notifier)
        {
            _memos = memos;
            _notifier = notifier;
        }

        public void CheckNow()
        {
            var overdueMemos = _memos.Where(memo => memo.DueAt < DateTime.Now);

            foreach (var memo in overdueMemos)
                _notifier.MemoIsDue(memo);
        }
    }
    public class Memo
    {
        public string Title { get; set; }
        public DateTime DueAt { get; set; }
    }

    public interface IMemoDueNotifier
    {
        void MemoIsDue(Memo memo);
    }
}
