namespace BankingMigration.Domain.Jobs.UseCases
{
    public interface IJobUseCases
    {
        /// <summary>
        /// Asynchronously creates a new Migration job in the system.
        /// </summary>
        /// <param name="job">The job to be created</param>
        /// <returns>The id of the created job</returns>
        public Task<int> CreateJobAsync(Job job);

        /// <summary>
        /// Gets the summary or status of a job.
        /// </summary>
        /// <param name="jobId">The id of the job to provide the summary for.</param>
        /// <returns></returns>
        public Task<JobSummary> GetJobStatus(int jobId);
    }
}
