using Biblioteka.Core.DTOs;

namespace Biblioteka.Core.Services;

public interface IAuthService
{
    Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDTO);
    Task<AuthResponseDTO?> RegisterAsync(RegisterDTO registerDTO);
    Task<bool> ValidateTokenAsync(string token);
}

