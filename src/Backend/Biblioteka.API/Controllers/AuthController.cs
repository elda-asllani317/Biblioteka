using Biblioteka.Core.DTOs;
using Biblioteka.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO loginDTO)
    {
        if (loginDTO == null || string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
        {
            return BadRequest(new { message = "Email dhe password janë të detyrueshëm" });
        }

        var result = await _authService.LoginAsync(loginDTO);
        if (result == null)
        {
            return Unauthorized(new { message = "Email ose password i gabuar" });
        }
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterDTO registerDTO)
    {
        if (registerDTO == null)
        {
            return BadRequest(new { message = "Të dhënat janë të detyrueshëm" });
        }

        if (string.IsNullOrWhiteSpace(registerDTO.Email) || string.IsNullOrWhiteSpace(registerDTO.Password))
        {
            return BadRequest(new { message = "Email dhe password janë të detyrueshëm" });
        }

        if (string.IsNullOrWhiteSpace(registerDTO.FirstName) || string.IsNullOrWhiteSpace(registerDTO.LastName))
        {
            return BadRequest(new { message = "Emri dhe mbiemri janë të detyrueshëm" });
        }

        if (registerDTO.Password.Length < 6)
        {
            return BadRequest(new { message = "Password-i duhet të jetë të paktën 6 karaktere" });
        }

        var result = await _authService.RegisterAsync(registerDTO);
        if (result == null)
        {
            return BadRequest(new { message = "Përdoruesi me këtë email ekziston tashmë" });
        }
        return Ok(result);
    }
}

