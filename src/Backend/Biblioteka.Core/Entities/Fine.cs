namespace Biblioteka.Core.Entities;

public class Fine
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty; // Overdue, Damage, Lost
    public DateTime IssueDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Paid
    
    // Foreign keys
    public int UserId { get; set; }
    public int? LoanId { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Loan? Loan { get; set; }
}

