namespace Biblioteka.Core.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}

