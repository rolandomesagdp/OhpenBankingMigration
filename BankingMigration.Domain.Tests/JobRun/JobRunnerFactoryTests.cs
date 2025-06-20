using BankingMigration.Domain.BankAccounts;
using BankingMigration.Domain.JobRun;
using BankingMigration.Domain.Jobs;
using BankingMigration.Domain.Migration;
using BankingMigration.Domain.Tests.Jobs;
using Microsoft.Extensions.Logging;
using Moq;

namespace BankingMigration.Domain.Tests.JobRun
{
    [TestClass]
    public sealed class JobRunnerFactoryTests
    {
        private Job job;
        private IMigrationService migrationService = Mock.Of<IMigrationService>();
        private IJobsRepository jobRepository = Mock.Of<IJobsRepository>();
        private IBankAccountRepository accountRepository = Mock.Of<IBankAccountRepository>();
        private ILogger logger = Mock.Of<ILogger>();

        [TestMethod]
        public void ShouldCreateBatchRunner_IfJobTypeIsBatch()
        {
            // Arrange
            job = JobMockData.GetJob();
            job.SetStatus(JobStatus.Created);
            job.SetType(JobType.Batch);
            var factory = new JobRunnerFactory(job, migrationService, jobRepository, accountRepository, logger);

            // Act
            var runner = factory.Create();

            // Assert
            Assert.IsInstanceOfType(runner, typeof(Batch));
        }

        [TestMethod]
        public void ShouldCreateBulkRunner_IfJobTypeIsBulk()
        {
            // Arrange
            job = JobMockData.GetJob();
            job.SetStatus(JobStatus.Created);
            job.SetType(JobType.Bulk);
            var factory = new JobRunnerFactory(job, migrationService, jobRepository, accountRepository, logger);

            // Act
            var runner = factory.Create();

            // Assert
            Assert.IsInstanceOfType(runner, typeof(Bulk));
        }
    }
}
