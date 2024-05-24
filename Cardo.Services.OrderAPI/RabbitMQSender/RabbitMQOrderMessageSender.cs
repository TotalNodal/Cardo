﻿using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Cardo.Services.OrderAPI.RabbitMQSender
{
    public class RabbitMQOrderMessageSender : IRabbitMQOrderMessageSender
    {
        private readonly string _hostName;
        private readonly string _username;
        private readonly string _password;
        private IConnection _connection;
        private const string OrderCreated_RewardsUpdateQueue = "RewardsUpdateQueue";
        private const string OrderCreated_EmailUpdateQueue = "EmailUpdateQueue";

        public RabbitMQOrderMessageSender()
        {
            _hostName = "localhost";
            _password = "guest";
            _username = "guest";
        }

        //direct example
        public void SendMessage(object message, string exchangeName)
        {
            if (ConnectionExists())
            {
                //creates a channel to communicate with RabbitMQ
                using var channel = _connection.CreateModel();
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: false);
                channel.QueueDeclare(OrderCreated_RewardsUpdateQueue, false, false, false, null);
                channel.QueueDeclare(OrderCreated_EmailUpdateQueue, false, false, false, null);

                channel.QueueBind(OrderCreated_RewardsUpdateQueue, exchangeName, "RewardsUpdate");
                channel.QueueBind(OrderCreated_EmailUpdateQueue, exchangeName, "EmailUpdate");

                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);
                channel.BasicPublish(exchange: exchangeName, "RewardsUpdate", null, body: body);
                channel.BasicPublish(exchange: exchangeName, "EmailUpdate", null, body: body);
            }
        }

        /* fan out example
        public void SendMessage(object message, string exchangeName)
        {
            if (ConnectionExists())
            {
                //creates a channel to communicate with RabbitMQ
                using var channel = _connection.CreateModel();
                channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable: false);
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);
                channel.BasicPublish(exchange: exchangeName, routingKey: "", null, body: body);
            }
        }*/

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _hostName,
                    UserName = _username,
                    Password = _password
                };

                //establishes connection to RabbitMQ factory
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                
            }
            
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }
            CreateConnection();
            return true;
        }
    }
}
