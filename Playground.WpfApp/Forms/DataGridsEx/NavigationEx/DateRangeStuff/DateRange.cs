using System;

namespace Playground.WpfApp.Forms.DataGridsEx.NavigationEx.DateRangeStuff
{
    public struct DateRange : IEquatable<DateRange>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateRange" /> struct.
        /// </summary>
        /// <param name="start">The date representing the start of the range.</param>
        /// <param name="end">The date representing the end of the range.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <paramref name="start" /> date is after the <paramref name="end" /> date.
        /// </exception>
        public DateRange(DateTime start, DateTime end)
        {
            if (start > end)
            {
                throw new ArgumentOutOfRangeException(nameof(end), @"Start date must be lower than end date");
            }

            StartDate = start;
            EndDate = end;
        }

        /// <summary>
        /// Gets the date representing the end of the range.
        /// </summary>
        public DateTime EndDate { get; }

        /// <summary>
        /// Gets the date representing the start of the range.
        /// </summary>
        public DateTime StartDate { get; }

        public static bool operator !=(DateRange rangeA, DateRange rangeB)
        {
            return !(rangeA == rangeB);
        }

        public static bool operator ==(DateRange rangeA, DateRange rangeB)
        {
            return rangeA.Equals(rangeB);
        }

        /// <summary>
        /// Returns a value indicating whether the <paramref name="date" /> is within the range
        /// represented by [ <see cref="StartDate" />, <see cref="EndDate" />].
        /// </summary>
        /// <param name="date">The date to compare.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="date" /> is in the range; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(DateTime date)
        {
            return StartDate <= date && date <= EndDate;
        }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is DateRange range
                && Equals(range);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" />
        /// parameter; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(DateRange other)
        {
            return StartDate == other.StartDate
                && EndDate == other.EndDate;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // choose some large prime numbers to avoid hashing collisions
                const int hashingBase = (int)2166136261;
                const int hashingMultiplier = 16777619;

                int hash = hashingBase;
                hash = (hash * hashingMultiplier) ^ StartDate.GetHashCode();
                hash = (hash * hashingMultiplier) ^ EndDate.GetHashCode();
                return hash;
            }
        }
    }
}
