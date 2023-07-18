using Microsoft.EntityFrameworkCore;
using Transactions.Features.Accounts.Domain;
using Transactions.Features.Withdrawals.Domain;

namespace Transactions.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<VersionedAccount> VersionedAccounts { get; set; }
    public DbSet<Withdrawal> Withdrawals { get; set; }
    public DbSet<WithdrawalOutboxRecord> WithdrawalOutboxRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapAccounts(modelBuilder);
        MapVersionedAccounts(modelBuilder);
        // TODO: map Withdrawal
        // TODO: map WithdrawalOutboxRecord
    }

    private static void MapAccounts(ModelBuilder builder)
    {
        builder.Entity<Account>(account =>
        {
            account.HasKey(x => x.Id);

            account.Property(x => x.UserId)
                .IsRequired();

            account.Property(x => x.CurrencyCode)
                .IsRequired();

            account.Property(x => x.Amount)
                .IsRequired();

            account.Property(x => x.OpenedAt)
                .IsRequired();
        });
    }

    private static void MapVersionedAccounts(ModelBuilder builder)
    {
        builder.Entity<VersionedAccount>(account =>
        {
            account.HasKey(x => x.Id);

            account.Property(x => x.UserId)
                .IsRequired();

            account.Property(x => x.CurrencyCode)
                .IsRequired();

            account.Property(x => x.Amount)
                .IsRequired();

            account.Property(x => x.OpenedAt)
                .IsRequired();

            account.Property(x => x.Version)
                .IsRowVersion();
        });
    }
}
