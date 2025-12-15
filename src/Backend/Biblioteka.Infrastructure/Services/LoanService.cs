using Biblioteka.Core.Entities;
using Biblioteka.Core.Interfaces;
using Biblioteka.Core.Services;

namespace Biblioteka.Infrastructure.Services;

public class LoanService : ILoanService
{
    private readonly IUnitOfWork _unitOfWork;

    public LoanService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Loan> CreateLoanAsync(int userId, int bookCopyId, int daysToLoan)
    {
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(bookCopyId);
            if (bookCopy == null)
                throw new ArgumentException("Book copy not found");

            if (!bookCopy.IsAvailable)
                throw new InvalidOperationException("Book copy is not available");

            var loan = new Loan
            {
                UserId = userId,
                BookCopyId = bookCopyId,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(daysToLoan),
                Status = "Active"
            };

            bookCopy.IsAvailable = false;
            await _unitOfWork.BookCopies.UpdateAsync(bookCopy);
            await _unitOfWork.Loans.AddAsync(loan);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return loan;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<Loan?> GetLoanByIdAsync(int id)
    {
        return await _unitOfWork.Loans.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Loan>> GetUserLoansAsync(int userId)
    {
        return await _unitOfWork.Loans.FindAsync(l => l.UserId == userId);
    }

    public async Task ReturnLoanAsync(int loanId)
    {
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var loan = await _unitOfWork.Loans.GetByIdAsync(loanId);
            if (loan == null)
                throw new ArgumentException("Loan not found");

            loan.ReturnDate = DateTime.Now;
            loan.Status = "Returned";

            var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(loan.BookCopyId);
            if (bookCopy != null)
            {
                bookCopy.IsAvailable = true;
                await _unitOfWork.BookCopies.UpdateAsync(bookCopy);
            }

            // Check if overdue and create fine
            if (loan.ReturnDate > loan.DueDate)
            {
                var daysOverdue = (loan.ReturnDate.Value - loan.DueDate).Days;
                var fine = new Fine
                {
                    UserId = loan.UserId,
                    LoanId = loanId,
                    Amount = daysOverdue * 5, // 5 per day
                    Reason = "Overdue",
                    IssueDate = DateTime.Now,
                    Status = "Pending"
                };
                await _unitOfWork.Fines.AddAsync(fine);
            }

            await _unitOfWork.Loans.UpdateAsync(loan);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
    {
        var allLoans = await _unitOfWork.Loans.GetAllAsync();
        return allLoans.Where(l => 
            l.Status == "Active" && 
            l.DueDate < DateTime.Now
        );
    }
}

