﻿using System.Text;
using Azure.Messaging.ServiceBus;
using Cardo.Services.EmailAPI.Models.Dto;
using Cardo.Services.EmailAPI.Services;
using Newtonsoft.Json;

namespace Cardo.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string registerUserQueue;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        private ServiceBusProcessor _emailCartProcessor;
        private ServiceBusProcessor _registerUserProcessor;


        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _emailService = emailService;
            _configuration = configuration;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");

            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);

            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
            _registerUserProcessor = client.CreateProcessor(registerUserQueue);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();

            _registerUserProcessor.ProcessMessageAsync += OnUserRegisterRequestReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
            await _registerUserProcessor.StartProcessingAsync();
        }


        public async Task Stop()
        {
            await _emailCartProcessor.StartProcessingAsync();
            await _emailCartProcessor.DisposeAsync();

            await _registerUserProcessor.StartProcessingAsync();
            await _registerUserProcessor.DisposeAsync();
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
                await _emailService.EmailCartAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                //log error
                throw;
            }
        }
        
        private async Task OnUserRegisterRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            //receiving a CartDto here and deserialize it
            string email = JsonConvert.DeserializeObject<string>(body);
            try
            {
                //TOTO - try to log email
                await _emailService.RegisterUserEmailAndLog(email);
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