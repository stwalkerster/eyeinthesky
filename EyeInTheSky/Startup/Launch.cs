namespace EyeInTheSky.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services;
    using EyeInTheSky.Services.Interfaces;
    using Prometheus;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.Bot.MediaWikiLib.Services;
    using Stwalkerster.IrcClient;
    using Stwalkerster.IrcClient.Interfaces;

    public class Launch : IApplication
    {
        private static readonly Gauge VersionInfo = Metrics.CreateGauge(
            "eyeinthesky_build_info",
            "Build info",
            new GaugeConfiguration
            {
                LabelNames = new[] {"assembly", "irclib", "commandlib", "mediawikilib", "runtime", "os"}
            });
        
        private static WindsorContainer container;

        private readonly ILogger logger;
        private readonly IIrcClient freenodeClient;
        private readonly IIrcClient wikimediaClient;
        private readonly IAppConfiguration appConfig;
        private readonly IChannelConfiguration channelConfiguration;
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
            container.Install(FromAssembly.This(), Configuration.FromXmlFile("modules.xml"));

            MetricServer metricsServer;
            var appConfig = container.Resolve<IAppConfiguration>();
            if (appConfig.MetricsPort != 0)
            {
                metricsServer = new MetricServer(appConfig.MetricsPort);
                metricsServer.Start();

                VersionInfo.WithLabels(
                        FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion,
                        FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(IrcClient)).Location)
                            .FileVersion,
                        FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(CommandBase)).Location)
                            .FileVersion,
                        FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(MediaWikiApi)).Location)
                            .FileVersion,
                        Environment.Version.ToString(),
                        Environment.OSVersion.ToString()
                    )
                    .Set(1);
            }

            var app = container.Resolve<IApplication>();

            app.Run();
            container.Release(app);
            return 0;
        }

        public Launch(
            ILogger logger,
            IIrcClient freenodeClient,
            IIrcClient wikimediaClient,
            IAppConfiguration appConfig,
            ITemplateConfiguration templateConfiguration,
            IBotUserConfiguration userConfiguration,
            IChannelConfiguration channelConfiguration,
            IStalkFactory stalkFactory,
            IFileService fileService)
        {
            this.logger = logger;
            this.freenodeClient = freenodeClient;
            this.wikimediaClient = wikimediaClient;
            this.appConfig = appConfig;
            this.channelConfiguration = channelConfiguration;

            if (!this.channelConfiguration.Items.Any())
            {
                this.logger.InfoFormat("Migrating to channel configuration file...");

                var defaultChannel = new IrcChannel(this.appConfig.FreenodeChannel);
                this.channelConfiguration.Add(defaultChannel);

                if (appConfig.StalkConfigFile != null)
                {
                    var stalkConfig = new StalkConfiguration(
                        appConfig,
                        logger.CreateChildLogger("LegacyStalkConfig"),
                        stalkFactory,
                        fileService
                    );
                    stalkConfig.Initialize();

                    foreach (var stalk in stalkConfig.Items)
                    {
                        stalk.Channel = this.appConfig.FreenodeChannel;
                        defaultChannel.Stalks.Add(stalk.Identifier, stalk);
                        stalkConfig.Remove(stalk.Identifier);
                    }

                    stalkConfig.Save();
                }

                this.channelConfiguration.Save();
            }

            this.logger.InfoFormat(
                "Tracking {0} stalks, {1} templates, {2} users, and {3} channels.",
                this.channelConfiguration.Items.Aggregate(0, (i, channel) => i + channel.Stalks.Count),
                templateConfiguration.Items.Count,
                userConfiguration.Items.Count,
                channelConfiguration.Items.Count
            );

            this.freenodeClient.DisconnectedEvent += this.OnDisconnect;
            this.wikimediaClient.DisconnectedEvent += this.OnDisconnect;
        }

        public void Run()
        {
            var watchChannels = new List<string> {this.appConfig.WikimediaChannel};
            foreach (var channel in this.channelConfiguration.Items)
            {
                this.freenodeClient.JoinChannel(channel.Identifier);

                foreach (var stalk in channel.Stalks.Values)
                {
                    if (!watchChannels.Contains(stalk.WatchChannel) && !string.IsNullOrWhiteSpace(stalk.WatchChannel) )
                    {
                        watchChannels.Add(stalk.WatchChannel);
                    }
                }
            }

            foreach (var channel in watchChannels)
            {
                this.wikimediaClient.JoinChannel(channel);
            }

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