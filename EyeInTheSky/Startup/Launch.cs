namespace EyeInTheSky.Startup
{
    using System;
    using System.IO;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;

    public class Launch
    {
        private static WindsorContainer container;
        public static int Main(string[] args)
        {
            string configurationFile = "configuration.xml";
            
            if (args.Length >= 1)
            {
                configurationFile = args[0];
            }

            if (!File.Exists(configurationFile))
            {
                Console.WriteLine("Configuration file does not exist!");
                return 1;
            }
            
            container = new WindsorContainer(configurationFile);
            container.Install(FromAssembly.This());

            var app = container.Resolve<IApplication>();

            app.Run();
            container.Release(app);
            return 0;
        }
    }
}