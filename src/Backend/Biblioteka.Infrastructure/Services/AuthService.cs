using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Biblioteka.Core.DTOs;
using Biblioteka.Core.Interfaces;
using Biblioteka.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Biblioteka.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _secretKey = configuration["Jwt:Key"] ?? "BibliotekaSecretKey12345678901234567890";
        _issuer = configuration["Jwt:Issuer"] ?? "BibliotekaAPI";
        _audience = configuration["Jwt:Audience"] ?? "BibliotekaClient";
    }

    public async Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDTO)
    {
        if (string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
        {
            return null;
        }

        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => 
            u.Email.ToLower().Trim() == loginDTO.Email.ToLower().Trim() && u.IsActive);

        if (user == null)
        {
            return null;
        }

        // Simple password check - in production use hashing
        if (user.Password?.Trim() != loginDTO.Password.Trim())
        {
            return null;
        }

        var token = GenerateToken(user);
        return new AuthResponseDTO
        {
            Token = token,
            User = new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            }
        };
    }

    public async Task<AuthResponseDTO?> RegisterAsync(RegisterDTO registerDTO)
    {
        var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == registerDTO.Email);
        if (existingUser != null)
        {
            return null; // User already exists
        }

        var user = new Biblioteka.Core.Entities.User
        {
            FirstName = registerDTO.FirstName,
            LastName = registerDTO.LastName,
            Email = registerDTO.Email,
            Password = registerDTO.Password, // In production, hash this
            Phone = registerDTO.Phone,
            Address = registerDTO.Address,
            RegistrationDate = DateTime.Now,
            IsActive = true
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var token = GenerateToken(user);
        return new AuthResponseDTO
        {
            Token = token,
            User = new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            }
        };
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private string GenerateToken(Biblioteka.Core.Entities.User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

