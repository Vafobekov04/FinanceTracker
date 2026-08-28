using System.Security.Claims;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;


public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Получить все категории текущего пользователя
    [HttpGet]
    public async Task<IActionResult> GetMyCategories()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type
            })
            .ToListAsync();

        return Ok(categories);
    }

    // Получить категорию по ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var category = await _context.Categories
            .AsNoTracking()
            .Where(c => c.Id == id && c.UserId == userId)
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type
            })
            .FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound("Категория не найдена.");
        }

        return Ok(category);
    }

    // Создать категорию
    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest("Название категории не может быть пустым.");
        }

        var exists = await _context.Categories
            .AnyAsync(c =>
                c.UserId == userId &&
                c.Name.ToLower() == dto.Name.Trim().ToLower());

        if (exists)
        {
            return BadRequest("Такая категория уже существует.");
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = dto.Name.Trim(),
            Type = dto.Type
        };

        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return Ok(new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type
        });
    }

    // Изменить категорию
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCategoryDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.UserId == userId);

        if (category == null)
        {
            return NotFound("Категория не найдена.");
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest("Название категории не может быть пустым.");
        }

        var exists = await _context.Categories
            .AnyAsync(c =>
                c.Id != id &&
                c.UserId == userId &&
                c.Name.ToLower() == dto.Name.Trim().ToLower());

        if (exists)
        {
            return BadRequest("Такая категория уже существует.");
        }

        category.Name = dto.Name.Trim();
        category.Type = dto.Type;

        await _context.SaveChangesAsync();

        return Ok(new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type
        });
    }

    // Удалить категорию
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.UserId == userId);

        if (category == null)
        {
            return NotFound("Категория не найдена.");
        }

        var hasTransactions = await _context.Transactions
            .AnyAsync(t => t.CategoryId == id);

        if (hasTransactions)
        {
            return BadRequest(
                "Нельзя удалить категорию, у которой есть транзакции.");
        }

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();

        return Ok("Категория удалена.");
    }


}
