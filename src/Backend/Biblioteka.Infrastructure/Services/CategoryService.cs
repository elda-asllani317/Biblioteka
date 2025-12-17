using Biblioteka.Core.Entities;
using Biblioteka.Core.Interfaces;
using Biblioteka.Core.Services;

namespace Biblioteka.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _unitOfWork.Categories.GetAllAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _unitOfWork.Categories.GetByIdAsync(id);
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return category;
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        await _unitOfWork.Categories.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category != null)
        {
            await _unitOfWork.Categories.DeleteAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}


