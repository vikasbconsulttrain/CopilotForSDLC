namespace LoanApplication.Api.Orm.Entities;

public sealed class AccountTransaction
{
    public int TransactionId { get; set; }
    public int CustomerId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal TransactionAmount { get; set; }
    public string TransactionType { get; set; } = string.Empty;

    public Customer Customer { get; set; } = null!;
}
