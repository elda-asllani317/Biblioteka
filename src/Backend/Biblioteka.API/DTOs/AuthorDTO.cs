using System.ComponentModel.DataAnnotations;

namespace Biblioteka.API.DTOs;

public class AuthorDTO
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Biography { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public string Nationality { get; set; } = string.Empty;
}

public class CreateAuthorDTO
{
    [Required(ErrorMessage = "FirstName është i detyrueshëm")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "LastName është i detyrueshëm")]
    public string LastName { get; set; } = string.Empty;

    public string Biography { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public string Nationality { get; set; } = string.Empty;
}


