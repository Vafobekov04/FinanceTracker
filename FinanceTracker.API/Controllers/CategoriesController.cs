using FinanceTracker.API.DTOs;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryDto dto)
    {
        var userExists = await _context.Users
            .AnyAsync(u => u.Id == dto.UserId);

        if (!userExists)
        {
            return NotFound("Пользователь не найден.");
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Name = dto.Name,
            Type = dto.Type
        };

        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return Ok(category);
    }
   
}
