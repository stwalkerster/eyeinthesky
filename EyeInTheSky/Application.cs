namespace EyeInTheSky
{
    using System;
    using System.Threading;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using Stwalkerster.Bot.CommandLib.Services;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;

    public class Application : IApplication
    {
        private readonly ILogger logger;
        private readonly IIrcClient freenodeClient;
        private readonly IIrcClient wikimediaClient;
        private readonly IAppConfiguration appConfig;
        private bool alive = true;

        public Application(ILogger logger, IIrcClient freenodeClient, IIrcClient wikimediaClient, IAppConfiguration appConfig)
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
            this.wikimediaClient.JoinChannel(this.appConfig.WikimediaChannel);
            
            while (this.alive)
            {
                Thread.Sleep(1000);    
            }
        }

        public void OnDisconnect(object sender, EventArgs e)
        {
            this.logger.Error("Disconnected from IRC!");
        }

        public void Stop()
        {
            this.alive = false;
        }
    }
}