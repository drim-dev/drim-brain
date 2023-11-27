using BankingService.Domain;
using Microsoft.EntityFrameworkCore;

namespace BankingService.Database;

public class BankingDbContext(DbContextOptions<BankingDbContext> options) : DbContext(options)
{
    public DbSet<Withdrawal> Withdrawals => Set<Withdrawal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Withdrawal>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .IsRequired();

            entity.Property(e => e.UserId)
                .IsRequired();

            entity.Property(e => e.AccountNumber)
                .IsRequired();

            entity.Property(e => e.Currency)
                .IsRequired();

            entity.Property(e => e.Amount)
                .IsRequired();

            entity.Property(e => e.CryptoAddress)
                .IsRequired();

            entity.Property(e => e.TxId)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();
        });
    }
}
