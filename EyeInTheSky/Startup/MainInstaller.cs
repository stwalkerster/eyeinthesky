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
    using EyeInTheSky.Model;
    using EyeInTheSky.Services;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
    using Stwalkerster.Bot.CommandLib.Services;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.TypedFactories;
    using Stwalkerster.IrcClient;
    using Stwalkerster.IrcClient.Interfaces;
    
    using LegacyCommandHandler = EyeInTheSky.Services.CommandHandler;
    using NewCommandHandler = Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.CommandHandler;

    public class MainInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Facilities
            container.AddFacility<LoggingFacility>(f => f.LogUsing<Log4netFactory>().WithConfig("log4net.xml"));
            container.AddFacility<EventWiringFacility>();
            container.AddFacility<StartableFacility>(f => f.DeferredStart());
            container.AddFacility<TypedFactoryFacility>();

            container.Register(
                // CommandLib commands
                Classes.FromAssemblyContaining<CommandBase>().BasedOn<ICommand>().LifestyleTransient(),
                // CommandLib services
                Classes.FromAssemblyContaining<CommandParser>()
                    .InSameNamespaceAs<CommandParser>()
                    .WithServiceAllInterfaces(),
                Component.For<ICommandTypedFactory>().AsFactory(),

                
                // Helpers
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Helpers").WithServiceAllInterfaces(),
                
                // Services
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Services").WithServiceAllInterfaces(),
                
                // Main application
                Component.For<IApplication>()
                    .ImplementedBy<Application>()
                    .DependsOn(
                        Dependency.OnComponent("freenodeClient", "freenodeClient"),
                        Dependency.OnComponent("wikimediaClient", "wikimediaClient")
                    ),
                
                // Individual components
                Component.For<LegacyCommandHandler>().Named("commandHandler"),
                Component.For<ICommandHandler>().ImplementedBy<NewCommandHandler>().Named("newCommandHandler"),
                
                Component.For<ISupportHelper>().ImplementedBy<SupportHelper>(),
                Component.For<RecentChangeHandler>().Named("rcHandler")
                    .DependsOn(
                        Dependency.OnComponent("freenodeClient", "freenodeClient")
                    ),
                Component.For<StalkConfiguration>()
                    .OnCreate((kernel, instance) => instance.Initialise()),
                Component.For<IIrcClient>()
                    .ImplementedBy<IrcClient>()
                    .Named("freenodeClient")
                    .DependsOn(Dependency.OnComponent("configuration", "freenodeIrcConfig"))
                    .Start()
                    .PublishEvent(p => p.ReceivedMessage += null, x=>x.To<LegacyCommandHandler>("commandHandler", l=>l.OnReceivedMessage(null, null)))
                    .PublishEvent(p => p.ReceivedMessage += null, x=>x.To<NewCommandHandler>("newCommandHandler", l=>l.OnMessageReceived(null, null))),
                Component.For<IIrcClient>()
                    .ImplementedBy<IrcClient>()
                    .Named("wikimediaClient")
                    .DependsOn(Dependency.OnComponent("configuration", "wikimediaIrcConfig"))
                    .Start()
                    .PublishEvent(p => p.ReceivedMessage += null, x=>x.To<RecentChangeHandler>("rcHandler", l=>l.OnReceivedMessage(null, null))),
                Component.For<Nagios>().Start().StopUsingMethod("Stop")
            );
        }
    }
}