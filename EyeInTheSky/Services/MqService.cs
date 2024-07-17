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
    using Model;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;

    public class MqService : IMqService
    {
        private readonly RabbitMqConfiguration mqConfig;
        private readonly ILogger logger;
        private IConnection connection;

        private readonly List<IModel> channels = new List<IModel>();
        private IModel managementChannel;

        public MqService(RabbitMqConfiguration mqConfig, ILogger logger)
        {
            this.mqConfig = mqConfig;
            this.logger = logger;
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
        }
        
        public void Stop()
        {
            if (!this.mqConfig.Enabled)
            {
                return;
            }

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