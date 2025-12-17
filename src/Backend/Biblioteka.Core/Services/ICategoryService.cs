using Biblioteka.Core.Entities;

namespace Biblioteka.Core.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(int id);
}


