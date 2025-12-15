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
    public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO loginDTO)
    {
        var result = await _authService.LoginAsync(loginDTO);
        if (result == null)
        {
            return Unauthorized(new { message = "Email ose password i gabuar" });
        }
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDTO>> Register(RegisterDTO registerDTO)
    {
        var result = await _authService.RegisterAsync(registerDTO);
        if (result == null)
        {
            return BadRequest(new { message = "Përdoruesi me këtë email ekziston tashmë" });
        }
        return Ok(result);
    }
}

