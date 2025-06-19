using Microsoft.EntityFrameworkCore;
using BankingMigration.Domain.BankAccounts;
using BankingMigration.Domain.Jobs;

namespace BankingMigration.Data.Context
{
    public class MigrationJobsDbContext : DbContext
    {
        public DbSet<Job> Jobs { get; set; }

        public DbSet<BankAccount> BankAccounts { get; set; }

        public MigrationJobsDbContext(DbContextOptions<MigrationJobsDbContext> options) : base(options) { }
    }
}
