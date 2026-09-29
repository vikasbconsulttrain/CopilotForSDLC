namespace LoanApplication.Api.Orm.Entities;

public sealed class Customer
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<AccountTransaction> Transactions { get; set; } = new List<AccountTransaction>();
}
