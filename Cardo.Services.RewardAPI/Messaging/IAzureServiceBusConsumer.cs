namespace Cardo.Services.RewardAPI.Messaging
{
    /// <summary>
    /// Represents an interface for managing Azure Service Bus consumers.
    /// </summary>
    public interface IAzureServiceBusConsumer
    {
        /// <summary>
        /// Starts the Azure Service Bus consumer.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Start();
        /// <summary>
        /// Stops the Azure Service Bus consumer.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Stop();
    }
}
