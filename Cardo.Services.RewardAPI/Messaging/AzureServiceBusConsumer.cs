using System.Text;
using Azure.Messaging.ServiceBus;
using Cardo.Services.RewardAPI.Message;
using Cardo.Services.RewardAPI.Services;
using Newtonsoft.Json;

namespace Cardo.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedRewardSubscription;
        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;

        private ServiceBusProcessor _rewardProcessor;


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

        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;
            await _rewardProcessor.StartProcessingAsync();

        }


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

            //receiving a CartDto here and deserialize it
            RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);
            try
            {
                //TOTO - try to log email
                await _rewardService.UpdateRewards(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                //log error
                throw;
            }
        }
       
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            //can also send email someone here if an error occurs
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
            
        }

    }
}
