using BankingMigration.Domain.BankAccounts;
using BankingMigration.Data.Context;

namespace BankingMigration.Data.BankAccounts
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly MigrationJobsDbContext context;

        public BankAccountRepository(MigrationJobsDbContext context)
        {
            this.context = context;
        }

        public async Task<BankAccount> UpdateBankAccount(BankAccount account)
        {
            if (account.IsValid())
            {
                var updatedEntity = context.BankAccounts.Update(account);
                await context.SaveChangesAsync();
                return updatedEntity.Entity;
            }
            else
            {
                throw new ArgumentException("The provided Bank account is not valid");
            }
        }
    }
}
