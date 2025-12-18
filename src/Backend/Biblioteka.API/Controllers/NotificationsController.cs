using Biblioteka.API.DTOs;
using Biblioteka.Core.Entities;
using Biblioteka.Core.Interfaces;
using Biblioteka.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationsController(INotificationService notificationService, IUnitOfWork unitOfWork)
    {
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetNotificationsByUser(int userId, [FromQuery] bool unreadOnly = false)
    {
        IEnumerable<Notification> notifications;

        if (unreadOnly)
        {
            notifications = await _notificationService.GetUnreadNotificationsByUserIdAsync(userId);
        }
        else
        {
            notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);
        }

        var notificationsDto = new List<NotificationDTO>();
        foreach (var notification in notifications)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(notification.UserId);
            notificationsDto.Add(new NotificationDTO
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                CreatedDate = notification.CreatedDate,
                IsRead = notification.IsRead,
                UserId = notification.UserId,
                UserName = $"{user?.FirstName} {user?.LastName}"
            });
        }

        return Ok(notificationsDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NotificationDTO>> GetNotification(int id)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(id);
        if (notification == null)
            return NotFound(new { message = "Njoftimi nuk u gjet." });

        var user = await _unitOfWork.Users.GetByIdAsync(notification.UserId);
        var notificationDto = new NotificationDTO
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type,
            CreatedDate = notification.CreatedDate,
            IsRead = notification.IsRead,
            UserId = notification.UserId,
            UserName = $"{user?.FirstName} {user?.LastName}"
        };

        return Ok(notificationDto);
    }

    [HttpPost]
    public async Task<ActionResult<NotificationDTO>> CreateNotification([FromBody] CreateNotificationDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var notification = new Notification
            {
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type,
                UserId = dto.UserId,
                CreatedDate = DateTime.Now,
                IsRead = false
            };

            var created = await _notificationService.CreateNotificationAsync(notification);

            var user = await _unitOfWork.Users.GetByIdAsync(created.UserId);
            var notificationDto = new NotificationDTO
            {
                Id = created.Id,
                Title = created.Title,
                Message = created.Message,
                Type = created.Type,
                CreatedDate = created.CreatedDate,
                IsRead = created.IsRead,
                UserId = created.UserId,
                UserName = $"{user?.FirstName} {user?.LastName}"
            };

            return CreatedAtAction(nameof(GetNotification), new { id = notificationDto.Id }, notificationDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNotification(int id, [FromBody] UpdateNotificationDTO dto)
    {
        try
        {
            var existing = await _notificationService.GetNotificationByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Njoftimi nuk u gjet." });

            if (dto.Title != null) existing.Title = dto.Title;
            if (dto.Message != null) existing.Message = dto.Message;
            if (dto.Type != null) existing.Type = dto.Type;
            if (dto.IsRead.HasValue) existing.IsRead = dto.IsRead.Value;

            await _notificationService.UpdateNotificationAsync(existing);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/mark-read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        try
        {
            await _notificationService.MarkAsReadAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        try
        {
            await _notificationService.DeleteNotificationAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

