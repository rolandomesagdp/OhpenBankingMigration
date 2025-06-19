using Microsoft.Extensions.Logging;
using BankingMigration.Domain.BankAccounts;
using BankingMigration.Domain.Notifications;

namespace BankingMigration.Domain.Jobs.UseCases
{
    /// <summary>
    /// All these use cases may live in different classes. They are all together
    /// for the sake of time.
    /// </summary>
    public class JobUseCases : IJobUseCases
    {
        private readonly IJobsRepository jobRepository;
        private readonly IBankAccountRepository jobSummaryRepository;
        private readonly IJobNotifications jobNotifications;
        private readonly ILogger<JobUseCases> logger;

        public JobUseCases(IJobsRepository jobRepository,
            IBankAccountRepository jobSummaryRepository,
            IJobNotifications jobNotifications,
            ILogger<JobUseCases> _logger)
        {
            this.jobRepository = jobRepository;
            this.jobSummaryRepository = jobSummaryRepository;
            this.jobNotifications = jobNotifications;
            logger = _logger;
        }

        public async Task<int> CreateJobAsync(Job job)
        {
            try
            {
                logger.LogDebug("New job creation requested", [job]);
                var newJobId = await jobRepository.CreateAsync(job);
                logger.LogDebug($"New job correctly created with id {newJobId}");
                logger.LogDebug($"Sending job creation notification for job {newJobId}");
                jobNotifications.NotifyJobMigrationCreated(newJobId);
                logger.LogDebug($"Job creation notification correctly send for job {newJobId}");
                return newJobId;
            }
            catch (Exception ex)
            {
                var message = $"There was an error during the creation of the job. Details: {ex.Message}";
                logger.LogError(message, ex);
                throw new Exception(message);
            }
        }

        public async Task<JobSummary> GetJobStatus(int jobId)
        {
            try
            {
                logger.LogDebug("Job Summary requested for job", [jobId]);
                var job = await jobRepository.GetByIdAsync(jobId);
                return JobSummary.Create(job);
            }
            catch (Exception ex)
            {
                var message = $"There was an error the build of the job summary for job with id {jobId}. Details: {ex.Message}";
                logger.LogError(message, ex);
                throw new Exception(message);
            }
        }
    }
}
