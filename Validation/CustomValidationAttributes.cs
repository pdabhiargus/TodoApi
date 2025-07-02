using System.ComponentModel.DataAnnotations;

namespace TodoApi.Validation
{
    /// <summary>
    /// Custom validation attribute to ensure a date is in the past
    /// </summary>
    public class PastDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true; // Allow null values, use [Required] for mandatory fields

            if (value is DateTime dateValue)
            {
                return dateValue.Date < DateTime.Today;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field must be a date in the past.";
        }
    }

    /// <summary>
    /// Custom validation attribute for strong password requirements
    /// </summary>
    public class StrongPasswordAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true; // Allow null values, use [Required] for mandatory fields

            var password = value.ToString();
            if (string.IsNullOrEmpty(password))
                return true; // Let [Required] handle empty strings

            // Check for at least one uppercase letter
            bool hasUpperCase = password.Any(char.IsUpper);
            
            // Check for at least one lowercase letter
            bool hasLowerCase = password.Any(char.IsLower);
            
            // Check for at least one digit
            bool hasDigit = password.Any(char.IsDigit);
            
            // Check for at least one special character
            bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";
        }
    }

    /// <summary>
    /// Custom validation attribute to validate age based on date of birth
    /// </summary>
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
                return true; // Allow null values

            if (value is DateTime dateOfBirth)
            {
                var age = DateTime.Today.Year - dateOfBirth.Year;
                if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
                    age--;

                return age >= _minimumAge;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"You must be at least {_minimumAge} years old.";
        }
    }

    /// <summary>
    /// Custom validation attribute for validating file extensions
    /// </summary>
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(params string[] extensions)
        {
            _extensions = extensions;
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            var fileName = value.ToString();
            if (string.IsNullOrEmpty(fileName))
                return true;

            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
            return _extensions.Contains(extension);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field only accepts files with the following extensions: {string.Join(", ", _extensions)}.";
        }
    }
}
