namespace EyeInTheSky.Web.Startup
{
    using System;
    using System.IO;
    using System.Reflection;
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Authentication.Stateless;
    using Nancy.Bootstrapper;
    using Nancy.Bootstrappers.Windsor;
    using Nancy.Conventions;
    using Nancy.Security;
    using Nancy.ViewEngines;

    public class NancyBootstrapper : WindsorNancyBootstrapper
    {
        private ILogger logger;
        private byte[] favicon;

        protected override byte[] FavIcon
        {
            get { return this.favicon ?? (this.favicon = this.LoadFavicon()); }
        }

        private byte[] LoadFavicon()
        {
            using (var resourceStream = this.GetType().Assembly.GetManifestResourceStream("EyeInTheSky.Web.Content.logo.ico"))
            {
                var memoryStream = new MemoryStream();
                resourceStream.CopyTo(memoryStream);
                return memoryStream.GetBuffer();
            }
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.AddDirectory("Scripts", "Scripts");
        }

        protected override void RequestStartup(IWindsorContainer container, IPipelines pipelines, NancyContext context)
        {
            SSLProxy.RewriteSchemeUsingForwardedHeaders(pipelines);

#if SKIPAUTH
            var statelessAuthenticationConfiguration = new StatelessAuthenticationConfiguration(ctx =>
                {
                    var userMapper = container.Resolve<IUserMapper>();
                    return userMapper.GetUserFromIdentifier(Guid.Empty, null);
                });
            StatelessAuthentication.Enable(pipelines, statelessAuthenticationConfiguration);
#else
            FormsAuthenticationConfiguration formsAuthConfiguration;           
            try
            {
                var userMapper = container.Resolve<IUserMapper>();
                
                formsAuthConfiguration = new FormsAuthenticationConfiguration
                {
                    UserMapper = userMapper,
                    RedirectUrl = "~/login"
                };
            }
            catch (Exception e)
            {
                this.logger.Error("Error getting user mapper", e);
                throw;
            }

            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
#endif
            
        }

        protected override void ApplicationStartup(IWindsorContainer container, IPipelines pipelines)
        {
            ResourceViewLocationProvider.RootNamespaces.Add(Assembly.GetExecutingAssembly(), "EyeInTheSky.Web");

            this.logger = container.Resolve<ILogger>();
            
            base.ApplicationStartup(container, pipelines);
        }

        /// <summary>
        /// Configures Nancy to use EmbeddedResources for it's views
        /// </summary>
        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(
                    nic => nic.ViewLocationProvider = typeof(ResourceViewLocationProvider));
            }
        }

        /// <summary>
        /// Provides Nancy the main application's Windsor container
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected override IWindsorContainer GetApplicationContainer()
        {
            var typeName = Assembly.GetEntryAssembly().GetName().Name + ".Startup.Launch";
            var type = Assembly.GetEntryAssembly().GetType(typeName);

            var fieldInfo = type.GetField("container", BindingFlags.NonPublic | BindingFlags.Static);
            if (fieldInfo == null)
            {
                throw new Exception("Cannot find container");
            }
            
            var windsorContainer = (IWindsorContainer)fieldInfo.GetValue(null);
            return windsorContainer;
        }
    }
}