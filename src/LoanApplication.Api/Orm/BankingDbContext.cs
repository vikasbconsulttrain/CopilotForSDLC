using LoanApplication.Api.Orm.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanApplication.Api.Orm;

public sealed class BankingDbContext : DbContext
{
    public BankingDbContext(DbContextOptions<BankingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<AccountTransaction> Transactions => Set<AccountTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasKey(entity => entity.CustomerId);
        modelBuilder.Entity<Loan>().HasKey(entity => entity.LoanId);
        modelBuilder.Entity<Branch>().HasKey(entity => entity.BranchId);
        modelBuilder.Entity<AccountTransaction>().HasKey(entity => entity.TransactionId);

        modelBuilder.Entity<Loan>()
            .HasOne(entity => entity.Customer)
            .WithMany(entity => entity.Loans)
            .HasForeignKey(entity => entity.CustomerId);

        modelBuilder.Entity<Loan>()
            .HasOne(entity => entity.Branch)
            .WithMany(entity => entity.Loans)
            .HasForeignKey(entity => entity.BranchId);

        modelBuilder.Entity<AccountTransaction>()
            .HasOne(entity => entity.Customer)
            .WithMany(entity => entity.Transactions)
            .HasForeignKey(entity => entity.CustomerId);

        modelBuilder.Entity<Loan>()
            .HasIndex(entity => new { entity.LoanStatus, entity.ApplicationDate, entity.CustomerId, entity.BranchId });

        modelBuilder.Entity<AccountTransaction>()
            .HasIndex(entity => new { entity.CustomerId, entity.TransactionAmount });

        modelBuilder.Entity<Customer>()
            .HasIndex(entity => new { entity.LastName, entity.FirstName });
    }
}
