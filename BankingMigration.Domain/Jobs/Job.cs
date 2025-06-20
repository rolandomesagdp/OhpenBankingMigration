using BankingMigration.Domain.BankAccounts;

namespace BankingMigration.Domain.Jobs
{
    public class Job
    {
        public int Id { get; set; }

        public JobType Type { get; private set; }

        public JobStatus Status { get; private set; }

        public required List<BankAccount> BancAccounts { get; set; }

        public bool IsValid()
        {
            // ToDo: Implement real validation for a job
            return BancAccounts.Count > 0 && AccountsAreValid();
        }

        public bool AccountsAreValid()
        {
            return !BancAccounts.Any(x => x.IsValid() == false);
        }

        public void SetStatus(JobStatus status)
        {
            Status = status;
        }

        public void SetType(JobType type)
        {
            Type = type;
        }
    }
}
