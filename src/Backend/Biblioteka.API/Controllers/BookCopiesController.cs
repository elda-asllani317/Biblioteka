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
public class BookCopiesController : ControllerBase
{
    private readonly IBookCopyService _bookCopyService;
    private readonly IUnitOfWork _unitOfWork;

    public BookCopiesController(IBookCopyService bookCopyService, IUnitOfWork unitOfWork)
    {
        _bookCopyService = bookCopyService;
        _unitOfWork = unitOfWork;
    }

    private static BookCopyDTO MapToDto(BookCopy copy, string bookTitle)
    {
        return new BookCopyDTO
        {
            Id = copy.Id,
            CopyNumber = copy.CopyNumber,
            IsAvailable = copy.IsAvailable,
            Condition = copy.Condition,
            PurchaseDate = copy.PurchaseDate,
            BookId = copy.BookId,
            BookTitle = bookTitle
        };
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookCopyDTO>>> GetAll()
    {
        var copies = await _bookCopyService.GetAllAsync();
        var result = new List<BookCopyDTO>();

        foreach (var copy in copies)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(copy.BookId);
            result.Add(MapToDto(copy, book?.Title ?? string.Empty));
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookCopyDTO>> GetById(int id)
    {
        var copy = await _bookCopyService.GetByIdAsync(id);
        if (copy == null)
            return NotFound();

        var book = await _unitOfWork.Books.GetByIdAsync(copy.BookId);
        return Ok(MapToDto(copy, book?.Title ?? string.Empty));
    }

    [HttpGet("book/{bookId}")]
    public async Task<ActionResult<IEnumerable<BookCopyDTO>>> GetByBookId(int bookId)
    {
        var copies = await _bookCopyService.GetByBookIdAsync(bookId);
        var result = new List<BookCopyDTO>();

        var book = await _unitOfWork.Books.GetByIdAsync(bookId);
        var title = book?.Title ?? string.Empty;

        foreach (var copy in copies)
        {
            result.Add(MapToDto(copy, title));
        }

        return Ok(result);
    }

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<BookCopyDTO>>> GetAvailableBookCopies()
    {
        var copies = await _unitOfWork.BookCopies.FindAsync(c => c.IsAvailable);
        var result = new List<BookCopyDTO>();

        foreach (var copy in copies)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(copy.BookId);
            result.Add(MapToDto(copy, book?.Title ?? string.Empty));
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<BookCopyDTO>> Create([FromBody] CreateBookCopyDTO dto)
    {
        var copy = new BookCopy
        {
            CopyNumber = dto.CopyNumber,
            Condition = dto.Condition,
            PurchaseDate = dto.PurchaseDate,
            BookId = dto.BookId,
            IsAvailable = true
        };

        try
        {
            var created = await _bookCopyService.CreateAsync(copy);
            var book = await _unitOfWork.Books.GetByIdAsync(created.BookId);
            var result = MapToDto(created, book?.Title ?? string.Empty);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookCopyDTO dto)
    {
        var copy = await _bookCopyService.GetByIdAsync(id);
        if (copy == null)
            return NotFound();

        copy.Condition = dto.Condition;
        copy.PurchaseDate = dto.PurchaseDate;
        copy.IsAvailable = dto.IsAvailable;

        try
        {
            await _bookCopyService.UpdateAsync(copy);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _bookCopyService.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

