namespace EyeInTheSky.Startup
{
    using Castle.Facilities.EventWiring;
    using Castle.Facilities.Logging;
    using Castle.Facilities.Startable;
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.MicroKernel.SubSystems.Conversion;
    using Castle.Services.Logging.Log4netIntegration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using EyeInTheSky.Services.RecentChanges.Irc;
    using EyeInTheSky.Startables;
    using EyeInTheSky.Startup.Converters;
    using EyeInTheSky.TypedFactories;

    using Stwalkerster.Bot.CommandLib.Services;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.Bot.MediaWikiLib.Services;
    using Stwalkerster.Bot.MediaWikiLib.Services.Interfaces;
    using Stwalkerster.IrcClient;
    using Stwalkerster.IrcClient.Interfaces;

    public class MainInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Facilities
            container.AddFacility<LoggingFacility>(f => f.LogUsing<Log4netFactory>().WithConfig("log4net.xml"));
            container.AddFacility<EventWiringFacility>();
            container.AddFacility<StartableFacility>(f => f.DeferredStart());
            container.AddFacility<TypedFactoryFacility>();

            // Configuration converters
            var conversionManager =
                (IConversionManager) container.Kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);
            conversionManager.Add(new MediaWikiConfigMapEntryConverter());
            
            container.Install(
                Configuration.FromXmlFile("alert-templates.xml"),
                new Stwalkerster.IrcClient.Installer(),
                new Stwalkerster.Bot.CommandLib.Startup.Installer()
            );

            container.Register(
                // MediaWiki stuff
                Component.For<IMediaWikiApiTypedFactory>().AsFactory(),
                Component.For<IMediaWikiApi>().ImplementedBy<MediaWikiApi>().LifestyleTransient(),
                Component.For<IWebServiceClient>().ImplementedBy<WebServiceClient>(),
                
                // Services
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Services").WithServiceAllInterfaces(),
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Services.ExternalProviders").WithServiceAllInterfaces(),
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Services.RecentChanges").WithServiceAllInterfaces(),
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Services.RecentChanges.Irc").WithServiceAllInterfaces(),
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Commands").LifestyleTransient()
                    .Configure(x => x.DependsOn(Dependency.OnComponent("wikimediaClient", "wikimediaClient"))),

                // Main application
                Component.For<IApplication>()
                    .ImplementedBy<Launch>()
                    .DependsOn(Dependency.OnComponent("wikimediaClient", "wikimediaClient")),

                Component.For<IIrcClient>()
                    .ImplementedBy<IrcClient>()
                    .IsDefault()
                    .DependsOn(Dependency.OnComponent("configuration", "freenodeIrcConfig"))
                    .PublishEvent(
                        p => p.ReceivedMessage += null,
                        x => x.To<CommandHandler>(l => l.OnMessageReceived(null, null))),
                Component.For<IIrcClient>()
                    .ImplementedBy<IrcClient>()
                    .Named("wikimediaClient")
                    .DependsOn(Dependency.OnComponent("configuration", "wikimediaIrcConfig"))
                    .PublishEvent(
                        p => p.ReceivedMessage += null,
                        x => x.To<IrcRecentChangeHandler>(l => l.OnReceivedMessage(null, null))),

                // Linked to IRC services, so needs special configuration.
                Component.For<NagiosMonitoringService>()
                    .DependsOn(Dependency.OnComponent("wikimediaClient", "wikimediaClient"))
            );
        }
    }
}