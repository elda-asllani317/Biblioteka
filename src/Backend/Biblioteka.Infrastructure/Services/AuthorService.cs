using Biblioteka.Core.Entities;
using Biblioteka.Core.Interfaces;
using Biblioteka.Core.Services;

namespace Biblioteka.Infrastructure.Services;

public class AuthorService : IAuthorService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
    {
        return await _unitOfWork.Authors.GetAllAsync();
    }

    public async Task<Author?> GetAuthorByIdAsync(int id)
    {
        return await _unitOfWork.Authors.GetByIdAsync(id);
    }

    public async Task<Author> CreateAuthorAsync(Author author)
    {
        await _unitOfWork.Authors.AddAsync(author);
        await _unitOfWork.SaveChangesAsync();
        return author;
    }

    public async Task UpdateAuthorAsync(Author author)
    {
        await _unitOfWork.Authors.UpdateAsync(author);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAuthorAsync(int id)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(id);
        if (author != null)
        {
            await _unitOfWork.Authors.DeleteAsync(author);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}


