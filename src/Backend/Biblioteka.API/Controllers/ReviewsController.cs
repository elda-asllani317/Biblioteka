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
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly IUnitOfWork _unitOfWork;

    public ReviewsController(IReviewService reviewService, IUnitOfWork unitOfWork)
    {
        _reviewService = reviewService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("book/{bookId}")]
    public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviewsByBook(int bookId)
    {
        var reviews = await _reviewService.GetReviewsByBookIdAsync(bookId);
        var reviewsDto = new List<ReviewDTO>();

        foreach (var review in reviews)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(review.UserId);
            var book = await _unitOfWork.Books.GetByIdAsync(review.BookId);
            reviewsDto.Add(new ReviewDTO
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.ReviewDate,
                UserId = review.UserId,
                UserName = $"{user?.FirstName} {user?.LastName}",
                BookId = review.BookId,
                BookTitle = book?.Title ?? ""
            });
        }

        return Ok(reviewsDto);
    }

    [HttpGet("book/{bookId}/average")]
    public async Task<ActionResult<object>> GetAverageRating(int bookId)
    {
        var averageRating = await _reviewService.GetAverageRatingByBookIdAsync(bookId);
        return Ok(new { bookId, averageRating });
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviewsByUser(int userId)
    {
        var reviews = await _reviewService.GetReviewsByUserIdAsync(userId);
        var reviewsDto = new List<ReviewDTO>();

        foreach (var review in reviews)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(review.UserId);
            var book = await _unitOfWork.Books.GetByIdAsync(review.BookId);
            reviewsDto.Add(new ReviewDTO
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.ReviewDate,
                UserId = review.UserId,
                UserName = $"{user?.FirstName} {user?.LastName}",
                BookId = review.BookId,
                BookTitle = book?.Title ?? ""
            });
        }

        return Ok(reviewsDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDTO>> GetReview(int id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        if (review == null)
            return NotFound(new { message = "Recensioni nuk u gjet." });

        var user = await _unitOfWork.Users.GetByIdAsync(review.UserId);
        var book = await _unitOfWork.Books.GetByIdAsync(review.BookId);
        var reviewDto = new ReviewDTO
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            ReviewDate = review.ReviewDate,
            UserId = review.UserId,
            UserName = $"{user?.FirstName} {user?.LastName}",
            BookId = review.BookId,
            BookTitle = book?.Title ?? ""
        };

        return Ok(reviewDto);
    }

    [HttpPost]
    public async Task<ActionResult<ReviewDTO>> CreateReview([FromBody] CreateReviewDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var review = new Review
            {
                Rating = dto.Rating,
                Comment = dto.Comment ?? string.Empty,
                UserId = dto.UserId,
                BookId = dto.BookId,
                ReviewDate = DateTime.Now
            };

            var created = await _reviewService.CreateReviewAsync(review);

            var user = await _unitOfWork.Users.GetByIdAsync(created.UserId);
            var book = await _unitOfWork.Books.GetByIdAsync(created.BookId);
            var reviewDto = new ReviewDTO
            {
                Id = created.Id,
                Rating = created.Rating,
                Comment = created.Comment,
                ReviewDate = created.ReviewDate,
                UserId = created.UserId,
                UserName = $"{user?.FirstName} {user?.LastName}",
                BookId = created.BookId,
                BookTitle = book?.Title ?? ""
            };

            return CreatedAtAction(nameof(GetReview), new { id = reviewDto.Id }, reviewDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewDTO dto)
    {
        try
        {
            var existing = await _reviewService.GetReviewByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Recensioni nuk u gjet." });

            if (dto.Rating.HasValue) existing.Rating = dto.Rating.Value;
            if (dto.Comment != null) existing.Comment = dto.Comment;

            await _reviewService.UpdateReviewAsync(existing);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        try
        {
            await _reviewService.DeleteReviewAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("check/{userId}/{bookId}")]
    public async Task<ActionResult<object>> CheckUserHasReviewed(int userId, int bookId)
    {
        var hasReviewed = await _reviewService.UserHasReviewedBookAsync(userId, bookId);
        return Ok(new { userId, bookId, hasReviewed });
    }
}

