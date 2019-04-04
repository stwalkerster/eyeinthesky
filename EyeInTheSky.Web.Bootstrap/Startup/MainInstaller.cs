namespace EyeInTheSky.Web.Bootstrap.Startup
{
    using System.Collections.Generic;

    using Castle.Facilities.Logging;
    using Castle.Facilities.Startable;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Services.Logging.Log4netIntegration;
    using Castle.Windsor;
    using EyeInTheSky.Web.Startup;
    using Moq;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;

    public class MainInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Facilities
            container.AddFacility<LoggingFacility>(f => f.LogUsing<Log4netFactory>().WithConfig("log4net.xml"));
            container.AddFacility<StartableFacility>(f => f.DeferredStart());

            container.Install(new WebInstaller());
            
            var ircClientMock = new Mock<IIrcClient>();
            ircClientMock.Setup(x => x.ExtBanTypes).Returns("a");
            ircClientMock.Setup(x => x.ExtBanDelimiter).Returns("$");
            ircClientMock.Setup(x => x.Nickname).Returns("EyeInTheSkyBot");
            ircClientMock.Setup(x => x.ClientName).Returns("Freenode");
            ircClientMock.Setup(x => x.Channels).Returns(new Dictionary<string, IrcChannel>());

            container.Register(
                // Main application
                Component.For<IApplication>().ImplementedBy<Launch>(),
                Classes.FromAssemblyNamed("EyeInTheSky").InNamespace("EyeInTheSky.Services").WithServiceAllInterfaces(),
                Classes.FromAssemblyNamed("EyeInTheSky").InNamespace("EyeInTheSky.Services.ExternalProviders").WithServiceAllInterfaces(),
                Component.For<IIrcClient>().Instance(ircClientMock.Object)
            );
        }
    }
}