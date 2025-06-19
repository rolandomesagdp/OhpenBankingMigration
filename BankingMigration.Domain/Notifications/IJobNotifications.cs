namespace BankingMigration.Domain.Notifications
{
    public interface IJobNotifications
    {
        /// <summary>
        /// Notifies the creation of a new Migration Job
        /// in order to start the execution of the Migration.
        /// This service is supposed to be an implementation of
        /// an Azure Service Bus client that should send a message to
        /// Azure Service Bus so that Azure Service Bus can trigger the Azure Function
        /// that will actually run the migration job. The implementation of that
        /// Azure function lives in the project Migration.Func
        /// </summary>
        /// <param name="jobId"></param>
        public void NotifyJobMigrationCreated(int jobId);
    }
}
