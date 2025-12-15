namespace Biblioteka.Core.Entities;

public class Publisher
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}

