using Microsoft.EntityFrameworkCore;

// Department entity representing a department table
public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
    // Navigation property for users in this department
    public ICollection<User> Users { get; set; }
}

// User entity representing a table in the database
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    // Foreign key for Department
    public int DepartmentId { get; set; }
    // Navigation property
    public Department Department { get; set; }
}

// DbContext for managing User entities
public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Department> Departments { get; set; }
}
