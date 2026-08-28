using FinanceTracker.Domain.Enums;
/*DTO — Data Transfer Object. */

namespace FinanceTracker.API.DTOs
{
    public class CreateTransactionDto
    {
        public Guid CategoryId { get; set; }

        public decimal Amount {  get; set; }

        public string Type { get; set; } = string.Empty;

        public string? Description { get; set; }

    }
}
