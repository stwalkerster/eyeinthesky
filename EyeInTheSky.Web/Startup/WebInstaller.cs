namespace EyeInTheSky.Web.Startup
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class WebInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly().InNamespace("EyeInTheSky.Web.Services").WithServiceAllInterfaces()
            );
        }
    }
}