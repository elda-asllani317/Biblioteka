namespace Biblioteka.API.DTOs;

public class FineDTO
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int? LoanId { get; set; }
}

public class CreateFineDTO
{
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? Status { get; set; }
    public int UserId { get; set; }
    public int? LoanId { get; set; }
}

public class UpdateFineDTO
{
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? PaymentDate { get; set; }
}


