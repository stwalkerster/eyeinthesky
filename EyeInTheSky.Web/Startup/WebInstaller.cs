namespace EyeInTheSky.Web.Startup
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;

    public class WebInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Install(Configuration.FromXmlFile("web.xml"));

            container.Register(
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Web.Services").WithServiceAllInterfaces()
            );
        }
    }
}