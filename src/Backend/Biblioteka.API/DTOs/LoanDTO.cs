namespace Biblioteka.API.DTOs;

public class LoanDTO
{
    public int Id { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int BookCopyId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
}

public class CreateLoanDTO
{
    public int UserId { get; set; }
    public int BookCopyId { get; set; }
    public int DaysToLoan { get; set; } = 14;
    public DateTime? DueDate { get; set; }
}

public class UpdateLoanDTO
{
    public DateTime? DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string? Status { get; set; }
}

