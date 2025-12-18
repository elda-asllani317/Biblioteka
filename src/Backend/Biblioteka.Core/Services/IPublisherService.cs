using Biblioteka.Core.Entities;

namespace Biblioteka.Core.Services;

public interface IPublisherService
{
    Task<IEnumerable<Publisher>> GetAllPublishersAsync();
    Task<Publisher?> GetPublisherByIdAsync(int id);
    Task<IEnumerable<Publisher>> SearchPublishersByNameAsync(string searchTerm);
    Task<Publisher> CreatePublisherAsync(Publisher publisher);
    Task UpdatePublisherAsync(Publisher publisher);
    Task DeletePublisherAsync(int id);
    Task<bool> HasBooksAsync(int publisherId);
    Task<int> GetBookCountAsync(int publisherId);
}

