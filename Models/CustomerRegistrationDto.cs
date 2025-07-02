using System.ComponentModel.DataAnnotations;
using TodoApi.Validation;

namespace TodoApi.Models
{
    /// <summary>
    /// Customer registration DTO with comprehensive validation
    /// </summary>
    public class CustomerRegistrationDto : IValidatableObject
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 20 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 10 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [PastDate(ErrorMessage = "Date of birth must be in the past")]
        [MinimumAge(18, ErrorMessage = "You must be at least 18 years old to register")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StrongPassword]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions")]
        public bool AcceptTerms { get; set; }

        /// <summary>
        /// Custom validation logic using IValidatableObject
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Custom business rule: Email domain validation for corporate customers
            if (!string.IsNullOrEmpty(Email) && Email.Contains("@"))
            {
                var domain = Email.Split('@')[1].ToLowerInvariant();
                var restrictedDomains = new[] { "tempmail.com", "10minutemail.com", "guerrillamail.com" };
                
                if (restrictedDomains.Contains(domain))
                {
                    results.Add(new ValidationResult(
                        "Temporary email addresses are not allowed",
                        new[] { nameof(Email) }));
                }
            }

            // Custom business rule: Age and date consistency
            if (DateOfBirth != default)
            {
                var calculatedAge = DateTime.Today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > DateTime.Today.AddYears(-calculatedAge))
                    calculatedAge--;

                if (calculatedAge > 120)
                {
                    results.Add(new ValidationResult(
                        "Age cannot exceed 120 years",
                        new[] { nameof(DateOfBirth) }));
                }
            }

            // Custom business rule: Password cannot contain first or last name
            if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
            {
                if (Password.ToLowerInvariant().Contains(FirstName.ToLowerInvariant()) ||
                    Password.ToLowerInvariant().Contains(LastName.ToLowerInvariant()))
                {
                    results.Add(new ValidationResult(
                        "Password cannot contain your first or last name",
                        new[] { nameof(Password) }));
                }
            }

            return results;
        }
    }
}
