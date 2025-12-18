using Biblioteka.Core.Entities;
using Biblioteka.Core.Interfaces;
using Biblioteka.Core.Services;

namespace Biblioteka.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId)
    {
        var notifications = await _unitOfWork.Notifications.FindAsync(n => n.UserId == userId);
        return notifications.OrderByDescending(n => n.CreatedDate);
    }

    public async Task<IEnumerable<Notification>> GetUnreadNotificationsByUserIdAsync(int userId)
    {
        var notifications = await _unitOfWork.Notifications.FindAsync(
            n => n.UserId == userId && !n.IsRead
        );
        return notifications.OrderByDescending(n => n.CreatedDate);
    }

    public async Task<Notification?> GetNotificationByIdAsync(int id)
    {
        return await _unitOfWork.Notifications.GetByIdAsync(id);
    }

    public async Task<Notification> CreateNotificationAsync(Notification notification)
    {
        // Validate UserId exists
        var user = await _unitOfWork.Users.GetByIdAsync(notification.UserId);
        if (user == null)
        {
            throw new InvalidOperationException($"Përdoruesi me ID {notification.UserId} nuk ekziston.");
        }

        // Set CreatedDate if not set
        if (notification.CreatedDate == default)
        {
            notification.CreatedDate = DateTime.Now;
        }

        // Ensure IsRead is false by default
        notification.IsRead = false;

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();
        return notification;
    }

    public async Task UpdateNotificationAsync(Notification notification)
    {
        var existing = await _unitOfWork.Notifications.GetByIdAsync(notification.Id);
        if (existing == null)
        {
            throw new InvalidOperationException("Njoftimi nuk u gjet.");
        }

        existing.Title = notification.Title;
        existing.Message = notification.Message;
        existing.Type = notification.Type;
        existing.IsRead = notification.IsRead;

        await _unitOfWork.Notifications.UpdateAsync(existing);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task MarkAsReadAsync(int id)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
        if (notification == null)
        {
            throw new InvalidOperationException("Njoftimi nuk u gjet.");
        }

        notification.IsRead = true;
        await _unitOfWork.Notifications.UpdateAsync(notification);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteNotificationAsync(int id)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
        if (notification == null)
        {
            throw new InvalidOperationException("Njoftimi nuk u gjet.");
        }

        await _unitOfWork.Notifications.DeleteAsync(notification);
        await _unitOfWork.SaveChangesAsync();
    }
}

