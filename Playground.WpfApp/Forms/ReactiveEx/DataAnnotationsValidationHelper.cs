using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
// ReSharper disable PossibleNullReferenceException

namespace Playground.WpfApp.Forms.ReactiveEx
{
    /// <summary>
    /// Helper to validate objects and members using DataAnnotations.
    /// </summary>
    public static class DataAnnotationsValidationHelper
    {
        /// <summary>
        /// Determines whether the specified object is valid by validating all properties on the object.
        /// </summary>
        /// <param name="object">The object to validate.</param>
        /// <param name="results">The validation results.</param>
        /// <returns><c>true</c> if the object validates; otherwise, <c>false</c>.</returns>
        public static bool TryValidateObject(object @object, out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            var context = new ValidationContext(@object, serviceProvider: null, items: null);
            return Validator.TryValidateObject(@object, context, results, validateAllProperties: true);
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="object">The object to validate.</param>
        /// <param name="propertyName">Name of the property to be validated.</param>
        /// <param name="results">The validation results.</param>
        /// <returns><c>true</c> if the property validates; otherwise, <c>false</c>.</returns>
        public static bool TryValidateProperty(object @object, string propertyName, out ICollection<ValidationResult> results)
        {
            if (@object == null)
            {
                throw new ArgumentNullException(nameof(@object));
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException($@"{nameof(propertyName)} cannot be an empty string.", nameof(propertyName));
            }

            results = new List<ValidationResult>();
            var propertyInfo = @object.GetType().GetProperty(propertyName);
            var context = new ValidationContext(@object, serviceProvider: null, items: null)
            {
                MemberName = propertyName
            };

            return Validator.TryValidateProperty(propertyInfo.GetValue(@object, null), context, results);
        }

        /// <summary>
        /// Gets the error message for the property with the given name. Pass null or empty to
        /// validate the entire object.
        /// </summary>
        /// <param name="instance">The item to validate.</param>
        /// <returns>The validation results for the given member.</returns>
        public static IEnumerable<ValidationResult> ValidateObject(object instance)
        {
            TryValidateObject(instance, out var validationResults);
            return validationResults;
        }

        /// <summary>
        /// Gets the error message for the property with the given name. Pass null or empty to
        /// validate the entire object.
        /// </summary>
        /// <param name="instance">The item to validate.</param>
        /// <param name="memberName">The name of the property whose error message to get.</param>
        /// <returns>The validation results for the given member.</returns>
        public static IEnumerable<ValidationResult> ValidateProperty(object instance, string memberName)
        {
            TryValidateProperty(instance, memberName, out var validationResults);
            return validationResults;
        }
    }
}
