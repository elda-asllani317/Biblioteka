using Biblioteka.Core.Entities;

namespace Biblioteka.Core.Services;

public interface ILoanService
{
    Task<Loan> CreateLoanAsync(int userId, int bookCopyId, int daysToLoan);
    Task<Loan?> GetLoanByIdAsync(int id);
    Task<IEnumerable<Loan>> GetUserLoansAsync(int userId);
    Task ReturnLoanAsync(int loanId);
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
}

