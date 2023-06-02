using Microsoft.EntityFrameworkCore;
using Testing.Features.Deposits.Domain;
using Testing.Features.Users.Domain;
using Testing.Features.Variables.Domain;

namespace Testing.Database;

public class TestingDbContext : DbContext
{
    public DbSet<Variable> Variables { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Xpub> Xpubs { get; set; }
    public DbSet<DepositAddress> DepositAddresses { get; set; }

    public TestingDbContext(DbContextOptions<TestingDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        MapVariables(modelBuilder);
        MapUsers(modelBuilder);
        MapXpubs(modelBuilder);
        MapDepositAddresses(modelBuilder);
    }

    private void MapVariables(ModelBuilder modelBuilder)
    {
        throw new NotImplementedException();
    }

    private void MapUsers(ModelBuilder modelBuilder)
    {
        throw new NotImplementedException();
    }

    private void MapXpubs(ModelBuilder modelBuilder)
    {
        throw new NotImplementedException();
    }

    private void MapDepositAddresses(ModelBuilder modelBuilder)
    {
        throw new NotImplementedException();
    }
}
