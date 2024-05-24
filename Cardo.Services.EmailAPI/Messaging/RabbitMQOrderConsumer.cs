using System.Text;
using Cardo.Services.EmailAPI.Services;
using RabbitMQ.Client;
using System.Threading.Channels;
using Cardo.Services.EmailAPI.Message;
using Cardo.Services.EmailAPI.Models.Dto;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace Cardo.Services.EmailAPI.Messaging
{
    public class RabbitMQOrderConsumer : BackgroundService
    {

        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private IConnection _connection;
        private IModel _channel;
        private const string OrderCreated_EmailUpdateQueue = "EmailUpdateQueue";
        private string ExchangeName = "";
        string queueName = "";

        public RabbitMQOrderConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            ExchangeName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");


            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Password = "guest",
                UserName = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName , ExchangeType.Direct);
            _channel.QueueDeclare(OrderCreated_EmailUpdateQueue, false, false, false, null);
            //binding the queue to the exchange
            _channel.QueueBind(OrderCreated_EmailUpdateQueue, ExchangeName, "EmailUpdate");
        }


        //ch = channel ea = event args
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                RewardsMessage rewardsMessage = JsonConvert.DeserializeObject<RewardsMessage>(content);

                HandleMessage(rewardsMessage).GetAwaiter().GetResult();

                _channel.BasicAck(ea.DeliveryTag, false);

            };
            _channel.BasicConsume(OrderCreated_EmailUpdateQueue, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(RewardsMessage rewardsMessage)
        {
            _emailService.LogOrderPlaced(rewardsMessage).GetAwaiter().GetResult();
        }
    }
}
