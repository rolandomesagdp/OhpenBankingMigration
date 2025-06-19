using Microsoft.Extensions.Logging;
using BankingMigration.Domain.BankAccounts;
using BankingMigration.Domain.Jobs;
using BankingMigration.Domain.Migration;

namespace BankingMigration.Domain.JobRun
{
    public class Bulk : JobRunner
    {
        public Bulk(Job job, 
            IMigrationService migrationService,
            IJobsRepository jobRepository,
            IBankAccountRepository accountRepository,
            ILogger logger) : base(job, migrationService, jobRepository, accountRepository, logger) { }

        public override async Task ProcessAllAccounts()
        {
            foreach (var account in Job.BancAccounts)
            {
                try
                {
                    logger.LogInformation($"Starting to process account {account.Id}");
                    await ProcessAccount(account);
                    
                }
                catch (Exception ex)
                {
                    logger.LogError($"Account {account.Id} failed to process. Details: {ex.Message}");
                }
            }
        }
    }
}
