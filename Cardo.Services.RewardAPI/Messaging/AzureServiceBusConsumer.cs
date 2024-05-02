using System.Text;
using Azure.Messaging.ServiceBus;
using Cardo.Services.RewardAPI.Message;
using Cardo.Services.RewardAPI.Services;
using Newtonsoft.Json;

namespace Cardo.Services.RewardAPI.Messaging
{
    /// <summary>
    /// Represents a consumer for Azure Service Bus messages.
    /// </summary>
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedRewardSubscription;
        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;

        private ServiceBusProcessor _rewardProcessor;


        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServiceBusConsumer"/> class.
        /// </summary>
        /// <param name="configuration">The configuration containing service bus connection information.</param>
        /// <param name="rewardService">The service for handling rewards.</param>
        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _rewardService = rewardService;
            _configuration = configuration;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");

            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreatedRewardSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription");

            var client = new ServiceBusClient(serviceBusConnectionString);

            _rewardProcessor = client.CreateProcessor(orderCreatedTopic,orderCreatedRewardSubscription);
        }

        /// <summary>
        /// Starts processing messages from Azure Service Bus.
        /// </summary>
        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;
            await _rewardProcessor.StartProcessingAsync();

        }

        /// <summary>
        /// Stops processing messages from Azure Service Bus.
        /// </summary>
        public async Task Stop()
        {
            await _rewardProcessor.StartProcessingAsync();
            await _rewardProcessor.DisposeAsync();
        }


        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where we will send the email
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            // Deserialize the message body into a RewardsMessage object
            RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);
            try
            {
                // Update rewards based on the received message
                await _rewardService.UpdateRewards(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                // Log error and rethrow
                throw;
            }
        }
       
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            // Handle and log errors
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
            
        }

    }
}
