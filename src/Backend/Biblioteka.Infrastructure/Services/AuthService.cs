using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Biblioteka.Core.DTOs;
using Biblioteka.Core.Interfaces;
using Biblioteka.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Biblioteka.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _secretKey = configuration["Jwt:Key"] ?? "BibliotekaSecretKey12345678901234567890";
        _issuer = configuration["Jwt:Issuer"] ?? "BibliotekaAPI";
        _audience = configuration["Jwt:Audience"] ?? "BibliotekaClient";
        _logger = logger;
    }

    public async Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDTO)
    {
        if (loginDTO == null || string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
        {
            _logger.LogWarning("Login attempt with null or empty credentials");
            return null;
        }

        // Normalize email for comparison
        var normalizedEmail = loginDTO.Email.ToLower().Trim();
        var inputPassword = (loginDTO.Password ?? string.Empty).Trim();

        _logger.LogInformation($"Login attempt for email: '{normalizedEmail}' (length: {normalizedEmail.Length})");

        // First try direct query to database (bypass cache)
        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => 
            u.Email.ToLower().Trim() == normalizedEmail && u.IsActive);

        // If not found, get all users and search in memory (for debugging)
        if (user == null)
        {
            _logger.LogWarning($"User not found with direct query, trying GetAllAsync...");
            var allUsers = await _unitOfWork.Users.GetAllAsync();
            var usersList = allUsers.ToList();
            _logger.LogInformation($"Total users in database: {usersList.Count}");
            
            // Log all users for debugging
            foreach (var u in usersList)
            {
                var normalized = u.Email.ToLower().Trim();
                _logger.LogInformation($"  - Email: '{u.Email}' (normalized: '{normalized}', length: {normalized.Length}), IsActive: {u.IsActive}, Role: {u.Role}");
            }
            
            // Try to find user with exact match (case-insensitive, trimmed)
            user = usersList.FirstOrDefault(u => 
            {
                var emailNormalized = u.Email.ToLower().Trim();
                var matches = emailNormalized == normalizedEmail && u.IsActive;
                if (matches)
                {
                    _logger.LogInformation($"Found user match: '{u.Email}' == '{normalizedEmail}'");
                }
                return matches;
            });
        }
        else
        {
            _logger.LogInformation($"User found with direct query: {user.Email}, Role: {user.Role}, IsActive: {user.IsActive}");
        }

        if (user == null)
        {
            _logger.LogWarning($"User not found or inactive: '{normalizedEmail}'");
            _logger.LogWarning($"Searched for: '{normalizedEmail}' (length: {normalizedEmail.Length})");
            return null;
        }

        _logger.LogInformation($"User found: {user.Email}, Role: {user.Role}, IsActive: {user.IsActive}");

        // Simple password check - in production use hashing
        // Compare passwords - handle null and whitespace
        var dbPassword = user.Password ?? string.Empty;
        var loginPassword = loginDTO.Password ?? string.Empty;
        
        // Trim both for comparison
        var trimmedDbPassword = dbPassword.Trim();
        var trimmedLoginPassword = loginPassword.Trim();
        
        _logger.LogInformation($"Password comparison - DB: '{trimmedDbPassword}' (length: {trimmedDbPassword.Length}), Input: '{trimmedLoginPassword}' (length: {trimmedLoginPassword.Length})");
        _logger.LogInformation($"Passwords match: {trimmedDbPassword == trimmedLoginPassword}");
        
        // Compare passwords (case-sensitive)
        if (trimmedDbPassword != trimmedLoginPassword)
        {
            _logger.LogWarning($"Password mismatch for user: {normalizedEmail}");
            return null;
        }

        _logger.LogInformation($"Login successful for user: {normalizedEmail}, Role: {user.Role}");

        var token = GenerateToken(user);
        return new AuthResponseDTO
        {
            Token = token,
            User = new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role ?? "User"
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
            IsActive = true,
            Role = "User" // Default role for new registrations
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
                Email = user.Email,
                Role = user.Role ?? "User"
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
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
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

