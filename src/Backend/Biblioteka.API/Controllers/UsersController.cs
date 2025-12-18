using Biblioteka.Core.Entities;
using Biblioteka.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public UsersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        // Don't return passwords
        var usersWithoutPasswords = users.Select(u => new
        {
            u.Id,
            u.FirstName,
            u.LastName,
            u.Email,
            u.Phone,
            u.Address,
            u.RegistrationDate,
            u.IsActive,
            u.Role
        });
        return Ok(usersWithoutPasswords);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(new
        {
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Phone,
            user.Address,
            user.RegistrationDate,
            user.IsActive,
            user.Role
        });
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserDTO dto)
    {
        var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingUser != null)
            return BadRequest(new { message = "Përdoruesi me këtë email ekziston tashmë" });

        // Validate role - only Admin or User allowed
        var role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role;
        if (role != "Admin" && role != "User")
        {
            return BadRequest(new { message = "Role duhet të jetë 'Admin' ose 'User'" });
        }

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Password = dto.Password,
            Phone = dto.Phone,
            Address = dto.Address,
            RegistrationDate = DateTime.Now,
            IsActive = true,
            Role = role
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
        {
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Phone,
            user.Address,
            user.RegistrationDate,
            user.IsActive,
            user.Role
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Phone = dto.Phone;
        user.Address = dto.Address;
        if (!string.IsNullOrEmpty(dto.Password))
            user.Password = dto.Password;
        
        // Allow admin to update role
        if (!string.IsNullOrWhiteSpace(dto.Role))
        {
            if (dto.Role != "Admin" && dto.Role != "User")
            {
                return BadRequest(new { message = "Role duhet të jetë 'Admin' ose 'User'" });
            }
            user.Role = dto.Role;
        }

        // Allow admin to activate/deactivate users
        if (dto.IsActive.HasValue)
        {
            user.IsActive = dto.IsActive.Value;
        }

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        await _unitOfWork.Users.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateUserDTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // Default to User, admin can set to Admin
}

public class UpdateUserDTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? Role { get; set; } // Admin can update role
    public bool? IsActive { get; set; } // Admin can activate/deactivate users
}

