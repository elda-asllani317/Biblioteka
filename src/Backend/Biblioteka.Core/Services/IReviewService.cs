using Biblioteka.Core.Entities;

namespace Biblioteka.Core.Services;

public interface IReviewService
{
    Task<IEnumerable<Review>> GetReviewsByBookIdAsync(int bookId);
    Task<IEnumerable<Review>> GetReviewsByUserIdAsync(int userId);
    Task<Review?> GetReviewByIdAsync(int id);
    Task<Review> CreateReviewAsync(Review review);
    Task UpdateReviewAsync(Review review);
    Task DeleteReviewAsync(int id);
    Task<double> GetAverageRatingByBookIdAsync(int bookId);
    Task<bool> UserHasReviewedBookAsync(int userId, int bookId);
}

