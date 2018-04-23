namespace EyeInTheSky.Startables
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using Castle.Core;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;

    /// <summary>
    /// NagiosMonitoringService monitoring service
    /// </summary>
    public class NagiosMonitoringService : IStartable
    {
        private readonly ILogger logger;
        private readonly IIrcClient freenode;
        private readonly IIrcClient wikimedia;
        private readonly TcpListener tcpService;

        private readonly bool enabled;
        
        private bool isAlive;
        private readonly Thread monitorthread;

        /// <summary>
        /// Initializes a new instance of the NagiosMonitoringService class.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="configuration"></param>
        /// <param name="freenodeClient"></param>
        /// <param name="wikimediaClient"></param>
        public NagiosMonitoringService(ILogger logger, IAppConfiguration configuration, IIrcClient freenodeClient, IIrcClient wikimediaClient)
        {
            this.logger = logger;
            this.freenode = freenodeClient;
            this.wikimedia = wikimediaClient;

            var port = configuration.MonitoringPort;
            this.enabled = port != 0;

            if (!this.enabled)
            {
                this.logger.WarnFormat("{0} is disabled and will not function.", this.GetType().Name);
                return;
            }

            this.monitorthread = new Thread(this.MonitorThreadMethod);

            this.tcpService = new TcpListener(IPAddress.Any, port);

            this.logger.Info("Initialised Monitoring Client.");
        }

        private void MonitorThreadMethod()
        {
            this.isAlive = true;
            this.tcpService.Start();
            this.logger.DebugFormat("Started Monitoring Client at {0}", this.tcpService.LocalEndpoint);
           
            while (this.isAlive)
            {
                try
                {
                    if (!this.tcpService.Pending())
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    
                    this.logger.Debug("Found waiting request.");

                    var client = this.tcpService.AcceptTcpClient();

                    var sw = new StreamWriter(client.GetStream());

                    sw.WriteLine(
                        "EyeInTheSky - {0}: {1}; {2}: {3}",
                        this.freenode.ClientName,
                        this.freenode.NetworkConnected ? "connected" : "disconnected",
                        this.wikimedia.ClientName,
                        this.wikimedia.NetworkConnected ? "connected" : "disconnected");
                    
                    sw.Flush();
                    client.Close();
                }
                catch (Exception ex)
                {
                    this.logger.Error("Error encountered in monitoring thread.", ex);
                    throw;
                }
            }
            
            this.tcpService.Stop();
            this.logger.Info("Stopped Monitoring Client.");
        }
        
        /// <summary>
        ///     The start.
        /// </summary>
        public void Start()
        {
            if (!this.enabled)
            {
                return;
            }

            this.logger.Info("Starting Monitoring Client...");
            this.monitorthread.Start();
        }

        /// <summary>
        /// Stop all threads in this instance to allow for a clean shutdown.
        /// </summary>
        public void Stop()
        {
            this.logger.Info("Stopping Monitoring Client.");
            this.isAlive = false;
        }
    }
}