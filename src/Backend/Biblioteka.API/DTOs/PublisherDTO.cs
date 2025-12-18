using System.ComponentModel.DataAnnotations;

namespace Biblioteka.API.DTOs;

public class PublisherDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int BookCount { get; set; }
}

public class CreatePublisherDTO
{
    [Required(ErrorMessage = "Name është i detyrueshëm")]
    [StringLength(200, ErrorMessage = "Name nuk mund të jetë më i gjatë se 200 karaktere")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address është e detyrueshme")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone është i detyrueshëm")]
    [Phone(ErrorMessage = "Phone duhet të jetë në format të vlefshëm")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email është i detyrueshëm")]
    [EmailAddress(ErrorMessage = "Email duhet të jetë në format të vlefshëm")]
    public string Email { get; set; } = string.Empty;
}

