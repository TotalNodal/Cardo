﻿namespace Cardo.Services.AuthAPI.RabbitMQSender
{
    public interface IRabbitMQAuthMessageSender
    {
        void SendMessage(Object message, string queueName);
    }
}
