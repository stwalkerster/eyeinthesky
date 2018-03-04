#region Usings

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

#endregion

namespace EyeInTheSky
{
    using Castle.Core.Logging;

    /// <summary>
    /// Nagios monitoring service
    /// </summary>
    internal class Nagios
    {
        private readonly ILogger logger;
        private readonly TcpListener tcpService;

        private bool isAlive;


        private const string Message = "EyeInTheSky (Nagios Monitor service)";

        /// <summary>
        /// Initializes a new instance of the Nagios class.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="port">The port.</param>
        public Nagios(ILogger logger, int port = 62168)
        {
            this.logger = logger;
            
            this.logger.InfoFormat("Starting monitoring service on port {0}", port);
            this.tcpService = new TcpListener(IPAddress.Any, port);
            
            new Thread(this.MonitorThreadMethod).Start();
        }

        private void MonitorThreadMethod()
        {
            this.isAlive = true;
            this.tcpService.Start();
            try
            {
                while (this.isAlive)
                {
                    try
                    {
                        if (!this.tcpService.Pending())
                        {
                            Thread.Sleep(10);
                            continue;
                        }

                        var client = this.tcpService.AcceptTcpClient();

                        var sw = new StreamWriter(client.GetStream());

                        sw.WriteLine(Message);
                        sw.Flush();
                        client.Close();
                    }
                    catch (Exception ex)
                    {
                        this.logger.Error("Error encountered in monitoring thread.", ex);
                        throw;
                    }
                }
            }
            finally
            {
                this.tcpService.Stop();
            }
        }

        /// <summary>
        /// Stop all threads in this instance to allow for a clean shutdown.
        /// </summary>
        public void Stop()
        {
            this.isAlive = false;
        }
    }
}