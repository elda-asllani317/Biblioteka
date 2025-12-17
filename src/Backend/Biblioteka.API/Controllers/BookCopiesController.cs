using Biblioteka.API.DTOs;
using Biblioteka.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookCopiesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public BookCopiesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<BookCopyDTO>>> GetAvailableBookCopies()
    {
        var copies = await _unitOfWork.BookCopies.FindAsync(c => c.IsAvailable);
        var result = new List<BookCopyDTO>();

        foreach (var copy in copies)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(copy.BookId);
            result.Add(new BookCopyDTO
            {
                Id = copy.Id,
                CopyNumber = copy.CopyNumber,
                IsAvailable = copy.IsAvailable,
                BookId = copy.BookId,
                BookTitle = book?.Title ?? string.Empty
            });
        }

        return Ok(result);
    }
}


