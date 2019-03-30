namespace EyeInTheSky.Services.Web
{
    using System;
    using Castle.Core;
    using Castle.Core.Logging;
    using Nancy.Hosting.Self;

    public class HttpService : IStartable, IDisposable
    {
        private ILogger logger;
        private ushort webListenPort;
        private NancyHost server;
    
        public HttpService(ILogger logger)
        {   
            this.logger = logger;
            this.webListenPort = 8080;
        }

        public void Start()
        {
            try
            {
                this.logger.Debug("Starting management web service");
                this.server = new NancyHost(
                    new HostConfiguration {RewriteLocalhost = true},
                    new Uri("http://localhost:8080"));
                this.server.Start();
                this.logger.Info("Started management web service");
                
            }
            catch (Exception ex)
            {
                this.logger.Error("Error encountered starting web interface", ex);
                throw;
            }
        }

        public void Stop()
        {
            this.logger.Info("Stopping management web service");
            this.server.Stop();
        }

        public void Dispose()
        {
            if (this.server != null) this.server.Dispose();
        }
    }
}