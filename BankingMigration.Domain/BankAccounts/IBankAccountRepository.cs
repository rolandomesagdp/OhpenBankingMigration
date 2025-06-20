using BankingMigration.Domain.Migration;

namespace BankingMigration.Domain.BankAccounts
{
    public interface IBankAccountRepository
    {
        /// <summary>
        /// Updates a Bank Account in the system
        /// </summary>
        /// <param name="account">The Bank Account to update.</param>
        /// <returns>The allready updated Bank Account</returns>
        public Task<BankAccount> UpdateBankAccount(BankAccount account);
    }
}
