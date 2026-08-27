using FinanceTracker.Domain.Enums;
/*DTO — Data Transfer Object. */

namespace FinanceTracker.API.DTOs
{
    public class CreateTransactionDto
    {
        public Guid UserId { get; set; }

        public Guid CategoryId { get; set; }

        public decimal Amount {  get; set; }

        public string Description { get; set; } = string.Empty; 

        public DateTime Date {  get; set; }

        public TransactionType Type {  get; set; }

    }
}
