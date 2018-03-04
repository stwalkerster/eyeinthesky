namespace EyeInTheSky.Startup
{
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using Microsoft.Practices.ServiceLocation;

    public class Launch
    {
        private static WindsorContainer container;

        public static void Main()
        {
            container = new WindsorContainer();
            container.Install(
                FromAssembly.This(),
                Configuration.FromXmlFile("configuration.xml"));

            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
            
            var app = container.Resolve<Application>();
            app.Run();
            container.Release(app);
        }
    }
}