using LoanService.Domain;
using Microsoft.EntityFrameworkCore;

namespace LoanService.Database;

public class LoanServiceDbContext(DbContextOptions<LoanServiceDbContext> options) : DbContext(options)
{
    public DbSet<LoanOffering> LoanOfferings => Set<LoanOffering>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoanOffering>(builder =>
        {
            builder.HasKey(o => o.Name);

            builder.Property(o => o .Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(o => o.Months)
                .IsRequired();

            builder.Property(o => o.InterestRate)
                .IsRequired();

            builder.Property(o => o.MaxAmount)
                .IsRequired();
        });

        modelBuilder.Entity<LoanOffering>()
            .HasData([
                new("Best Loan", 12, 0.05m, 10000),
                new("Worst Loan", 12, 0.25m, 50000)
            ]);
    }
}

public static class DatabaseExtensions
{
    public static WebApplication MigrateDb(this WebApplication app)
    {
        // TODO: Remove this when we have a better way to wait for the database to be ready
        Thread.Sleep(5000);

        using var scope = app.Services.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<LoanServiceDbContext>();
        db.Database.Migrate();
        return app;
    }
}
