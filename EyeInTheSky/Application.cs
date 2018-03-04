namespace EyeInTheSky
{
    using System;
    using System.Threading;
    using Castle.Core.Logging;
    using Stwalkerster.IrcClient.Interfaces;

    public class Application
    {
        private readonly ILogger logger;
        private readonly IIrcClient freenodeClient;
        private readonly IIrcClient wikimediaClient;
        private readonly AppConfiguration appConfig;

        public Application(ILogger logger, IIrcClient freenodeClient, IIrcClient wikimediaClient, AppConfiguration appConfig)
        {
            this.logger = logger;
            this.freenodeClient = freenodeClient;
            this.wikimediaClient = wikimediaClient;
            this.appConfig = appConfig;

            this.freenodeClient.DisconnectedEvent += this.OnDisconnect;
            this.wikimediaClient.DisconnectedEvent += this.OnDisconnect;
        }

        public void Run()
        {
            this.freenodeClient.JoinChannel(this.appConfig.FreenodeChannel);
            
            bool alive = true;
            
            while (alive)
            {
                Thread.Sleep(1000);    
            }
        }

        public void OnDisconnect(object sender, EventArgs e)
        {
            this.logger.Error("Disconnected from IRC!");
        }
    }
}