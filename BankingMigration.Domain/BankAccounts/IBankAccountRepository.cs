using BankingMigration.Domain.Migration;

namespace BankingMigration.Domain.BankAccounts
{
    public interface IBankAccountRepository
    {
        /// <summary>
        /// Updates a Bank Account's migration status. Once created, this is the only property that may be updated
        /// in a Bank Account. All other properties are inmutable.
        /// </summary>
        /// <param name="id">The id of the Bank Account to update.</param>
        /// /// <param name="status">The new status of the Bank Account.</param>
        /// <returns>The allready updated Bank Account</returns>
        public Task<BankAccount> UpdateBankAccountStatus(int id, MigrationStatus status);
    }
}
