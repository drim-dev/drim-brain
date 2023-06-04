using Microsoft.EntityFrameworkCore;
using Testing.Features.Accounts.Domain;
using Testing.Features.Users.Domain;

namespace Testing.Database;

public class TestingDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }

    public TestingDbContext(DbContextOptions<TestingDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        MapUsers(modelBuilder);
        MapAccounts(modelBuilder);
    }

    private static void MapUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(x => x.Id);

            user.Property(x => x.Id).UseIdentityAlwaysColumn();

            user.Property(x => x.Email).IsRequired();

            user.Property(x => x.PasswordHash).IsRequired();

            user.Property(x => x.DateOfBirth).IsRequired();

            user.Property(x => x.RegisteredAt).IsRequired();
        });
    }

    private static void MapAccounts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(account =>
        {
            account.HasKey(x => x.Number);

            account.Property(x => x.Number).ValueGeneratedNever();

            account.Property(x => x.Currency).IsRequired();

            account.Property(x => x.Amount).IsRequired();

            account.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired();
        });
    }
}
