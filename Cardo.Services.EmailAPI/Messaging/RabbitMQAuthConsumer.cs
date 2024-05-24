﻿using System.Text;
using Cardo.Services.EmailAPI.Services;
using RabbitMQ.Client;
using System.Threading.Channels;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace Cardo.Services.EmailAPI.Messaging
{
    public class RabbitMQAuthConsumer : BackgroundService
    {

        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private IConnection _connection;
        private IModel _channel;


        public RabbitMQAuthConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Password = "guest",
                UserName = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"), false, false, false, null);
        }


        //ch = channel ea = event args
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                String email = JsonConvert.DeserializeObject<String>(content);

                HandleMessage(email).GetAwaiter().GetResult();

                _channel.BasicAck(ea.DeliveryTag, false);

            };
            _channel.BasicConsume(_configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"), false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(string email)
        {
            _emailService.RegisterUserEmailAndLog(email).GetAwaiter().GetResult();
        }
    }
}