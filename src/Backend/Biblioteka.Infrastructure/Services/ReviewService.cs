using Biblioteka.Core.Entities;
using Biblioteka.Core.Interfaces;
using Biblioteka.Core.Services;

namespace Biblioteka.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Review>> GetReviewsByBookIdAsync(int bookId)
    {
        var reviews = await _unitOfWork.Reviews.FindAsync(r => r.BookId == bookId);
        return reviews.OrderByDescending(r => r.ReviewDate);
    }

    public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(int userId)
    {
        var reviews = await _unitOfWork.Reviews.FindAsync(r => r.UserId == userId);
        return reviews.OrderByDescending(r => r.ReviewDate);
    }

    public async Task<Review?> GetReviewByIdAsync(int id)
    {
        return await _unitOfWork.Reviews.GetByIdAsync(id);
    }

    public async Task<Review> CreateReviewAsync(Review review)
    {
        // Validate Rating is between 1 and 5
        if (review.Rating < 1 || review.Rating > 5)
        {
            throw new InvalidOperationException("Rating duhet të jetë midis 1 dhe 5.");
        }

        // Validate UserId exists
        var user = await _unitOfWork.Users.GetByIdAsync(review.UserId);
        if (user == null)
        {
            throw new InvalidOperationException($"Përdoruesi me ID {review.UserId} nuk ekziston.");
        }

        // Validate BookId exists
        var book = await _unitOfWork.Books.GetByIdAsync(review.BookId);
        if (book == null)
        {
            throw new InvalidOperationException($"Libri me ID {review.BookId} nuk ekziston.");
        }

        // Check if user has already reviewed this book
        var existingReview = await _unitOfWork.Reviews.FirstOrDefaultAsync(
            r => r.UserId == review.UserId && r.BookId == review.BookId
        );
        if (existingReview != null)
        {
            throw new InvalidOperationException("Ju keni dhënë tashmë një recension për këtë libër.");
        }

        // Set ReviewDate if not set
        if (review.ReviewDate == default)
        {
            review.ReviewDate = DateTime.Now;
        }

        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();
        return review;
    }

    public async Task UpdateReviewAsync(Review review)
    {
        var existing = await _unitOfWork.Reviews.GetByIdAsync(review.Id);
        if (existing == null)
        {
            throw new InvalidOperationException("Recensioni nuk u gjet.");
        }

        // Validate Rating is between 1 and 5
        if (review.Rating < 1 || review.Rating > 5)
        {
            throw new InvalidOperationException("Rating duhet të jetë midis 1 dhe 5.");
        }

        existing.Rating = review.Rating;
        existing.Comment = review.Comment;

        await _unitOfWork.Reviews.UpdateAsync(existing);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteReviewAsync(int id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
        {
            throw new InvalidOperationException("Recensioni nuk u gjet.");
        }

        await _unitOfWork.Reviews.DeleteAsync(review);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<double> GetAverageRatingByBookIdAsync(int bookId)
    {
        var reviews = await _unitOfWork.Reviews.FindAsync(r => r.BookId == bookId);
        if (!reviews.Any())
            return 0;

        return reviews.Average(r => r.Rating);
    }

    public async Task<bool> UserHasReviewedBookAsync(int userId, int bookId)
    {
        var review = await _unitOfWork.Reviews.FirstOrDefaultAsync(
            r => r.UserId == userId && r.BookId == bookId
        );
        return review != null;
    }
}

