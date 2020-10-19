using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
// ReSharper disable PossibleMultipleEnumeration

namespace Playground.WpfApp.Mvvm.AttributedValidation
{
    /// <summary>
    /// Custom DataAnnotations validation attribute to validate that a given collection does not 
    /// have any empty or null element.
    /// </summary>
    public sealed class CollectionNotNullOrEmptyAttribute : ValidationAttribute
    {

        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <param name="name">The name to include in the formatted message.</param>
        /// <returns>An instance of the formatted error message.</returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
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
            var collection = value as IEnumerable<string>;
            if (collection == null)
            {
                // Automatically pass if value is null. RequiredAttribute should be used to assert a
                // value is not null.
                return ValidationResult.Success;
            }

            // ensure the length of the collections is at least the minimum length allowed
            if (collection.GetEnumerator().MoveNext())
            {
                var enumerator = collection.GetEnumerator();

                try
                {
                    var foundEmptyValue = false;
                    foreach (var item in collection)
                    {
                        if (string.IsNullOrEmpty(item))
                        {
                            foundEmptyValue = true;
                            break;
                        }
                    }

                    if (!foundEmptyValue)
                    {
                        return ValidationResult.Success;
                    }
                }
                finally
                {
                    (enumerator as IDisposable)?.Dispose();
                }
            }

            // if we get here, the collection was too short. Validation fails.
            return new ValidationResult(
                FormatErrorMessage(validationContext.DisplayName),
                validationContext.MemberName?.Yield());
        }
    }
}
