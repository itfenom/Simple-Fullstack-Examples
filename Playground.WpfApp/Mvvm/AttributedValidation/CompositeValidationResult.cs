using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Playground.WpfApp.Mvvm.AttributedValidation
{
    /// <summary>
    /// A derived implementation of a validation result that can be used to hold multiple validation
    /// results in a single <see cref="CompositeValidationResult" />.
    /// </summary>
    public class CompositeValidationResult : ValidationResult
    {
        private readonly List<ValidationResult> _results = new List<ValidationResult>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult" /> class by using an error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public CompositeValidationResult(string errorMessage) : base(errorMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult" /> class by using an error
        /// message and a list of members that have validation errors.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="memberNames">The list of member names that have validation errors.</param>
        public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult" /> class by using a
        /// <see cref="ValidationResult" /> object.
        /// </summary>
        /// <param name="validationResult">The validation result object.</param>
        protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult)
        {
        }

        /// <summary>
        /// Gets the collection of validation results.
        /// </summary>
        public IReadOnlyCollection<ValidationResult> Results => _results.AsReadOnly();

        /// <summary>
        /// Add a new validation result to the composite result.
        /// </summary>
        /// <param name="validationResult">The validation result to add to the composite result.</param>
        public void AddResult(ValidationResult validationResult)
        {
            if (validationResult != null)
            {
                _results.Add(validationResult);
            }
        }
    }
}
