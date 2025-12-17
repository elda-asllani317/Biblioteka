using Biblioteka.Core.Entities;

namespace Biblioteka.Core.Services;

public interface IAuthorService
{
    Task<IEnumerable<Author>> GetAllAuthorsAsync();
    Task<Author?> GetAuthorByIdAsync(int id);
    Task<Author> CreateAuthorAsync(Author author);
    Task UpdateAuthorAsync(Author author);
    Task DeleteAuthorAsync(int id);
}


