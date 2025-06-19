namespace BankingMigration.Data.Context
{
    public static class DbInitializer
    {
        public static void InitializeDb(MigrationJobsDbContext dbContext) 
        {
            dbContext.Database.EnsureCreated();
        }
    }
}
