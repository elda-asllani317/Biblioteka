using Biblioteka.Core.Entities;

namespace Biblioteka.Core.Services;

public interface IBookCopyService
{
    Task<IEnumerable<BookCopy>> GetAllAsync();
    Task<BookCopy?> GetByIdAsync(int id);
    Task<IEnumerable<BookCopy>> GetByBookIdAsync(int bookId);
    Task<BookCopy> CreateAsync(BookCopy copy);
    Task UpdateAsync(BookCopy copy);
    Task DeleteAsync(int id);
}


