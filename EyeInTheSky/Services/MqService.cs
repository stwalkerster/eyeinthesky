namespace EyeInTheSky.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using Castle.Core.Logging;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Model;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Client.Exceptions;
    using Stwalkerster.IrcClient;
    using Stwalkerster.IrcClient.Interfaces;

    public class MqService : IMqService
    {
        private readonly RabbitMqConfiguration mqConfig;
        private readonly ILogger logger;
        private readonly IIrcClient client;
        private IConnection connection;

        private readonly List<IModel> channels = new List<IModel>();
        private IModel managementChannel;
        private EventingBasicConsumer consumer;

        public MqService(RabbitMqConfiguration mqConfig, ILogger logger, IIrcClient freenodeClient)
        {
            this.mqConfig = mqConfig;
            this.logger = logger;
            this.client = freenodeClient;
        }

        public void Start()
        {
            this.logger.Debug("Starting MQ connection...");

            if (!this.mqConfig.Enabled)
            {
                this.logger.Warn("RabbitMQ disabled, refusing to start.");
                return;
            }
            
            var factory = new ConnectionFactory
            {
                HostName = this.mqConfig.Hostname,
                Port = this.mqConfig.Port,
                VirtualHost = this.mqConfig.VirtualHost,
                UserName = this.mqConfig.Username,
                Password = this.mqConfig.Password,
                ClientProvidedName = this.mqConfig.UserAgent,
                ClientProperties = new Dictionary<string, object>
                {
                    {"product", Encoding.UTF8.GetBytes("EyeInTheSky")},
                    {"version", Encoding.UTF8.GetBytes(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion)},
                    {"platform", Encoding.UTF8.GetBytes(RuntimeInformation.FrameworkDescription)},
                    {"os", Encoding.UTF8.GetBytes(Environment.OSVersion.ToString())}
                },
               Ssl = new SslOption{Enabled = this.mqConfig.Tls, ServerName = this.mqConfig.Hostname}
            };

            try
            {
                this.connection = factory.CreateConnection();
                this.logger.Debug("MQ connected.");
            }
            catch (BrokerUnreachableException ex)
            {
                this.logger.Error("MQ failed to connect.", ex);
                this.mqConfig.Enabled = false;
                return;
            }

            this.managementChannel = this.CreateChannel();
            
            var queue = this.mqConfig.ObjectPrefix + "errorlog";
            
            this.managementChannel.ExchangeDeclare(queue, "topic", true);
            this.managementChannel.QueueDeclare(queue, true, false, false);
            this.managementChannel.QueueBind(queue, queue, "#");
            
            var notificationQueue = this.mqConfig.ObjectPrefix + "notification";
            this.managementChannel.QueueDeclare(notificationQueue, true, false, false);
            
            this.consumer = new EventingBasicConsumer(this.managementChannel);
            this.consumer.Received += this.ConsumerOnReceived;
            
            this.managementChannel.BasicConsume(notificationQueue, true, this.consumer);

        }

        private void ConsumerOnReceived(object sender, BasicDeliverEventArgs e)
        {
            if (!e.BasicProperties.IsAppIdPresent())
            {
                this.logger.WarnFormat("Command message received with no app ID");
                return;
            }
            
            if (!e.BasicProperties.IsUserIdPresent())
            {
                this.logger.ErrorFormat("Command message received with no user ID");
                return;
            }

            var content = Encoding.UTF8.GetString(e.Body.ToArray());
            
            this.logger.InfoFormat(
                "Publishing message to {3} IRC target {4} per request of {1}@{2}: {0}",
                content,
                e.BasicProperties.UserId,
                e.BasicProperties.AppId,
                this.client.ClientName,
                e.RoutingKey);
            
            this.client.SendMessage(e.RoutingKey, content);
        }
        
        public void Stop()
        {
            if (!this.mqConfig.Enabled)
            {
                return;
            }

            this.consumer.Received -= this.ConsumerOnReceived;
            this.consumer = null;
            this.ReturnChannel(this.managementChannel);
            this.managementChannel = null;
            
            foreach (var channel in this.channels)
            {
                channel.Close();
            }

            this.connection.Close();
        }

        public IModel CreateChannel()
        {
            if (!this.mqConfig.Enabled || !this.connection.IsOpen)
            {
                return null;
            }

            var model = this.connection.CreateModel();
            this.channels.Add(model);
            return model;
        }

        public void ReturnChannel(IModel channel)
        {
            if (channel == null)
            {
                return;
            }

            if (this.channels.Contains(channel))
            {
                this.channels.Remove(channel);
                if (channel.IsOpen)
                {
                    channel.Close();
                }
            }
        }
    }
}