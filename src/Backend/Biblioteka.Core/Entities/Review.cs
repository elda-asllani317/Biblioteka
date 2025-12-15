namespace Biblioteka.Core.Entities;

public class Review
{
    public int Id { get; set; }
    public int Rating { get; set; } // 1-5
    public string Comment { get; set; } = string.Empty;
    public DateTime ReviewDate { get; set; }
    
    // Foreign keys
    public int UserId { get; set; }
    public int BookId { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Book Book { get; set; } = null!;
}

