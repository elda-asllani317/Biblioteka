using System.ComponentModel.DataAnnotations;

namespace Biblioteka.API.DTOs;

public class ReviewDTO
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime ReviewDate { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
}

public class CreateReviewDTO
{
    [Required(ErrorMessage = "Rating është i detyrueshëm")]
    [Range(1, 5, ErrorMessage = "Rating duhet të jetë midis 1 dhe 5")]
    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    [Required(ErrorMessage = "UserId është i detyrueshëm")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "BookId është i detyrueshëm")]
    public int BookId { get; set; }
}

public class UpdateReviewDTO
{
    [Range(1, 5, ErrorMessage = "Rating duhet të jetë midis 1 dhe 5")]
    public int? Rating { get; set; }

    public string? Comment { get; set; }
}

