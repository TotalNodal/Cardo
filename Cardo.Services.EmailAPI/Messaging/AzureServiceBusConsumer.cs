using System.Text;
using Azure.Messaging.ServiceBus;
using Cardo.Services.EmailAPI.Models.Dto;
using Newtonsoft.Json;

namespace Cardo.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly IConfiguration _configuration;

        private ServiceBusProcessor _emailCartProcessor;


        public AzureServiceBusConsumer(IConfiguration configuration)
        {
            _configuration = configuration;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");

            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);

            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();
        }


        public async Task Stop()
        {
            await _emailCartProcessor.StartProcessingAsync();
            await _emailCartProcessor.DisposeAsync();
        }


        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where we will send the email
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            //receiving a CartDto here and deserialize it
            CartDto objMessage = JsonConvert.DeserializeObject<CartDto>(body);
            try
            {
                //TOTO - try to log email
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
