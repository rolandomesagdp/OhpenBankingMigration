using BankingMigration.Domain.BankAccounts;
using BankingMigration.Domain.JobRun;
using BankingMigration.Domain.Jobs;
using BankingMigration.Domain.Migration;
using BankingMigration.Domain.Tests.Jobs;
using Microsoft.Extensions.Logging;
using Moq;

namespace BankingMigration.Domain.Tests.JobRun;

[TestClass]
public class JobRunnerTests
{
    private Job job;
    private IMigrationService migrationService = Mock.Of<IMigrationService>();
    private IJobsRepository jobRepository = Mock.Of<IJobsRepository>();
    private IBankAccountRepository accountRepository = Mock.Of<IBankAccountRepository>();
    private ILogger logger = Mock.Of<ILogger>();


    public void SetUp()
    {
        var migrationServiceMock = new Mock<IMigrationService>();
        migrationServiceMock.Setup(x => x.MigrateBankAccount(null)).Returns(Task.FromResult(MigrationStatus.Proccessed));
    }

    [TestMethod]
    public async Task Run_ShouldFail_IfJobIsNotValid()
    {
        // Arrange
        job = JobMockData.GetInvalidJob();
        job.SetStatus(JobStatus.Created);
        job.SetType(JobType.Batch);
        var factory = new JobRunnerFactory(job, migrationService, jobRepository, accountRepository, logger);
        var runner = factory.Create();

        // Act
        await runner.Run();

        // Assert
        Assert.AreEqual(job.Status, JobStatus.Failed);
    }

    [TestMethod]
    public async Task Run_ShouldFail_IfJobIsRunningAlready()
    {
        // Arrange
        job = JobMockData.GetInvalidJob();
        job.SetStatus(JobStatus.Running);
        job.SetType(JobType.Batch);
        var factory = new JobRunnerFactory(job, migrationService, jobRepository, accountRepository, logger);
        var runner = factory.Create();

        // Act
        await runner.Run();

        // Assert
        Assert.AreEqual(job.Status, JobStatus.Failed);
    }

    [TestMethod]
    public async Task Run_ShouldSucceed_IfJobIsValid()
    {
        // Arrange
        job = JobMockData.GetJob();
        job.SetStatus(JobStatus.Created);
        job.SetType(JobType.Batch);
        var factory = new JobRunnerFactory(job, migrationService, jobRepository, accountRepository, logger);
        var runner = factory.Create();

        // Act
        await runner.Run();

        // Assert
        Assert.AreEqual(job.Status, JobStatus.Succeed);
    }

    [TestMethod]
    public async Task StartRunning_ShouldSetAppropriateStatusOnJob()
    {
        // Arrange
        job = JobMockData.GetJob();
        job.SetStatus(JobStatus.Created);
        job.SetType(JobType.Batch);
        var factory = new JobRunnerFactory(job, migrationService, jobRepository, accountRepository, logger);
        var runner = factory.Create();

        // Act
        await runner.StartRunning();

        // Assert
        Assert.AreEqual(job.Status, JobStatus.Running);
    }

    [TestMethod]
    public async Task StartRunning_ShouldThrowException_IfJobRepoDependencyCallFails()
    {
        // Arrange
        job = JobMockData.GetJob();
        var jobRepoMock = new Mock<IJobsRepository>();
        jobRepoMock.Setup(x => x.Update(job))
            .Throws(() => new Exception("Some error in the update job process"));
        var failingJobRepo = jobRepoMock.Object;

        var factory = new JobRunnerFactory(job, migrationService, failingJobRepo, accountRepository, logger);
        var runner = factory.Create();

        // Assert
        await Assert.ThrowsExceptionAsync<Exception>(() => runner.StartRunning());
    }

    [TestMethod]
    public async Task EndRunning_ShouldAcceptSucceedJobStatus()
    {
        // Arrange
        job = JobMockData.GetJob();
        job.SetStatus(JobStatus.Created);
        job.SetType(JobType.Batch);
        var factory = new JobRunnerFactory(job, migrationService, jobRepository, accountRepository, logger);
        var runner = factory.Create();
        var succeedJobStatus = JobStatus.Succeed;

        // Act
        await runner.EndRunning(succeedJobStatus);

        // Assert
        Assert.AreEqual(job.Status, succeedJobStatus);
    }

