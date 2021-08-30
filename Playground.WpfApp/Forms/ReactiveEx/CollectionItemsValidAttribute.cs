using Playground.WpfApp.Mvvm.AttributedValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Playground.WpfApp.Forms.ReactiveEx
{
    /// <summary>
    /// Custom DataAnnotations validation attribute to validate that a collection is valid. It does
    /// this by checking that the items within the collection that implement
    /// <see cref="IDataErrorInfo" /> are valid.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CollectionItemsValidAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionItemsValidAttribute" /> class.
        /// </summary>
        public CollectionItemsValidAttribute()
            : base("The {0} collection contains items with validation errors.")
        {
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
            var collection = value as System.Collections.IEnumerable;
            if (collection == null || !collection.Cast<object>().Any())
            {
                // if the collection is null or empty, just return success. Other attributes can
                // determine required-ness or length requirements
                return ValidationResult.Success;
            }

            // validate each item in the collection
            CompositeValidationResult result = null;
            foreach (var item in collection)
            {
                var results = new List<ValidationResult>();
                var context = new ValidationContext(item, null, null);

                Validator.TryValidateObject(item, context, results, true);

                if (results.Count > 0)
                {
                    if (result == null)
                    {
                        result = new CompositeValidationResult(
                            FormatErrorMessage(validationContext.DisplayName),
                            validationContext.MemberName?.Yield());
                    }

                    results.ForEach(result.AddResult);
                }
            }

            // return the result of the validation
            return result ?? ValidationResult.Success;
        }
    }
}
