using Biblioteka.API.DTOs;
using Biblioteka.Core.Entities;
using Biblioteka.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PublishersController : ControllerBase
{
    private readonly IPublisherService _publisherService;

    public PublishersController(IPublisherService publisherService)
    {
        _publisherService = publisherService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PublisherDTO>>> GetAllPublishers(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        IEnumerable<Publisher> publishers;

        if (!string.IsNullOrWhiteSpace(search))
        {
            publishers = await _publisherService.SearchPublishersByNameAsync(search);
        }
        else
        {
            publishers = await _publisherService.GetAllPublishersAsync();
        }

        // Apply pagination
        var totalCount = publishers.Count();
        var paginatedPublishers = publishers
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var publishersDto = new List<PublisherDTO>();
        foreach (var publisher in paginatedPublishers)
        {
            var bookCount = await _publisherService.GetBookCountAsync(publisher.Id);
            publishersDto.Add(new PublisherDTO
            {
                Id = publisher.Id,
                Name = publisher.Name,
                Address = publisher.Address,
                Phone = publisher.Phone,
                Email = publisher.Email,
                BookCount = bookCount
            });
        }

        // Add pagination metadata to response headers
        Response.Headers.Add("X-Total-Count", totalCount.ToString());
        Response.Headers.Add("X-Page", page.ToString());
        Response.Headers.Add("X-Page-Size", pageSize.ToString());
        Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling(totalCount / (double)pageSize)).ToString());

        return Ok(publishersDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PublisherDTO>> GetPublisher(int id)
    {
        var publisher = await _publisherService.GetPublisherByIdAsync(id);
        if (publisher == null)
            return NotFound(new { message = "Botuesi nuk u gjet." });

        var bookCount = await _publisherService.GetBookCountAsync(publisher.Id);
        var publisherDto = new PublisherDTO
        {
            Id = publisher.Id,
            Name = publisher.Name,
            Address = publisher.Address,
            Phone = publisher.Phone,
            Email = publisher.Email,
            BookCount = bookCount
        };

        return Ok(publisherDto);
    }

    [HttpPost]
    public async Task<ActionResult<PublisherDTO>> CreatePublisher([FromBody] CreatePublisherDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var publisher = new Publisher
            {
                Name = dto.Name,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email
            };

            var created = await _publisherService.CreatePublisherAsync(publisher);

            var publisherDto = new PublisherDTO
            {
                Id = created.Id,
                Name = created.Name,
                Address = created.Address,
                Phone = created.Phone,
                Email = created.Email,
                BookCount = 0
            };

            return CreatedAtAction(nameof(GetPublisher), new { id = publisherDto.Id }, publisherDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePublisher(int id, [FromBody] CreatePublisherDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var existing = await _publisherService.GetPublisherByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Botuesi nuk u gjet." });

            existing.Name = dto.Name;
            existing.Address = dto.Address;
            existing.Phone = dto.Phone;
            existing.Email = dto.Email;

            await _publisherService.UpdatePublisherAsync(existing);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePublisher(int id)
    {
        try
        {
            var existing = await _publisherService.GetPublisherByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Botuesi nuk u gjet." });

            await _publisherService.DeletePublisherAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