    [TestMethod]
    public async Task EndRunning_ShouldAcceptFailedJobStatus()
    {
        // Arrange
        job = JobMockData.GetJob();
        job.SetStatus(JobStatus.Created);
        job.SetType(JobType.Batch);
        var factory = new JobRunnerFactory(job, migrationService, jobRepository, accountRepository, logger);
        var runner = factory.Create();
        var succeedJobStatus = JobStatus.Failed;

        // Act
        await runner.EndRunning(succeedJobStatus);

        // Assert
        Assert.AreEqual(job.Status, succeedJobStatus);
    }

    [TestMethod]
    public async Task EndRunning_ShouldFail_IfInvalidJobStatusIsProvided()
    {
        // Arrange
        job = JobMockData.GetJob();
        job.SetStatus(JobStatus.Created);
        job.SetType(JobType.Batch);
        var factory = new JobRunnerFactory(job, migrationService, jobRepository, accountRepository, logger);
        var runner = factory.Create();

        // Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => runner.EndRunning(JobStatus.Running));
    }

    [TestMethod]
    public async Task EndRunning_ShouldThrowException_IfJobRepoDependencyCallFails()
    {
        // Arrange
        job = JobMockData.GetJob();
        var jobRepoMock = new Mock<IJobsRepository>();
        jobRepoMock.Setup(x => x.Update(job))
            .Throws(() => new Exception("Some error in the update job process"));
        var failingJobRepo = jobRepoMock.Object;

        var factory = new JobRunnerFactory(job, migrationService, failingJobRepo, accountRepository, logger);
        var runner = factory.Create();

        // Assert
        await Assert.ThrowsExceptionAsync<Exception>(() => runner.EndRunning(JobStatus.Succeed));
    }

    [TestMethod]
    public async Task ProcessAccount_ShouldSetAccountStatusToProccessed_IfSuccess()
    {
        // Arrange
        job = JobMockData.GetJob();
        var factory = new JobRunnerFactory(job, migrationService, jobRepository, accountRepository, logger);
        var runner = factory.Create();

        // Act
        await runner.ProcessAccount(job.BancAccounts[0]);

        // Assert
        Assert.AreEqual(job.BancAccounts[0].Status, MigrationStatus.Proccessed);
    }

    [TestMethod]
    public async Task ProcessAccount_ShouldSetAccountStatusToFailed_IfFails()
    {
        // Arrange
        job = JobMockData.GetJob();
        var migrationServiceMock = new Mock<IMigrationService>();
        migrationServiceMock.Setup(x => x.MigrateBankAccount(job.BancAccounts[0]))
            .Throws(() => new Exception("Some error in the migration process"));
        var failingMigrationService = migrationServiceMock.Object;

        var factory = new JobRunnerFactory(job, failingMigrationService, jobRepository, accountRepository, logger);
        var runner = factory.Create();

        // Act
        try
        {
            await runner.ProcessAccount(job.BancAccounts[0]);
        }
        catch
        {
            // Assert
            Assert.AreEqual(job.BancAccounts[0].Status, MigrationStatus.Failed);
        }
    }

    [TestMethod]
    public async Task ProcessAccount_ShouldThrowException_IfMigrationServiceDependencyCallFails()
    {
        // Arrange
        job = JobMockData.GetJob();
        var migrationServiceMock = new Mock<IMigrationService>();
        migrationServiceMock.Setup(x => x.MigrateBankAccount(job.BancAccounts[0]))
            .Throws(() => new Exception("Some error in the migration process"));
        var failingMigrationService = migrationServiceMock.Object;

        var factory = new JobRunnerFactory(job, failingMigrationService, jobRepository, accountRepository, logger);
        var runner = factory.Create();

        // Assert
        await Assert.ThrowsExceptionAsync<Exception>(() => runner.ProcessAccount(job.BancAccounts[0]));
    }

    [TestMethod]
    public async Task ProcessAccount_ShouldThrowException_IfAccountRepoDependencyCallFails()
    {
        // Arrange
        job = JobMockData.GetJob();
        var accountRepoMock = new Mock<IBankAccountRepository>();
        accountRepoMock.Setup(x => x.UpdateBankAccount(job.BancAccounts[0]))
            .Throws(() => new Exception("Some error in Bank Account update process"));
        var failingBankRepo = accountRepoMock.Object;

        var factory = new JobRunnerFactory(job, migrationService, jobRepository, failingBankRepo, logger);
        var runner = factory.Create();

        // Assert
        await Assert.ThrowsExceptionAsync<Exception>(() => runner.ProcessAccount(job.BancAccounts[0]));
    }
}
