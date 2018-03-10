namespace EyeInTheSky.Startup
{
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;

    public class Launch
    {
        private static WindsorContainer container;
        public static void Main()
        {
            container = new WindsorContainer("configuration.xml");
            container.Install(FromAssembly.This());

            var app = container.Resolve<IApplication>();

            app.Run();
            container.Release(app);
        }
    }
}