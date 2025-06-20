using BankingMigration.Domain.BankAccounts;
using BankingMigration.Domain.Migration;

namespace BankingMigration.Domain.Tests.BankAccounts
{
    internal static class BankAccountsMockData
    {
        public static List<BankAccount> GetAccountsForJob(int jobId)
        {
            var accounts = new List<BankAccount>();
            accounts.Add(new BankAccount
            {
                Id = 1,
                JobId = jobId,
                Alias = $"Alias 1",
                CustomerName = $"Customer Name 1"
            });

            accounts.Add(new BankAccount
            {
                Id = 2,
                JobId = jobId,
                Alias = $"Alias 2",
                CustomerName = $"Customer Name 2"
            });

            return accounts;
        }
    }
}
