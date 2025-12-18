namespace Biblioteka.Core.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string Role { get; set; } = "User"; // Admin or User
    
    // Navigation properties
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();
}

