namespace EyeInTheSky.Startup
{
    using Castle.Facilities.EventWiring;
    using Castle.Facilities.Logging;
    using Castle.Facilities.Startable;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Services.Logging.Log4netIntegration;
    using Castle.Windsor;
    using EyeInTheSky.Helpers;
    using EyeInTheSky.Model;
    using EyeInTheSky.Services;
    using EyeInTheSky.Services.Interfaces;
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

            container.Register(
                Component.For<Application>()
                    .Named("bot")
                    .DependsOn(
                        Dependency.OnComponent("freenodeClient", "freenodeClient"),
                        Dependency.OnComponent("wikimediaClient", "wikimediaClient")
                    ),
                Component.For<CommandHandler>().Named("commandHandler"),
                Component.For<StalkFactory>(),
                Component.For<IRecentChangeParser>().ImplementedBy<RecentChangeParser>(),
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
                    .PublishEvent(p => p.ReceivedMessage += null, x=>x.To<CommandHandler>("commandHandler", l=>l.OnReceivedMessage(null, null))),
                Component.For<IIrcClient>()
                    .ImplementedBy<IrcClient>()
                    .Named("wikimediaClient")
                    .DependsOn(Dependency.OnComponent("configuration", "wikimediaIrcConfig"))
                    .Start()
                    .PublishEvent(p => p.ReceivedMessage += null, x=>x.To<CommandHandler>("rcHandler", l=>l.OnReceivedMessage(null, null))),
                Component.For<Nagios>().Start().StopUsingMethod("Stop")
            );
        }
    }
}