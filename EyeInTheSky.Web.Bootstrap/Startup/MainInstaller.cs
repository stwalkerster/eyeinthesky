namespace EyeInTheSky.Web.Bootstrap.Startup
{
    using Castle.Facilities.Logging;
    using Castle.Facilities.Startable;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Services.Logging.Log4netIntegration;
    using Castle.Windsor;
    using EyeInTheSky.Web.Startup;
    using Moq;
    using Stwalkerster.IrcClient.Interfaces;

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