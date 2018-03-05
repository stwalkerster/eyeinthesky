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

            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            container.Install(
                FromAssembly.This(),
                Configuration.FromXmlFile("configuration.xml"));

            var app = container.Resolve<Application>();
            app.Run();
            container.Release(app);
        }
    }
}