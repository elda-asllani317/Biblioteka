namespace Biblioteka.Core.Entities;

public class Loan
{
    public int Id { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = "Active"; // Active, Returned, Overdue
    
    // Foreign keys
    public int UserId { get; set; }
    public int BookCopyId { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual BookCopy BookCopy { get; set; } = null!;
    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();
}

