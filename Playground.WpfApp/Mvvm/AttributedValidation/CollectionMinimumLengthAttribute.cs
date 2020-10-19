using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Playground.WpfApp.Mvvm.AttributedValidation
{
    /// <summary>
    /// Custom DataAnnotations validation attribute to validate that a collection has at least the
    /// specified number of items.
    /// </summary>
    /// <seealso cref="System.ComponentModel.DataAnnotations.ValidationAttribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CollectionMinimumLengthAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionMinimumLengthAttribute" /> class.
        /// </summary>
        /// <param name="length">The length.</param>
        public CollectionMinimumLengthAttribute(int length)
            : base(@"{0} must contain at least {1} item(s).")
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), $@"{nameof(length)} must be greater than or equal to zero.");
            }

            this.Length = length;
        }

        /// <summary>
        /// Gets the minimum length allowed by the collection.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <param name="name">The name to include in the formatted message.</param>
        /// <returns>An instance of the formatted error message.</returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(this.ErrorMessageString, name, Length);
        }

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// An instance of the
        /// <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var collection = value as IEnumerable;
            if (collection == null)
            {
                // Automatically pass if value is null. RequiredAttribute should be used to assert a
                // value is not null.
                return ValidationResult.Success;
            }

            if (Length == 0)
            {
                // if the minimum length is zero, the enumerable will be considered valid even if
                // it's empty. We can short-circuit the check and just return success
                return ValidationResult.Success;
            }

            // we can gain efficiency by checking if the collection has a count property before we go
            // ahead an enumerate it
            var col = value as ICollection;
            if (col != null)
            {
                if (col.Count >= Length)
                {
                    return ValidationResult.Success;
                }

                // if we get here, the collection was too short. Validation fails.
                return new ValidationResult(
                    FormatErrorMessage(validationContext.DisplayName),
                    validationContext.MemberName?.Yield());
            }

            // ensure the length of the collections is at least the minimum length allowed
            var enumerator = collection.GetEnumerator();
            try
            {
                int count = 0;
                while (enumerator.MoveNext())
                {
                    count++;
                    if (count >= Length)
                    {
                        // if we've hit the minimum we need, return success
                        return ValidationResult.Success;
                    }
                }
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }

            // if we get here, the collection was too short. Validation fails.
            return new ValidationResult(
                FormatErrorMessage(validationContext.DisplayName),
                validationContext.MemberName?.Yield());
        }
    }
}
