namespace EyeInTheSky.Web.Bootstrap.Startup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Castle.Windsor.Installer;

    public interface IApplication
    {
        void Stop();

        void Run();
    }
    
    public class Launch : IApplication
    {
        private static WindsorContainer container;

        private readonly ILogger logger;
        private bool alive = true;

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

        public Launch(
            ILogger logger)
        {
            this.logger = logger;
        }

        public void Run()
        {
            while (this.alive)
            {
                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            this.alive = false;
        }
    }
}