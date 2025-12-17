using Biblioteka.Core.Entities;
using Biblioteka.Core.Interfaces;
using Biblioteka.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Infrastructure.Services;

public class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _unitOfWork.Books.GetAllAsync();
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _unitOfWork.Books.GetByIdAsync(id);
    }

    public async Task<Book> CreateBookAsync(Book book)
    {
        // Së pari krijojmë librin
        await _unitOfWork.Books.AddAsync(book);
        await _unitOfWork.SaveChangesAsync();

        // Pastaj krijojmë automatikisht një kopje fizike të librit
        var copy = new BookCopy
        {
            BookId = book.Id,
            CopyNumber = $"COPY-{book.Id}-1",
            IsAvailable = true,
            Condition = "New",
            PurchaseDate = DateTime.Now
        };

        await _unitOfWork.BookCopies.AddAsync(copy);
        await _unitOfWork.SaveChangesAsync();

        return book;
    }

    public async Task UpdateBookAsync(Book book)
    {
        await _unitOfWork.Books.UpdateAsync(book);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(id);
        if (book != null)
        {
            await _unitOfWork.Books.DeleteAsync(book);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
    {
        var allBooks = await _unitOfWork.Books.GetAllAsync();
        return allBooks.Where(b => 
            b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            b.ISBN.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            b.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        );
    }
}

