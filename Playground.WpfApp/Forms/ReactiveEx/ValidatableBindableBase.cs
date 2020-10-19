using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using ReactiveUI;
// ReSharper disable InconsistentNaming

namespace Playground.WpfApp.Forms.ReactiveEx
{
    /// <summary>
    /// Base implementation of a bindable object that implements the
    /// <see cref="INotifyDataErrorInfo" /> interface and supports validation through DataAnnotations.
    /// </summary>
    public abstract class ValidatableBindableBase : BindableBase, INotifyDataErrorInfo, IValidationExceptionHandler
    {
        private readonly ObservableAsPropertyHelper<IEnumerable<ValidationResult>> _allErrors;
        private readonly ObservableAsPropertyHelper<bool> _hasErrors;
        private ErrorsContainer<ValidationResult> _errorsContainer;
        private int _uiValidationErrorCount;

        protected ValidatableBindableBase()
        {
            // get all the properties that have the DoNotValidate attribute defined
            var fieldsThatDoNotCauseValidation = GetAllProperties(GetType())
                .Where(prop => prop.IsDefined(typeof(DoNotValidateAttribute), true))
                .Select(prop => prop.Name)
                .ToList();

            Changed
                .Where(_ => AreChangeNotificationsEnabled())
                .Select(args => args.PropertyName)
                .Where(prop => !fieldsThatDoNotCauseValidation.Any(field => field.Equals(prop, StringComparison.Ordinal)))
                .Subscribe(ValidateProperty)
                .DisposeWith(Disposables.Value);

            _hasErrors = this.WhenAnyValue(x => x.ErrorsContainer.HasErrors, x => x.UIValidationErrorCount,
                (hasError, uiErrorCount) => hasError || uiErrorCount > 0)
                .ToProperty(this, x => x.HasErrors, scheduler: Scheduler.Immediate)
                .DisposeWith(Disposables.Value);

            _allErrors = Observable
                .FromEventPattern<DataErrorsChangedEventArgs>(
                    h => ErrorsChanged += h,
                    h => ErrorsChanged -= h)
                .Select(_ => ErrorsContainer?.GetAllErrors() ?? Enumerable.Empty<ValidationResult>())
                .ToProperty(this, x => x.AllErrors, scheduler: Scheduler.Immediate)
                .DisposeWith(Disposables.Value);
        }

        /// <summary>
        /// Fires when the error state for the object has changed.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets all the errors associated with the object.
        /// </summary>
        [DoNotValidate]
        [DoesNotModifyEditState]
        public IEnumerable<ValidationResult> AllErrors => _allErrors?.Value ?? Enumerable.Empty<ValidationResult>();

        /// <summary>
        /// Gets a value indicating whether or not this object contains errors.
        /// </summary>
        [DoNotValidate]
        [DoesNotModifyEditState]
        public bool HasErrors => _hasErrors?.Value ?? false;

        /// <summary>
        /// Gets or sets the number of UI validation errors.
        /// </summary>
        public int UIValidationErrorCount
        {
            get => _uiValidationErrorCount;
            private set => this.RaiseAndSetIfChanged(ref _uiValidationErrorCount, value);
        }

        /// <summary>
        /// The container holding errors for the object.
        /// </summary>
        protected ErrorsContainer<ValidationResult> ErrorsContainer
        {
            get
            {
                if (_errorsContainer == null)
                {
                    _errorsContainer = new ErrorsContainer<ValidationResult>(RaiseErrorsChanged);
                    Disposables.Value.Add(_errorsContainer);
                    ValidateObject();
                }

                return _errorsContainer;
            }
        }

        /// <summary>
        /// Get the errors for the property specified.
        /// </summary>
        /// <param name="propertyName">The name of the property to get errors for.</param>
        /// <returns>The collection of errors for the property.</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            return ErrorsContainer.GetErrors(propertyName);
        }

        /// <summary>
        /// Performs validation for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        public void ValidateProperty(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                // if the property name is not provided, the entire object will be validated
                ValidateObject();
                return;
            }

            // validate the property provided
            ErrorsContainer.SetErrors(propertyName, DataAnnotationsValidationHelper.ValidateProperty(this, propertyName));
        }

        public void ValidationExceptionsChanged(int count)
        {
            UIValidationErrorCount = count;
        }

        /// <summary>
        /// Gets all the properties up the type hierarchy.
        /// </summary>
        /// <param name="t">The type to get properties for.</param>
        /// <returns>An enumerable containing properties defined on the object.</returns>
        protected static IEnumerable<PropertyInfo> GetAllProperties(Type t)
        {
            if (t == null)
            {
                return Enumerable.Empty<PropertyInfo>();
            }

            return t
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Concat(GetAllProperties(t.BaseType));
        }

        /// <summary>
        /// Fires the <see cref="ErrorsChanged" /> event.
        /// </summary>
        /// <param name="e">The argument containing event data.</param>
        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="ErrorsChanged" /> event.
        /// </summary>
        /// <param name="propertyName">The property for which the error state has changed.</param>
        protected void RaiseErrorsChanged(string propertyName)
        {
            OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Performs validation on the object.
        /// </summary>
        protected void ValidateObject()
        {
            ErrorsContainer.ClearAllErrors();

            var errors = from error in DataAnnotationsValidationHelper.ValidateObject(this)
                         from property in error.MemberNames
                         group error by property into g
                         select g;
            foreach (var property in errors)
            {
                ErrorsContainer.SetErrors(property.Key, property);
            }
        }
    }
}
