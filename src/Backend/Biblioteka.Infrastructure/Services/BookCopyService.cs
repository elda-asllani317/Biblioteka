using Biblioteka.Core.Entities;
using Biblioteka.Core.Interfaces;
using Biblioteka.Core.Services;

namespace Biblioteka.Infrastructure.Services;

public class BookCopyService : IBookCopyService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookCopyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BookCopy>> GetAllAsync()
    {
        return await _unitOfWork.BookCopies.GetAllAsync();
    }

    public async Task<BookCopy?> GetByIdAsync(int id)
    {
        return await _unitOfWork.BookCopies.GetByIdAsync(id);
    }

    public async Task<IEnumerable<BookCopy>> GetByBookIdAsync(int bookId)
    {
        return await _unitOfWork.BookCopies.FindAsync(bc => bc.BookId == bookId);
    }

    public async Task<BookCopy> CreateAsync(BookCopy copy)
    {
        // Validim që libri ekziston
        var book = await _unitOfWork.Books.GetByIdAsync(copy.BookId);
        if (book == null)
            throw new ArgumentException("Book not found");

        // CopyNumber unik për BookId
        var exists = await _unitOfWork.BookCopies.ExistsAsync(c =>
            c.BookId == copy.BookId && c.CopyNumber == copy.CopyNumber);
        if (exists)
            throw new ArgumentException("CopyNumber duhet të jetë unik për librin e zgjedhur");

        copy.IsAvailable = true;

        await _unitOfWork.BookCopies.AddAsync(copy);
        await _unitOfWork.SaveChangesAsync();

        return copy;
    }

    public async Task UpdateAsync(BookCopy copy)
    {
        // Validim i CopyNumber për unikësi në librin përkatës
        var exists = await _unitOfWork.BookCopies.ExistsAsync(c =>
            c.BookId == copy.BookId &&
            c.CopyNumber == copy.CopyNumber &&
            c.Id != copy.Id);
        if (exists)
            throw new ArgumentException("CopyNumber duhet të jetë unik për librin e zgjedhur");

        await _unitOfWork.BookCopies.UpdateAsync(copy);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var copy = await _unitOfWork.BookCopies.GetByIdAsync(id);
        if (copy == null)
            return;

        // Mos lejo fshirjen nëse ka huazime aktive
        var hasActiveLoans = await _unitOfWork.Loans.ExistsAsync(l =>
            l.BookCopyId == id && l.Status == "Active");
        if (hasActiveLoans)
            throw new InvalidOperationException("Nuk mund të fshihet një kopje me huazime aktive.");

        await _unitOfWork.BookCopies.DeleteAsync(copy);
        await _unitOfWork.SaveChangesAsync();
    }
}


