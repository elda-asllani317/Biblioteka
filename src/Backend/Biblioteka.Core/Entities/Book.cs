namespace Biblioteka.Core.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public int Pages { get; set; }
    public string Language { get; set; } = string.Empty;
    
    // Foreign keys
    public int AuthorId { get; set; }
    public int CategoryId { get; set; }
    public int PublisherId { get; set; }
    
    // Navigation properties
    public virtual Author Author { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
    public virtual Publisher Publisher { get; set; } = null!;
    public virtual ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}

