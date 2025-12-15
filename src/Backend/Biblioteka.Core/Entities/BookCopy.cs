namespace Biblioteka.Core.Entities;

public class BookCopy
{
    public int Id { get; set; }
    public string CopyNumber { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public string Condition { get; set; } = string.Empty; // New, Good, Fair, Poor
    public DateTime PurchaseDate { get; set; }
    
    // Foreign key
    public int BookId { get; set; }
    
    // Navigation properties
    public virtual Book Book { get; set; } = null!;
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}

