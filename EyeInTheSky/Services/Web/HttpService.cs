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
            this.server = new NancyHost(new Uri("http://0.0.0.0:8080"));
            this.server.Start();
        }

        public void Stop()
        {
            this.server.Stop();
        }

        public void Dispose()
        {
            if (this.server != null) this.server.Dispose();
        }
    }
}