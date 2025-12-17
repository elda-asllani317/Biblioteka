using Biblioteka.API.DTOs;
using Biblioteka.Core.Interfaces;
using Biblioteka.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;
    private readonly IUnitOfWork _unitOfWork;

    public LoansController(ILoanService loanService, IUnitOfWork unitOfWork)
    {
        _loanService = loanService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanDTO>>> GetAllLoans()
    {
        var loans = await _unitOfWork.Loans.GetAllAsync();
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
            // Përdor DueDate nga klienti nëse është dhënë, përndryshe DaysToLoan
            int daysToLoan = createLoanDto.DaysToLoan;
            if (createLoanDto.DueDate.HasValue)
            {
                var nowDate = DateTime.Now.Date;
                var dueDate = createLoanDto.DueDate.Value.Date;
                if (dueDate <= nowDate)
                {
                    return BadRequest("DueDate duhet të jetë në të ardhmen.");
                }
                daysToLoan = (dueDate - nowDate).Days;
            }

            var loan = await _loanService.CreateLoanAsync(
                createLoanDto.UserId,
                createLoanDto.BookCopyId,
                daysToLoan
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLoan(int id, [FromBody] UpdateLoanDTO dto)
    {
        var loan = await _unitOfWork.Loans.GetByIdAsync(id);
        if (loan == null)
            return NotFound();

        if (dto.DueDate.HasValue)
        {
            loan.DueDate = dto.DueDate.Value;
        }

        if (dto.ReturnDate.HasValue)
        {
            loan.ReturnDate = dto.ReturnDate.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            loan.Status = dto.Status;
        }

        if (loan.ReturnDate.HasValue && loan.ReturnDate.Value.Date < loan.DueDate.Date)
        {
            return BadRequest("ReturnDate nuk mund të jetë para DueDate.");
        }

        await _unitOfWork.Loans.UpdateAsync(loan);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLoan(int id)
    {
        var loan = await _unitOfWork.Loans.GetByIdAsync(id);
        if (loan == null)
            return NotFound();

        // Nëse huazimi është aktiv, liro edhe kopjen e librit
        if (loan.Status == "Active")
        {
            var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(loan.BookCopyId);
            if (bookCopy != null)
            {
                bookCopy.IsAvailable = true;
                await _unitOfWork.BookCopies.UpdateAsync(bookCopy);
            }
        }

        await _unitOfWork.Loans.DeleteAsync(loan);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
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

