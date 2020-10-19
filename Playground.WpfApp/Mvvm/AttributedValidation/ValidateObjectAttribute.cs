using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Playground.WpfApp.Mvvm.AttributedValidation
{
    /// <summary>
    /// An attribute that can be used to mark a property as an object requiring validation. If there
    /// are errors on the object, they are returned as a <see cref="CompositeValidationResult" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidateObjectAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateObjectAttribute" /> class.
        /// </summary>
        public ValidateObjectAttribute()
            : base(@"Validation for {0} failed!")
        {
        }

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <returns>An instance of the <see cref="ValidationResult" /> class.</returns>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                // Automatically pass if value is null. RequiredAttribute should be used to assert a
                // value is not null.
                return ValidationResult.Success;
            }

            var results = new List<ValidationResult>();
            var context = new ValidationContext(value, null, null);

            Validator.TryValidateObject(value, context, results, true);

            if (results.Count != 0)
            {
                var compositeResults = new CompositeValidationResult(
                    FormatErrorMessage(validationContext.DisplayName),
                    validationContext.MemberName != null ? new string[] { validationContext.MemberName } : null);
                results.ForEach(compositeResults.AddResult);
                return compositeResults;
            }

            // anything else will return success
            return ValidationResult.Success;
        }
    }
}
