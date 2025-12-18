using Biblioteka.Core.Entities;
using Biblioteka.Core.Interfaces;
using Biblioteka.Core.Services;

namespace Biblioteka.Infrastructure.Services;

public class FineService : IFineService
{
    private readonly IUnitOfWork _unitOfWork;

    public FineService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Fine>> GetAllAsync()
    {
        return await _unitOfWork.Fines.GetAllAsync();
    }

    public async Task<Fine?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Fines.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Fine>> GetByUserIdAsync(int userId)
    {
        return await _unitOfWork.Fines.FindAsync(f => f.UserId == userId);
    }

    public async Task<IEnumerable<Fine>> GetByStatusAsync(string status)
    {
        status = status.Trim();
        return await _unitOfWork.Fines.FindAsync(f => f.Status == status);
    }

    public async Task<Fine> CreateAsync(Fine fine)
    {
        if (fine.Amount <= 0)
            throw new ArgumentException("Amount duhet të jetë më i madh se 0.");

        // Validim User
        var userExists = await _unitOfWork.Users.ExistsAsync(u => u.Id == fine.UserId);
        if (!userExists)
            throw new ArgumentException("UserId nuk ekziston.");

        // Validim Loan (nëse ka)
        if (fine.LoanId.HasValue)
        {
            var loanExists = await _unitOfWork.Loans.ExistsAsync(l => l.Id == fine.LoanId.Value);
            if (!loanExists)
                throw new ArgumentException("LoanId nuk ekziston.");
        }

        if (fine.IssueDate == default)
            fine.IssueDate = DateTime.Now;

        if (string.IsNullOrWhiteSpace(fine.Status))
            fine.Status = "Pending";

        await _unitOfWork.Fines.AddAsync(fine);
        await _unitOfWork.SaveChangesAsync();

        return fine;
    }

    public async Task UpdateAsync(Fine fine)
    {
        if (fine.Amount <= 0)
            throw new ArgumentException("Amount duhet të jetë më i madh se 0.");

        if (fine.Status == "Paid" && !fine.PaymentDate.HasValue)
            throw new ArgumentException("PaymentDate duhet të vendoset kur statusi është 'Paid'.");

        await _unitOfWork.Fines.UpdateAsync(fine);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var fine = await _unitOfWork.Fines.GetByIdAsync(id);
        if (fine == null)
            return;

        await _unitOfWork.Fines.DeleteAsync(fine);
        await _unitOfWork.SaveChangesAsync();
    }
}


