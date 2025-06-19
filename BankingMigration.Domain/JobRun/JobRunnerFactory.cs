using Microsoft.Extensions.Logging;
using BankingMigration.Domain.BankAccounts;
using BankingMigration.Domain.Jobs;
using BankingMigration.Domain.Migration;

namespace BankingMigration.Domain.JobRun
{
    public class JobRunnerFactory
    {
        private readonly Job job;
        private readonly IMigrationService migrationService;
        private readonly IJobsRepository jobRepository;
        private readonly IBankAccountRepository accountRepository;
        private readonly ILogger logger;

        public JobRunnerFactory(Job job, 
            IMigrationService migrationService,
            IJobsRepository jobRepository,
            IBankAccountRepository accountRepository,
            ILogger logger)
        {
            this.job = job;
            this.migrationService = migrationService;
            this.jobRepository = jobRepository;
            this.accountRepository = accountRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Builds a Job Runner instance for the specified Job
        /// </summary>
        /// <returns>The build JobRunner instance</returns>
        public JobRunner Create()
        {
            switch (job.Type)
            {
                case JobType.Bulk:
                default:
                    return new Bulk(job, migrationService, jobRepository, accountRepository, logger);
                case JobType.Batch:
                    return new Batch(job, migrationService, jobRepository, accountRepository, logger);
            }
        }
    }
}
