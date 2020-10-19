using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Playground.WpfApp.Mvvm.AttributedValidation
{
    // ReSharper disable once InconsistentNaming
    public static class IEnumerableExtensions
    {
        public static string ToTextList<T>(this IEnumerable<T> iEnumerable, string separator)
        {
            //TODO: Replace this with string.Join()
            StringBuilder stringBuilder = new StringBuilder();

            foreach (T element in iEnumerable)
            {
                stringBuilder.Append(Convert.ToString(element));
                stringBuilder.Append(separator);
            }

            if (stringBuilder.Length >= separator.Length) //safety check to deal with empty IEnumerables
            {
                stringBuilder.Remove(stringBuilder.Length - separator.Length, separator.Length);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Wraps an object instance into an <see cref="IEnumerable{T}" /> consisting of a single item.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="item">The instance that will be wrapped.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> consisting of a single item.</returns>
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// Finds duplicated items in the <paramref name="source"/> enumerable by using the key provided by the <paramref name="keySelector"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the key defining duplicate items.</typeparam>
        /// <typeparam name="TElement">Type of the elements of the source sequence.</typeparam>
        /// <param name="source">The collection of items.</param>
        /// <param name="keySelector">Function that transforms each item of source sequence into a key to be compared against the others.</param>
        /// <returns>The duplicated items.</returns>
        public static IEnumerable<IGrouping<TKey, TElement>> SelectDuplicatesBy<TKey, TElement>(this IEnumerable<TElement> source, Func<TElement, TKey> keySelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            // get the items matching the selector, where there are more than one of those items
            return source
                .GroupBy(keySelector)
                .Where(group => group.Skip(1).Any());
        }
    }
}
