using Biblioteka.API.DTOs;
using Biblioteka.Core.Interfaces;
using Biblioteka.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;
    private readonly IUnitOfWork _unitOfWork;

    public LoansController(ILoanService loanService, IUnitOfWork unitOfWork)
    {
        _loanService = loanService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LoanDTO>> GetLoan(int id)
    {
        var loan = await _loanService.GetLoanByIdAsync(id);
        if (loan == null)
            return NotFound();

        var user = await _unitOfWork.Users.GetByIdAsync(loan.UserId);
        var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(loan.BookCopyId);
        var book = bookCopy != null ? await _unitOfWork.Books.GetByIdAsync(bookCopy.BookId) : null;

        var loanDto = new LoanDTO
        {
            Id = loan.Id,
            LoanDate = loan.LoanDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            Status = loan.Status,
            UserId = loan.UserId,
            UserName = $"{user?.FirstName} {user?.LastName}",
            BookCopyId = loan.BookCopyId,
            BookTitle = book?.Title ?? ""
        };

        return Ok(loanDto);
    }

    [HttpPost]
    public async Task<ActionResult<LoanDTO>> CreateLoan(CreateLoanDTO createLoanDto)
    {
        try
        {
            var loan = await _loanService.CreateLoanAsync(
                createLoanDto.UserId,
                createLoanDto.BookCopyId,
                createLoanDto.DaysToLoan
            );

            var user = await _unitOfWork.Users.GetByIdAsync(loan.UserId);
            var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(loan.BookCopyId);
            var book = bookCopy != null ? await _unitOfWork.Books.GetByIdAsync(bookCopy.BookId) : null;

            var loanDto = new LoanDTO
            {
                Id = loan.Id,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                Status = loan.Status,
                UserId = loan.UserId,
                UserName = $"{user?.FirstName} {user?.LastName}",
                BookCopyId = loan.BookCopyId,
                BookTitle = book?.Title ?? ""
            };

            return CreatedAtAction(nameof(GetLoan), new { id = loanDto.Id }, loanDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/return")]
    public async Task<IActionResult> ReturnLoan(int id)
    {
        try
        {
            await _loanService.ReturnLoanAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<LoanDTO>>> GetUserLoans(int userId)
    {
        var loans = await _loanService.GetUserLoansAsync(userId);
        var loansDto = new List<LoanDTO>();

        foreach (var loan in loans)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(loan.UserId);
            var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(loan.BookCopyId);
            var book = bookCopy != null ? await _unitOfWork.Books.GetByIdAsync(bookCopy.BookId) : null;

            loansDto.Add(new LoanDTO
            {
                Id = loan.Id,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                Status = loan.Status,
                UserId = loan.UserId,
                UserName = $"{user?.FirstName} {user?.LastName}",
                BookCopyId = loan.BookCopyId,
                BookTitle = book?.Title ?? ""
            });
        }

        return Ok(loansDto);
    }

    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<LoanDTO>>> GetOverdueLoans()
    {
        var loans = await _loanService.GetOverdueLoansAsync();
        var loansDto = new List<LoanDTO>();

        foreach (var loan in loans)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(loan.UserId);
            var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(loan.BookCopyId);
            var book = bookCopy != null ? await _unitOfWork.Books.GetByIdAsync(bookCopy.BookId) : null;

            loansDto.Add(new LoanDTO
            {
                Id = loan.Id,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                Status = loan.Status,
                UserId = loan.UserId,
                UserName = $"{user?.FirstName} {user?.LastName}",
                BookCopyId = loan.BookCopyId,
                BookTitle = book?.Title ?? ""
            });
        }

        return Ok(loansDto);
    }
}

