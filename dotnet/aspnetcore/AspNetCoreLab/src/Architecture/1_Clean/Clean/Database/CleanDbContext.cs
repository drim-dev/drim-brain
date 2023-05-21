using Clean.Domain;
using Microsoft.EntityFrameworkCore;

namespace Clean.Database;

public class CleanDbContext : DbContext
{
    public DbSet<DepositAddress> DepositAddresses { get; set; }
    public DbSet<Deposit> Deposits { get; set; }

    public CleanDbContext(DbContextOptions<CleanDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        MapDepositAddress(modelBuilder);
        MapDeposits(modelBuilder);
    }

    private void MapDepositAddress(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DepositAddress>(address =>
        {
            address.HasKey(x => x.Id);

            address.Property(x => x.CurrencyCode)
                .IsRequired();

            address.Property(x => x.UserId)
                .IsRequired();

            address.Property(x => x.XpubId)
                .IsRequired();

            address.Property(x => x.DerivationIndex)
                .IsRequired();

            address.Property(x => x.CryptoAddress)
                .IsRequired();
        });
    }

    private void MapDeposits(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Deposit>(deposit =>
        {
            deposit.HasKey(x => x.Id);

            deposit.Property(x => x.CurrencyCode)
                .IsRequired();

            deposit.Property(x => x.UserId)
                .IsRequired();

            deposit.Property(x => x.AddressId)
                .IsRequired();

            deposit.HasOne(x => x.Address)
                .WithMany()
                .HasForeignKey(x => x.AddressId)
                .IsRequired();

            deposit.Property(x => x.Amount)
                .IsRequired();

            deposit.Property(x => x.TxId)
                .IsRequired();

            deposit.Property(x => x.Confirmations)
                .IsRequired();

            deposit.Property(x => x.Status)
                .IsRequired();

            deposit.Property(x => x.CreatedAt)
                .IsRequired();

            deposit.Property(x => x.UpdatedAt)
                .IsRequired();
        });
    }
}
