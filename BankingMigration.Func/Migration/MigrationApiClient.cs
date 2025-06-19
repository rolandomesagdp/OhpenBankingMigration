using BankingMigration.Domain.BankAccounts;
using BankingMigration.Domain.Migration;

namespace BankingMigration.Func.Migration
{
    internal class MigrationApiClient : IMigrationService
    {
        public async Task<MigrationStatus> MigrateBankAccount(BankAccount bankAccount)
        {
            await Task.Delay(500);
            return MigrationStatus.Proccessed;
        }
    }
}
