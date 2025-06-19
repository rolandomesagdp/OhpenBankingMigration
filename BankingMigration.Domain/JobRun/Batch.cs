
using Microsoft.Extensions.Logging;
using BankingMigration.Domain.BankAccounts;
using BankingMigration.Domain.JobRun;
using BankingMigration.Domain.Jobs;
using BankingMigration.Domain.Migration;

namespace BankingMigration.Domain.JobRun
{
    public class Batch : JobRunner
    {
        public Batch(Job job,
            IMigrationService migrationService,
            IJobsRepository jobRepository,
            IBankAccountRepository accountRepository,
            ILogger logger) : base(job, migrationService, jobRepository, accountRepository, logger) { }

        public override async Task ProcessAllAccounts()
        {
            logger.LogInformation($"Starting to proccess job {Job.Id}");

            foreach (var account in Job.BancAccounts)
            {
                try
                {
                    logger.LogInformation($"Starting to process account {account.Id}");
                    await ProcessAccount(account);

                }
                catch (Exception ex)
                {
                    var message = $"Account {account.Id} failed to process. Details: {ex.Message}";
                    logger.LogError(message, [account, ex]);
                    throw new Exception(message, ex);
                }
            }
        }
    }
}
