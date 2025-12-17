namespace Biblioteka.API.DTOs;

public class BookCopyDTO
{
    public int Id { get; set; }
    public string CopyNumber { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
}


