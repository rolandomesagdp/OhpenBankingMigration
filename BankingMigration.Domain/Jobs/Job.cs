using BankingMigration.Domain.BankAccounts;

namespace BankingMigration.Domain.Jobs
{
    public class Job
    {
        public int Id { get; set; }

        public JobType Type { get; set; }

        public JobStatus Status { get; private set; }

        public required List<BankAccount> BancAccounts { get; set; }

        public bool IsValid()
        {
            // ToDo: Implement real validation for a job
            return true && AccountsAreValid();
        }

        public bool AccountsAreValid()
        {
            return !BancAccounts.Any(x => x.IsValid() == false);
        }

        public void ChangeStatus(JobStatus status)
        {
            Status = status;
        }
    }
}
