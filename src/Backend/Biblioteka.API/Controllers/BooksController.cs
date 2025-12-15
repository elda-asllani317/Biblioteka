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
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly IUnitOfWork _unitOfWork;

    public BooksController(IBookService bookService, IUnitOfWork unitOfWork)
    {
        _bookService = bookService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDTO>>> GetAllBooks()
    {
        var books = await _bookService.GetAllBooksAsync();
        var booksDto = new List<BookDTO>();

        foreach (var book in books)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(book.AuthorId);
            var category = await _unitOfWork.Categories.GetByIdAsync(book.CategoryId);
            var publisher = await _unitOfWork.Publishers.GetByIdAsync(book.PublisherId);

            booksDto.Add(new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                Description = book.Description,
                PublicationYear = book.PublicationYear,
                Pages = book.Pages,
                Language = book.Language,
                AuthorId = book.AuthorId,
                AuthorName = $"{author?.FirstName} {author?.LastName}",
                CategoryId = book.CategoryId,
                CategoryName = category?.Name ?? "",
                PublisherId = book.PublisherId,
                PublisherName = publisher?.Name ?? ""
            });
        }

        return Ok(booksDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDTO>> GetBook(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
            return NotFound();

        var author = await _unitOfWork.Authors.GetByIdAsync(book.AuthorId);
        var category = await _unitOfWork.Categories.GetByIdAsync(book.CategoryId);
        var publisher = await _unitOfWork.Publishers.GetByIdAsync(book.PublisherId);

        var bookDto = new BookDTO
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            Description = book.Description,
            PublicationYear = book.PublicationYear,
            Pages = book.Pages,
            Language = book.Language,
            AuthorId = book.AuthorId,
            AuthorName = $"{author?.FirstName} {author?.LastName}",
            CategoryId = book.CategoryId,
            CategoryName = category?.Name ?? "",
            PublisherId = book.PublisherId,
            PublisherName = publisher?.Name ?? ""
        };

        return Ok(bookDto);
    }

    [HttpPost]
    public async Task<ActionResult<BookDTO>> CreateBook(CreateBookDTO createBookDto)
    {
        var book = new Book
        {
            Title = createBookDto.Title,
            ISBN = createBookDto.ISBN,
            Description = createBookDto.Description,
            PublicationYear = createBookDto.PublicationYear,
            Pages = createBookDto.Pages,
            Language = createBookDto.Language,
            AuthorId = createBookDto.AuthorId,
            CategoryId = createBookDto.CategoryId,
            PublisherId = createBookDto.PublisherId
        };

        var createdBook = await _bookService.CreateBookAsync(book);
        
        var author = await _unitOfWork.Authors.GetByIdAsync(createdBook.AuthorId);
        var category = await _unitOfWork.Categories.GetByIdAsync(createdBook.CategoryId);
        var publisher = await _unitOfWork.Publishers.GetByIdAsync(createdBook.PublisherId);

        var bookDto = new BookDTO
        {
            Id = createdBook.Id,
            Title = createdBook.Title,
            ISBN = createdBook.ISBN,
            Description = createdBook.Description,
            PublicationYear = createdBook.PublicationYear,
            Pages = createdBook.Pages,
            Language = createdBook.Language,
            AuthorId = createdBook.AuthorId,
            AuthorName = $"{author?.FirstName} {author?.LastName}",
            CategoryId = createdBook.CategoryId,
            CategoryName = category?.Name ?? "",
            PublisherId = createdBook.PublisherId,
            PublisherName = publisher?.Name ?? ""
        };

        return CreatedAtAction(nameof(GetBook), new { id = bookDto.Id }, bookDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, CreateBookDTO updateBookDto)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
            return NotFound();

        book.Title = updateBookDto.Title;
        book.ISBN = updateBookDto.ISBN;
        book.Description = updateBookDto.Description;
        book.PublicationYear = updateBookDto.PublicationYear;
        book.Pages = updateBookDto.Pages;
        book.Language = updateBookDto.Language;
        book.AuthorId = updateBookDto.AuthorId;
        book.CategoryId = updateBookDto.CategoryId;
        book.PublisherId = updateBookDto.PublisherId;

        await _bookService.UpdateBookAsync(book);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        await _bookService.DeleteBookAsync(id);
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<BookDTO>>> SearchBooks([FromQuery] string term)
    {
        var books = await _bookService.SearchBooksAsync(term);
        var booksDto = new List<BookDTO>();

        foreach (var book in books)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(book.AuthorId);
            var category = await _unitOfWork.Categories.GetByIdAsync(book.CategoryId);
            var publisher = await _unitOfWork.Publishers.GetByIdAsync(book.PublisherId);

            booksDto.Add(new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                Description = book.Description,
                PublicationYear = book.PublicationYear,
                Pages = book.Pages,
                Language = book.Language,
                AuthorId = book.AuthorId,
                AuthorName = $"{author?.FirstName} {author?.LastName}",
                CategoryId = book.CategoryId,
                CategoryName = category?.Name ?? "",
                PublisherId = book.PublisherId,
                PublisherName = publisher?.Name ?? ""
            });
        }

        return Ok(booksDto);
    }
}

