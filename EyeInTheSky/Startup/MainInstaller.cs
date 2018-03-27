namespace EyeInTheSky.Startup
{
    using Castle.Facilities.EventWiring;
    using Castle.Facilities.Logging;
    using Castle.Facilities.Startable;
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Services.Logging.Log4netIntegration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using EyeInTheSky.Services;
    using EyeInTheSky.Startables;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
    using Stwalkerster.Bot.CommandLib.Services;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.TypedFactories;
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

            container.Install(Configuration.FromXmlFile("templates.xml"));
            
            container.Register(
                // CommandLib commands
                Classes.FromAssemblyContaining<CommandBase>().BasedOn<ICommand>().LifestyleTransient(),
                // CommandLib services
                Classes.FromAssemblyContaining<CommandParser>()
                    .InSameNamespaceAs<CommandParser>()
                    .WithServiceAllInterfaces(),
                
                // Factories
                Component.For<ICommandTypedFactory>().AsFactory(),
                
                // Services
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Services").WithServiceAllInterfaces(),
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Commands").LifestyleTransient(),
                
                // Main application
                Component.For<IApplication>()
                    .ImplementedBy<Launch>()
                    .DependsOn(
                        Dependency.OnComponent("freenodeClient", "freenodeClient"),
                        Dependency.OnComponent("wikimediaClient", "wikimediaClient")
                    ),
                
                // Individual components
                Component.For<ICommandHandler>().ImplementedBy<CommandHandler>().Named("commandHandler"),
                Component.For<RecentChangeHandler>().Named("rcHandler")
                    .DependsOn(
                        Dependency.OnComponent("freenodeClient", "freenodeClient")
                    ),
                
                // IRC - not using separate installer because we need special configuration here.
                Component.For<ISupportHelper>().ImplementedBy<SupportHelper>(),
                Component.For<IIrcClient>()
                    .ImplementedBy<IrcClient>()
                    .Named("freenodeClient")
                    .DependsOn(Dependency.OnComponent("configuration", "freenodeIrcConfig"))
                    .Start()
                    .PublishEvent(p => p.ReceivedMessage += null, x=>x.To<CommandHandler>("commandHandler", l=>l.OnMessageReceived(null, null))),
                Component.For<IIrcClient>()
                    .ImplementedBy<IrcClient>()
                    .Named("wikimediaClient")
                    .DependsOn(Dependency.OnComponent("configuration", "wikimediaIrcConfig"))
                    .Start()
                    .PublishEvent(p => p.ReceivedMessage += null, x=>x.To<RecentChangeHandler>("rcHandler", l=>l.OnReceivedMessage(null, null))),
                
                // Linked to IRC services, so needs special configuration.
                Component.For<NagiosMonitoringService>()
                    .Start()
                    .DependsOn(
                        Dependency.OnComponent("freenodeClient", "freenodeClient"),
                        Dependency.OnComponent("wikimediaClient", "wikimediaClient")
                    )
            );
        }
    }
}