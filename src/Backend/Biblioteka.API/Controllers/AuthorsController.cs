using Biblioteka.API.DTOs;
using Biblioteka.Core.Entities;
using Biblioteka.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetAllAuthors()
    {
        var authors = await _authorService.GetAllAuthorsAsync();
        var authorsDto = authors.Select(a => new AuthorDTO
        {
            Id = a.Id,
            FirstName = a.FirstName,
            LastName = a.LastName,
            Biography = a.Biography,
            DateOfBirth = a.DateOfBirth,
            Nationality = a.Nationality
        });

        return Ok(authorsDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDTO>> GetAuthor(int id)
    {
        var author = await _authorService.GetAuthorByIdAsync(id);
        if (author == null)
            return NotFound();

        var authorDto = new AuthorDTO
        {
            Id = author.Id,
            FirstName = author.FirstName,
            LastName = author.LastName,
            Biography = author.Biography,
            DateOfBirth = author.DateOfBirth,
            Nationality = author.Nationality
        };

        return Ok(authorDto);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDTO>> CreateAuthor([FromBody] CreateAuthorDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var author = new Author
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Biography = dto.Biography,
            DateOfBirth = dto.DateOfBirth,
            Nationality = dto.Nationality
        };

        var created = await _authorService.CreateAuthorAsync(author);

        var authorDto = new AuthorDTO
        {
            Id = created.Id,
            FirstName = created.FirstName,
            LastName = created.LastName,
            Biography = created.Biography,
            DateOfBirth = created.DateOfBirth,
            Nationality = created.Nationality
        };

        return CreatedAtAction(nameof(GetAuthor), new { id = authorDto.Id }, authorDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthor(int id, [FromBody] CreateAuthorDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _authorService.GetAuthorByIdAsync(id);
        if (existing == null)
            return NotFound();

        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Biography = dto.Biography;
        existing.DateOfBirth = dto.DateOfBirth;
        existing.Nationality = dto.Nationality;

        await _authorService.UpdateAuthorAsync(existing);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        var existing = await _authorService.GetAuthorByIdAsync(id);
        if (existing == null)
            return NotFound();

        await _authorService.DeleteAuthorAsync(id);
        return NoContent();
    }
}


