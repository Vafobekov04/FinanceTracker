using FinanceTracker.API.DTOs;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TransactionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionDto dto)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            CategoryId = dto.CategoryId,
            Amount = dto.Amount,
            Description = dto.Description,
            Date = dto.Date,
            Type = dto.Type
        };

        _context.Transactions.Add(transaction);

        await _context.SaveChangesAsync();

        return Ok(transaction);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var transactions = await _context.Transactions
            .Include(t => t.Category)
            .OrderByDescending(t => t.Date)
            .ToListAsync();

        return Ok(transactions);
    }
    [HttpGet("statistics/{userId}")]
    public async Task<IActionResult> GetStatistics(Guid userId)
    {
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId)
            .ToListAsync();

        var totalIncome = transactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var totalExpense = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        var balance = totalIncome - totalExpense;

        return Ok(new
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            Balance = balance
        });
    }
}
