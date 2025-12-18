using Biblioteka.Core.Entities;

namespace Biblioteka.Core.Services;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId);
    Task<IEnumerable<Notification>> GetUnreadNotificationsByUserIdAsync(int userId);
    Task<Notification?> GetNotificationByIdAsync(int id);
    Task<Notification> CreateNotificationAsync(Notification notification);
    Task UpdateNotificationAsync(Notification notification);
    Task MarkAsReadAsync(int id);
    Task DeleteNotificationAsync(int id);
}

