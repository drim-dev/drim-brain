using Microsoft.EntityFrameworkCore;
using WebApi.Features.Accounts.Domain;

namespace WebApi.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapAccounts(modelBuilder);
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
}
