using System.ComponentModel.DataAnnotations;
using TodoApi.Validation;

namespace TodoApi.Models
{
    /// <summary>
    /// Customer model with comprehensive validation attributes
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }

        /// <summary>
        /// First name of the customer (Required, max 20 characters)
        /// </summary>
        [Required(ErrorMessage = "First name is required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 20 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name can only contain letters and spaces")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Last name of the customer (Required, max 10 characters)
        /// </summary>
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 10 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name can only contain letters and spaces")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Email address of the customer (Required, valid email format)
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Customer age (Range validation: 18-120)
        /// </summary>
        [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
        public int Age { get; set; }

        /// <summary>
        /// Phone number with custom validation
        /// </summary>
        [Phone(ErrorMessage = "Invalid phone number format")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Phone number must be in international format (e.g., +1234567890)")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Website URL (optional)
        /// </summary>
        [Url(ErrorMessage = "Invalid URL format")]
        public string? Website { get; set; }

        /// <summary>
        /// Date of birth with custom validation
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [PastDate(ErrorMessage = "Date of birth must be in the past")]
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Salary with range validation
        /// </summary>
        [Range(0.01, double.MaxValue, ErrorMessage = "Salary must be greater than 0")]
        [DataType(DataType.Currency)]
        public decimal? Salary { get; set; }

        /// <summary>
        /// Password with custom validation
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [StrongPassword(ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Confirm password (must match password)
        /// </summary>
        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Credit card number (optional, with validation)
        /// </summary>
        [CreditCard(ErrorMessage = "Invalid credit card number")]
        public string? CreditCardNumber { get; set; }

        /// <summary>
        /// Customer type with enum validation
        /// </summary>
        [Required(ErrorMessage = "Customer type is required")]
        [EnumDataType(typeof(CustomerType), ErrorMessage = "Invalid customer type")]
        public CustomerType CustomerType { get; set; }

        /// <summary>
        /// Terms and conditions acceptance
        /// </summary>
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions")]
        public bool AcceptTerms { get; set; }
    }

    /// <summary>
    /// Customer type enumeration
    /// </summary>
    public enum CustomerType
    {
        Regular = 1,
        Premium = 2,
        VIP = 3,
        Corporate = 4
    }
}
