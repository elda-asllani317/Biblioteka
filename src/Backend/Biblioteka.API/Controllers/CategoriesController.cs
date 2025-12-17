using Biblioteka.API.DTOs;
using Biblioteka.Core.Entities;
using Biblioteka.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        var categoriesDto = categories.Select(c => new CategoryDTO
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        });

        return Ok(categoriesDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
            return NotFound();

        var categoryDto = new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        return Ok(categoryDto);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CreateCategoryDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description
        };

        var created = await _categoryService.CreateCategoryAsync(category);

        var categoryDto = new CategoryDTO
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description
        };

        return CreatedAtAction(nameof(GetCategory), new { id = categoryDto.Id }, categoryDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _categoryService.GetCategoryByIdAsync(id);
        if (existing == null)
            return NotFound();

        existing.Name = dto.Name;
        existing.Description = dto.Description;

        await _categoryService.UpdateCategoryAsync(existing);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var existing = await _categoryService.GetCategoryByIdAsync(id);
        if (existing == null)
            return NotFound();

        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
}


