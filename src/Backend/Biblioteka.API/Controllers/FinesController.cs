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
public class FinesController : ControllerBase
{
    private readonly IFineService _fineService;
    private readonly IUnitOfWork _unitOfWork;

    public FinesController(IFineService fineService, IUnitOfWork unitOfWork)
    {
        _fineService = fineService;
        _unitOfWork = unitOfWork;
    }

    private async Task<FineDTO> MapToDtoAsync(Fine fine)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(fine.UserId);
        return new FineDTO
        {
            Id = fine.Id,
            Amount = fine.Amount,
            Reason = fine.Reason,
            IssueDate = fine.IssueDate,
            PaymentDate = fine.PaymentDate,
            Status = fine.Status,
            UserId = fine.UserId,
            UserName = user != null ? $"{user.FirstName} {user.LastName}" : string.Empty,
            LoanId = fine.LoanId
        };
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FineDTO>>> GetAll()
    {
        var fines = await _fineService.GetAllAsync();
        var result = new List<FineDTO>();
        foreach (var fine in fines)
        {
            result.Add(await MapToDtoAsync(fine));
        }
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FineDTO>> GetById(int id)
    {
        var fine = await _fineService.GetByIdAsync(id);
        if (fine == null)
            return NotFound();

        return Ok(await MapToDtoAsync(fine));
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<FineDTO>>> GetByUserId(int userId)
    {
        var fines = await _fineService.GetByUserIdAsync(userId);
        var result = new List<FineDTO>();
        foreach (var fine in fines)
        {
            result.Add(await MapToDtoAsync(fine));
        }
        return Ok(result);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<FineDTO>>> GetByStatus(string status)
    {
        var fines = await _fineService.GetByStatusAsync(status);
        var result = new List<FineDTO>();
        foreach (var fine in fines)
        {
            result.Add(await MapToDtoAsync(fine));
        }
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<FineDTO>> Create([FromBody] CreateFineDTO dto)
    {
        var fine = new Fine
        {
            Amount = dto.Amount,
            Reason = dto.Reason,
            IssueDate = dto.IssueDate ?? DateTime.Now,
            PaymentDate = dto.PaymentDate,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Pending" : dto.Status,
            UserId = dto.UserId,
            LoanId = dto.LoanId
        };

        try
        {
            var created = await _fineService.CreateAsync(fine);
            var result = await MapToDtoAsync(created);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFineDTO dto)
    {
        var fine = await _fineService.GetByIdAsync(id);
        if (fine == null)
            return NotFound();

        fine.Amount = dto.Amount;
        fine.Reason = dto.Reason;
        fine.Status = dto.Status;
        fine.PaymentDate = dto.PaymentDate;

        try
        {
            await _fineService.UpdateAsync(fine);
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
        await _fineService.DeleteAsync(id);
        return NoContent();
    }
}


