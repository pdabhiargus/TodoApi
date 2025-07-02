using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ILogger<CustomerController> _logger;
    private static readonly List<Customer> _customers = new();
    private static int _nextId = 1;

    public CustomerController(ILogger<CustomerController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all customers
    /// </summary>
    /// <returns>List of customers</returns>
    [HttpGet]
    public ActionResult<IEnumerable<Customer>> GetCustomers()
    {
        _logger.LogInformation("Retrieved all customers");
        return Ok(_customers);
    }

    /// <summary>
    /// Get a customer by ID
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Customer object</returns>
    [HttpGet("{id}")]
    public ActionResult<Customer> GetCustomer(int id)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {Id} not found", id);
            return NotFound($"Customer with ID {id} not found");
        }

        _logger.LogInformation("Retrieved customer with ID {Id}", id);
        return Ok(customer);
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    /// <param name="customer">Customer object to create</param>
    /// <returns>Created customer</returns>
    [HttpPost]
    public ActionResult<Customer> CreateCustomer([FromBody] Customer customer)
    {
        // Model validation is automatically handled by ASP.NET Core
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid customer data submitted");
            return BadRequest(ModelState);
        }

        // Check if email already exists
        if (_customers.Any(c => c.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Customer with email {Email} already exists", customer.Email);
            return Conflict($"Customer with email {customer.Email} already exists");
        }

        customer.Id = _nextId++;
        _customers.Add(customer);

        _logger.LogInformation("Created new customer with ID {Id}", customer.Id);
        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
    }

    /// <summary>
    /// Update an existing customer
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="customer">Updated customer data</param>
    /// <returns>Updated customer</returns>
    [HttpPut("{id}")]
    public ActionResult<Customer> UpdateCustomer(int id, [FromBody] Customer customer)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid customer data submitted for update");
            return BadRequest(ModelState);
        }

        var existingCustomer = _customers.FirstOrDefault(c => c.Id == id);
        if (existingCustomer == null)
        {
            _logger.LogWarning("Customer with ID {Id} not found for update", id);
            return NotFound($"Customer with ID {id} not found");
        }

        // Check if email already exists for another customer
        if (_customers.Any(c => c.Id != id && c.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Customer with email {Email} already exists", customer.Email);
            return Conflict($"Customer with email {customer.Email} already exists");
        }

        existingCustomer.FirstName = customer.FirstName;
        existingCustomer.LastName = customer.LastName;
        existingCustomer.Email = customer.Email;

        _logger.LogInformation("Updated customer with ID {Id}", id);
        return Ok(existingCustomer);
    }

    /// <summary>
    /// Delete a customer
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    public ActionResult DeleteCustomer(int id)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {Id} not found for deletion", id);
            return NotFound($"Customer with ID {id} not found");
        }

        _customers.Remove(customer);
        _logger.LogInformation("Deleted customer with ID {Id}", id);
        return NoContent();
    }

    /// <summary>
    /// Register a new customer with comprehensive validation
    /// </summary>
    /// <param name="registrationDto">Customer registration data</param>
    /// <returns>Created customer</returns>
    [HttpPost("register")]
    public ActionResult<Customer> RegisterCustomer([FromBody] CustomerRegistrationDto registrationDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid customer registration data submitted");
            return BadRequest(ModelState);
        }

        // Check if email already exists
        if (_customers.Any(c => c.Email.Equals(registrationDto.Email, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Customer with email {Email} already exists", registrationDto.Email);
            return Conflict($"Customer with email {registrationDto.Email} already exists");
        }

        // Map DTO to Customer entity
        var customer = new Customer
        {
            Id = _nextId++,
            FirstName = registrationDto.FirstName,
            LastName = registrationDto.LastName,
            Email = registrationDto.Email,
            DateOfBirth = registrationDto.DateOfBirth,
            Password = registrationDto.Password, // In real apps, hash the password!
            ConfirmPassword = registrationDto.ConfirmPassword,
            AcceptTerms = registrationDto.AcceptTerms,
            Age = DateTime.Today.Year - registrationDto.DateOfBirth.Year,
            CustomerType = CustomerType.Regular // Default type
        };

        // Adjust age if birthday hasn't occurred this year
        if (registrationDto.DateOfBirth.Date > DateTime.Today.AddYears(-customer.Age))
            customer.Age--;

        _customers.Add(customer);

        _logger.LogInformation("Registered new customer with ID {Id}", customer.Id);
        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
    }
}
