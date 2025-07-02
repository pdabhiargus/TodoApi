using System.ComponentModel.DataAnnotations;
using TodoApi.Validation;

namespace TodoApi.Examples
{
    /// <summary>
    /// Example model demonstrating various validation scenarios
    /// This is a comprehensive example showing all validation types
    /// </summary>
    public class ValidationExampleModel
    {
        // Basic required field validation
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        // String length validation with minimum and maximum
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 50 characters")]
        public string Description { get; set; } = string.Empty;

        // Email validation
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        // Phone number validation
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? PhoneNumber { get; set; }

        // URL validation
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? Website { get; set; }

        // Range validation for numbers
        [Range(18, 65, ErrorMessage = "Age must be between 18 and 65")]
        public int Age { get; set; }

        // Decimal range validation
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between $0.01 and $999,999.99")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        // Regular expression validation
        [RegularExpression(@"^[A-Z]{2}\d{4}$", ErrorMessage = "Code must be 2 uppercase letters followed by 4 digits (e.g., AB1234)")]
        public string ProductCode { get; set; } = string.Empty;

        // Date validation with custom attribute
        [DataType(DataType.Date)]
        [PastDate(ErrorMessage = "Birth date must be in the past")]
        public DateTime BirthDate { get; set; }

        // Password with strong validation
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [StrongPassword(ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
        public string Password { get; set; } = string.Empty;

        // Confirm password
        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Credit card validation
        [CreditCard(ErrorMessage = "Please enter a valid credit card number")]
        public string? CreditCardNumber { get; set; }

        // Enum validation
        [Required(ErrorMessage = "Please select a category")]
        [EnumDataType(typeof(ProductCategory), ErrorMessage = "Please select a valid category")]
        public ProductCategory Category { get; set; }

        // Boolean validation (for terms acceptance)
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions")]
        public bool AcceptTerms { get; set; }

        // File extension validation (custom attribute)
        [AllowedExtensions(".jpg", ".jpeg", ".png", ".gif", ErrorMessage = "Only image files (.jpg, .jpeg, .png, .gif) are allowed")]
        public string? ProfilePicture { get; set; }

        // Minimum age validation based on birth date
        [MinimumAge(21, ErrorMessage = "You must be at least 21 years old")]
        public DateTime? MinimumAgeCheck { get; set; }
    }

    /// <summary>
    /// Example enumeration for validation
    /// </summary>
    public enum ProductCategory
    {
        Electronics = 1,
        Clothing = 2,
        Books = 3,
        Home = 4,
        Sports = 5
    }

    /// <summary>
    /// Example of a complex validation model using IValidatableObject
    /// This allows for custom business rules that span multiple properties
    /// </summary>
    public class ComplexValidationExample : IValidatableObject
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Budget must be a positive number")]
        public decimal Budget { get; set; }

        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100")]
        public int DiscountPercentage { get; set; }

        /// <summary>
        /// Custom validation logic
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Business rule: End date must be after start date
            if (EndDate <= StartDate)
            {
                results.Add(new ValidationResult(
                    "End date must be after start date",
                    new[] { nameof(EndDate) }));
            }

            // Business rule: Start date cannot be in the past
            if (StartDate.Date < DateTime.Today)
            {
                results.Add(new ValidationResult(
                    "Start date cannot be in the past",
                    new[] { nameof(StartDate) }));
            }

            // Business rule: High discount requires high budget
            if (DiscountPercentage > 50 && Budget < 10000)
            {
                results.Add(new ValidationResult(
                    "Discounts over 50% require a minimum budget of $10,000",
                    new[] { nameof(DiscountPercentage), nameof(Budget) }));
            }

            // Business rule: Email domain validation for corporate accounts
            if (!string.IsNullOrEmpty(Email) && Email.Contains("@"))
            {
                var domain = Email.Split('@')[1].ToLowerInvariant();
                var personalDomains = new[] { "gmail.com", "yahoo.com", "hotmail.com", "outlook.com" };
                
                if (personalDomains.Contains(domain) && Budget > 50000)
                {
                    results.Add(new ValidationResult(
                        "Corporate email address required for budgets over $50,000",
                        new[] { nameof(Email) }));
                }
            }

            // Business rule: Name validation
            if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
            {
                if (FirstName.Equals(LastName, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(new ValidationResult(
                        "First name and last name cannot be the same",
                        new[] { nameof(FirstName), nameof(LastName) }));
                }
            }

            return results;
        }
    }
}
