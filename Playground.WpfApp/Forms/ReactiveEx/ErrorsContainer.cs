using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;


namespace Playground.WpfApp.Forms.ReactiveEx
{
    /// <summary>
    /// Manages validation errors for an object, notifying when the error state changes.
    /// </summary>
    /// <typeparam name="T">The type of the error object.</typeparam>
    public class ErrorsContainer<T> : BindableBase
    {
        protected readonly BehaviorSubject<string> _raiseErrorsChanged = new BehaviorSubject<string>(null);
        protected readonly Dictionary<string, List<T>> _validationResults = new Dictionary<string, List<T>>();
        private readonly ObservableAsPropertyHelper<bool> _hasErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorsContainer{T}" /> class.
        /// </summary>
        /// <param name="raiseErrorsChanged">
        /// The action that is invoked when errors are added for an object.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="raiseErrorsChanged" /> is null.
        /// </exception>
        public ErrorsContainer(Action<string> raiseErrorsChanged)
        {
            if (raiseErrorsChanged == null)
            {
                throw new ArgumentNullException(nameof(raiseErrorsChanged));
            }

            // take the errors observable and create a connected observable
            var errors = _raiseErrorsChanged
                .Skip(1)
                .Publish()
                .RefCount();

            // when errors are observed, determine whether this instance has errors or not
            _hasErrors = errors
                .Select(_ => _validationResults.Count != 0)
                .ToProperty(this, x => x.HasErrors, initialValue: false, scheduler: Scheduler.Immediate)
                .DisposeWith(Disposables.Value);

            // push observed values into the action provided
            errors
                .Subscribe(raiseErrorsChanged)
                .DisposeWith(Disposables.Value);

            // dispose of the subject when not needed anymore
            _raiseErrorsChanged
                .DisposeWith(Disposables.Value);
        }

        /// <summary>
        /// Gets a value indicating whether the object has validation errors.
        /// </summary>
        public bool HasErrors => _hasErrors?.Value ?? false;

        /// <summary>
        /// Clears all errors from the validation result collection.
        /// </summary>
        public void ClearAllErrors()
        {
            while (_validationResults.Count > 0)
            {
                ClearErrors(_validationResults.Keys.First());
            }
        }

        /// <summary>
        /// Clears the errors for a property.
        /// </summary>
        /// <param name="propertyName">The name of the property for which to clear errors.</param>
        public void ClearErrors(string propertyName)
        {
            SetErrors(propertyName, Enumerable.Empty<T>());
        }

        /// <summary>
        /// Gets all validation errors associated with all properties.
        /// </summary>
        /// <returns>The validation errors of type <typeparamref name="T" />.</returns>
        public IEnumerable<T> GetAllErrors()
        {
            return _validationResults.SelectMany(x => x.Value);
        }

        /// <summary>
        /// Gets the validation errors for a specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The validation errors of type <typeparamref name="T" /> for the property.</returns>
        public IEnumerable<T> GetErrors(string propertyName)
        {
            var localPropertyName = propertyName ?? string.Empty;
            if (_validationResults.TryGetValue(localPropertyName, out var currentValidationResults))
            {
                return currentValidationResults.AsReadOnly();
            }

            return Enumerable.Empty<T>();
        }

        /// <summary>
        /// Sets the validation errors for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="newValidationResults">The new validation errors.</param>
        /// <remarks>If a change is detected then the errors changed event is raised.</remarks>
        public void SetErrors(string propertyName, IEnumerable<T> newValidationResults)
        {
            var localPropertyName = propertyName ?? string.Empty;
            var hasCurrentValidationResults = _validationResults.ContainsKey(localPropertyName);
            var hasNewValidationResults = newValidationResults?.Any() ?? false;

            if (hasCurrentValidationResults || hasNewValidationResults)
            {
                if (hasNewValidationResults)
                {
                    _validationResults[localPropertyName] = new List<T>(newValidationResults);
                }
                else
                {
                    _validationResults.Remove(localPropertyName);
                }

                _raiseErrorsChanged.OnNext(localPropertyName);
            }
        }
    }
}
