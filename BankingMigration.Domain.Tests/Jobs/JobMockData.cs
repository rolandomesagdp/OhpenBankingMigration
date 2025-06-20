using BankingMigration.Domain.BankAccounts;
using BankingMigration.Domain.Jobs;
using BankingMigration.Domain.Tests.BankAccounts;

namespace BankingMigration.Domain.Tests.Jobs
{
    internal static class JobMockData
    {
        public static Job GetJob()
        {
            return new Job
            {
                Id = 1,
                BancAccounts = BankAccountsMockData.GetAccountsForJob(1)
            };
        }

        public static Job GetInvalidJob()
        {
            return new Job
            {
                Id = 1,
                BancAccounts = new List<BankAccount>()
            };
        }
    }
}
