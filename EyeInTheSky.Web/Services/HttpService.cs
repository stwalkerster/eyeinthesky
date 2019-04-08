namespace EyeInTheSky.Web.Services
{
    using System;
    using Castle.Core;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using Nancy.Hosting.Self;

    public class HttpService : IStartable, IDisposable
    {
        private readonly ILogger logger;
        private NancyHost server;
        private readonly string listenHostPort;

        public HttpService(ILogger logger, IAppConfiguration appConfiguration)
        {
            this.logger = logger;
            this.listenHostPort = appConfiguration.WebServiceHostPort;
        }

        public void Start()
        {
            if (this.listenHostPort == null)
            {
                this.logger.Warn("Web service is disabled and will not start!");
                return;
            }

            try
            {
                this.logger.Debug("Starting management web service");
                this.server = new NancyHost(
                    new HostConfiguration {RewriteLocalhost = false},
                    new Uri("http://" + this.listenHostPort));

                this.server.Start();
                this.logger.InfoFormat("Started management web service on http://{0}", this.listenHostPort);
            }
            catch (Exception ex)
            {
                this.logger.Error("Error encountered starting web interface", ex);
                // throw;
            }
        }

        public void Stop()
        {
            if (this.listenHostPort == null)
            {
                return;
            }

            this.logger.Info("Stopping management web service");
            this.server.Stop();
        }

        public void Dispose()
        {
            if (this.server != null) this.server.Dispose();
        }
    }
}