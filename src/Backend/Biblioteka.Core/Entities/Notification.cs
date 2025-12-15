namespace Biblioteka.Core.Entities;

public class Notification
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Info, Warning, Alert
    public DateTime CreatedDate { get; set; }
    public bool IsRead { get; set; } = false;
    
    // Foreign key
    public int UserId { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
}

