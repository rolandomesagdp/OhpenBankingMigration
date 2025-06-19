using BankingMigration.Domain.BankAccounts;

namespace BankingMigration.Domain.Migration
{
    public interface IMigrationService
    {
        /// <summary>
        /// Runs the migration of a given Bank Account
        /// </summary>
        /// <param name="bankAccount">The Bank Account to be migrated</param>
        /// <returns>The status of the migration</returns>
        public Task<MigrationStatus> MigrateBankAccount(BankAccount bankAccount);
    }
}
