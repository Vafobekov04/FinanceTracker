using FinanceTracker.Domain.Enums;
namespace FinanceTracker.Domain.Entities;

public class Transaction
{
    //transactionID
    public Guid Id { get; set; }

    //UserId
    public Guid UserId { get; set; }
    
    //CategoryID
    public Guid CategoryId { get; set; }

    //DecimalAmount
    public decimal Amount { get; set; }

    //Description
    public string Description { get; set; } = string.Empty;

    //Date
    public DateTime Date { get; set; }

    //TransactionTYPE
    public TransactionType Type { get; set; }

    public User User { get; set; } = null!;

    public Category Category { get; set; }=null!;  

}