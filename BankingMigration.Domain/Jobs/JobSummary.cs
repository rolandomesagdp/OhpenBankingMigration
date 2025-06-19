using System.Runtime.InteropServices;

namespace BankingMigration.Domain.Jobs
{
    public class JobSummary
    {
        public int JobId { get; set; }

        public JobType Type { get; set; }

        public JobStatus Status { get; set; }

        public int TotalItems { get; set; }

        public int ProcessedItems { get; set; }

        public int FailedItems { get; set; }

        private JobSummary() { }

        public static JobSummary Create(Job job)
        {
            var summary = new JobSummary()
            {
                JobId = job.Id,
                Type = job.Type,
                Status = job.Status,
                TotalItems = job.BancAccounts.Count,
                ProcessedItems = job.BancAccounts.Where(x => x.Status == Migration.MigrationStatus.Proccessed).Count(),
                FailedItems = job.BancAccounts.Where(x => x.Status == Migration.MigrationStatus.Failed).Count(),
            };

            return summary;
        }
    }
}
