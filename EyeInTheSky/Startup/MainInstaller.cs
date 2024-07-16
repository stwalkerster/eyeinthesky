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
    using Microsoft.Extensions.Logging;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
    using Stwalkerster.Bot.CommandLib.Services;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.TypedFactories;
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

            var loggerFactory = new LoggerFactory().AddLog4Net("log4net.xml");

            // Configuration converters
            var conversionManager =
                (IConversionManager) container.Kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);
            conversionManager.Add(new MediaWikiConfigMapEntryConverter());

            container.Install(
                Configuration.FromXmlFile("alert-templates.xml")
            );

            container.Register(
                // CommandParser
                Component.For<ILogger<CommandParser>>().UsingFactoryMethod(loggerFactory.CreateLogger<CommandParser>),
                Classes.FromAssemblyContaining<CommandBase>().BasedOn<ICommand>().LifestyleTransient(),
                Component.For<ICommandTypedFactory>().AsFactory(),
                Classes.FromAssemblyContaining<CommandParser>().InSameNamespaceAs<CommandParser>().WithServiceAllInterfaces(),
                
                // MediaWiki stuff
                Component.For<IMediaWikiApiTypedFactory>().AsFactory(),
                Component.For<IMediaWikiApi>().ImplementedBy<MediaWikiApi>().LifestyleTransient(),
                Component.For<IWebServiceClient>().ImplementedBy<WebServiceClient>(),

                // Services
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Services").WithServiceAllInterfaces(),
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Services.Email").WithServiceAllInterfaces(),
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Services.RecentChanges").WithServiceAllInterfaces(),
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Services.RecentChanges.Irc").WithServiceAllInterfaces(),
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Commands").LifestyleTransient()
                    .Configure(x => x.DependsOn(Dependency.OnComponent("wikimediaClient", "wikimediaClient"))),

                // Main application
                Component.For<IApplication>()
                    .ImplementedBy<Launch>()
                    .DependsOn(Dependency.OnComponent("wikimediaClient", "wikimediaClient")),

                
                Component.For<ILoggerFactory>().Instance(loggerFactory),
                Component.For<ILogger<SupportHelper>>().UsingFactoryMethod(loggerFactory.CreateLogger<SupportHelper>),
                
                // Support helper holds client-specific state, mark as transient so we get a different for each client.
                Component.For<ISupportHelper>().ImplementedBy<SupportHelper>().LifestyleTransient(),

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