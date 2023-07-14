using Microsoft.EntityFrameworkCore;
using Transactions.Features.Accounts.Domain;

namespace Transactions.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<VersionedAccount> VersionedAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapAccounts(modelBuilder);
        MapVersionedAccounts(modelBuilder);
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
