using Microsoft.EntityFrameworkCore;
using BankingMigration.Domain.Jobs;
using BankingMigration.Data.Context;

namespace BankingMigration.Data.Migration
{
    public class JobsRepository : IJobsRepository
    {
        private readonly MigrationJobsDbContext context;

        public JobsRepository(MigrationJobsDbContext context)
        {
            this.context = context;
        }

        public async Task<int> CreateAsync(Job job)
        {
            var createdJob = context.Jobs.Add(job);
            await context.SaveChangesAsync();
            return createdJob.Entity.Id;

        }

        public async Task<Job> GetByIdAsync(int id)
        {
            var job = await context.Jobs.Include(x => x.BancAccounts).FirstOrDefaultAsync(x => x.Id == id);
            if (job == null) 
            {
                throw new Exception($"No such job with id {id} exists in the system. Please, verify that the provided id is correct");
            }
            return job;
        }

        public async Task<Job> Update(Job job)
        {
            var updatedJob = context.Jobs.Update(job);
            await context.SaveChangesAsync();
            return updatedJob.Entity;
        }
    }
}
