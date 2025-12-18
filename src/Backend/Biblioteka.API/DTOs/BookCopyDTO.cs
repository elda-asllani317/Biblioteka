namespace Biblioteka.API.DTOs;

public class BookCopyDTO
{
    public int Id { get; set; }
    public string CopyNumber { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public string Condition { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
}

public class CreateBookCopyDTO
{
    public string CopyNumber { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public int BookId { get; set; }
}

public class UpdateBookCopyDTO
{
    public string Condition { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public bool IsAvailable { get; set; }
}


