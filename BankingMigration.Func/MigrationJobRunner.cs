using Azure.Messaging.ServiceBus;
using BankingMigration.Domain.BankAccounts;
using BankingMigration.Domain.JobRun;
using BankingMigration.Domain.Jobs;
using BankingMigration.Domain.Migration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BankingMigration.Func;

public class MigrationJobRunner
{
    private int _jobId;
    private readonly IJobsRepository jobRepository;
    private readonly IMigrationService migrationService;
    private readonly IBankAccountRepository bankAccountRepository;
    private readonly ILogger<MigrationJobRunner> _logger;

    public MigrationJobRunner(IJobsRepository jobRepository,
        IMigrationService migrationService,
        IBankAccountRepository bankAccountRepository,
        ILogger<MigrationJobRunner> logger)
    {
        this.jobRepository = jobRepository;
        this.migrationService = migrationService;
        this.bankAccountRepository = bankAccountRepository;
        _logger = logger;
    }

    [Function(nameof(MigrationJobRunner))]
    public async Task Run(
        [ServiceBusTrigger("MigrationJobs", Connection = "ServiceBusQueue")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        try
        {
            _jobId = TryParseJobId(message.Body.ToString());
            _logger.LogInformation($"Migration requested for job {_jobId}", [_jobId]);

            var job = await jobRepository.GetByIdAsync(_jobId);

            _logger.LogInformation($"Job {_jobId} properly fetched from the DB", [_jobId, job]);
            _logger.LogInformation($"Starting execution for Job {_jobId}.", [_jobId, job]);

            await RunMigrationJob(job);

            _logger.LogInformation($"Job {_jobId} properly executed.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An unexpected error occured during the execution of the job {_jobId}. Details: {ex.Message}", [_jobId]);
        }

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }

    private async Task RunMigrationJob(Job job)
    {
        var runnerFactory = new JobRunnerFactory(job, migrationService, jobRepository, bankAccountRepository, _logger);
        var jobRunner = runnerFactory.Create();
        await jobRunner.Run();
    }

    private int TryParseJobId(string input)
    {
        try
        {
            _logger.LogInformation($"Parsing function input: {input}");
            var jobId = int.Parse(input);
            _logger.LogInformation($"Function input properly parsed as jobId: {jobId}");
            return jobId;
        }
        catch (Exception ex)
        {
            var message = $"Failed to parse the function input {input}. Make sure to provide a valid input value";
            _logger.LogError(message, [input]);
            throw new InvalidCastException(message, ex);
        }
    }
}