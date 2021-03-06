namespace EyeInTheSky.Web.Startup
{
    using System;
    using System.IO;
    using System.Reflection;
    using Castle.Core.Logging;
    using Castle.Windsor;

    using EyeInTheSky.Web.Misc;

    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Authentication.Stateless;
    using Nancy.Bootstrapper;
    using Nancy.Bootstrappers.Windsor;
    using Nancy.Conventions;
    using Nancy.Diagnostics;
    using Nancy.Security;

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
            this.logger = container.Resolve<ILogger>();

            base.ApplicationStartup(container, pipelines);
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

        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get
            {
                var config = base.DiagnosticsConfiguration;
                var container = this.GetApplicationContainer();
                var webConfiguration = container.Resolve<WebConfiguration>();

                config.Password = webConfiguration.DiagnosticsPassword;
                config.Path = webConfiguration.DiagnosticsPath;

                return config;
            }
        }
    }
}