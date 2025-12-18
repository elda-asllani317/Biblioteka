using System.ComponentModel.DataAnnotations;

namespace Biblioteka.API.DTOs;

public class NotificationDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsRead { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public class CreateNotificationDTO
{
    [Required(ErrorMessage = "Title është i detyrueshëm")]
    [StringLength(200, ErrorMessage = "Title nuk mund të jetë më i gjatë se 200 karaktere")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Message është i detyrueshëm")]
    public string Message { get; set; } = string.Empty;

    [Required(ErrorMessage = "Type është i detyrueshëm")]
    public string Type { get; set; } = "Info"; // Default to Info

    [Required(ErrorMessage = "UserId është i detyrueshëm")]
    public int UserId { get; set; }
}

public class UpdateNotificationDTO
{
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string? Type { get; set; }
    public bool? IsRead { get; set; }
}

