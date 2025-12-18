using Biblioteka.Core.Entities;
using Biblioteka.Core.Interfaces;
using Biblioteka.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Infrastructure.Services;

public class PublisherService : IPublisherService
{
    private readonly IUnitOfWork _unitOfWork;

    public PublisherService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Publisher>> GetAllPublishersAsync()
    {
        return await _unitOfWork.Publishers.GetAllAsync();
    }

    public async Task<Publisher?> GetPublisherByIdAsync(int id)
    {
        return await _unitOfWork.Publishers.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Publisher>> SearchPublishersByNameAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllPublishersAsync();

        var publishers = await _unitOfWork.Publishers.FindAsync(
            p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        );
        return publishers;
    }

    public async Task<Publisher> CreatePublisherAsync(Publisher publisher)
    {
        // Validate that Name and Email are unique
        var existingByName = await _unitOfWork.Publishers.FirstOrDefaultAsync(
            p => p.Name.ToLower() == publisher.Name.ToLower()
        );
        if (existingByName != null)
        {
            throw new InvalidOperationException($"Një botues me emrin '{publisher.Name}' ekziston tashmë.");
        }

        var existingByEmail = await _unitOfWork.Publishers.FirstOrDefaultAsync(
            p => p.Email.ToLower() == publisher.Email.ToLower()
        );
        if (existingByEmail != null)
        {
            throw new InvalidOperationException($"Një botues me email-in '{publisher.Email}' ekziston tashmë.");
        }

        await _unitOfWork.Publishers.AddAsync(publisher);
        await _unitOfWork.SaveChangesAsync();
        return publisher;
    }

    public async Task UpdatePublisherAsync(Publisher publisher)
    {
        // Validate that Name and Email are unique (excluding current publisher)
        var existingByName = await _unitOfWork.Publishers.FirstOrDefaultAsync(
            p => p.Name.ToLower() == publisher.Name.ToLower() && p.Id != publisher.Id
        );
        if (existingByName != null)
        {
            throw new InvalidOperationException($"Një botues me emrin '{publisher.Name}' ekziston tashmë.");
        }

        var existingByEmail = await _unitOfWork.Publishers.FirstOrDefaultAsync(
            p => p.Email.ToLower() == publisher.Email.ToLower() && p.Id != publisher.Id
        );
        if (existingByEmail != null)
        {
            throw new InvalidOperationException($"Një botues me email-in '{publisher.Email}' ekziston tashmë.");
        }

        await _unitOfWork.Publishers.UpdateAsync(publisher);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeletePublisherAsync(int id)
    {
        var publisher = await _unitOfWork.Publishers.GetByIdAsync(id);
        if (publisher == null)
        {
            throw new InvalidOperationException("Botuesi nuk u gjet.");
        }

        // Check if publisher has books
        var hasBooks = await HasBooksAsync(id);
        if (hasBooks)
        {
            throw new InvalidOperationException("Nuk mund të fshihet botuesi sepse ka libra të lidhur me të.");
        }

        await _unitOfWork.Publishers.DeleteAsync(publisher);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> HasBooksAsync(int publisherId)
    {
        var books = await _unitOfWork.Books.FindAsync(b => b.PublisherId == publisherId);
        return books.Any();
    }

    public async Task<int> GetBookCountAsync(int publisherId)
    {
        var books = await _unitOfWork.Books.FindAsync(b => b.PublisherId == publisherId);
        return books.Count();
    }
}

