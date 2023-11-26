using Microsoft.EntityFrameworkCore;

namespace BankingService.Database;

public class BankingDbContext(DbContextOptions<BankingDbContext> options) : DbContext(options);
