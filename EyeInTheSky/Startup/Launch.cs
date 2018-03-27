namespace EyeInTheSky.Startup
{
    using System;
    using System.IO;
    using System.Threading;
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using EyeInTheSky.Model.Interfaces;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;

    public class Launch : IApplication
    {
        private static WindsorContainer container;
        
        private readonly ILogger logger;
        private readonly IIrcClient freenodeClient;
        private readonly IIrcClient wikimediaClient;
        private readonly IAppConfiguration appConfig;
        private bool alive = true;
        
        public static int Main(string[] args)
        {
            string configurationFile = "configuration.xml";
            
            if (args.Length >= 1)
            {
                configurationFile = args[0];
            }

            if (!File.Exists(configurationFile))
            {
                Console.WriteLine("Configuration file does not exist!");
                return 1;
            }
            
            container = new WindsorContainer(configurationFile);
            container.Install(FromAssembly.This());

            var app = container.Resolve<IApplication>();

            app.Run();
            container.Release(app);
            return 0;
        }
       
        public Launch(ILogger logger, IIrcClient freenodeClient, IIrcClient wikimediaClient, IAppConfiguration appConfig)
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