namespace BankingMigration.Domain.Jobs
{
    public interface IJobsRepository
    {
        /// <summary>
        /// Adds a new Migration job to the system.
        /// </summary>
        /// <param name="job">The Job to be added</param>
        /// <returns>The id of the newly added job</returns>
        public Task<int> CreateAsync(Job job);

        /// <summary>
        /// Gets a Job filtered by the Id.
        /// </summary>
        /// <param name="id">The id of the job to search for.</param>
        /// <returns>
        /// The job which id is the provided. The job will eagerly load all the entities
        /// that should be migrated.
        /// </returns>
        public Task<Job> GetByIdAsync(int id);

        /// <summary>
        /// Updates a Job in the repository
        /// </summary>
        /// <returns>The allready update job</returns>
        public Task<Job> Update(Job job);
    }
}
