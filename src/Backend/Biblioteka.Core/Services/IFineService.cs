using Biblioteka.Core.Entities;

namespace Biblioteka.Core.Services;

public interface IFineService
{
    Task<IEnumerable<Fine>> GetAllAsync();
    Task<Fine?> GetByIdAsync(int id);
    Task<IEnumerable<Fine>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Fine>> GetByStatusAsync(string status);
    Task<Fine> CreateAsync(Fine fine);
    Task UpdateAsync(Fine fine);
    Task DeleteAsync(int id);
}


