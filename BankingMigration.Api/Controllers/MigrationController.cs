using BankingMigration.Domain.Jobs;
using BankingMigration.Domain.Jobs.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingMigration.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MigrationController : ControllerBase
    {
        private readonly ILogger<MigrationController> _logger;
        private readonly IJobUseCases jobUseCases;

        public MigrationController(ILogger<MigrationController> logger, IJobUseCases jobUseCases)
        {
            _logger = logger;
            this.jobUseCases = jobUseCases;
        }

        /// <summary>
        /// Starts a new Migration process
        /// </summary>
        /// <param name="migrationJob">The migration payload containing the type of migration to execute and the items to migrate</param>
        /// <returns>The id of the migration job for a proper follow up.</returns>
        [HttpPost(Name = "Run")]
        [Authorize("run:migration")]
        public async Task<IActionResult> RunMigration(Job migrationJob)
        {
            try
            {
                _logger.LogDebug("New migration process requested.", [migrationJob]);
                var createdJob = await jobUseCases.CreateJobAsync(migrationJob);

                return Ok(createdJob);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Starts a new Migration process
        /// </summary>
        /// <param name="jobId">The id of the job to provide the summary or status for.</param>
        /// <returns>The up to date status of a given migration job</returns>
        [HttpGet("{jobId}/status")]
        [Authorize("run:migration")]
        public async Task<IActionResult> GetJobStatus(int jobId)
        {
            _logger.LogDebug("Status requested for migration job.", [jobId]);
            try
            {
                var jobStatus = await jobUseCases.GetJobStatus(jobId);

                return Ok(jobStatus);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
