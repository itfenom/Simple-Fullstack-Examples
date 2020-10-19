using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Playground.WpfApp.Mvvm.AttributedValidation
{ 
    public class ExcludeChar : ValidationAttribute
    {
        private readonly string _characters;

        public ExcludeChar(string characters)
        {
            _characters = characters;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                for (int i = 0; i < _characters.Length; i++)
                {
                    var valueAsString = value.ToString();
                    if (valueAsString.Contains(_characters[i]))
                    {
                        var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(errorMessage, new List<string> { validationContext.DisplayName });
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}