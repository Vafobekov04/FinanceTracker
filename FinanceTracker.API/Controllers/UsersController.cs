using FinanceTracker.API.DTOs;
using FinanceTracker.API.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public UsersController(
        ApplicationDbContext context,
        IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (existingUser != null)
        {
            return BadRequest("Пользователь с таким email уже существует.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.CreatedAt
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
        {
            return Unauthorized("Неверный email или пароль.");
        }

        var passwordValid = BCrypt.Net.BCrypt.Verify(
            dto.Password,
            user.PasswordHash);

        if (!passwordValid)
        {
            return Unauthorized("Неверный email или пароль.");
        }

        var token = _jwtService.GenerateToken(user);

        return Ok(new
        {
            token,
            user.Id,
            user.Email
        });
    }
}