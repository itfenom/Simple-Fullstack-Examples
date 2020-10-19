using System;
using System.Collections.Generic;

namespace Playground.ConsoleApp.GenericEx
{
    public class MyGenericList<T>
    {
        private readonly List<T> _arrays;

        public MyGenericList()
        {
            _arrays = new List<T>();
        }

        public void Add(T value)
        {
            _arrays.Add(value);
        }

        public bool AreEqual(T value1, T value2)
        {
            return value1.Equals(value2);
        }

        public void Show()
        {
            Console.WriteLine(_arrays.Count > 0 ? string.Join("\n", _arrays) : "Nothing to show!");
        }
    }
}
