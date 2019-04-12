namespace EyeInTheSky.Web.Startup
{
    using Castle.Core.Logging;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;

    using EyeInTheSky.Web.Misc;

    using Nancy.Diagnostics;

    public class WebInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Install(Configuration.FromXmlFile("web.xml"));

            var webConfig = container.Resolve<WebConfiguration>();
            var logger = container.Resolve<ILogger>();
            if (string.IsNullOrWhiteSpace(webConfig.DiagnosticsPassword))
            {
                logger.Info("No web diagnostics password set, disabling diagnostics");
                container.Register(Component.For<IDiagnostics>().ImplementedBy<DisabledDiagnostics>());
            }
            
            container.Register(
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Web.Services").WithServiceAllInterfaces()
            );
        }
    }
}