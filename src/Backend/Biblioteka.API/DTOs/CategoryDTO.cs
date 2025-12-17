using System.ComponentModel.DataAnnotations;

namespace Biblioteka.API.DTOs;

public class CategoryDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}

public class CreateCategoryDTO
{
    [Required(ErrorMessage = "Name është i detyrueshëm")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}


