using Microsoft.EntityFrameworkCore;
using WebApi.Features.CryptoTransfers.Domain;
using WebApi.Features.Currencies.Domain;
using WebApi.Features.Users.Domain;
using WebApi.Features.Variables.Domain;

namespace WebApi.Database
{
    public class BlockchainDbContext : DbContext
    {
        public BlockchainDbContext(DbContextOptions<BlockchainDbContext> options) : base(options)
        {
        }

        public DbSet<Variable> Variables { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Xpub> Xpubs { get; set; }
        public DbSet<DepositAddress> DepositAddresses { get; set; }
    }
}


