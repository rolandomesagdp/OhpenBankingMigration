using Microsoft.Extensions.Logging;
using BankingMigration.Domain.BankAccounts;
using BankingMigration.Domain.Jobs;
using BankingMigration.Domain.Migration;

namespace BankingMigration.Domain.JobRun
{
    public abstract class JobRunner
    {
        protected readonly IMigrationService migrationService;
        private readonly IJobsRepository jobRepository;
        protected readonly IBankAccountRepository accountRepository;
        protected readonly ILogger logger;

        public Job Job { get; protected set; }

        public JobRunner(Job job, 
            IMigrationService migrationService,
            IJobsRepository jobRepository,
            IBankAccountRepository accountRepository, 
            ILogger logger)
        {
            this.migrationService = migrationService;
            this.jobRepository = jobRepository;
            this.accountRepository = accountRepository;
            this.logger = logger;
            Job = job;
        }

        public async Task Run()
        {
            try
            {
                if (Job.IsValid())
                {
                    await StartRunning();
                    await ProcessAllAccounts();
                    await EndRunning();
                }
                else
                {
                    var message = $"The job with id {Job.Id} cannot be proccessed because is invalid. Please, provide a valid job";
                    logger.LogError(message);
                    throw new InvalidOperationException(message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"The Job with id {Job.Id} could not end running bacause of an unexpected error. Details: {ex.Message}", [Job, ex]);
                await EndRunning();
            }
        }

        public abstract Task ProcessAllAccounts();

        public async Task ProcessAccount(BankAccount account)
        {
            try
            {
                await migrationService.MigrateBankAccount(account);
                await accountRepository.UpdateBankAccountStatus(account.Id, MigrationStatus.Proccessed);
            }
            catch (Exception ex)
            {
                await accountRepository.UpdateBankAccountStatus(account.Id, MigrationStatus.Failed);
                throw new Exception($"Bank Account {account.Id} migration failed. Details: {ex.Message}");
            }
        }

        public async Task StartRunning()
        {
            Job.ChangeStatus(JobStatus.Running);
            await jobRepository.Update(Job);
        }

        public async Task EndRunning()
        {
            Job.ChangeStatus(JobStatus.Finished);
            await jobRepository.Update(Job);
        }
    }
}
