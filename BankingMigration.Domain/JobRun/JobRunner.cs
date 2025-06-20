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
                    await EndRunning(JobStatus.Succeed);
                }
                else
                {
                    var message = $"The job with id {Job.Id} cannot run either because it is invalid, it was already proccessed or it is currently running. Please, provide a valid job";
                    logger.LogError(message);
                    throw new InvalidOperationException(message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"The Job with id {Job.Id} could not end running bacause of an unexpected error. Details: {ex.Message}", [Job, ex]);
                await EndRunning(JobStatus.Failed);
            }
        }

        public abstract Task ProcessAllAccounts();

        public async Task ProcessAccount(BankAccount account)
        {
            try
            {
                await migrationService.MigrateBankAccount(account);
                await UpdateAccountStatus(account, MigrationStatus.Proccessed);
            }
            catch (Exception ex)
            {
                await UpdateAccountStatus(account, MigrationStatus.Failed);
                throw new Exception($"Bank Account {account.Id} migration failed. Details: {ex.Message}");
            }
        }

        public async Task StartRunning()
        {
            try
            {
                Job.SetStatus(JobStatus.Running);
                await jobRepository.Update(Job);
            }
            catch (Exception ex)
            {
                var message = $"Error during the job update to the running status. Job id: {Job.Id}";
                logger.LogError(message, ex);
                throw new Exception(message);
            }
        }

        public async Task EndRunning(JobStatus jobStatus)
        {
            try
            {
                if (jobStatus == JobStatus.Succeed || jobStatus == JobStatus.Failed)
                {
                    Job.SetStatus(jobStatus);
                    await jobRepository.Update(Job);
                }
                else
                {
                    throw new ArgumentException($"The provided job status is not valid as ending status. Please, make sure to provide a valid status to end the job");
                }
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException)
                    throw;
                var message = $"Error during the job update to the {jobStatus} status. Job id: {Job.Id}";
                logger.LogError(message, ex);
                throw new Exception(message);
            }
        }

        private async Task UpdateAccountStatus(BankAccount account, MigrationStatus status)
        {
            account.SetStatus(status);
            await accountRepository.UpdateBankAccount(account);
        }
    }
}
