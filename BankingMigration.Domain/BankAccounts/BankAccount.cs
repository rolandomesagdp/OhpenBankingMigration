using BankingMigration.Domain.Jobs;
using BankingMigration.Domain.Migration;

namespace BankingMigration.Domain.BankAccounts
{
    public class BankAccount
    {
        public int Id { get; set; }

        public int JobId { get; set; }

        public required string Alias { get; set; }

        public required string CustomerName { get; set; }

        public MigrationStatus Status { get; set; }

        public Job? Job { get; set; }

        public bool IsValid()
        {
            // ToDo: Implement real validation for an account
            return true;
        }
    }
}
