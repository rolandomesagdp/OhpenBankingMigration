using Microsoft.EntityFrameworkCore;
using BankingMigration.Domain.BankAccounts;
using BankingMigration.Domain.Migration;
using BankingMigration.Data.Context;

namespace BankingMigration.Data.Jobs
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly MigrationJobsDbContext context;

        public BankAccountRepository(MigrationJobsDbContext context)
        {
            this.context = context;
        }

        public async Task<BankAccount> UpdateBankAccountStatus(int id, MigrationStatus status)
        {
            var bankAccount = await context.BankAccounts.FirstOrDefaultAsync(x => x.Id == id);
            if (bankAccount == null)
            {
                throw new InvalidOperationException($"There is no such Bank account with id {id}");
            }

            bankAccount.Status = status;
            context.BankAccounts.Update(bankAccount);
            await context.SaveChangesAsync();
            return bankAccount;
        }
    }
}
